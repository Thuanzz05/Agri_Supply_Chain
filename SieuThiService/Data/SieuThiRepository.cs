using Microsoft.Data.SqlClient;
using SieuThiService.Models.DTOs;
using System.Data;

namespace SieuThiService.Data
{
    public class SieuThiRepository : ISieuThiRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<SieuThiRepository> _logger;

        public SieuThiRepository(IConfiguration config, ILogger<SieuThiRepository> logger)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
            _logger = logger;
        }

        public List<SieuThiDTO> GetAll()
        {
            var list = new List<SieuThiDTO>();
            try
            {
                _logger.LogInformation("Starting to retrieve all supermarket records");
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT st.MaSieuThi, st.MaTaiKhoan, st.TenSieuThi, st.SoDienThoai, st.DiaChi,
                           tk.TenDangNhap, tk.NgayTao
                    FROM SieuThi st
                    LEFT JOIN TaiKhoan tk ON st.MaTaiKhoan = tk.MaTaiKhoan
                    ORDER BY st.MaSieuThi", conn);

                conn.Open();
                _logger.LogInformation("Database connection opened successfully");
                
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} supermarket records from database", list.Count);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting all supermarket records. Error: {ErrorMessage}, Number: {ErrorNumber}", ex.Message, ex.Number);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General error occurred while getting all supermarket records: {ErrorMessage}", ex.Message);
                throw;
            }
            return list;
        }

        public SieuThiDTO? GetById(int maSieuThi)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT st.MaSieuThi, st.MaTaiKhoan, st.TenSieuThi, st.SoDienThoai, st.DiaChi,
                           tk.TenDangNhap, tk.NgayTao
                    FROM SieuThi st
                    LEFT JOIN TaiKhoan tk ON st.MaTaiKhoan = tk.MaTaiKhoan
                    WHERE st.MaSieuThi = @MaSieuThi", conn);
                
                cmd.Parameters.AddWithValue("@MaSieuThi", maSieuThi);

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
                _logger.LogError(ex, "SQL error occurred while getting supermarket by ID {SupermarketId}", maSieuThi);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
        }

        public SieuThiDTO? GetByTaiKhoan(int maTaiKhoan)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT st.MaSieuThi, st.MaTaiKhoan, st.TenSieuThi, st.SoDienThoai, st.DiaChi,
                           tk.TenDangNhap, tk.NgayTao
                    FROM SieuThi st
                    LEFT JOIN TaiKhoan tk ON st.MaTaiKhoan = tk.MaTaiKhoan
                    WHERE st.MaTaiKhoan = @MaTaiKhoan", conn);
                
                cmd.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);

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
                _logger.LogError(ex, "SQL error occurred while getting supermarket by account ID {AccountId}", maTaiKhoan);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
        }
        public bool Create(SieuThiCreateDTO sieuThiDto)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var transaction = conn.BeginTransaction();
            
            try
            {
                // Tạo tài khoản trước
                using var cmdAccount = new SqlCommand(@"
                    INSERT INTO TaiKhoan (TenDangNhap, MatKhau, LoaiTaiKhoan, NgayTao) 
                    OUTPUT INSERTED.MaTaiKhoan
                    VALUES (@TenDangNhap, @MatKhau, @LoaiTaiKhoan, @NgayTao)", conn, transaction);

                cmdAccount.Parameters.AddWithValue("@TenDangNhap", sieuThiDto.TenDangNhap);
                cmdAccount.Parameters.AddWithValue("@MatKhau", BCrypt.Net.BCrypt.HashPassword(sieuThiDto.MatKhau));
                cmdAccount.Parameters.AddWithValue("@LoaiTaiKhoan", "sieuthi");
                cmdAccount.Parameters.AddWithValue("@NgayTao", DateTime.Now);

                var maTaiKhoan = (int)cmdAccount.ExecuteScalar();

                // Tạo siêu thị
                using var cmdSupermarket = new SqlCommand(@"
                    INSERT INTO SieuThi (MaTaiKhoan, TenSieuThi, SoDienThoai, DiaChi) 
                    VALUES (@MaTaiKhoan, @TenSieuThi, @SoDienThoai, @DiaChi)", conn, transaction);

                cmdSupermarket.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
                cmdSupermarket.Parameters.AddWithValue("@TenSieuThi", sieuThiDto.TenSieuThi);
                cmdSupermarket.Parameters.AddWithValue("@SoDienThoai", (object?)sieuThiDto.SoDienThoai ?? DBNull.Value);
                cmdSupermarket.Parameters.AddWithValue("@DiaChi", (object?)sieuThiDto.DiaChi ?? DBNull.Value);

                var rowsAffected = cmdSupermarket.ExecuteNonQuery();
                
                transaction.Commit();
                _logger.LogInformation("Created supermarket with account ID {AccountId}", maTaiKhoan);
                return rowsAffected > 0;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "SQL error occurred while creating supermarket");
                if (ex.Number == 2627 || ex.Number == 2601) // Unique constraint
                    throw new Exception("Tên đăng nhập hoặc email đã tồn tại trong hệ thống", ex);
                throw new Exception("Lỗi tạo siêu thị trong cơ sở dữ liệu: " + ex.Message, ex);
            }
        }

        public bool Update(int maSieuThi, SieuThiUpdateDTO sieuThiDto)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    UPDATE SieuThi 
                    SET TenSieuThi = @TenSieuThi, SoDienThoai = @SoDienThoai, DiaChi = @DiaChi
                    WHERE MaSieuThi = @MaSieuThi", conn);

                cmd.Parameters.AddWithValue("@MaSieuThi", maSieuThi);
                cmd.Parameters.AddWithValue("@TenSieuThi", sieuThiDto.TenSieuThi);
                cmd.Parameters.AddWithValue("@SoDienThoai", (object?)sieuThiDto.SoDienThoai ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DiaChi", (object?)sieuThiDto.DiaChi ?? DBNull.Value);

                conn.Open();
                var rowsAffected = cmd.ExecuteNonQuery();
                
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Updated supermarket {SupermarketId}", maSieuThi);
                    return true;
                }
                
                return false;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while updating supermarket {SupermarketId}", maSieuThi);
                throw new Exception("Lỗi cập nhật siêu thị trong cơ sở dữ liệu", ex);
            }
        }

        public bool Delete(int maSieuThi)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("DELETE FROM SieuThi WHERE MaSieuThi = @MaSieuThi", conn);
                cmd.Parameters.AddWithValue("@MaSieuThi", maSieuThi);

                conn.Open();
                var rowsAffected = cmd.ExecuteNonQuery();
                
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Deleted supermarket {SupermarketId}", maSieuThi);
                    return true;
                }
                
                return false;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while deleting supermarket {SupermarketId}", maSieuThi);
                if (ex.Number == 547) // Foreign key constraint
                    throw new Exception("Không thể xóa siêu thị vì có dữ liệu liên quan", ex);
                throw new Exception("Lỗi xóa siêu thị trong cơ sở dữ liệu", ex);
            }
        }

        public bool ExistsByTenDangNhap(string tenDangNhap)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SELECT COUNT(1) FROM TaiKhoan WHERE TenDangNhap = @TenDangNhap", conn);
                cmd.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);

                conn.Open();
                var count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while checking username existence");
                throw new Exception("Lỗi kiểm tra tên đăng nhập", ex);
            }
        }

        public bool ExistsByEmail(string email)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SELECT COUNT(1) FROM TaiKhoan WHERE Email = @Email", conn);
                cmd.Parameters.AddWithValue("@Email", email);

                conn.Open();
                var count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while checking email existence");
                throw new Exception("Lỗi kiểm tra email", ex);
            }
        }

        private static SieuThiDTO MapToDTO(SqlDataReader reader)
        {
            return new SieuThiDTO
            {
                MaSieuThi = reader.GetInt32("MaSieuThi"),
                MaTaiKhoan = reader.GetInt32("MaTaiKhoan"),
                TenSieuThi = reader.GetString("TenSieuThi"),
                SoDienThoai = reader.IsDBNull("SoDienThoai") ? null : reader.GetString("SoDienThoai"),
                DiaChi = reader.IsDBNull("DiaChi") ? null : reader.GetString("DiaChi"),
                TenDangNhap = reader.IsDBNull("TenDangNhap") ? null : reader.GetString("TenDangNhap"),
                Email = null, // Bảng TaiKhoan không có cột Email
                NgayTao = reader.IsDBNull("NgayTao") ? null : reader.GetDateTime("NgayTao")
            };
        }
    }
}