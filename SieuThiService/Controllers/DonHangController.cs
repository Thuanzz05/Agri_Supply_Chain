using Microsoft.AspNetCore.Mvc;
using SieuThiService.Services;

namespace SieuThiService.Controllers
{
    [Route("api/don-hang")]
    [ApiController]
    public class DonHangController : ControllerBase
    {
        private readonly IDonHangService _donHangService;

        public DonHangController(IDonHangService donHangService)
        {
            _donHangService = donHangService;
        }

        /// <summary>
        /// Lấy danh sách đơn hàng của siêu thị
        /// </summary>
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

                var data = _donHangService.GetBySieuThi(maSieuThi);
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
        /// Xác nhận đơn hàng (chuyển từ cho_xac_nhan sang dang_van_chuyen)
        /// </summary>
        [HttpPut("xac-nhan/{id}")]
        public IActionResult XacNhan(int id)
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

                var result = _donHangService.UpdateTrangThai(id, "dang_van_chuyen");
                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Xác nhận đơn hàng thành công. Hệ thống đã tự động tạo phiếu vận chuyển."
                    });
                }

                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy đơn hàng để xác nhận"
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
        /// Hủy đơn hàng
        /// </summary>
        [HttpPut("huy/{id}")]
        public IActionResult Huy(int id)
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

                var result = _donHangService.UpdateTrangThai(id, "da_huy");
                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Hủy đơn hàng thành công"
                    });
                }

                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy đơn hàng để hủy"
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