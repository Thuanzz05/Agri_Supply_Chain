using Microsoft.AspNetCore.Mvc;
using NongDanService.Models.DTOs;
using NongDanService.Services;

namespace NongDanService.Controllers
{
    [Route("api/trang-trai")]
    [ApiController]
    public class TrangTraiController : ControllerBase
    {
        private readonly ITrangTraiService _trangTraiService;

        public TrangTraiController(ITrangTraiService trangTraiService)
        {
            _trangTraiService = trangTraiService;
        }

        /// <summary>
        /// Lấy tất cả trang trại
        /// </summary>
        /// <returns>Danh sách trang trại</returns>
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                var data = _trangTraiService.GetAll();
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách trang trại thành công",
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
        /// Lấy trang trại theo nông dân
        /// </summary>
        /// <param name="maNongDan">Mã nông dân</param>
        /// <returns>Danh sách trang trại của nông dân</returns>
        [HttpGet("get-by-nong-dan/{maNongDan}")]
        public IActionResult GetByNongDan(int maNongDan)
        {
            try
            {
                if (maNongDan <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Mã nông dân không hợp lệ"
                    });
                }

                var data = _trangTraiService.GetByNongDan(maNongDan);
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách trang trại thành công",
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
        /// Lấy trang trại theo ID
        /// </summary>
        /// <param name="id">Mã trang trại</param>
        /// <returns>Thông tin trang trại</returns>
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
                        message = "ID trang trại không hợp lệ"
                    });
                }

                var data = _trangTraiService.GetById(id);
                if (data == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy trang trại"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin trang trại thành công",
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
        /// Tạo trang trại mới
        /// </summary>
        /// <param name="dto">Thông tin trang trại</param>
        /// <returns>ID trang trại mới</returns>
        [HttpPost("create")]
        public IActionResult Create([FromBody] TrangTraiCreateDTO dto)
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

                var newId = _trangTraiService.Create(dto);
                return Ok(new
                {
                    success = true,
                    message = "Tạo trang trại thành công",
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
        /// Cập nhật trang trại
        /// </summary>
        /// <param name="id">Mã trang trại</param>
        /// <param name="dto">Thông tin cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] TrangTraiUpdateDTO dto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID trang trại không hợp lệ"
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

                bool result = _trangTraiService.Update(id, dto);
                if (!result)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy trang trại để cập nhật"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật trang trại thành công"
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
        /// Xóa trang trại
        /// </summary>
        /// <param name="id">Mã trang trại</param>
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
                        message = "ID trang trại không hợp lệ"
                    });
                }

                bool result = _trangTraiService.Delete(id);
                if (!result)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy trang trại để xóa"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Xóa trang trại thành công"
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
