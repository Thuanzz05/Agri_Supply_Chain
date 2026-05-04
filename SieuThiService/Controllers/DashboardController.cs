using Microsoft.AspNetCore.Mvc;
using SieuThiService.Services;

namespace SieuThiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IDashboardService service, ILogger<DashboardController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Lấy thống kê dashboard cho siêu thị
        /// </summary>
        /// <param name="maSieuThi">Mã siêu thị</param>
        /// <returns>Thống kê dashboard</returns>
        [HttpGet("{maSieuThi}")]
        public async Task<IActionResult> GetDashboardStats(int maSieuThi)
        {
            try
            {
                _logger.LogInformation("API: Getting dashboard stats for supermarket {SupermarketId}", maSieuThi);
                var stats = await _service.GetDashboardStats(maSieuThi);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API: Error getting dashboard stats for supermarket {SupermarketId}", maSieuThi);
                return StatusCode(500, new { message = "Lỗi khi lấy thống kê dashboard", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy thống kê đơn hàng cho siêu thị
        /// </summary>
        /// <param name="maSieuThi">Mã siêu thị</param>
        /// <returns>Thống kê đơn hàng</returns>
        [HttpGet("don-hang/{maSieuThi}")]
        public async Task<IActionResult> GetDonHangStats(int maSieuThi)
        {
            try
            {
                _logger.LogInformation("API: Getting order stats for supermarket {SupermarketId}", maSieuThi);
                var stats = await _service.GetDonHangStats(maSieuThi);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API: Error getting order stats for supermarket {SupermarketId}", maSieuThi);
                return StatusCode(500, new { message = "Lỗi khi lấy thống kê đơn hàng", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy thống kê kho hàng cho siêu thị
        /// </summary>
        /// <param name="maSieuThi">Mã siêu thị</param>
        /// <returns>Thống kê kho hàng</returns>
        [HttpGet("kho/{maSieuThi}")]
        public async Task<IActionResult> GetKhoStats(int maSieuThi)
        {
            try
            {
                _logger.LogInformation("API: Getting warehouse stats for supermarket {SupermarketId}", maSieuThi);
                var stats = await _service.GetKhoStats(maSieuThi);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API: Error getting warehouse stats for supermarket {SupermarketId}", maSieuThi);
                return StatusCode(500, new { message = "Lỗi khi lấy thống kê kho hàng", error = ex.Message });
            }
        }
    }
}
