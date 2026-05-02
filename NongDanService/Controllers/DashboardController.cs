using Microsoft.AspNetCore.Mvc;
using NongDanService.Services;

namespace NongDanService.Controllers
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
        /// Lấy thống kê dashboard cho nông dân
        /// </summary>
        [HttpGet("stats/{maNongDan}")]
        public IActionResult GetStats(int maNongDan)
        {
            try
            {
                var stats = _service.GetDashboardStats(maNongDan);
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
        /// Lấy đơn hàng gần đây của nông dân
        /// </summary>
        [HttpGet("recent-orders/{maNongDan}")]
        public IActionResult GetRecentOrders(int maNongDan, [FromQuery] int limit = 5)
        {
            try
            {
                var orders = _service.GetRecentOrders(maNongDan, limit);
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
        /// Lấy thống kê đơn hàng của nông dân
        /// </summary>
        [HttpGet("order-stats/{maNongDan}")]
        public IActionResult GetOrderStats(int maNongDan)
        {
            try
            {
                var stats = _service.GetOrderStats(maNongDan);
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
