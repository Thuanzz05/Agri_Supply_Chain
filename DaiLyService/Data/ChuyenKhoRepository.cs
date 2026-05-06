using Microsoft.Data.SqlClient;
using DaiLyService.Models.DTOs;

namespace DaiLyService.Data
{
    public class ChuyenKhoRepository : IChuyenKhoRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<ChuyenKhoRepository> _logger;

        public ChuyenKhoRepository(IConfiguration config, ILogger<ChuyenKhoRepository> logger)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
            _logger = logger;
        }

        public int Create(ChuyenKhoCreateDTO dto)
        {
            if (dto.MaKhoNguon <= 0 || dto.MaKhoDich <= 0 || dto.MaLo <= 0)
                throw new Exception("Dữ liệu không hợp lệ");

            if (dto.MaKhoNguon == dto.MaKhoDich)
                throw new Exception("Kho nguồn và kho đích phải khác nhau");

            if (dto.SoLuong <= 0)
                throw new Exception("Số lượng chuyển phải lớn hơn 0");

            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {
                // Validate 2 kho thuộc cùng 1 đại lý
                int maDaiLy;
                using (var cmd = new SqlCommand(@"
                    SELECT TOP 1 k1.MaChuSoHuu
                    FROM Kho k1
                    INNER JOIN Kho k2 ON k2.MaKho = @MaKhoDich
                    WHERE k1.MaKho = @MaKhoNguon
                      AND k1.LoaiChuSoHuu = 'daily'
                      AND k2.LoaiChuSoHuu = 'daily'
                      AND k1.MaChuSoHuu = k2.MaChuSoHuu", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@MaKhoNguon", dto.MaKhoNguon);
                    cmd.Parameters.AddWithValue("@MaKhoDich", dto.MaKhoDich);
                    var ownerObj = cmd.ExecuteScalar();
                    if (ownerObj == null)
                        throw new Exception("Kho nguồn/kho đích không hợp lệ hoặc không cùng thuộc một đại lý");
                    maDaiLy = Convert.ToInt32(ownerObj);
                }

                // Check tồn kho nguồn đủ số lượng
                decimal soLuongHienTai;
                using (var cmd = new SqlCommand(@"
                    SELECT SoLuong
                    FROM TonKho
                    WHERE MaKho = @MaKhoNguon AND MaLo = @MaLo", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@MaKhoNguon", dto.MaKhoNguon);
                    cmd.Parameters.AddWithValue("@MaLo", dto.MaLo);
                    var qtyObj = cmd.ExecuteScalar();
                    if (qtyObj == null)
                        throw new Exception("Kho nguồn không có lô hàng này trong tồn kho");
                    soLuongHienTai = Convert.ToDecimal(qtyObj);
                }

                if (soLuongHienTai < dto.SoLuong)
                    throw new Exception("Số lượng chuyển vượt quá số lượng tồn kho của kho nguồn");

                // Trừ kho nguồn
                using (var cmd = new SqlCommand(@"
                    UPDATE TonKho
                    SET SoLuong = SoLuong - @SoLuong, NgayCapNhat = GETDATE()
                    WHERE MaKho = @MaKhoNguon AND MaLo = @MaLo", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@SoLuong", dto.SoLuong);
                    cmd.Parameters.AddWithValue("@MaKhoNguon", dto.MaKhoNguon);
                    cmd.Parameters.AddWithValue("@MaLo", dto.MaLo);
                    var rows = cmd.ExecuteNonQuery();
                    if (rows == 0)
                        throw new Exception("Không thể cập nhật tồn kho kho nguồn");
                }

                // Cộng kho đích (upsert)
                using (var cmd = new SqlCommand(@"
                    IF EXISTS (SELECT 1 FROM TonKho WHERE MaKho = @MaKhoDich AND MaLo = @MaLo)
                    BEGIN
                        UPDATE TonKho
                        SET SoLuong = SoLuong + @SoLuong, NgayCapNhat = GETDATE()
                        WHERE MaKho = @MaKhoDich AND MaLo = @MaLo
                    END
                    ELSE
                    BEGIN
                        INSERT INTO TonKho (MaKho, MaLo, SoLuong, NgayCapNhat)
                        VALUES (@MaKhoDich, @MaLo, @SoLuong, GETDATE())
                    END", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@SoLuong", dto.SoLuong);
                    cmd.Parameters.AddWithValue("@MaKhoDich", dto.MaKhoDich);
                    cmd.Parameters.AddWithValue("@MaLo", dto.MaLo);
                    cmd.ExecuteNonQuery();
                }

                // Ghi lịch sử
                using var insertCmd = new SqlCommand(@"
                    INSERT INTO PhieuChuyenKho (MaKhoNguon, MaKhoDich, MaLo, SoLuong, NgayChuyen, GhiChu)
                    OUTPUT INSERTED.MaPhieu
                    VALUES (@MaKhoNguon, @MaKhoDich, @MaLo, @SoLuong, GETDATE(), @GhiChu)", conn, tx);
                insertCmd.Parameters.AddWithValue("@MaKhoNguon", dto.MaKhoNguon);
                insertCmd.Parameters.AddWithValue("@MaKhoDich", dto.MaKhoDich);
                insertCmd.Parameters.AddWithValue("@MaLo", dto.MaLo);
                insertCmd.Parameters.AddWithValue("@SoLuong", dto.SoLuong);
                insertCmd.Parameters.AddWithValue("@GhiChu", (object?)dto.GhiChu ?? DBNull.Value);

                var maPhieu = Convert.ToInt32(insertCmd.ExecuteScalar());
                tx.Commit();

                _logger.LogInformation("Transferred lot {LotId} qty {Qty} from warehouse {From} to {To} for distributor {DistributorId}. Receipt {ReceiptId}",
                    dto.MaLo, dto.SoLuong, dto.MaKhoNguon, dto.MaKhoDich, maDaiLy, maPhieu);

                return maPhieu;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public List<PhieuChuyenKhoDTO> GetByDaiLy(int maDaiLy)
        {
            var list = new List<PhieuChuyenKhoDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT p.MaPhieu, p.MaKhoNguon, p.MaKhoDich, p.MaLo, p.SoLuong, p.NgayChuyen, p.GhiChu,
                           kn.TenKho AS TenKhoNguon,
                           kd.TenKho AS TenKhoDich,
                           sp.TenSanPham, sp.DonViTinh, ln.MaQR
                    FROM PhieuChuyenKho p
                    INNER JOIN Kho kn ON p.MaKhoNguon = kn.MaKho
                    INNER JOIN Kho kd ON p.MaKhoDich = kd.MaKho
                    LEFT JOIN LoNongSan ln ON p.MaLo = ln.MaLo
                    LEFT JOIN SanPham sp ON ln.MaSanPham = sp.MaSanPham
                    WHERE kn.LoaiChuSoHuu = 'daily'
                      AND kd.LoaiChuSoHuu = 'daily'
                      AND kn.MaChuSoHuu = @MaDaiLy
                    ORDER BY p.NgayChuyen DESC, p.MaPhieu DESC", conn);
                cmd.Parameters.AddWithValue("@MaDaiLy", maDaiLy);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new PhieuChuyenKhoDTO
                    {
                        MaPhieu = reader.GetInt32(reader.GetOrdinal("MaPhieu")),
                        MaKhoNguon = reader.GetInt32(reader.GetOrdinal("MaKhoNguon")),
                        MaKhoDich = reader.GetInt32(reader.GetOrdinal("MaKhoDich")),
                        MaLo = reader.GetInt32(reader.GetOrdinal("MaLo")),
                        SoLuong = reader.GetDecimal(reader.GetOrdinal("SoLuong")),
                        NgayChuyen = reader.GetDateTime(reader.GetOrdinal("NgayChuyen")),
                        GhiChu = reader.IsDBNull(reader.GetOrdinal("GhiChu")) ? null : reader.GetString(reader.GetOrdinal("GhiChu")),
                        TenKhoNguon = reader.IsDBNull(reader.GetOrdinal("TenKhoNguon")) ? null : reader.GetString(reader.GetOrdinal("TenKhoNguon")),
                        TenKhoDich = reader.IsDBNull(reader.GetOrdinal("TenKhoDich")) ? null : reader.GetString(reader.GetOrdinal("TenKhoDich")),
                        TenSanPham = reader.IsDBNull(reader.GetOrdinal("TenSanPham")) ? null : reader.GetString(reader.GetOrdinal("TenSanPham")),
                        DonViTinh = reader.IsDBNull(reader.GetOrdinal("DonViTinh")) ? null : reader.GetString(reader.GetOrdinal("DonViTinh")),
                        MaQR = reader.IsDBNull(reader.GetOrdinal("MaQR")) ? null : reader.GetString(reader.GetOrdinal("MaQR")),
                    });
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting transfer receipts for distributor {DistributorId}", maDaiLy);
                throw new Exception("Lỗi truy vấn lịch sử chuyển kho", ex);
            }
            return list;
        }
    }
}

