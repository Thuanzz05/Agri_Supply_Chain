using Microsoft.AspNetCore.Mvc;
using SieuThiService.Models.DTOs;
using SieuThiService.Services;

namespace SieuThiService.Controllers
{
    [Route("api/sieu-thi")]
    [ApiController]
    public class SieuThiController : ControllerBase
    {
        private readonly ISieuThiService _sieuThiService;

        public SieuThiController(ISieuThiService sieuThiService)
        {
            _sieuThiService = sieuThiService;
        }

        /// <summary>
        /// Lấy tất cả siêu thị
        /// </summary>
        /// <returns>Danh sách siêu thị</returns>
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                var data = _sieuThiService.GetAll();
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách siêu thị thành công",
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
        /// Lấy siêu thị theo ID
        /// </summary>
        /// <param name="id">Mã siêu thị</param>
        /// <returns>Thông tin siêu thị</returns>
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
                        message = "Mã siêu thị không hợp lệ"
                    });
                }

                var data = _sieuThiService.GetById(id);
                if (data == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy siêu thị"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin siêu thị thành công",
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
        /// Lấy siêu thị theo tài khoản
        /// </summary>
        /// <param name="maTaiKhoan">Mã tài khoản</param>
        /// <returns>Thông tin siêu thị</returns>
        [HttpGet("get-by-account/{maTaiKhoan}")]
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

                var data = _sieuThiService.GetByTaiKhoan(maTaiKhoan);
                if (data == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy siêu thị với tài khoản này"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin siêu thị thành công",
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
        /// Tạo mới siêu thị
        /// </summary>
        /// <param name="sieuThiDto">Thông tin siêu thị</param>
        /// <returns>Kết quả tạo siêu thị</returns>
        [HttpPost("create")]
        public IActionResult Create([FromBody] SieuThiCreateDTO sieuThiDto)
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

                // Kiểm tra tên đăng nhập đã tồn tại
                if (_sieuThiService.ExistsByTenDangNhap(sieuThiDto.TenDangNhap))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Tên đăng nhập đã tồn tại trong hệ thống"
                    });
                }

                // Kiểm tra email đã tồn tại (nếu có)
                if (!string.IsNullOrEmpty(sieuThiDto.Email) && _sieuThiService.ExistsByEmail(sieuThiDto.Email))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Email đã tồn tại trong hệ thống"
                    });
                }

                var result = _sieuThiService.Create(sieuThiDto);
                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Tạo siêu thị thành công"
                    });
                }

                return BadRequest(new
                {
                    success = false,
                    message = "Không thể tạo siêu thị"
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
        /// Cập nhật thông tin siêu thị
        /// </summary>
        /// <param name="id">Mã siêu thị</param>
        /// <param name="sieuThiDto">Thông tin cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] SieuThiUpdateDTO sieuThiDto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Mã siêu thị không hợp lệ"
                    });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ",
                        errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                // Kiểm tra siêu thị có tồn tại không
                var existing = _sieuThiService.GetById(id);
                if (existing == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy siêu thị"
                    });
                }

                var result = _sieuThiService.Update(id, sieuThiDto);
                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Cập nhật siêu thị thành công"
                    });
                }

                return BadRequest(new
                {
                    success = false,
                    message = "Không thể cập nhật siêu thị"
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
        /// Xóa siêu thị
        /// </summary>
        /// <param name="id">Mã siêu thị</param>
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
                        message = "Mã siêu thị không hợp lệ"
                    });
                }

                // Kiểm tra siêu thị có tồn tại không
                var existing = _sieuThiService.GetById(id);
                if (existing == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy siêu thị"
                    });
                }

                var result = _sieuThiService.Delete(id);
                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Xóa siêu thị thành công"
                    });
                }

                return BadRequest(new
                {
                    success = false,
                    message = "Không thể xóa siêu thị"
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