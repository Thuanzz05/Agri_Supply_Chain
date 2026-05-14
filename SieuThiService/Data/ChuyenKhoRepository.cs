using Microsoft.Data.SqlClient;
using SieuThiService.Models.DTOs;
using System.Data;

namespace SieuThiService.Data
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
                // Validate 2 kho thuộc cùng 1 siêu thị
                int maSieuThi;
                using (var cmd = new SqlCommand(@"
                    SELECT TOP 1 k1.MaChuSoHuu
                    FROM Kho k1
                    INNER JOIN Kho k2 ON k2.MaKho = @MaKhoDich
                    WHERE k1.MaKho = @MaKhoNguon
                      AND k1.LoaiChuSoHuu = 'sieuthi'
                      AND k2.LoaiChuSoHuu = 'sieuthi'
                      AND k1.MaChuSoHuu = k2.MaChuSoHuu", conn, tx))
                {
                    cmd.Parameters.Add("@MaKhoNguon", SqlDbType.Int).Value = dto.MaKhoNguon;
                    cmd.Parameters.Add("@MaKhoDich", SqlDbType.Int).Value = dto.MaKhoDich;
                    var ownerObj = cmd.ExecuteScalar();
                    if (ownerObj == null)
                        throw new Exception("Kho nguồn/kho đích không hợp lệ hoặc không cùng thuộc một siêu thị");
                    maSieuThi = Convert.ToInt32(ownerObj);
                }

                // Check tồn kho nguồn đủ số lượng
                decimal soLuongHienTai;
                using (var cmd = new SqlCommand(@"
                    SELECT SoLuong
                    FROM TonKho
                    WHERE MaKho = @MaKhoNguon AND MaLo = @MaLo", conn, tx))
                {
                    cmd.Parameters.Add("@MaKhoNguon", SqlDbType.Int).Value = dto.MaKhoNguon;
                    cmd.Parameters.Add("@MaLo", SqlDbType.Int).Value = dto.MaLo;
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
                    cmd.Parameters.Add("@SoLuong", SqlDbType.Decimal).Value = dto.SoLuong;
                    cmd.Parameters.Add("@MaKhoNguon", SqlDbType.Int).Value = dto.MaKhoNguon;
                    cmd.Parameters.Add("@MaLo", SqlDbType.Int).Value = dto.MaLo;
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
                    cmd.Parameters.Add("@SoLuong", SqlDbType.Decimal).Value = dto.SoLuong;
                    cmd.Parameters.Add("@MaKhoDich", SqlDbType.Int).Value = dto.MaKhoDich;
                    cmd.Parameters.Add("@MaLo", SqlDbType.Int).Value = dto.MaLo;
                    cmd.ExecuteNonQuery();
                }

                // Ghi lịch sử
                using var insertCmd = new SqlCommand(@"
                    INSERT INTO PhieuChuyenKho (MaKhoNguon, MaKhoDich, MaLo, SoLuong, NgayChuyen, GhiChu)
                    OUTPUT INSERTED.MaPhieu
                    VALUES (@MaKhoNguon, @MaKhoDich, @MaLo, @SoLuong, GETDATE(), @GhiChu)", conn, tx);
                insertCmd.Parameters.Add("@MaKhoNguon", SqlDbType.Int).Value = dto.MaKhoNguon;
                insertCmd.Parameters.Add("@MaKhoDich", SqlDbType.Int).Value = dto.MaKhoDich;
                insertCmd.Parameters.Add("@MaLo", SqlDbType.Int).Value = dto.MaLo;
                insertCmd.Parameters.Add("@SoLuong", SqlDbType.Decimal).Value = dto.SoLuong;
                insertCmd.Parameters.Add("@GhiChu", SqlDbType.NVarChar, 500).Value = (object?)dto.GhiChu ?? DBNull.Value;

                var maPhieu = Convert.ToInt32(insertCmd.ExecuteScalar());
                tx.Commit();

                _logger.LogInformation("Transferred lot {LotId} qty {Qty} from warehouse {From} to {To} for supermarket {SupermarketId}. Receipt {ReceiptId}",
                    dto.MaLo, dto.SoLuong, dto.MaKhoNguon, dto.MaKhoDich, maSieuThi, maPhieu);

                return maPhieu;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public List<ChuyenKhoDTO> GetBySieuThi(int maSieuThi)
        {
            var list = new List<ChuyenKhoDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT p.MaPhieu, p.MaKhoNguon, p.MaKhoDich, p.MaLo, p.SoLuong, p.NgayChuyen AS NgayTao, p.GhiChu,
                           kn.TenKho AS TenKhoNguon,
                           kd.TenKho AS TenKhoDich,
                           sp.TenSanPham
                    FROM PhieuChuyenKho p
                    INNER JOIN Kho kn ON p.MaKhoNguon = kn.MaKho
                    INNER JOIN Kho kd ON p.MaKhoDich = kd.MaKho
                    LEFT JOIN LoNongSan ln ON p.MaLo = ln.MaLo
                    LEFT JOIN SanPham sp ON ln.MaSanPham = sp.MaSanPham
                    WHERE kn.LoaiChuSoHuu = 'sieuthi'
                      AND kd.LoaiChuSoHuu = 'sieuthi'
                      AND kn.MaChuSoHuu = @MaSieuThi
                    ORDER BY p.NgayChuyen DESC, p.MaPhieu DESC", conn);
                cmd.Parameters.Add("@MaSieuThi", SqlDbType.Int).Value = maSieuThi;

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new ChuyenKhoDTO
                    {
                        MaPhieu = reader.GetInt32("MaPhieu"),
                        MaKhoNguon = reader.GetInt32("MaKhoNguon"),
                        MaKhoDich = reader.GetInt32("MaKhoDich"),
                        MaLo = reader.GetInt32("MaLo"),
                        SoLuong = reader.GetDecimal("SoLuong"),
                        NgayTao = reader.GetDateTime("NgayTao"),
                        GhiChu = reader.IsDBNull("GhiChu") ? null : reader.GetString("GhiChu"),
                        TenKhoNguon = reader.IsDBNull("TenKhoNguon") ? null : reader.GetString("TenKhoNguon"),
                        TenKhoDich = reader.IsDBNull("TenKhoDich") ? null : reader.GetString("TenKhoDich"),
                        TenSanPham = reader.IsDBNull("TenSanPham") ? null : reader.GetString("TenSanPham"),
                    });
                }
                _logger.LogInformation("Retrieved {Count} transfer receipts for supermarket {SupermarketId}", list.Count, maSieuThi);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting transfer receipts for supermarket {SupermarketId}", maSieuThi);
                throw new Exception("Lỗi truy vấn lịch sử chuyển kho", ex);
            }
            return list;
        }
    }
}
