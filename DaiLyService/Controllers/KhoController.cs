using Microsoft.AspNetCore.Mvc;
using DaiLyService.Models.DTOs;
using DaiLyService.Services;

namespace DaiLyService.Controllers
{
    [Route("api/kho")]
    [ApiController]
    public class KhoController : ControllerBase
    {
        private readonly IKhoService _khoService;

        public KhoController(IKhoService khoService)
        {
            _khoService = khoService;
        }

        /// <summary>
        /// Lấy tất cả kho
        /// </summary>
        /// <returns>Danh sách kho</returns>
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                var data = _khoService.GetAll();
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách kho thành công",
                    data = data,
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

        /// <summary>
        /// Lấy kho theo đại lý
        /// </summary>
        /// <param name="maDaiLy">Mã đại lý</param>
        /// <returns>Danh sách kho của đại lý</returns>
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

                var data = _khoService.GetByDaiLy(maDaiLy);
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách kho thành công",
                    data = data,
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

        /// <summary>
        /// Lấy kho theo ID
        /// </summary>
        /// <param name="id">Mã kho</param>
        /// <returns>Thông tin kho</returns>
        [HttpGet("get-by-id/{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID kho không hợp lệ"
                    });
                }

                var data = _khoService.GetById(id);
                if (data == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy kho"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin kho thành công",
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
        /// Tạo kho mới
        /// </summary>
        /// <param name="dto">Thông tin kho</param>
        /// <returns>ID kho mới</returns>
        [HttpPost("create")]
        public IActionResult Create([FromBody] KhoCreateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ",
                        errors = ModelState
                    });
                }

                var newId = _khoService.Create(dto);
                return Ok(new
                {
                    success = true,
                    message = "Tạo kho thành công",
                    data = new { id = newId }
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
        /// Cập nhật kho
        /// </summary>
        /// <param name="id">Mã kho</param>
        /// <param name="dto">Thông tin cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] KhoUpdateDTO dto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID kho không hợp lệ"
                    });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ",
                        errors = ModelState
                    });
                }

                bool result = _khoService.Update(id, dto);
                if (!result)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy kho để cập nhật"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật kho thành công"
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
        /// Xóa kho
        /// </summary>
        /// <param name="id">Mã kho</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID kho không hợp lệ"
                    });
                }

                bool result = _khoService.Delete(id);
                if (!result)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy kho để xóa"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Xóa kho thành công"
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