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
                using var cmd = new SqlCommand("SELECT MaNongDan, MaTaiKhoan, HoTen, SoDienThoai, DiaChi FROM NongDan", conn);

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
                using var cmd = new SqlCommand("SELECT MaNongDan, MaTaiKhoan, HoTen, SoDienThoai, DiaChi FROM NongDan WHERE MaNongDan = @MaNongDan", conn);
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
                using var cmd = new SqlCommand("SELECT MaNongDan, MaTaiKhoan, HoTen, SoDienThoai, DiaChi FROM NongDan WHERE MaTaiKhoan = @MaTaiKhoan", conn);
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
                    INSERT INTO TaiKhoan (TenDangNhap, MatKhau, LoaiTaiKhoan) 
                    OUTPUT INSERTED.MaTaiKhoan 
                    VALUES (@TenDangNhap, @MatKhau, 'nongdan')", conn);
                
                cmd1.Parameters.AddWithValue("@TenDangNhap", dto.TenDangNhap);
                cmd1.Parameters.AddWithValue("@MatKhau", dto.MatKhau);
                
                var maTaiKhoan = (int)cmd1.ExecuteScalar();
                
                // Tạo nông dân
                using var cmd2 = new SqlCommand(@"
                    INSERT INTO NongDan (MaTaiKhoan, HoTen, SoDienThoai, DiaChi) 
                    OUTPUT INSERTED.MaNongDan 
                    VALUES (@MaTaiKhoan, @HoTen, @SoDienThoai, @DiaChi)", conn);
                
                cmd2.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
                cmd2.Parameters.AddWithValue("@HoTen", (object?)dto.HoTen ?? DBNull.Value);
                cmd2.Parameters.AddWithValue("@SoDienThoai", (object?)dto.SoDienThoai ?? DBNull.Value);
                cmd2.Parameters.AddWithValue("@DiaChi", (object?)dto.DiaChi ?? DBNull.Value);
                
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
                using var cmd = new SqlCommand(@"
                    UPDATE NongDan 
                    SET HoTen = @HoTen, SoDienThoai = @SoDienThoai, DiaChi = @DiaChi 
                    WHERE MaNongDan = @MaNongDan", conn);

                cmd.Parameters.Add("@MaNongDan", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@HoTen", SqlDbType.NVarChar, 100).Value = (object?)dto.HoTen ?? DBNull.Value;
                cmd.Parameters.Add("@SoDienThoai", SqlDbType.NVarChar, 20).Value = (object?)dto.SoDienThoai ?? DBNull.Value;
                cmd.Parameters.Add("@DiaChi", SqlDbType.NVarChar, 255).Value = (object?)dto.DiaChi ?? DBNull.Value;

                conn.Open();
                var rowsAffected = cmd.ExecuteNonQuery();
                
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Updated farmer with ID {FarmerId}", id);
                    return true;
                }
                
                _logger.LogWarning("No farmer found with ID {FarmerId} to update", id);
                return false;
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
                DiaChi = reader.IsDBNull("DiaChi") ? null : reader.GetString("DiaChi")
            };
        }
    }
}