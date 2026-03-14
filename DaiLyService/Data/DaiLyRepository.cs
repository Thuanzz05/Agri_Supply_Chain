using Microsoft.Data.SqlClient;
using DaiLyService.Models.DTOs;
using System.Data;

namespace DaiLyService.Data
{
    public class DaiLyRepository : IDaiLyRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<DaiLyRepository> _logger;

        public DaiLyRepository(IConfiguration config, ILogger<DaiLyRepository> logger)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
            _logger = logger;
        }

        public List<DaiLyDTO> GetAll()
        {
            var list = new List<DaiLyDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT dl.MaDaiLy, dl.MaTaiKhoan, dl.TenDaiLy, dl.DiaChi, dl.SoDienThoai, 
                           dl.Email, dl.MoTa, dl.NgayTao, tk.TenDangNhap, tk.HoTen
                    FROM DaiLy dl
                    LEFT JOIN TaiKhoan tk ON dl.MaTaiKhoan = tk.MaTaiKhoan
                    ORDER BY dl.NgayTao DESC", conn);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} distributors from database", list.Count);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting all distributors");
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public DaiLyDTO? GetById(int id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT dl.MaDaiLy, dl.MaTaiKhoan, dl.TenDaiLy, dl.DiaChi, dl.SoDienThoai, 
                           dl.Email, dl.MoTa, dl.NgayTao, tk.TenDangNhap, tk.HoTen
                    FROM DaiLy dl
                    LEFT JOIN TaiKhoan tk ON dl.MaTaiKhoan = tk.MaTaiKhoan
                    WHERE dl.MaDaiLy = @MaDaiLy", conn);
                
                cmd.Parameters.AddWithValue("@MaDaiLy", id);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    _logger.LogWarning("Distributor with ID {DistributorId} not found", id);
                    return null;
                }
                return MapToDTO(reader);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting distributor with ID {DistributorId}", id);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
        }

        public DaiLyDTO? GetByTaiKhoan(int maTaiKhoan)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT dl.MaDaiLy, dl.MaTaiKhoan, dl.TenDaiLy, dl.DiaChi, dl.SoDienThoai, 
                           dl.Email, dl.MoTa, dl.NgayTao, tk.TenDangNhap, tk.HoTen
                    FROM DaiLy dl
                    LEFT JOIN TaiKhoan tk ON dl.MaTaiKhoan = tk.MaTaiKhoan
                    WHERE dl.MaTaiKhoan = @MaTaiKhoan", conn);
                
                cmd.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    _logger.LogWarning("Distributor with account ID {AccountId} not found", maTaiKhoan);
                    return null;
                }
                return MapToDTO(reader);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting distributor with account ID {AccountId}", maTaiKhoan);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
        }

        public int Create(DaiLyCreateDTO dto)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    INSERT INTO DaiLy (MaTaiKhoan, TenDaiLy, DiaChi, SoDienThoai, Email, MoTa, NgayTao) 
                    OUTPUT INSERTED.MaDaiLy 
                    VALUES (@MaTaiKhoan, @TenDaiLy, @DiaChi, @SoDienThoai, @Email, @MoTa, @NgayTao)", conn);

                cmd.Parameters.AddWithValue("@MaTaiKhoan", dto.MaTaiKhoan);
                cmd.Parameters.AddWithValue("@TenDaiLy", dto.TenDaiLy);
                cmd.Parameters.AddWithValue("@DiaChi", dto.DiaChi);
                cmd.Parameters.AddWithValue("@SoDienThoai", dto.SoDienThoai);
                cmd.Parameters.AddWithValue("@Email", dto.Email);
                cmd.Parameters.AddWithValue("@MoTa", (object?)dto.MoTa ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NgayTao", DateTime.Now);

                conn.Open();
                var maDaiLy = (int)cmd.ExecuteScalar();
                
                _logger.LogInformation("Created new distributor with ID {DistributorId}", maDaiLy);
                return maDaiLy;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while creating distributor");
                if (ex.Number == 547) // Foreign key constraint
                    throw new Exception("Tài khoản không tồn tại trong hệ thống", ex);
                if (ex.Number == 2627 || ex.Number == 2601) // Unique constraint
                    throw new Exception("Email hoặc số điện thoại đã tồn tại", ex);
                throw new Exception("Lỗi tạo đại lý trong cơ sở dữ liệu: " + ex.Message, ex);
            }
        }

        public bool Update(int id, DaiLyUpdateDTO dto)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    UPDATE DaiLy 
                    SET TenDaiLy = @TenDaiLy, DiaChi = @DiaChi, SoDienThoai = @SoDienThoai, 
                        Email = @Email, MoTa = @MoTa 
                    WHERE MaDaiLy = @MaDaiLy", conn);

                cmd.Parameters.AddWithValue("@MaDaiLy", id);
                cmd.Parameters.AddWithValue("@TenDaiLy", dto.TenDaiLy);
                cmd.Parameters.AddWithValue("@DiaChi", dto.DiaChi);
                cmd.Parameters.AddWithValue("@SoDienThoai", dto.SoDienThoai);
                cmd.Parameters.AddWithValue("@Email", dto.Email);
                cmd.Parameters.AddWithValue("@MoTa", (object?)dto.MoTa ?? DBNull.Value);

                conn.Open();
                var rowsAffected = cmd.ExecuteNonQuery();
                
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Updated distributor with ID {DistributorId}", id);
                    return true;
                }
                
                _logger.LogWarning("No distributor found with ID {DistributorId} to update", id);
                return false;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while updating distributor with ID {DistributorId}", id);
                throw new Exception("Lỗi cập nhật đại lý trong cơ sở dữ liệu", ex);
            }
        }

        public bool Delete(int id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("DELETE FROM DaiLy WHERE MaDaiLy = @MaDaiLy", conn);
                cmd.Parameters.AddWithValue("@MaDaiLy", id);

                conn.Open();
                var rowsAffected = cmd.ExecuteNonQuery();
                
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Deleted distributor with ID {DistributorId}", id);
                    return true;
                }
                
                _logger.LogWarning("No distributor found with ID {DistributorId} to delete", id);
                return false;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while deleting distributor with ID {DistributorId}", id);
                if (ex.Number == 547)
                    throw new Exception("Không thể xóa đại lý này vì đang có dữ liệu liên quan", ex);
                throw new Exception("Lỗi xóa đại lý trong cơ sở dữ liệu", ex);
            }
        }

        private static DaiLyDTO MapToDTO(SqlDataReader reader)
        {
            return new DaiLyDTO
            {
                MaDaiLy = reader.GetInt32("MaDaiLy"),
                MaTaiKhoan = reader.GetInt32("MaTaiKhoan"),
                TenDaiLy = reader.GetString("TenDaiLy"),
                DiaChi = reader.GetString("DiaChi"),
                SoDienThoai = reader.GetString("SoDienThoai"),
                Email = reader.GetString("Email"),
                MoTa = reader.IsDBNull("MoTa") ? null : reader.GetString("MoTa"),
                NgayTao = reader.GetDateTime("NgayTao"),
                TenDangNhap = reader.IsDBNull("TenDangNhap") ? null : reader.GetString("TenDangNhap"),
                HoTen = reader.IsDBNull("HoTen") ? null : reader.GetString("HoTen")
            };
        }
    }
}