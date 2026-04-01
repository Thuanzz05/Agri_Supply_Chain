using Gateway.Helpers;
using Gateway.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Gateway.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly string _connectionString;

        public AuthController(IOptions<AppSettings> appSettings, IConfiguration configuration)
        {
            _appSettings = appSettings.Value;
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        /// <summary>
        /// Đăng nhập đơn giản
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
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

                // Lấy thông tin user từ database 
                var user = GetUser(request.TenDangNhap, request.MatKhau);
                if (user == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Tên đăng nhập hoặc mật khẩu không đúng"
                    });
                }

                // Tạo JWT token
                var token = GenerateJwtToken(user);

                return Ok(new
                {
                    success = true,
                    message = "Đăng nhập thành công",
                    data = new
                    {
                        MaTaiKhoan = user.MaTaiKhoan,
                        TenDangNhap = user.TenDangNhap,
                        LoaiTaiKhoan = user.LoaiTaiKhoan,
                        Token = token
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
        /// Lấy thông tin user hiện tại từ token
        /// </summary>
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                var usernameClaim = User.FindFirst(ClaimTypes.Name);
                var roleClaim = User.FindFirst(ClaimTypes.Role);

                if (userIdClaim == null)
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Token không hợp lệ"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        MaTaiKhoan = int.Parse(userIdClaim.Value),
                        TenDangNhap = usernameClaim?.Value,
                        LoaiTaiKhoan = roleClaim?.Value
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

        private string GenerateJwtToken(UserInfo user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.MaTaiKhoan.ToString()),
                    new Claim(ClaimTypes.Name, user.TenDangNhap),
                    new Claim(ClaimTypes.Role, user.LoaiTaiKhoan)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private UserInfo? GetUser(string tenDangNhap, string matKhau)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(@"
                SELECT MaTaiKhoan, TenDangNhap, LoaiTaiKhoan
                FROM TaiKhoan 
                WHERE TenDangNhap = @TenDangNhap AND MatKhau = @MatKhau AND TrangThai = N'hoat_dong'", conn);

            cmd.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
            cmd.Parameters.AddWithValue("@MatKhau", matKhau);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new UserInfo
            {
                MaTaiKhoan = (int)reader["MaTaiKhoan"],
                TenDangNhap = reader["TenDangNhap"].ToString()!,
                LoaiTaiKhoan = reader["LoaiTaiKhoan"].ToString()!
            };
        }
    }

    public class LoginRequest
    {
        public string TenDangNhap { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
    }
}