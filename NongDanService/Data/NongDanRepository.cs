using Microsoft.Data.SqlClient;
using NongDanService.Models.DTOs;
using System.Data;

namespace NongDanService.Data
{
    public class NongDanRepository : INongDanRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<NongDanRepository> _logger;

        public NongDanRepository(IConfiguration config, ILogger<NongDanRepository> logger)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
            _logger = logger;
        }

        public List<NongDanDTO> GetAll()
        {
            var list = new List<NongDanDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT nd.MaNongDan, nd.MaTaiKhoan, nd.HoTen, nd.SoDienThoai, nd.DiaChi,
                           nd.Facebook, nd.TikTok,
                           tk.TenDangNhap, tk.Email, tk.NgayTao
                    FROM NongDan nd
                    LEFT JOIN TaiKhoan tk ON nd.MaTaiKhoan = tk.MaTaiKhoan
                    ORDER BY nd.MaNongDan DESC", conn);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} farmers from database", list.Count);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting all farmers");
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public NongDanDTO? GetById(int id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT nd.MaNongDan, nd.MaTaiKhoan, nd.HoTen, nd.SoDienThoai, nd.DiaChi,
                           nd.Facebook, nd.TikTok,
                           tk.TenDangNhap, tk.Email, tk.NgayTao
                    FROM NongDan nd
                    LEFT JOIN TaiKhoan tk ON nd.MaTaiKhoan = tk.MaTaiKhoan
                    WHERE nd.MaNongDan = @MaNongDan", conn);
                cmd.Parameters.Add("@MaNongDan", SqlDbType.Int).Value = id;

                conn.Open();
                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    _logger.LogWarning("Farmer with ID {FarmerId} not found", id);
                    return null;
                }
                return MapToDTO(reader);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting farmer with ID {FarmerId}", id);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
        }

        public NongDanDTO? GetByAccount(int maTaiKhoan)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT nd.MaNongDan, nd.MaTaiKhoan, nd.HoTen, nd.SoDienThoai, nd.DiaChi,
                           nd.Facebook, nd.TikTok,
                           tk.TenDangNhap, tk.Email, tk.NgayTao
                    FROM NongDan nd
                    LEFT JOIN TaiKhoan tk ON nd.MaTaiKhoan = tk.MaTaiKhoan
                    WHERE nd.MaTaiKhoan = @MaTaiKhoan", conn);
                cmd.Parameters.Add("@MaTaiKhoan", SqlDbType.Int).Value = maTaiKhoan;

                conn.Open();
                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    _logger.LogWarning("Farmer with Account ID {AccountId} not found", maTaiKhoan);
                    return null;
                }
                return MapToDTO(reader);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting farmer with Account ID {AccountId}", maTaiKhoan);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
        }

        public int Create(NongDanCreateDTO dto)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                
                // Tạo tài khoản trước
                using var cmd1 = new SqlCommand(@"
                    INSERT INTO TaiKhoan (TenDangNhap, MatKhau, Email, LoaiTaiKhoan, NgayTao) 
                    OUTPUT INSERTED.MaTaiKhoan 
                    VALUES (@TenDangNhap, @MatKhau, @Email, 'nongdan', @NgayTao)", conn);
                
                cmd1.Parameters.AddWithValue("@TenDangNhap", dto.TenDangNhap);
                cmd1.Parameters.AddWithValue("@MatKhau", dto.MatKhau);
                cmd1.Parameters.AddWithValue("@Email", (object?)dto.Email ?? DBNull.Value);
                cmd1.Parameters.AddWithValue("@NgayTao", DateTime.Now);
                
                var maTaiKhoan = (int)cmd1.ExecuteScalar();
                
                // Tạo nông dân
                using var cmd2 = new SqlCommand(@"
                    INSERT INTO NongDan (MaTaiKhoan, HoTen, SoDienThoai, DiaChi, Facebook, TikTok)
                    OUTPUT INSERTED.MaNongDan 
                    VALUES (@MaTaiKhoan, @HoTen, @SoDienThoai, @DiaChi, @Facebook, @TikTok)", conn);
                
                cmd2.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
                cmd2.Parameters.AddWithValue("@HoTen", (object?)dto.HoTen ?? DBNull.Value);
                cmd2.Parameters.AddWithValue("@SoDienThoai", (object?)dto.SoDienThoai ?? DBNull.Value);
                cmd2.Parameters.AddWithValue("@DiaChi", (object?)dto.DiaChi ?? DBNull.Value);
                cmd2.Parameters.AddWithValue("@Facebook", (object?)dto.Facebook ?? DBNull.Value);
                cmd2.Parameters.AddWithValue("@TikTok", (object?)dto.TikTok ?? DBNull.Value);
                
                var maNongDan = (int)cmd2.ExecuteScalar();
                
                _logger.LogInformation("Created new farmer with ID {FarmerId}", maNongDan);
                return maNongDan;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while creating farmer");
                if (ex.Number == 2627 || ex.Number == 2601)
                    throw new Exception("Tên đăng nhập đã tồn tại trong hệ thống", ex);
                throw new Exception("Lỗi tạo nông dân trong cơ sở dữ liệu: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating farmer");
                throw new Exception("Lỗi tạo nông dân: " + ex.Message, ex);
            }
        }

        public bool Update(int id, NongDanUpdateDTO dto)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                // Bắt đầu transaction để đảm bảo tính toàn vẹn dữ liệu
                using var transaction = conn.BeginTransaction();
                
                try
                {
                    // Cập nhật thông tin nông dân
                    using var cmd1 = new SqlCommand(@"
                        UPDATE NongDan 
                        SET HoTen = @HoTen, SoDienThoai = @SoDienThoai, DiaChi = @DiaChi,
                            Facebook = @Facebook, TikTok = @TikTok
                        WHERE MaNongDan = @MaNongDan", conn, transaction);

                    cmd1.Parameters.Add("@MaNongDan", SqlDbType.Int).Value = id;
                    cmd1.Parameters.Add("@HoTen", SqlDbType.NVarChar, 100).Value = (object?)dto.HoTen ?? DBNull.Value;
                    cmd1.Parameters.Add("@SoDienThoai", SqlDbType.NVarChar, 20).Value = (object?)dto.SoDienThoai ?? DBNull.Value;
                    cmd1.Parameters.Add("@DiaChi", SqlDbType.NVarChar, 255).Value = (object?)dto.DiaChi ?? DBNull.Value;
                    cmd1.Parameters.Add("@Facebook", SqlDbType.NVarChar, 255).Value = (object?)dto.Facebook ?? DBNull.Value;
                    cmd1.Parameters.Add("@TikTok", SqlDbType.NVarChar, 255).Value = (object?)dto.TikTok ?? DBNull.Value;

                    var rowsAffected = cmd1.ExecuteNonQuery();
                    
                    if (rowsAffected == 0)
                    {
                        _logger.LogWarning("No farmer found with ID {FarmerId} to update", id);
                        transaction.Rollback();
                        return false;
                    }

                    // Cập nhật Email trong bảng TaiKhoan nếu có
                    if (!string.IsNullOrWhiteSpace(dto.Email))
                    {
                        using var cmd2 = new SqlCommand(@"
                            UPDATE TaiKhoan 
                            SET Email = @Email 
                            WHERE MaTaiKhoan = (SELECT MaTaiKhoan FROM NongDan WHERE MaNongDan = @MaNongDan)", conn, transaction);

                        cmd2.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value = dto.Email;
                        cmd2.Parameters.Add("@MaNongDan", SqlDbType.Int).Value = id;
                        cmd2.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    _logger.LogInformation("Updated farmer with ID {FarmerId}", id);
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
                _logger.LogError(ex, "SQL error occurred while updating farmer with ID {FarmerId}", id);
                throw new Exception("Lỗi cập nhật nông dân trong cơ sở dữ liệu", ex);
            }
        }

        public bool Delete(int id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("DELETE FROM NongDan WHERE MaNongDan = @MaNongDan", conn);
                cmd.Parameters.Add("@MaNongDan", SqlDbType.Int).Value = id;

                conn.Open();
                var rowsAffected = cmd.ExecuteNonQuery();
                
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Deleted farmer with ID {FarmerId}", id);
                    return true;
                }
                
                _logger.LogWarning("No farmer found with ID {FarmerId} to delete", id);
                return false;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while deleting farmer with ID {FarmerId}", id);
                if (ex.Number == 547)
                    throw new Exception("Không thể xóa nông dân này vì đang có dữ liệu liên quan", ex);
                throw new Exception("Lỗi xóa nông dân trong cơ sở dữ liệu", ex);
            }
        }

        private static NongDanDTO MapToDTO(SqlDataReader reader)
        {
            return new NongDanDTO
            {
                MaNongDan = reader.GetInt32("MaNongDan"),
                MaTaiKhoan = reader.GetInt32("MaTaiKhoan"),
                HoTen = reader.IsDBNull("HoTen") ? null : reader.GetString("HoTen"),
                SoDienThoai = reader.IsDBNull("SoDienThoai") ? null : reader.GetString("SoDienThoai"),
                DiaChi = reader.IsDBNull("DiaChi") ? null : reader.GetString("DiaChi"),
                Facebook = reader.IsDBNull("Facebook") ? null : reader.GetString("Facebook"),
                TikTok = reader.IsDBNull("TikTok") ? null : reader.GetString("TikTok"),
                TenDangNhap = reader.IsDBNull("TenDangNhap") ? null : reader.GetString("TenDangNhap"),
                Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                NgayTao = reader.IsDBNull("NgayTao") ? null : reader.GetDateTime("NgayTao")
            };
        }
    }
}
