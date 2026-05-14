using Microsoft.AspNetCore.Mvc;
using SieuThiService.Models.DTOs;
using SieuThiService.Services;

namespace SieuThiService.Controllers
{
    [Route("api/chuyen-kho")]
    [ApiController]
    public class ChuyenKhoController : ControllerBase
    {
        private readonly IChuyenKhoService _service;

        public ChuyenKhoController(IChuyenKhoService service)
        {
            _service = service;
        }

        /// <summary>
        /// Tạo phiếu chuyển kho nội bộ
        /// </summary>
        /// <param name="dto">Thông tin chuyển kho</param>
        /// <returns>Mã phiếu chuyển kho</returns>
        [HttpPost("create")]
        public IActionResult Create([FromBody] ChuyenKhoCreateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ",
                        errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var maPhieu = _service.Create(dto);
                return Ok(new
                {
                    success = true,
                    message = "Chuyển kho thành công",
                    data = new { maPhieu }
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
        /// Lấy lịch sử chuyển kho theo siêu thị
        /// </summary>
        /// <param name="maSieuThi">Mã siêu thị</param>
        /// <returns>Danh sách phiếu chuyển kho</returns>
        [HttpGet("get-by-sieu-thi/{maSieuThi}")]
        public IActionResult GetBySieuThi(int maSieuThi)
        {
            try
            {
                if (maSieuThi <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Mã siêu thị không hợp lệ"
                    });
                }

                var data = _service.GetBySieuThi(maSieuThi);
                return Ok(new
                {
                    success = true,
                    message = "Lấy lịch sử chuyển kho thành công",
                    data,
                    count = data.Count
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
