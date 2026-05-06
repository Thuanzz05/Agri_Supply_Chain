using Microsoft.Data.SqlClient;
using DaiLyService.Models.DTOs;
using System.Data;

namespace DaiLyService.Data
{
    public class VanChuyenRepository : IVanChuyenRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<VanChuyenRepository> _logger;

        public VanChuyenRepository(IConfiguration config, ILogger<VanChuyenRepository> logger)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
            _logger = logger;
        }

        public List<VanChuyenDTO> GetAll()
        {
            var list = new List<VanChuyenDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT vc.MaVanChuyen, vc.MaLo, vc.DiemDi, vc.DiemDen, 
                           vc.NgayBatDau, vc.NgayKetThuc, vc.TrangThai,
                           sp.TenSanPham, sp.DonViTinh, ln.MaQR, ln.SoLuongHienTai,
                           ln.NgayThuHoach, ln.HanSuDung
                    FROM VanChuyen vc
                    LEFT JOIN LoNongSan ln ON vc.MaLo = ln.MaLo
                    LEFT JOIN SanPham sp ON ln.MaSanPham = sp.MaSanPham
                    ORDER BY vc.NgayBatDau DESC", conn);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} shipping records", list.Count);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting all shipping records");
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public List<VanChuyenDTO> GetByTrangThai(string trangThai)
        {
            var list = new List<VanChuyenDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT vc.MaVanChuyen, vc.MaLo, vc.DiemDi, vc.DiemDen, 
                           vc.NgayBatDau, vc.NgayKetThuc, vc.TrangThai,
                           sp.TenSanPham, sp.DonViTinh, ln.MaQR, ln.SoLuongHienTai,
                           ln.NgayThuHoach, ln.HanSuDung
                    FROM VanChuyen vc
                    LEFT JOIN LoNongSan ln ON vc.MaLo = ln.MaLo
                    LEFT JOIN SanPham sp ON ln.MaSanPham = sp.MaSanPham
                    WHERE vc.TrangThai = @TrangThai
                    ORDER BY vc.NgayBatDau DESC", conn);
                
                cmd.Parameters.AddWithValue("@TrangThai", trangThai);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} shipping records with status {Status}", list.Count, trangThai);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting shipping records by status");
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public List<VanChuyenDTO> GetByLo(int maLo)
        {
            var list = new List<VanChuyenDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT vc.MaVanChuyen, vc.MaLo, vc.DiemDi, vc.DiemDen, 
                           vc.NgayBatDau, vc.NgayKetThuc, vc.TrangThai,
                           sp.TenSanPham, sp.DonViTinh, ln.MaQR, ln.SoLuongHienTai,
                           ln.NgayThuHoach, ln.HanSuDung
                    FROM VanChuyen vc
                    LEFT JOIN LoNongSan ln ON vc.MaLo = ln.MaLo
                    LEFT JOIN SanPham sp ON ln.MaSanPham = sp.MaSanPham
                    WHERE vc.MaLo = @MaLo
                    ORDER BY vc.NgayBatDau DESC", conn);

                cmd.Parameters.AddWithValue("@MaLo", maLo);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} shipping records for batch {MaLo}", list.Count, maLo);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting shipping records by batch");
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public List<VanChuyenDTO> GetByDaiLy(int maDaiLy)
        {
            var list = new List<VanChuyenDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT DISTINCT vc.MaVanChuyen, vc.MaLo, vc.DiemDi, vc.DiemDen, 
                           vc.NgayBatDau, vc.NgayKetThuc, vc.TrangThai,
                           sp.TenSanPham, sp.DonViTinh, ln.MaQR, ln.SoLuongHienTai,
                           ln.NgayThuHoach, ln.HanSuDung
                    FROM VanChuyen vc
                    INNER JOIN LoNongSan ln ON vc.MaLo = ln.MaLo
                    INNER JOIN ChiTietDonHang ct ON ln.MaLo = ct.MaLo
                    INNER JOIN DonHang dh ON ct.MaDonHang = dh.MaDonHang
                    LEFT JOIN SanPham sp ON ln.MaSanPham = sp.MaSanPham
                    WHERE (
                        (dh.MaNguoiMua = @MaDaiLy AND dh.LoaiNguoiMua = 'daily')
                        OR
                        (dh.MaNguoiBan = @MaDaiLy AND dh.LoaiNguoiBan = 'daily')
                    )
                    ORDER BY vc.NgayBatDau DESC", conn);

                cmd.Parameters.AddWithValue("@MaDaiLy", maDaiLy);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} shipping records for agent {MaDaiLy}", list.Count, maDaiLy);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting shipping records by agent");
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public VanChuyenDTO? GetById(int maVanChuyen)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT vc.MaVanChuyen, vc.MaLo, vc.DiemDi, vc.DiemDen, 
                           vc.NgayBatDau, vc.NgayKetThuc, vc.TrangThai,
                           sp.TenSanPham, sp.DonViTinh, ln.MaQR, ln.SoLuongHienTai,
                           ln.NgayThuHoach, ln.HanSuDung
                    FROM VanChuyen vc
                    LEFT JOIN LoNongSan ln ON vc.MaLo = ln.MaLo
                    LEFT JOIN SanPham sp ON ln.MaSanPham = sp.MaSanPham
                    WHERE vc.MaVanChuyen = @MaVanChuyen", conn);
                
                cmd.Parameters.AddWithValue("@MaVanChuyen", maVanChuyen);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                return MapToDTO(reader);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting shipping record by ID {ShippingId}", maVanChuyen);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
        }

        public int Create(VanChuyenCreateDTO dto)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    INSERT INTO VanChuyen (MaLo, DiemDi, DiemDen, NgayBatDau, TrangThai) 
                    OUTPUT INSERTED.MaVanChuyen 
                    VALUES (@MaLo, @DiemDi, @DiemDen, @NgayBatDau, @TrangThai)", conn);

                cmd.Parameters.AddWithValue("@MaLo", dto.MaLo);
                cmd.Parameters.AddWithValue("@DiemDi", dto.DiemDi);
                cmd.Parameters.AddWithValue("@DiemDen", dto.DiemDen);
                cmd.Parameters.AddWithValue("@NgayBatDau", dto.NgayBatDau ?? DateTime.Now);
                cmd.Parameters.AddWithValue("@TrangThai", "dang_van_chuyen");

                conn.Open();
                var maVanChuyen = (int)cmd.ExecuteScalar();
                
                _logger.LogInformation("Created shipping record {ShippingId} for lot {LotId}", maVanChuyen, dto.MaLo);
                return maVanChuyen;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while creating shipping record");
                if (ex.Number == 547) // Foreign key constraint
                    throw new Exception("Lô nông sản không tồn tại trong hệ thống", ex);
                throw new Exception("Lỗi tạo vận chuyển trong cơ sở dữ liệu: " + ex.Message, ex);
            }
        }

        public bool UpdateTrangThai(int maVanChuyen, string trangThai, DateTime? ngayKetThuc = null, int? maKhoDich = null)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                using var transaction = conn.BeginTransaction();

                try
                {
                    int maLo;
                    using (var getTransportCmd = new SqlCommand(@"
                        SELECT MaLo, TrangThai
                        FROM VanChuyen
                        WHERE MaVanChuyen = @MaVanChuyen", conn, transaction))
                    {
                        getTransportCmd.Parameters.AddWithValue("@MaVanChuyen", maVanChuyen);
                        using var reader = getTransportCmd.ExecuteReader();
                        if (!reader.Read())
                        {
                            transaction.Rollback();
                            return false;
                        }
                        maLo = reader.GetInt32(reader.GetOrdinal("MaLo"));
                    }

                    string sql = @"
                        UPDATE VanChuyen 
                        SET TrangThai = @TrangThai";

                    if (trangThai == "hoan_thanh")
                    {
                        sql += ", NgayKetThuc = @NgayKetThuc";
                    }

                    sql += " WHERE MaVanChuyen = @MaVanChuyen";

                    using var cmd = new SqlCommand(sql, conn, transaction);
                    cmd.Parameters.AddWithValue("@MaVanChuyen", maVanChuyen);
                    cmd.Parameters.AddWithValue("@TrangThai", trangThai);

                    if (trangThai == "hoan_thanh")
                    {
                        cmd.Parameters.AddWithValue("@NgayKetThuc", ngayKetThuc ?? DateTime.Now);
                    }

                    var rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        transaction.Rollback();
                        return false;
                    }

                    if (trangThai == "hoan_thanh")
                    {
                        using var getTargetOrderCmd = new SqlCommand(@"
                            SELECT TOP 1 dh.MaDonHang, dh.MaNguoiMua
                            FROM DonHang dh
                            INNER JOIN ChiTietDonHang ct ON dh.MaDonHang = ct.MaDonHang
                            WHERE dh.LoaiDon = 'nongdan_to_daily'
                              AND dh.TrangThai = 'dang_van_chuyen'
                              AND ct.MaLo = @MaLo
                            ORDER BY dh.NgayDat DESC, dh.MaDonHang DESC", conn, transaction);
                        getTargetOrderCmd.Parameters.AddWithValue("@MaLo", maLo);

                        int? targetOrderId = null;
                        int? targetMaDaiLy = null;
                        using (var targetReader = getTargetOrderCmd.ExecuteReader())
                        {
                            if (targetReader.Read())
                            {
                                targetOrderId = targetReader.GetInt32(0);
                                targetMaDaiLy = targetReader.GetInt32(1);
                            }
                        }

                        if (targetOrderId.HasValue && targetMaDaiLy.HasValue)
                        {
                            using var getDetailCmd = new SqlCommand(@"
                                SELECT TOP 1 SoLuong
                                FROM ChiTietDonHang
                                WHERE MaDonHang = @MaDonHang AND MaLo = @MaLo", conn, transaction);
                            getDetailCmd.Parameters.AddWithValue("@MaDonHang", targetOrderId.Value);
                            getDetailCmd.Parameters.AddWithValue("@MaLo", maLo);
                            var orderQtyObj = getDetailCmd.ExecuteScalar();
                            var orderQty = orderQtyObj == null ? 0 : Convert.ToDecimal(orderQtyObj);

                            if (orderQty <= 0)
                            {
                                throw new Exception($"Không tìm thấy số lượng lô {maLo} trong đơn {targetOrderId.Value}");
                            }

                            using var getLotQtyCmd = new SqlCommand("SELECT SoLuongHienTai FROM LoNongSan WHERE MaLo = @MaLo", conn, transaction);
                            getLotQtyCmd.Parameters.AddWithValue("@MaLo", maLo);
                            var currentQtyObj = getLotQtyCmd.ExecuteScalar();
                            var currentQty = currentQtyObj == null ? 0 : Convert.ToDecimal(currentQtyObj);

                            if (currentQty >= orderQty)
                            {
                                using var updateLotCmd = new SqlCommand(@"
                                    UPDATE LoNongSan
                                    SET SoLuongHienTai = SoLuongHienTai - @SoLuong
                                    WHERE MaLo = @MaLo", conn, transaction);
                                updateLotCmd.Parameters.AddWithValue("@MaLo", maLo);
                                updateLotCmd.Parameters.AddWithValue("@SoLuong", orderQty);
                                updateLotCmd.ExecuteNonQuery();

                                using var updateLotStatusCmd = new SqlCommand(@"
                                    UPDATE LoNongSan
                                    SET TrangThai = N'da_ban'
                                    WHERE MaLo = @MaLo AND SoLuongHienTai = 0", conn, transaction);
                                updateLotStatusCmd.Parameters.AddWithValue("@MaLo", maLo);
                                updateLotStatusCmd.ExecuteNonQuery();
                            }
                            else
                            {
                                _logger.LogWarning("Skipped lot quantity deduction for transport {TransportId} because lot {LotId} current quantity {CurrentQty} is lower than order quantity {OrderQty}.", maVanChuyen, maLo, currentQty, orderQty);
                            }

                            if (!maKhoDich.HasValue || maKhoDich <= 0)
                            {
                                throw new Exception("Vui lòng chọn kho đích để nhập hàng (maKhoDich)");
                            }

                            using (var validateKhoCmd = new SqlCommand(@"
                                SELECT COUNT(*) 
                                FROM Kho 
                                WHERE MaKho = @MaKho AND MaChuSoHuu = @MaDaiLy AND LoaiChuSoHuu = 'daily'", conn, transaction))
                            {
                                validateKhoCmd.Parameters.AddWithValue("@MaKho", maKhoDich.Value);
                                validateKhoCmd.Parameters.AddWithValue("@MaDaiLy", targetMaDaiLy.Value);
                                var ok = (int)validateKhoCmd.ExecuteScalar();
                                if (ok == 0)
                                {
                                    throw new Exception("Kho đích không hợp lệ hoặc không thuộc đại lý");
                                }
                            }

                            var maKho = maKhoDich.Value;
                            if (maKho > 0)
                            {
                                using var checkInvCmd = new SqlCommand(@"
                                    SELECT SoLuong FROM TonKho WHERE MaKho = @MaKho AND MaLo = @MaLo", conn, transaction);
                                checkInvCmd.Parameters.AddWithValue("@MaKho", maKho);
                                checkInvCmd.Parameters.AddWithValue("@MaLo", maLo);
                                var existingQty = checkInvCmd.ExecuteScalar();

                                if (existingQty != null)
                                {
                                    using var updateInvCmd = new SqlCommand(@"
                                        UPDATE TonKho
                                        SET SoLuong = SoLuong + @SoLuong, NgayCapNhat = GETDATE()
                                        WHERE MaKho = @MaKho AND MaLo = @MaLo", conn, transaction);
                                    updateInvCmd.Parameters.AddWithValue("@MaKho", maKho);
                                    updateInvCmd.Parameters.AddWithValue("@MaLo", maLo);
                                    updateInvCmd.Parameters.AddWithValue("@SoLuong", orderQty);
                                    updateInvCmd.ExecuteNonQuery();
                                }
                                else
                                {
                                    using var insertInvCmd = new SqlCommand(@"
                                        INSERT INTO TonKho (MaKho, MaLo, SoLuong, NgayCapNhat)
                                        VALUES (@MaKho, @MaLo, @SoLuong, GETDATE())", conn, transaction);
                                    insertInvCmd.Parameters.AddWithValue("@MaKho", maKho);
                                    insertInvCmd.Parameters.AddWithValue("@MaLo", maLo);
                                    insertInvCmd.Parameters.AddWithValue("@SoLuong", orderQty);
                                    insertInvCmd.ExecuteNonQuery();
                                }
                            }

                            using var completeOrderCmd = new SqlCommand(@"
                                UPDATE DonHang
                                SET TrangThai = 'hoan_thanh'
                                WHERE MaDonHang = @MaDonHang
                                  AND TrangThai = 'dang_van_chuyen'", conn, transaction);
                            completeOrderCmd.Parameters.AddWithValue("@MaDonHang", targetOrderId.Value);
                            completeOrderCmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    _logger.LogInformation("Updated shipping {ShippingId} status to {Status}", maVanChuyen, trangThai);
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while updating shipping status");
                throw new Exception("Lỗi cập nhật trạng thái vận chuyển", ex);
            }
        }

        public bool Delete(int maVanChuyen)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("DELETE FROM VanChuyen WHERE MaVanChuyen = @MaVanChuyen", conn);
                cmd.Parameters.AddWithValue("@MaVanChuyen", maVanChuyen);

                conn.Open();
                var rowsAffected = cmd.ExecuteNonQuery();
                
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Deleted shipping record {ShippingId}", maVanChuyen);
                    return true;
                }
                
                return false;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while deleting shipping record");
                throw new Exception("Lỗi xóa vận chuyển trong cơ sở dữ liệu", ex);
            }
        }

        public int CountByTrangThai(string trangThai)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SELECT COUNT(*) FROM VanChuyen WHERE TrangThai = @TrangThai", conn);
                cmd.Parameters.AddWithValue("@TrangThai", trangThai);

                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while counting shipping records by status");
                throw new Exception("Lỗi đếm số lượng vận chuyển", ex);
            }
        }

        public object GetStatsByDaiLy(int maDaiLy)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT 
                        COUNT(DISTINCT vc.MaVanChuyen) AS TongVanChuyen,
                        COUNT(DISTINCT CASE WHEN vc.TrangThai = 'dang_van_chuyen' THEN vc.MaVanChuyen END) AS DangVanChuyen,
                        COUNT(DISTINCT CASE WHEN vc.TrangThai = 'hoan_thanh' THEN vc.MaVanChuyen END) AS HoanThanh
                    FROM VanChuyen vc
                    INNER JOIN LoNongSan ln ON vc.MaLo = ln.MaLo
                    INNER JOIN ChiTietDonHang ct ON ln.MaLo = ct.MaLo
                    INNER JOIN DonHang dh ON ct.MaDonHang = dh.MaDonHang
                    WHERE (
                        (dh.MaNguoiMua = @MaDaiLy AND dh.LoaiNguoiMua = 'daily')
                        OR
                        (dh.MaNguoiBan = @MaDaiLy AND dh.LoaiNguoiBan = 'daily')
                    )", conn);

                cmd.Parameters.AddWithValue("@MaDaiLy", maDaiLy);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new
                    {
                        tongVanChuyen = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                        dangVanChuyen = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                        hoanThanh = reader.IsDBNull(2) ? 0 : reader.GetInt32(2)
                    };
                }

                return new { tongVanChuyen = 0, dangVanChuyen = 0, hoanThanh = 0 };
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting shipping stats for agent {AgentId}", maDaiLy);
                throw new Exception("Lỗi truy vấn thống kê vận chuyển", ex);
            }
        }

        private static VanChuyenDTO MapToDTO(SqlDataReader reader)
        {
            return new VanChuyenDTO
            {
                MaVanChuyen = reader.GetInt32("MaVanChuyen"),
                MaLo = reader.GetInt32("MaLo"),
                DiemDi = reader.GetString("DiemDi"),
                DiemDen = reader.GetString("DiemDen"),
                NgayBatDau = reader.GetDateTime("NgayBatDau"),
                NgayKetThuc = reader.IsDBNull("NgayKetThuc") ? null : reader.GetDateTime("NgayKetThuc"),
                TrangThai = reader.GetString("TrangThai"),
                TenSanPham = reader.IsDBNull("TenSanPham") ? null : reader.GetString("TenSanPham"),
                DonViTinh = reader.IsDBNull("DonViTinh") ? null : reader.GetString("DonViTinh"),
                MaQR = reader.IsDBNull("MaQR") ? null : reader.GetString("MaQR"),
                SoLuongLo = reader.IsDBNull("SoLuongHienTai") ? null : reader.GetDecimal("SoLuongHienTai"),
                NgayThuHoach = reader.IsDBNull("NgayThuHoach") ? null : reader.GetDateTime("NgayThuHoach"),
                HanSuDung = reader.IsDBNull("HanSuDung") ? null : reader.GetDateTime("HanSuDung")
            };
        }
    }
}