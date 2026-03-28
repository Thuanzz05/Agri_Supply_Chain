using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        /// <summary>
        /// Dashboard tổng quan - Cần token admin
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult GetDashboard()
        {
            try
            {
                var data = new
                {
                    TongNongDan = 150,
                    TongDaiLy = 45,
                    TongSieuThi = 25,
                    TongGiaoDich = 1250,
                    DoanhThuThang = 2500000000,
                    SanPhamDatChatLuong = 95.5
                };

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin dashboard thành công",
                    data = data
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
        /// Thống kê người dùng - Không cần token
        /// </summary>
        [HttpGet("users")]
        public IActionResult GetUserStats()
        {
            try
            {
                var data = new
                {
                    NongDan = 150,
                    DaiLy = 45,
                    SieuThi = 25,
                    Admin = 5,
                    TongCong = 225
                };

                return Ok(new
                {
                    success = true,
                    message = "Lấy thống kê người dùng thành công",
                    data = data
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
        /// Test endpoint đơn giản
        /// </summary>
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new
            {
                success = true,
                message = "AdminService đang hoạt động bình thường",
                timestamp = DateTime.Now
            });
        }
    }
}