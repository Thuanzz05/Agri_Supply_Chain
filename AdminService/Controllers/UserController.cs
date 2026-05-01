using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AdminService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly string _connectionString;

        public UserController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        /// <summary>
        /// Lấy danh sách tất cả người dùng - Cần token admin
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult GetAllUsers([FromQuery] int page = 1, [FromQuery] int limit = 20, [FromQuery] string? loaiNguoiDung = null)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                var offset = (page - 1) * limit;
                var result = new List<object>();

                // Lấy danh sách nông dân
                if (string.IsNullOrEmpty(loaiNguoiDung) || loaiNguoiDung == "nongdan")
                {
                    using var cmdNongDan = new SqlCommand(@"
                        SELECT 
                            nd.MaNongDan as Id,
                            tk.TenDangNhap,
                            tk.Email,
                            nd.HoTen,
                            nd.SoDienThoai,
                            nd.DiaChi,
                            nd.Facebook,
                            nd.TikTok,
                            nd.NgayTao,
                            tk.TrangThai,
                            'nongdan' as LoaiNguoiDung
                        FROM NongDan nd
                        INNER JOIN TaiKhoan tk ON nd.MaTaiKhoan = tk.MaTaiKhoan
                        ORDER BY nd.NgayTao DESC", conn);

                    using var reader = cmdNongDan.ExecuteReader();
                    while (reader.Read())
                    {
                        result.Add(new
                        {
                            Id = (int)reader["Id"],
                            TenDangNhap = reader["TenDangNhap"].ToString(),
                            Email = reader["Email"].ToString(),
                            HoTen = reader["HoTen"].ToString(),
                            SoDienThoai = reader["SoDienThoai"].ToString(),
                            DiaChi = reader["DiaChi"].ToString(),
                            facebook = reader["Facebook"] != DBNull.Value ? reader["Facebook"].ToString() : null,
                            tiktok = reader["TikTok"] != DBNull.Value ? reader["TikTok"].ToString() : null,
                            NgayTao = reader["NgayTao"],
                            TrangThai = reader["TrangThai"].ToString(),
                            LoaiNguoiDung = reader["LoaiNguoiDung"].ToString()
                        });
                    }
                }

                // Lấy danh sách đại lý
                if (string.IsNullOrEmpty(loaiNguoiDung) || loaiNguoiDung == "daily")
                {
                    using var cmdDaiLy = new SqlCommand(@"
                        SELECT 
                            dl.MaDaiLy as Id,
                            tk.TenDangNhap,
                            tk.Email,
                            dl.TenDaiLy as HoTen,
                            dl.SoDienThoai,
                            dl.DiaChi,
                            dl.Facebook,
                            dl.TikTok,
                            dl.NgayTao,
                            tk.TrangThai,
                            'daily' as LoaiNguoiDung
                        FROM DaiLy dl
                        INNER JOIN TaiKhoan tk ON dl.MaTaiKhoan = tk.MaTaiKhoan
                        ORDER BY dl.NgayTao DESC", conn);

                    using var reader = cmdDaiLy.ExecuteReader();
                    while (reader.Read())
                    {
                        result.Add(new
                        {
                            Id = (int)reader["Id"],
                            TenDangNhap = reader["TenDangNhap"].ToString(),
                            Email = reader["Email"].ToString(),
                            HoTen = reader["HoTen"].ToString(),
                            SoDienThoai = reader["SoDienThoai"].ToString(),
                            DiaChi = reader["DiaChi"].ToString(),
                            facebook = reader["Facebook"] != DBNull.Value ? reader["Facebook"].ToString() : null,
                            tiktok = reader["TikTok"] != DBNull.Value ? reader["TikTok"].ToString() : null,
                            NgayTao = reader["NgayTao"],
                            TrangThai = reader["TrangThai"].ToString(),
                            LoaiNguoiDung = reader["LoaiNguoiDung"].ToString()
                        });
                    }
                }

                // Lấy danh sách siêu thị
                if (string.IsNullOrEmpty(loaiNguoiDung) || loaiNguoiDung == "sieuthi")
                {
                    using var cmdSieuThi = new SqlCommand(@"
                        SELECT 
                            st.MaSieuThi as Id,
                            tk.TenDangNhap,
                            tk.Email,
                            st.TenSieuThi as HoTen,
                            st.SoDienThoai,
                            st.DiaChi,
                            st.Facebook,
                            st.TikTok,
                            st.NgayTao,
                            tk.TrangThai,
                            'sieuthi' as LoaiNguoiDung
                        FROM SieuThi st
                        INNER JOIN TaiKhoan tk ON st.MaTaiKhoan = tk.MaTaiKhoan
                        ORDER BY st.NgayTao DESC", conn);

                    using var reader = cmdSieuThi.ExecuteReader();
                    while (reader.Read())
                    {
                        result.Add(new
                        {
                            Id = (int)reader["Id"],
                            TenDangNhap = reader["TenDangNhap"].ToString(),
                            Email = reader["Email"].ToString(),
                            HoTen = reader["HoTen"].ToString(),
                            SoDienThoai = reader["SoDienThoai"].ToString(),
                            DiaChi = reader["DiaChi"].ToString(),
                            facebook = reader["Facebook"] != DBNull.Value ? reader["Facebook"].ToString() : null,
                            tiktok = reader["TikTok"] != DBNull.Value ? reader["TikTok"].ToString() : null,
                            NgayTao = reader["NgayTao"],
                            TrangThai = reader["TrangThai"].ToString(),
                            LoaiNguoiDung = reader["LoaiNguoiDung"].ToString()
                        });
                    }
                }

                // Phân trang
                var pagedResult = result.Skip(offset).Take(limit).ToList();

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách người dùng thành công",
                    data = pagedResult,
                    pagination = new
                    {
                        page,
                        limit,
                        total = result.Count,
                        totalPages = (int)Math.Ceiling((double)result.Count / limit)
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
        /// Lấy thông tin chi tiết nông dân - Cần token admin
        /// </summary>
        [HttpGet("nongdan/{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult GetNongDanDetail(int id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                using var cmd = new SqlCommand(@"
                    SELECT 
                        nd.MaNongDan,
                        nd.MaTaiKhoan,
                        tk.TenDangNhap,
                        tk.Email,
                        tk.TrangThai,
                        nd.HoTen,
                        nd.SoDienThoai,
                        nd.DiaChi,
                        nd.Facebook,
                        nd.TikTok,
                        nd.NgayTao,
                        nd.NgayCapNhat,
                        COUNT(tt.MaTrangTrai) as SoTrangTrai,
                        COUNT(lo.MaLo) as SoLoNongSan
                    FROM NongDan nd
                    INNER JOIN TaiKhoan tk ON nd.MaTaiKhoan = tk.MaTaiKhoan
                    LEFT JOIN TrangTrai tt ON nd.MaNongDan = tt.MaNongDan
                    LEFT JOIN LoNongSan lo ON tt.MaTrangTrai = lo.MaTrangTrai
                    WHERE nd.MaNongDan = @Id
                    GROUP BY nd.MaNongDan, nd.MaTaiKhoan, tk.TenDangNhap, tk.Email, tk.TrangThai, 
                             nd.HoTen, nd.SoDienThoai, nd.DiaChi, nd.Facebook, nd.TikTok,
                             nd.NgayTao, nd.NgayCapNhat", conn);

                cmd.Parameters.AddWithValue("@Id", id);

                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy nông dân"
                    });
                }

                var result = new
                {
                    MaNongDan = (int)reader["MaNongDan"],
                    MaTaiKhoan = (int)reader["MaTaiKhoan"],
                    TenDangNhap = reader["TenDangNhap"].ToString(),
                    Email = reader["Email"].ToString(),
                    TrangThai = reader["TrangThai"].ToString(),
                    HoTen = reader["HoTen"].ToString(),
                    SoDienThoai = reader["SoDienThoai"].ToString(),
                    DiaChi = reader["DiaChi"].ToString(),
                    facebook = reader["Facebook"] != DBNull.Value ? reader["Facebook"].ToString() : null,
                    tiktok = reader["TikTok"] != DBNull.Value ? reader["TikTok"].ToString() : null,
                    NgayTao = reader["NgayTao"],
                    NgayCapNhat = reader["NgayCapNhat"] != DBNull.Value ? reader["NgayCapNhat"] : null,
                    SoTrangTrai = (int)reader["SoTrangTrai"],
                    SoLoNongSan = (int)reader["SoLoNongSan"]
                };

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin nông dân thành công",
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
        /// Lấy thông tin chi tiết đại lý - Cần token admin
        /// </summary>
        [HttpGet("daily/{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult GetDaiLyDetail(int id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                using var cmd = new SqlCommand(@"
                    SELECT 
                        dl.MaDaiLy,
                        dl.MaTaiKhoan,
                        tk.TenDangNhap,
                        tk.Email,
                        tk.TrangThai,
                        dl.TenDaiLy,
                        dl.SoDienThoai,
                        dl.DiaChi,
                        dl.Facebook,
                        dl.TikTok,
                        dl.NgayTao,
                        dl.NgayCapNhat,
                        COUNT(DISTINCT dh1.MaDonHang) as SoDonHangNhan,
                        COUNT(DISTINCT dh2.MaDonHang) as SoDonHangBan,
                        COUNT(DISTINCT kd.MaKiemDinh) as SoKiemDinh
                    FROM DaiLy dl
                    INNER JOIN TaiKhoan tk ON dl.MaTaiKhoan = tk.MaTaiKhoan
                    LEFT JOIN DonHang dh1 ON dl.MaDaiLy = dh1.MaNguoiMua AND dh1.LoaiDon = 'nongdan_to_daily'
                    LEFT JOIN DonHang dh2 ON dl.MaDaiLy = dh2.MaNguoiBan AND dh2.LoaiDon = 'daily_to_sieuthi'
                    LEFT JOIN KiemDinh kd ON kd.NguoiKiemDinh = tk.TenDangNhap
                    WHERE dl.MaDaiLy = @Id
                    GROUP BY dl.MaDaiLy, dl.MaTaiKhoan, tk.TenDangNhap, tk.Email, tk.TrangThai,
                             dl.TenDaiLy, dl.SoDienThoai, dl.DiaChi, dl.Facebook, dl.TikTok,
                             dl.NgayTao, dl.NgayCapNhat", conn);

                cmd.Parameters.AddWithValue("@Id", id);

                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy đại lý"
                    });
                }

                var result = new
                {
                    MaDaiLy = (int)reader["MaDaiLy"],
                    MaTaiKhoan = (int)reader["MaTaiKhoan"],
                    TenDangNhap = reader["TenDangNhap"].ToString(),
                    Email = reader["Email"].ToString(),
                    TrangThai = reader["TrangThai"].ToString(),
                    TenDaiLy = reader["TenDaiLy"].ToString(),
                    SoDienThoai = reader["SoDienThoai"].ToString(),
                    DiaChi = reader["DiaChi"].ToString(),
                    facebook = reader["Facebook"] != DBNull.Value ? reader["Facebook"].ToString() : null,
                    tiktok = reader["TikTok"] != DBNull.Value ? reader["TikTok"].ToString() : null,
                    NgayTao = reader["NgayTao"],
                    NgayCapNhat = reader["NgayCapNhat"] != DBNull.Value ? reader["NgayCapNhat"] : null,
                    SoDonHangNhan = (int)reader["SoDonHangNhan"],
                    SoDonHangBan = (int)reader["SoDonHangBan"],
                    SoKiemDinh = (int)reader["SoKiemDinh"]
                };

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin đại lý thành công",
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
        /// Lấy thông tin chi tiết siêu thị - Cần token admin
        /// </summary>
        [HttpGet("sieuthi/{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult GetSieuThiDetail(int id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                using var cmd = new SqlCommand(@"
                    SELECT 
                        st.MaSieuThi,
                        st.MaTaiKhoan,
                        tk.TenDangNhap,
                        tk.Email,
                        tk.TrangThai,
                        st.TenSieuThi,
                        st.SoDienThoai,
                        st.DiaChi,
                        st.Facebook,
                        st.TikTok,
                        st.NgayTao,
                        st.NgayCapNhat,
                        COUNT(DISTINCT dh.MaDonHang) as SoDonHangMua,
                        COUNT(DISTINCT k.MaKho) as SoKho
                    FROM SieuThi st
                    INNER JOIN TaiKhoan tk ON st.MaTaiKhoan = tk.MaTaiKhoan
                    LEFT JOIN DonHang dh ON st.MaSieuThi = dh.MaNguoiMua AND dh.LoaiDon = 'daily_to_sieuthi'
                    LEFT JOIN Kho k ON st.MaSieuThi = k.MaChuSoHuu AND k.LoaiChuSoHuu = 'sieuthi'
                    WHERE st.MaSieuThi = @Id
                    GROUP BY st.MaSieuThi, st.MaTaiKhoan, tk.TenDangNhap, tk.Email, tk.TrangThai,
                             st.TenSieuThi, st.SoDienThoai, st.DiaChi, st.Facebook, st.TikTok,
                             st.NgayTao, st.NgayCapNhat", conn);

                cmd.Parameters.AddWithValue("@Id", id);

                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy siêu thị"
                    });
                }

                var result = new
                {
                    MaSieuThi = (int)reader["MaSieuThi"],
                    MaTaiKhoan = (int)reader["MaTaiKhoan"],
                    TenDangNhap = reader["TenDangNhap"].ToString(),
                    Email = reader["Email"].ToString(),
                    TrangThai = reader["TrangThai"].ToString(),
                    TenSieuThi = reader["TenSieuThi"].ToString(),
                    SoDienThoai = reader["SoDienThoai"].ToString(),
                    DiaChi = reader["DiaChi"].ToString(),
                    facebook = reader["Facebook"] != DBNull.Value ? reader["Facebook"].ToString() : null,
                    tiktok = reader["TikTok"] != DBNull.Value ? reader["TikTok"].ToString() : null,
                    NgayTao = reader["NgayTao"],
                    NgayCapNhat = reader["NgayCapNhat"] != DBNull.Value ? reader["NgayCapNhat"] : null,
                    SoDonHangMua = (int)reader["SoDonHangMua"],
                    SoKho = (int)reader["SoKho"]
                };

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin siêu thị thành công",
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
        /// Tìm kiếm người dùng - Cần token admin
        /// </summary>
        [HttpGet("search")]
        [Authorize(Roles = "admin")]
        public IActionResult SearchUsers([FromQuery] string keyword, [FromQuery] string? loaiNguoiDung = null)
        {
            try
            {
                if (string.IsNullOrEmpty(keyword))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Từ khóa tìm kiếm không được để trống"
                    });
                }

                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                var result = new List<object>();

                // Tìm kiếm nông dân
                if (string.IsNullOrEmpty(loaiNguoiDung) || loaiNguoiDung == "nongdan")
                {
                    using var cmdNongDan = new SqlCommand(@"
                        SELECT 
                            nd.MaNongDan as Id,
                            tk.TenDangNhap,
                            nd.HoTen,
                            nd.SoDienThoai,
                            nd.DiaChi,
                            'nongdan' as LoaiNguoiDung
                        FROM NongDan nd
                        INNER JOIN TaiKhoan tk ON nd.MaTaiKhoan = tk.MaTaiKhoan
                        WHERE nd.HoTen LIKE @Keyword 
                           OR tk.TenDangNhap LIKE @Keyword 
                           OR nd.SoDienThoai LIKE @Keyword", conn);

                    cmdNongDan.Parameters.AddWithValue("@Keyword", $"%{keyword}%");

                    using var reader = cmdNongDan.ExecuteReader();
                    while (reader.Read())
                    {
                        result.Add(new
                        {
                            Id = (int)reader["Id"],
                            TenDangNhap = reader["TenDangNhap"].ToString(),
                            HoTen = reader["HoTen"].ToString(),
                            SoDienThoai = reader["SoDienThoai"].ToString(),
                            DiaChi = reader["DiaChi"].ToString(),
                            LoaiNguoiDung = reader["LoaiNguoiDung"].ToString()
                        });
                    }
                }

                // Tìm kiếm đại lý và siêu thị tương tự...

                return Ok(new
                {
                    success = true,
                    message = "Tìm kiếm thành công",
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
    }
}
