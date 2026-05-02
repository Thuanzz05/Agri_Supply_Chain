using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdminService.Services;

namespace AdminService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _service;

        public DashboardController(DashboardService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy thống kê tổng quan - Cần token admin
        /// </summary>
        [HttpGet("stats")]
        [Authorize(Roles = "admin")]
        public IActionResult GetStats()
        {
            var (success, message, data) = _service.GetDashboardStats();

            if (!success)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server: " + message
                });
            }

            return Ok(new
            {
                success = true,
                message,
                data
            });
        }
    }
}
