using Microsoft.AspNetCore.Mvc;
using SieuThiService.Services;

namespace SieuThiService.Controllers
{
    [Route("api/truy-xuat")]
    [ApiController]
    public class TruyXuatController : ControllerBase
    {
        private readonly ITruyXuatService _truyXuatService;
        private readonly ILogger<TruyXuatController> _logger;

        public TruyXuatController(ITruyXuatService truyXuatService, ILogger<TruyXuatController> logger)
        {
            _truyXuatService = truyXuatService;
            _logger = logger;
        }

        /// <summary>
        /// Truy xuất nguồn gốc sản phẩm theo mã QR
        /// </summary>
        /// <param name="maQR">Mã QR của lô nông sản</param>
        /// <returns>Thông tin truy xuất nguồn gốc</returns>
        [HttpGet("trace/{maQR}")]
        public IActionResult Trace(string maQR)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maQR))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Vui lòng nhập mã QR"
                    });
                }

                var result = _truyXuatService.TraceProductByQR(maQR);
                
                if (result == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy lô nông sản với mã QR: " + maQR
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Truy xuất nguồn gốc thành công",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracing product with QR: {QR}", maQR);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server: " + ex.Message
                });
            }
        }
    }
}
