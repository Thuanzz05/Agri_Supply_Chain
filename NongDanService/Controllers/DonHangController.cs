using Microsoft.AspNetCore.Mvc;
using NongDanService.Models.DTOs;
using NongDanService.Services;

namespace NongDanService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonHangController : ControllerBase
    {
        private readonly IDonHangService _donHangService;

        public DonHangController(IDonHangService donHangService)
        {
            _donHangService = donHangService;
        }

        /// <summary>
        /// Lấy tất cả đơn hàng của nông dân (nông dân là người bán)
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

                var data = _donHangService.GetByNongDan(maNongDan);

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
        /// Lấy chi tiết đơn hàng
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

                if (data == null)
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
        /// Xác nhận hoặc từ chối đơn hàng
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <param name="dto">Trạng thái mới</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("xac-nhan/{id}")]
        public IActionResult XacNhan(int id, [FromBody] UpdateTrangThaiDTO dto)
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

                if (string.IsNullOrEmpty(dto.TrangThai))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Trạng thái không được để trống"
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
