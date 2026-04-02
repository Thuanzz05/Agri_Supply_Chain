using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AdminService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaiKhoanController : ControllerBase
    {
        private readonly string _connectionString;

        public TaiKhoanController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        /// <summary>
        /// Lấy danh sách tất cả tài khoản - Cần token admin
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult GetAll([FromQuery] int page = 1, [FromQuery] int limit = 20, [FromQuery] string? loaiTaiKhoan = null)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                var whereClause = string.IsNullOrEmpty(loaiTaiKhoan) ? "" : "WHERE LoaiTaiKhoan = @LoaiTaiKhoan";
                var offset = (page - 1) * limit;

                using var cmd = new SqlCommand($@"
                    SELECT MaTaiKhoan, TenDangNhap, Email, LoaiTaiKhoan, TrangThai, NgayTao, NgayCapNhat
                    FROM TaiKhoan 
                    {whereClause}
                    ORDER BY NgayTao DESC
                    OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY", conn);

                cmd.Parameters.AddWithValue("@Offset", offset);
                cmd.Parameters.AddWithValue("@Limit", limit);
                if (!string.IsNullOrEmpty(loaiTaiKhoan))
                    cmd.Parameters.AddWithValue("@LoaiTaiKhoan", loaiTaiKhoan);

                var result = new List<object>();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new
                    {
                        MaTaiKhoan = (int)reader["MaTaiKhoan"],
                        TenDangNhap = reader["TenDangNhap"].ToString(),
                        Email = reader["Email"].ToString(),
                        LoaiTaiKhoan = reader["LoaiTaiKhoan"].ToString(),
                        TrangThai = reader["TrangThai"].ToString(),
                        NgayTao = reader["NgayTao"],
                        NgayCapNhat = reader["NgayCapNhat"] != DBNull.Value ? reader["NgayCapNhat"] : null
                    });
                }

                // Đếm tổng số
                reader.Close();
                using var countCmd = new SqlCommand($"SELECT COUNT(*) FROM TaiKhoan {whereClause}", conn);
                if (!string.IsNullOrEmpty(loaiTaiKhoan))
                    countCmd.Parameters.AddWithValue("@LoaiTaiKhoan", loaiTaiKhoan);
                var total = (int)countCmd.ExecuteScalar();

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách tài khoản thành công",
                    data = result,
                    pagination = new
                    {
                        page,
                        limit,
                        total,
                        totalPages = (int)Math.Ceiling((double)total / limit)
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy thông tin tài khoản theo ID - Cần token admin
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult GetById(int id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                using var cmd = new SqlCommand(@"
                    SELECT MaTaiKhoan, TenDangNhap, Email, LoaiTaiKhoan, TrangThai, NgayTao, NgayCapNhat
                    FROM TaiKhoan 
                    WHERE MaTaiKhoan = @Id", conn);

                cmd.Parameters.AddWithValue("@Id", id);

                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy tài khoản"
                    });
                }

                var result = new
                {
                    MaTaiKhoan = (int)reader["MaTaiKhoan"],
                    TenDangNhap = reader["TenDangNhap"].ToString(),
                    Email = reader["Email"].ToString(),
                    LoaiTaiKhoan = reader["LoaiTaiKhoan"].ToString(),
                    TrangThai = reader["TrangThai"].ToString(),
                    NgayTao = reader["NgayTao"],
                    NgayCapNhat = reader["NgayCapNhat"] != DBNull.Value ? reader["NgayCapNhat"] : null
                };

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin tài khoản thành công",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Tạo tài khoản mới - Cần token admin
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Create([FromBody] CreateTaiKhoanRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.TenDangNhap) || string.IsNullOrEmpty(request.MatKhau))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Tên đăng nhập và mật khẩu không được để trống"
                    });
                }

                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                // Kiểm tra tên đăng nhập đã tồn tại
                using var checkCmd = new SqlCommand("SELECT COUNT(*) FROM TaiKhoan WHERE TenDangNhap = @TenDangNhap", conn);
                checkCmd.Parameters.AddWithValue("@TenDangNhap", request.TenDangNhap);
                if ((int)checkCmd.ExecuteScalar() > 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Tên đăng nhập đã tồn tại"
                    });
                }

                // Tạo tài khoản mới
                using var cmd = new SqlCommand(@"
                    INSERT INTO TaiKhoan (TenDangNhap, MatKhau, Email, LoaiTaiKhoan, TrangThai, NgayTao)
                    OUTPUT INSERTED.MaTaiKhoan
                    VALUES (@TenDangNhap, @MatKhau, @Email, @LoaiTaiKhoan, N'hoat_dong', SYSDATETIME())", conn);

                cmd.Parameters.AddWithValue("@TenDangNhap", request.TenDangNhap);
                cmd.Parameters.AddWithValue("@MatKhau", request.MatKhau);
                cmd.Parameters.AddWithValue("@Email", request.Email ?? "");
                cmd.Parameters.AddWithValue("@LoaiTaiKhoan", request.LoaiTaiKhoan);

                var newId = (int)cmd.ExecuteScalar();

                return Ok(new
                {
                    success = true,
                    message = "Tạo tài khoản thành công",
                    data = new { MaTaiKhoan = newId }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Cập nhật thông tin tài khoản - Cần token admin
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Update(int id, [FromBody] UpdateTaiKhoanRequest request)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                // Kiểm tra tài khoản tồn tại
                using var checkCmd = new SqlCommand("SELECT COUNT(*) FROM TaiKhoan WHERE MaTaiKhoan = @Id", conn);
                checkCmd.Parameters.AddWithValue("@Id", id);
                if ((int)checkCmd.ExecuteScalar() == 0)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy tài khoản"
                    });
                }

                // Cập nhật thông tin
                using var cmd = new SqlCommand(@"
                    UPDATE TaiKhoan 
                    SET Email = @Email, 
                        LoaiTaiKhoan = @LoaiTaiKhoan,
                        TrangThai = @TrangThai,
                        NgayCapNhat = SYSDATETIME()
                    WHERE MaTaiKhoan = @Id", conn);

                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Email", request.Email ?? "");
                cmd.Parameters.AddWithValue("@LoaiTaiKhoan", request.LoaiTaiKhoan);
                cmd.Parameters.AddWithValue("@TrangThai", request.TrangThai);

                cmd.ExecuteNonQuery();

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật tài khoản thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Đổi mật khẩu tài khoản - Cần token admin
        /// </summary>
        [HttpPut("{id}/change-password")]
        [Authorize(Roles = "admin")]
        public IActionResult ChangePassword(int id, [FromBody] ChangePasswordRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.MatKhauMoi))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Mật khẩu mới không được để trống"
                    });
                }

                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                using var cmd = new SqlCommand(@"
                    UPDATE TaiKhoan 
                    SET MatKhau = @MatKhauMoi, NgayCapNhat = SYSDATETIME()
                    WHERE MaTaiKhoan = @Id", conn);

                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@MatKhauMoi", request.MatKhauMoi);

                var rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy tài khoản"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Đổi mật khẩu thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Khóa/Mở khóa tài khoản - Cần token admin
        /// </summary>
        [HttpPut("{id}/toggle-status")]
        [Authorize(Roles = "admin")]
        public IActionResult ToggleStatus(int id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                using var cmd = new SqlCommand(@"
                    UPDATE TaiKhoan 
                    SET TrangThai = CASE 
                        WHEN TrangThai = N'hoat_dong' THEN N'tam_khoa'
                        ELSE N'hoat_dong'
                    END,
                    NgayCapNhat = SYSDATETIME()
                    WHERE MaTaiKhoan = @Id", conn);

                cmd.Parameters.AddWithValue("@Id", id);

                var rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy tài khoản"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Thay đổi trạng thái tài khoản thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Xóa tài khoản - Cần token admin
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                using var cmd = new SqlCommand("DELETE FROM TaiKhoan WHERE MaTaiKhoan = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);

                var rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy tài khoản"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Xóa tài khoản thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server: " + ex.Message
                });
            }
        }
    }

    // DTOs
    public class CreateTaiKhoanRequest
    {
        public string TenDangNhap { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string LoaiTaiKhoan { get; set; } = string.Empty; // admin, nongdan, daily, sieuthi
    }

    public class UpdateTaiKhoanRequest
    {
        public string? Email { get; set; }
        public string LoaiTaiKhoan { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty; // hoat_dong, tam_khoa
    }

    public class ChangePasswordRequest
    {
        public string MatKhauMoi { get; set; } = string.Empty;
    }
}