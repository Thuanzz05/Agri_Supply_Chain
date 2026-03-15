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
        /// Lấy tồn kho theo đại lý
        /// </summary>
        /// <param name="maDaiLy">Mã đại lý</param>
        /// <returns>Danh sách tồn kho của đại lý</returns>
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

                var data = _tonKhoService.GetByDaiLy(maDaiLy);
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
        /// Lấy tồn kho theo ID
        /// </summary>
        /// <param name="id">Mã tồn kho</param>
        /// <returns>Thông tin tồn kho</returns>
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
                        message = "ID tồn kho không hợp lệ"
                    });
                }

                var data = _tonKhoService.GetById(id);
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
        /// Lấy tồn kho theo lô nông sản
        /// </summary>
        /// <param name="maLoNongSan">Mã lô nông sản</param>
        /// <returns>Thông tin tồn kho</returns>
        [HttpGet("get-by-lo-nong-san/{maLoNongSan}")]
        public IActionResult GetByLoNongSan(int maLoNongSan)
        {
            try
            {
                if (maLoNongSan <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Mã lô nông sản không hợp lệ"
                    });
                }

                var data = _tonKhoService.GetByLoNongSan(maLoNongSan);
                if (data == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy tồn kho cho lô nông sản này"
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
        /// Lấy tồn kho theo trạng thái
        /// </summary>
        /// <param name="trangThai">Trạng thái (binh_thuong, sap_het, het_hang)</param>
        /// <returns>Danh sách tồn kho theo trạng thái</returns>
        [HttpGet("get-by-trang-thai")]
        public IActionResult GetByTrangThai([FromQuery] string trangThai)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(trangThai))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Trạng thái không được để trống"
                    });
                }

                var data = _tonKhoService.GetByTrangThai(trangThai);
                return Ok(new
                {
                    success = true,
                    message = $"Lấy danh sách tồn kho trạng thái '{trangThai}' thành công",
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
        /// Lấy danh sách hàng sắp hết
        /// </summary>
        /// <param name="maDaiLy">Mã đại lý</param>
        /// <returns>Danh sách hàng sắp hết</returns>
        [HttpGet("get-sap-het-hang/{maDaiLy}")]
        public IActionResult GetSapHetHang(int maDaiLy)
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

                var data = _tonKhoService.GetSapHetHang(maDaiLy);
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách hàng sắp hết thành công",
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
        /// Tạo tồn kho mới
        /// </summary>
        /// <param name="dto">Thông tin tồn kho</param>
        /// <returns>ID tồn kho mới</returns>
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
                        errors = ModelState
                    });
                }

                var newId = _tonKhoService.Create(dto);
                return Ok(new
                {
                    success = true,
                    message = "Tạo tồn kho thành công",
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
        /// Cập nhật tồn kho
        /// </summary>
        /// <param name="id">Mã tồn kho</param>
        /// <param name="dto">Thông tin cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] TonKhoUpdateDTO dto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID tồn kho không hợp lệ"
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

                bool result = _tonKhoService.Update(id, dto);
                if (!result)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy tồn kho để cập nhật"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật tồn kho thành công"
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
        /// <param name="id">Mã tồn kho</param>
        /// <param name="soLuongMoi">Số lượng mới</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("update-so-luong/{id}")]
        public IActionResult UpdateSoLuong(int id, [FromBody] decimal soLuongMoi)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID tồn kho không hợp lệ"
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

                bool result = _tonKhoService.UpdateSoLuong(id, soLuongMoi);
                if (!result)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy tồn kho để cập nhật"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật số lượng tồn kho thành công"
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
        /// <param name="id">Mã tồn kho</param>
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
                        message = "ID tồn kho không hợp lệ"
                    });
                }

                bool result = _tonKhoService.Delete(id);
                if (!result)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy tồn kho để xóa"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Xóa tồn kho thành công"
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