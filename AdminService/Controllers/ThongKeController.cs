using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AdminService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThongKeController : ControllerBase
    {
        private readonly string _connectionString;

        public ThongKeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        /// <summary>
        /// Thống kê tổng quan hệ thống - Cần token admin
        /// </summary>
        [HttpGet("tong-quan")]
        [Authorize(Roles = "admin")]
        public IActionResult GetTongQuan()
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                var stats = new
                {
                    TongNguoiDung = GetTongNguoiDung(conn),
                    TongNongDan = GetCount(conn, "NongDan"),
                    TongDaiLy = GetCount(conn, "DaiLy"),
                    TongSieuThi = GetCount(conn, "SieuThi"),
                    TongAdmin = GetCount(conn, "Admin"),
                    TongSanPham = GetCount(conn, "SanPham"),
                    TongTrangTrai = GetCount(conn, "TrangTrai"),
                    TongLoNongSan = GetCount(conn, "LoNongSan"),
                    TongDonHang = GetCount(conn, "DonHang"),
                    TongKiemDinh = GetCount(conn, "KiemDinh")
                };

                return Ok(new
                {
                    success = true,
                    message = "Lấy thống kê tổng quan thành công",
                    data = stats
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
        /// Thống kê người dùng theo loại - Cần token admin
        /// </summary>
        [HttpGet("nguoi-dung")]
        [Authorize(Roles = "admin")]
        public IActionResult GetThongKeNguoiDung()
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                using var cmd = new SqlCommand(@"
                    SELECT 
                        LoaiTaiKhoan,
                        COUNT(*) as SoLuong,
                        COUNT(CASE WHEN TrangThai = N'hoat_dong' THEN 1 END) as DangHoatDong,
                        COUNT(CASE WHEN TrangThai = N'tam_khoa' THEN 1 END) as TamKhoa
                    FROM TaiKhoan 
                    GROUP BY LoaiTaiKhoan", conn);

                var result = new List<object>();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new
                    {
                        LoaiTaiKhoan = reader["LoaiTaiKhoan"].ToString(),
                        SoLuong = (int)reader["SoLuong"],
                        DangHoatDong = (int)reader["DangHoatDong"],
                        TamKhoa = (int)reader["TamKhoa"]
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy thống kê người dùng thành công",
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
        /// Thống kê đơn hàng - Cần token admin
        /// </summary>
        [HttpGet("don-hang")]
        [Authorize(Roles = "admin")]
        public IActionResult GetThongKeDonHang()
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                using var cmd = new SqlCommand(@"
                    SELECT 
                        LoaiDon,
                        TrangThai,
                        COUNT(*) as SoLuong,
                        SUM(TongTien) as TongTien
                    FROM DonHang 
                    GROUP BY LoaiDon, TrangThai
                    ORDER BY LoaiDon, TrangThai", conn);

                var result = new List<object>();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new
                    {
                        LoaiDon = reader["LoaiDon"].ToString(),
                        TrangThai = reader["TrangThai"].ToString(),
                        SoLuong = (int)reader["SoLuong"],
                        TongTien = reader["TongTien"] != DBNull.Value ? (decimal)reader["TongTien"] : 0
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy thống kê đơn hàng thành công",
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
        /// Thống kê kiểm định chất lượng - Cần token admin
        /// </summary>
        [HttpGet("kiem-dinh")]
        [Authorize(Roles = "admin")]
        public IActionResult GetThongKeKiemDinh()
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                using var cmd = new SqlCommand(@"
                    SELECT 
                        KetQua,
                        COUNT(*) as SoLuong,
                        CAST(COUNT(*) * 100.0 / SUM(COUNT(*)) OVER() AS DECIMAL(5,2)) as TyLe
                    FROM KiemDinh 
                    GROUP BY KetQua", conn);

                var result = new List<object>();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new
                    {
                        KetQua = reader["KetQua"].ToString(),
                        SoLuong = (int)reader["SoLuong"],
                        TyLe = (decimal)reader["TyLe"]
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy thống kê kiểm định thành công",
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
        /// Thống kê theo tháng - Cần token admin
        /// </summary>
        [HttpGet("theo-thang")]
        [Authorize(Roles = "admin")]
        public IActionResult GetThongKeTheoThang([FromQuery] int? nam = null)
        {
            try
            {
                var targetYear = nam ?? DateTime.Now.Year;
                
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                using var cmd = new SqlCommand(@"
                    SELECT 
                        MONTH(NgayTao) as Thang,
                        COUNT(CASE WHEN LoaiDon = 'nongdan_to_daily' THEN 1 END) as DonHangNongDan,
                        COUNT(CASE WHEN LoaiDon = 'daily_to_sieuthi' THEN 1 END) as DonHangSieuThi,
                        SUM(TongTien) as DoanhThu
                    FROM DonHang 
                    WHERE YEAR(NgayTao) = @Nam
                    GROUP BY MONTH(NgayTao)
                    ORDER BY MONTH(NgayTao)", conn);

                cmd.Parameters.AddWithValue("@Nam", targetYear);

                var result = new List<object>();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new
                    {
                        Thang = (int)reader["Thang"],
                        DonHangNongDan = (int)reader["DonHangNongDan"],
                        DonHangSieuThi = (int)reader["DonHangSieuThi"],
                        DoanhThu = reader["DoanhThu"] != DBNull.Value ? (decimal)reader["DoanhThu"] : 0
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = $"Lấy thống kê năm {targetYear} thành công",
                    data = new
                    {
                        Nam = targetYear,
                        ThongKe = result
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
        /// Thống kê top sản phẩm - Không cần token
        /// </summary>
        [HttpGet("top-san-pham")]
        public IActionResult GetTopSanPham([FromQuery] int limit = 10)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                using var cmd = new SqlCommand(@"
                    SELECT TOP (@Limit)
                        sp.TenSanPham,
                        COUNT(lo.MaLo) as SoLuongLo,
                        SUM(lo.SoLuongBanDau) as TongSoLuongBanDau,
                        SUM(lo.SoLuongHienTai) as TongSoLuongHienTai
                    FROM SanPham sp
                    LEFT JOIN LoNongSan lo ON sp.MaSanPham = lo.MaSanPham
                    GROUP BY sp.MaSanPham, sp.TenSanPham
                    ORDER BY SUM(lo.SoLuongBanDau) DESC", conn);

                cmd.Parameters.AddWithValue("@Limit", limit);

                var result = new List<object>();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new
                    {
                        TenSanPham = reader["TenSanPham"].ToString(),
                        SoLuongLo = (int)reader["SoLuongLo"],
                        TongSoLuongBanDau = reader["TongSoLuongBanDau"] != DBNull.Value ? (decimal)reader["TongSoLuongBanDau"] : 0,
                        TongSoLuongHienTai = reader["TongSoLuongHienTai"] != DBNull.Value ? (decimal)reader["TongSoLuongHienTai"] : 0
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = $"Lấy top {limit} sản phẩm thành công",
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

        // Helper methods
        private int GetTongNguoiDung(SqlConnection conn)
        {
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM TaiKhoan WHERE TrangThai = N'hoat_dong'", conn);
            return (int)cmd.ExecuteScalar();
        }

        private int GetCount(SqlConnection conn, string tableName)
        {
            using var cmd = new SqlCommand($"SELECT COUNT(*) FROM {tableName}", conn);
            return (int)cmd.ExecuteScalar();
        }
    }
}