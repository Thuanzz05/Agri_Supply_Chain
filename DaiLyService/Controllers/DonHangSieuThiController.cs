using Microsoft.AspNetCore.Mvc;
using DaiLyService.Models.DTOs;
using DaiLyService.Services;

namespace DaiLyService.Controllers
{
    [Route("api/don-hang-sieu-thi")]
    [ApiController]
    public class DonHangSieuThiController : ControllerBase
    {
        private readonly IDonHangService _donHangService;

        public DonHangSieuThiController(IDonHangService donHangService)
        {
            _donHangService = donHangService;
        }

        /// <summary>
        /// Lấy tất cả đơn hàng bán cho siêu thị
        /// </summary>
        /// <returns>Danh sách đơn hàng bán cho siêu thị</returns>
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                var data = _donHangService.GetAll()
                    .Where(dh => dh.LoaiDon == "daily_to_sieuthi")
                    .ToList();
                
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách đơn hàng bán cho siêu thị thành công",
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
        /// Lấy đơn hàng bán cho siêu thị theo đại lý
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

                var data = _donHangService.GetByNguoiBan(maDaiLy, "daily")
                    .Where(dh => dh.LoaiDon == "daily_to_sieuthi")
                    .ToList();
                
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách đơn hàng bán cho siêu thị thành công",
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
        /// Tạo đơn hàng bán cho siêu thị
        /// </summary>
        /// <param name="dto">Thông tin đơn hàng</param>
        /// <returns>Kết quả tạo đơn hàng</returns>
        [HttpPost("create")]
        public IActionResult Create([FromBody] DonHangCreateDTO dto)
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

                // Đảm bảo là đơn hàng daily_to_sieuthi
                if (dto.LoaiDon != "daily_to_sieuthi" || dto.LoaiNguoiBan != "daily" || dto.LoaiNguoiMua != "sieuthi")
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Loại đơn hàng không hợp lệ cho endpoint này"
                    });
                }

                var maDonHang = _donHangService.Create(dto);
                return Ok(new
                {
                    success = true,
                    message = "Tạo đơn hàng bán cho siêu thị thành công",
                    data = new { MaDonHang = maDonHang }
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
                if (data == null || data.LoaiDon != "daily_to_sieuthi")
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
        /// Cập nhật trạng thái đơn hàng
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <param name="dto">Trạng thái mới</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("update-trang-thai/{id}")]
        public IActionResult UpdateTrangThai(int id, [FromBody] DonHangUpdateDTO dto)
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

                // Kiểm tra đơn hàng tồn tại và thuộc loại daily_to_sieuthi
                var donHang = _donHangService.GetById(id);
                if (donHang == null || donHang.LoaiDon != "daily_to_sieuthi")
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