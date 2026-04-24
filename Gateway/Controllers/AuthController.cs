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
        /// Đăng ký tài khoản mới
        /// </summary>
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            try
            {
                // Validation
                if (string.IsNullOrEmpty(request.TenDangNhap) || string.IsNullOrEmpty(request.MatKhau))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Tên đăng nhập và mật khẩu không được để trống"
                    });
                }

                if (string.IsNullOrEmpty(request.Email))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Email không được để trống"
                    });
                }

                if (string.IsNullOrEmpty(request.LoaiTaiKhoan) || 
                    (request.LoaiTaiKhoan != "nongdan" && request.LoaiTaiKhoan != "daily" && request.LoaiTaiKhoan != "sieuthi"))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Loại tài khoản không hợp lệ"
                    });
                }

                // Kiểm tra tên đăng nhập đã tồn tại
                if (CheckUsernameExists(request.TenDangNhap))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Tên đăng nhập đã tồn tại"
                    });
                }

                // Kiểm tra email đã tồn tại
                if (CheckEmailExists(request.Email))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Email đã được sử dụng"
                    });
                }

                // Tạo tài khoản
                var maTaiKhoan = CreateAccount(request);

                return Ok(new
                {
                    success = true,
                    message = "Đăng ký tài khoản thành công",
                    data = new { MaTaiKhoan = maTaiKhoan }
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
                        MaNongDan = user.MaNongDan,
                        MaDaiLy = user.MaDaiLy,
                        MaSieuThi = user.MaSieuThi,
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
                SELECT tk.MaTaiKhoan, tk.TenDangNhap, tk.LoaiTaiKhoan,
                       nd.MaNongDan, dl.MaDaiLy, st.MaSieuThi
                FROM TaiKhoan tk
                LEFT JOIN NongDan nd ON tk.MaTaiKhoan = nd.MaTaiKhoan
                LEFT JOIN DaiLy dl ON tk.MaTaiKhoan = dl.MaTaiKhoan
                LEFT JOIN SieuThi st ON tk.MaTaiKhoan = st.MaTaiKhoan
                WHERE tk.TenDangNhap = @TenDangNhap AND tk.MatKhau = @MatKhau AND tk.TrangThai = N'hoat_dong'", conn);

            cmd.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
            cmd.Parameters.AddWithValue("@MatKhau", matKhau);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new UserInfo
            {
                MaTaiKhoan = (int)reader["MaTaiKhoan"],
                TenDangNhap = reader["TenDangNhap"].ToString()!,
                LoaiTaiKhoan = reader["LoaiTaiKhoan"].ToString()!,
                MaNongDan = reader["MaNongDan"] != DBNull.Value ? (int?)reader["MaNongDan"] : null,
                MaDaiLy = reader["MaDaiLy"] != DBNull.Value ? (int?)reader["MaDaiLy"] : null,
                MaSieuThi = reader["MaSieuThi"] != DBNull.Value ? (int?)reader["MaSieuThi"] : null
            };
        }

        private bool CheckUsernameExists(string tenDangNhap)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM TaiKhoan WHERE TenDangNhap = @TenDangNhap", conn);
            cmd.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
            
            conn.Open();
            var count = (int)cmd.ExecuteScalar();
            return count > 0;
        }

        private bool CheckEmailExists(string email)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM TaiKhoan WHERE Email = @Email", conn);
            cmd.Parameters.AddWithValue("@Email", email);
            
            conn.Open();
            var count = (int)cmd.ExecuteScalar();
            return count > 0;
        }

        private int CreateAccount(RegisterRequest request)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var transaction = conn.BeginTransaction();

            try
            {
                // 1. Tạo tài khoản
                var cmdAccount = new SqlCommand(@"
                    INSERT INTO TaiKhoan (TenDangNhap, MatKhau, Email, LoaiTaiKhoan, TrangThai, NgayTao)
                    VALUES (@TenDangNhap, @MatKhau, @Email, @LoaiTaiKhoan, N'hoat_dong', GETDATE());
                    SELECT CAST(SCOPE_IDENTITY() as int);", conn, transaction);

                cmdAccount.Parameters.AddWithValue("@TenDangNhap", request.TenDangNhap);
                cmdAccount.Parameters.AddWithValue("@MatKhau", request.MatKhau);
                cmdAccount.Parameters.AddWithValue("@Email", request.Email);
                cmdAccount.Parameters.AddWithValue("@LoaiTaiKhoan", request.LoaiTaiKhoan);

                var maTaiKhoan = (int)cmdAccount.ExecuteScalar();

                // 2. Tạo bản ghi tương ứng theo loại tài khoản
                if (request.LoaiTaiKhoan == "nongdan")
                {
                    var cmdFarmer = new SqlCommand(@"
                        INSERT INTO NongDan (MaTaiKhoan, HoTen, SoDienThoai, DiaChi)
                        VALUES (@MaTaiKhoan, @HoTen, @SoDienThoai, @DiaChi)", conn, transaction);
                    
                    cmdFarmer.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
                    cmdFarmer.Parameters.AddWithValue("@HoTen", request.HoTen ?? "");
                    cmdFarmer.Parameters.AddWithValue("@SoDienThoai", request.SoDienThoai ?? "");
                    cmdFarmer.Parameters.AddWithValue("@DiaChi", request.DiaChi ?? "");
                    cmdFarmer.ExecuteNonQuery();
                }
                else if (request.LoaiTaiKhoan == "daily")
                {
                    var cmdAgent = new SqlCommand(@"
                        INSERT INTO DaiLy (MaTaiKhoan, TenDaiLy, SoDienThoai, DiaChi)
                        VALUES (@MaTaiKhoan, @TenDaiLy, @SoDienThoai, @DiaChi)", conn, transaction);
                    
                    cmdAgent.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
                    cmdAgent.Parameters.AddWithValue("@TenDaiLy", request.HoTen ?? "");
                    cmdAgent.Parameters.AddWithValue("@SoDienThoai", request.SoDienThoai ?? "");
                    cmdAgent.Parameters.AddWithValue("@DiaChi", request.DiaChi ?? "");
                    cmdAgent.ExecuteNonQuery();
                }
                else if (request.LoaiTaiKhoan == "sieuthi")
                {
                    var cmdSupermarket = new SqlCommand(@"
                        INSERT INTO SieuThi (MaTaiKhoan, TenSieuThi, SoDienThoai, DiaChi)
                        VALUES (@MaTaiKhoan, @TenSieuThi, @SoDienThoai, @DiaChi)", conn, transaction);
                    
                    cmdSupermarket.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
                    cmdSupermarket.Parameters.AddWithValue("@TenSieuThi", request.HoTen ?? "");
                    cmdSupermarket.Parameters.AddWithValue("@SoDienThoai", request.SoDienThoai ?? "");
                    cmdSupermarket.Parameters.AddWithValue("@DiaChi", request.DiaChi ?? "");
                    cmdSupermarket.ExecuteNonQuery();
                }

                transaction.Commit();
                return maTaiKhoan;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    public class LoginRequest
    {
        public string TenDangNhap { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string TenDangNhap { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string LoaiTaiKhoan { get; set; } = string.Empty;
        public string? HoTen { get; set; }
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }
    }
}