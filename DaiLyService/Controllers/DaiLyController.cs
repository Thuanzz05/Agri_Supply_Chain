using Microsoft.AspNetCore.Mvc;
using DaiLyService.Models.DTOs;
using DaiLyService.Services;

namespace DaiLyService.Controllers
{
    [Route("api/dai-ly")]
    [ApiController]
    public class DaiLyController : ControllerBase
    {
        private readonly IDaiLyService _daiLyService;

        public DaiLyController(IDaiLyService daiLyService)
        {
            _daiLyService = daiLyService;
        }

        /// <summary>
        /// Lấy tất cả đại lý
        /// </summary>
        /// <returns>Danh sách đại lý</returns>
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                var data = _daiLyService.GetAll();
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách đại lý thành công",
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
        /// Lấy đại lý theo ID
        /// </summary>
        /// <param name="id">Mã đại lý</param>
        /// <returns>Thông tin đại lý</returns>
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
                        message = "ID đại lý không hợp lệ"
                    });
                }

                var data = _daiLyService.GetById(id);
                if (data == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy đại lý"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin đại lý thành công",
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
        /// Lấy đại lý theo tài khoản
        /// </summary>
        /// <param name="maTaiKhoan">Mã tài khoản</param>
        /// <returns>Thông tin đại lý</returns>
        [HttpGet("get-by-tai-khoan/{maTaiKhoan}")]
        public IActionResult GetByTaiKhoan(int maTaiKhoan)
        {
            try
            {
                if (maTaiKhoan <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Mã tài khoản không hợp lệ"
                    });
                }

                var data = _daiLyService.GetByTaiKhoan(maTaiKhoan);
                if (data == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy đại lý với tài khoản này"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin đại lý thành công",
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
        /// Tạo đại lý mới
        /// </summary>
        /// <param name="dto">Thông tin đại lý</param>
        /// <returns>ID đại lý mới</returns>
        [HttpPost("create")]
        public IActionResult Create([FromBody] DaiLyCreateDTO dto)
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

                var newId = _daiLyService.Create(dto);
                return Ok(new
                {
                    success = true,
                    message = "Tạo đại lý thành công",
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
        /// Cập nhật đại lý
        /// </summary>
        /// <param name="id">Mã đại lý</param>
        /// <param name="dto">Thông tin cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] DaiLyUpdateDTO dto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID đại lý không hợp lệ"
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

                bool result = _daiLyService.Update(id, dto);
                if (!result)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy đại lý để cập nhật"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật đại lý thành công"
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
        /// Xóa đại lý
        /// </summary>
        /// <param name="id">Mã đại lý</param>
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
                        message = "ID đại lý không hợp lệ"
                    });
                }

                bool result = _daiLyService.Delete(id);
                if (!result)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy đại lý để xóa"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Xóa đại lý thành công"
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