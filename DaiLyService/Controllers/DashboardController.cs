using Microsoft.AspNetCore.Mvc;
using DaiLyService.Services;

namespace DaiLyService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;

        public DashboardController(IDashboardService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy thống kê dashboard cho đại lý
        /// </summary>
        [HttpGet("stats/{maDaiLy}")]
        public IActionResult GetStats(int maDaiLy)
        {
            try
            {
                var stats = _service.GetDashboardStats(maDaiLy);
                return Ok(new
                {
                    success = true,
                    message = "Lấy thống kê dashboard thành công",
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
        /// Lấy đơn hàng gần đây của đại lý
        /// </summary>
        [HttpGet("recent-orders/{maDaiLy}")]
        public IActionResult GetRecentOrders(int maDaiLy, [FromQuery] int limit = 5)
        {
            try
            {
                var orders = _service.GetRecentOrders(maDaiLy, limit);
                return Ok(new
                {
                    success = true,
                    message = "Lấy đơn hàng gần đây thành công",
                    data = orders
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
        /// Lấy thống kê đơn hàng của đại lý
        /// </summary>
        [HttpGet("order-stats/{maDaiLy}")]
        public IActionResult GetOrderStats(int maDaiLy)
        {
            try
            {
                var stats = _service.GetOrderStats(maDaiLy);
                return Ok(new
                {
                    success = true,
                    message = "Lấy thống kê đơn hàng thành công",
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
    }
}
