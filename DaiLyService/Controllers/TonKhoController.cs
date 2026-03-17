using Microsoft.AspNetCore.Mvc;
using DaiLyService.Models.DTOs;
using DaiLyService.Services;

namespace DaiLyService.Controllers
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
        /// Lấy tất cả tồn kho
        /// </summary>
        /// <returns>Danh sách tồn kho</returns>
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                var data = _tonKhoService.GetAll();
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách tồn kho thành công",
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
        /// Lấy tồn kho theo kho và lô
        /// </summary>
        /// <param name="maKho">Mã kho</param>
        /// <param name="maLo">Mã lô</param>
        /// <returns>Thông tin tồn kho cụ thể</returns>
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
                        message = "Mã kho và mã lô không hợp lệ"
                    });
                }

                var data = _tonKhoService.GetByKhoAndLo(maKho, maLo);
                if (data == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy tồn kho với mã kho và mã lô này"
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
        /// Tạo mới tồn kho
        /// </summary>
        /// <param name="maKho">Mã kho</param>
        /// <param name="maLo">Mã lô</param>
        /// <param name="soLuong">Số lượng</param>
        /// <returns>Kết quả tạo tồn kho</returns>
        [HttpPost("create")]
        public IActionResult Create([FromQuery] int maKho, [FromQuery] int maLo, [FromQuery] decimal soLuong)
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

                if (soLuong < 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Số lượng không được âm"
                    });
                }

                // Kiểm tra xem tồn kho đã tồn tại chưa
                var existing = _tonKhoService.GetByKhoAndLo(maKho, maLo);
                if (existing != null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Lô nông sản đã tồn tại trong kho này"
                    });
                }

                var result = _tonKhoService.Create(maKho, maLo, soLuong);
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
        /// <param name="soLuongMoi">Số lượng mới</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("update-so-luong")]
        public IActionResult UpdateSoLuong([FromQuery] int maKho, [FromQuery] int maLo, [FromQuery] decimal soLuongMoi)
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

                if (soLuongMoi < 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Số lượng không được âm"
                    });
                }

                // Kiểm tra xem tồn kho có tồn tại không
                var existing = _tonKhoService.GetByKhoAndLo(maKho, maLo);
                if (existing == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy tồn kho với mã kho và mã lô này"
                    });
                }

                var result = _tonKhoService.UpdateSoLuong(maKho, maLo, soLuongMoi);
                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Cập nhật số lượng tồn kho thành công"
                    });
                }

                return BadRequest(new
                {
                    success = false,
                    message = "Không thể cập nhật số lượng tồn kho"
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
        [HttpDelete("delete")]
        public IActionResult Delete([FromQuery] int maKho, [FromQuery] int maLo)
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

                // Kiểm tra xem tồn kho có tồn tại không
                var existing = _tonKhoService.GetByKhoAndLo(maKho, maLo);
                if (existing == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy tồn kho với mã kho và mã lô này"
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

                return BadRequest(new
                {
                    success = false,
                    message = "Không thể xóa tồn kho"
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