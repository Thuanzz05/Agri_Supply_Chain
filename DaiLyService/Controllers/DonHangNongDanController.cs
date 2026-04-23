using Microsoft.AspNetCore.Mvc;
using DaiLyService.Models.DTOs;
using DaiLyService.Services;

namespace DaiLyService.Controllers
{
    [Route("api/don-hang-nong-dan")]
    [ApiController]
    public class DonHangNongDanController : ControllerBase
    {
        private readonly IDonHangService _donHangService;

        public DonHangNongDanController(IDonHangService donHangService)
        {
            _donHangService = donHangService;
        }

        /// <summary>
        /// Lấy tất cả đơn hàng từ nông dân
        /// </summary>
        /// <returns>Danh sách đơn hàng từ nông dân</returns>
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                var data = _donHangService.GetAll()
                    .Where(dh => dh.LoaiDon == "nongdan_to_daily")
                    .ToList();
                
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách đơn hàng từ nông dân thành công",
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
        /// Lấy đơn hàng từ nông dân theo đại lý
        /// </summary>
        /// <param name="maDaiLy">Mã đại lý</param>
        /// <returns>Danh sách đơn hàng</returns>
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

                var data = _donHangService.GetByNguoiMua(maDaiLy, "daily")
                    .Where(dh => dh.LoaiDon == "nongdan_to_daily")
                    .ToList();
                
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách đơn hàng từ nông dân thành công",
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
        /// Lấy đơn hàng theo nông dân (nông dân là người bán)
        /// </summary>
        /// <param name="maNongDan">Mã nông dân</param>
        /// <returns>Danh sách đơn hàng</returns>
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

                var data = _donHangService.GetByNguoiBan(maNongDan, "nongdan")
                    .Where(dh => dh.LoaiDon == "nongdan_to_daily")
                    .ToList();
                
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách đơn hàng thành công",
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
        /// Lấy đơn hàng theo ID
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <returns>Thông tin đơn hàng</returns>
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
                        message = "ID đơn hàng không hợp lệ"
                    });
                }

                var data = _donHangService.GetById(id);
                if (data == null || data.LoaiDon != "nongdan_to_daily")
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy đơn hàng"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin đơn hàng thành công",
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
        /// Xác nhận đơn hàng từ nông dân
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <param name="dto">Trạng thái mới</param>
        /// <returns>Kết quả xác nhận</returns>
        [HttpPut("xac-nhan/{id}")]
        public IActionResult XacNhan(int id, [FromBody] DonHangUpdateDTO dto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID đơn hàng không hợp lệ"
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

                // Kiểm tra đơn hàng tồn tại và thuộc loại nongdan_to_daily
                var donHang = _donHangService.GetById(id);
                if (donHang == null || donHang.LoaiDon != "nongdan_to_daily")
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy đơn hàng"
                    });
                }

                var result = _donHangService.UpdateTrangThai(id, dto.TrangThai);
                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Cập nhật trạng thái đơn hàng thành công"
                    });
                }

                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy đơn hàng để cập nhật"
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