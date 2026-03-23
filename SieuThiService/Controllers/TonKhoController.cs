using Microsoft.AspNetCore.Mvc;
using SieuThiService.Models.DTOs;
using SieuThiService.Services;

namespace SieuThiService.Controllers
{
    [Route("api/ton-kho")]
    [ApiController]
    public class TonKhoController : ControllerBase
    {
        private readonly ITonKhoService _tonKhoService;

        public TonKhoController(ITonKhoService tonKhoService)
        {
            _tonKhoService = tonKhoService;
        }

        /// <summary>
        /// Lấy tất cả tồn kho của siêu thị
        /// </summary>
        /// <returns>Danh sách tồn kho siêu thị</returns>
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                var data = _tonKhoService.GetAll();
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách tồn kho siêu thị thành công",
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
        /// Lấy tồn kho theo kho
        /// </summary>
        /// <param name="maKho">Mã kho</param>
        /// <returns>Danh sách tồn kho của kho</returns>
        [HttpGet("get-by-kho/{maKho}")]
        public IActionResult GetByKho(int maKho)
        {
            try
            {
                if (maKho <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Mã kho không hợp lệ"
                    });
                }

                var data = _tonKhoService.GetByKho(maKho);
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách tồn kho theo kho thành công",
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
        /// Lấy tồn kho theo siêu thị
        /// </summary>
        /// <param name="maSieuThi">Mã siêu thị</param>
        /// <returns>Danh sách tồn kho của siêu thị</returns>
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

                var data = _tonKhoService.GetBySieuThi(maSieuThi);
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách tồn kho theo siêu thị thành công",
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
        /// Lấy tồn kho cụ thể theo kho và lô
        /// </summary>
        /// <param name="maKho">Mã kho</param>
        /// <param name="maLo">Mã lô</param>
        /// <returns>Thông tin tồn kho</returns>
        [HttpGet("get-by-kho-and-lo/{maKho}/{maLo}")]
        public IActionResult GetByKhoAndLo(int maKho, int maLo)
        {
            try
            {
                if (maKho <= 0 || maLo <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Mã kho và mã lô phải lớn hơn 0"
                    });
                }

                var data = _tonKhoService.GetByKhoAndLo(maKho, maLo);
                if (data == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy tồn kho"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin tồn kho thành công",
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
        /// Tạo tồn kho mới
        /// </summary>
        /// <param name="dto">Thông tin tồn kho</param>
        /// <returns>Kết quả tạo tồn kho</returns>
        [HttpPost("create")]
        public IActionResult Create([FromBody] TonKhoCreateDTO dto)
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

                var result = _tonKhoService.Create(dto.MaKho, dto.MaLo, dto.SoLuong);
                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Tạo tồn kho thành công"
                    });
                }

                return BadRequest(new
                {
                    success = false,
                    message = "Không thể tạo tồn kho"
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
        /// Cập nhật số lượng tồn kho
        /// </summary>
        /// <param name="maKho">Mã kho</param>
        /// <param name="maLo">Mã lô</param>
        /// <param name="dto">Số lượng mới</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("update-so-luong/{maKho}/{maLo}")]
        public IActionResult UpdateSoLuong(int maKho, int maLo, [FromBody] TonKhoUpdateDTO dto)
        {
            try
            {
                if (maKho <= 0 || maLo <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Mã kho và mã lô phải lớn hơn 0"
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

                var result = _tonKhoService.UpdateSoLuong(maKho, maLo, dto.SoLuongMoi);
                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Cập nhật số lượng tồn kho thành công"
                    });
                }

                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy tồn kho để cập nhật"
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
        /// Xóa tồn kho
        /// </summary>
        /// <param name="maKho">Mã kho</param>
        /// <param name="maLo">Mã lô</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("delete/{maKho}/{maLo}")]
        public IActionResult Delete(int maKho, int maLo)
        {
            try
            {
                if (maKho <= 0 || maLo <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Mã kho và mã lô phải lớn hơn 0"
                    });
                }

                var result = _tonKhoService.Delete(maKho, maLo);
                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Xóa tồn kho thành công"
                    });
                }

                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy tồn kho để xóa"
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