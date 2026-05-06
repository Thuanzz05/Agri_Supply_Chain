using Microsoft.AspNetCore.Mvc;
using DaiLyService.Models.DTOs;
using DaiLyService.Services;

namespace DaiLyService.Controllers
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

        [HttpGet("get-by-dai-ly/{maDaiLy}")]
        public IActionResult GetByDaiLy(int maDaiLy)
        {
            try
            {
                if (maDaiLy <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Mã đại lý không hợp lệ"
                    });
                }

                var data = _service.GetByDaiLy(maDaiLy);
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

