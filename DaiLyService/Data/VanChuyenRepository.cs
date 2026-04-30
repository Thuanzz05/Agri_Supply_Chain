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
                    INNER JOIN TonKho tk ON ln.MaLo = tk.MaLo
                    INNER JOIN Kho k ON tk.MaKho = k.MaKho
                    LEFT JOIN SanPham sp ON ln.MaSanPham = sp.MaSanPham
                    WHERE k.MaChuSoHuu = @MaDaiLy AND k.LoaiChuSoHuu = 'daily'
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

        public bool UpdateTrangThai(int maVanChuyen, string trangThai, DateTime? ngayKetThuc = null)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                
                string sql = @"
                    UPDATE VanChuyen 
                    SET TrangThai = @TrangThai";
                
                // Nếu trạng thái là hoàn thành và chưa có ngày kết thúc
                if (trangThai == "hoan_thanh")
                {
                    sql += ", NgayKetThuc = @NgayKetThuc";
                }
                
                sql += " WHERE MaVanChuyen = @MaVanChuyen";
                
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaVanChuyen", maVanChuyen);
                cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                
                if (trangThai == "hoan_thanh")
                {
                    cmd.Parameters.AddWithValue("@NgayKetThuc", ngayKetThuc ?? DateTime.Now);
                }

                conn.Open();
                var rowsAffected = cmd.ExecuteNonQuery();
                
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Updated shipping {ShippingId} status to {Status}", maVanChuyen, trangThai);
                    return true;
                }
                
                return false;
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