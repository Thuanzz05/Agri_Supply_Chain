using Microsoft.AspNetCore.Mvc;
using DaiLyService.Models.DTOs;
using DaiLyService.Services;

namespace DaiLyService.Controllers
{
    [Route("api/van-chuyen")]
    [ApiController]
    public class VanChuyenController : ControllerBase
    {
        private readonly IVanChuyenService _vanChuyenService;

        public VanChuyenController(IVanChuyenService vanChuyenService)
        {
            _vanChuyenService = vanChuyenService;
        }

        /// <summary>
        /// Lấy tất cả vận chuyển
        /// </summary>
        /// <returns>Danh sách vận chuyển</returns>
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                var data = _vanChuyenService.GetAll();
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách vận chuyển thành công",
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
        /// Lấy vận chuyển theo trạng thái
        /// </summary>
        /// <param name="trangThai">Trạng thái (dang_van_chuyen, hoan_thanh)</param>
        /// <returns>Danh sách vận chuyển</returns>
        [HttpGet("get-by-trang-thai/{trangThai}")]
        public IActionResult GetByTrangThai(string trangThai)
        {
            try
            {
                if (string.IsNullOrEmpty(trangThai))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Trạng thái không được để trống"
                    });
                }

                var data = _vanChuyenService.GetByTrangThai(trangThai);
                return Ok(new
                {
                    success = true,
                    message = $"Lấy danh sách vận chuyển trạng thái '{trangThai}' thành công",
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
        /// Lấy vận chuyển theo đại lý
        /// </summary>
        /// <param name="maDaiLy">Mã đại lý</param>
        /// <returns>Danh sách vận chuyển của đại lý</returns>
        [HttpGet("get-by-daily/{maDaiLy}")]
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

                var data = _vanChuyenService.GetByDaiLy(maDaiLy);
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách vận chuyển theo đại lý thành công",
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
        /// Lấy vận chuyển theo lô nông sản
        /// </summary>
        /// <param name="maLo">Mã lô nông sản</param>
        /// <returns>Danh sách vận chuyển</returns>
        [HttpGet("get-by-lo/{maLo}")]
        public IActionResult GetByLo(int maLo)
        {
            try
            {
                if (maLo <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Mã lô không hợp lệ"
                    });
                }

                var data = _vanChuyenService.GetByLo(maLo);
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách vận chuyển theo lô thành công",
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
        /// Lấy vận chuyển theo ID
        /// </summary>
        /// <param name="id">Mã vận chuyển</param>
        /// <returns>Thông tin vận chuyển</returns>
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
                        message = "ID vận chuyển không hợp lệ"
                    });
                }

                var data = _vanChuyenService.GetById(id);
                if (data == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy vận chuyển"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin vận chuyển thành công",
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
        /// Tạo vận chuyển mới
        /// </summary>
        /// <param name="dto">Thông tin vận chuyển</param>
        /// <returns>Kết quả tạo vận chuyển</returns>
        [HttpPost("create")]
        public IActionResult Create([FromBody] VanChuyenCreateDTO dto)
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

                var maVanChuyen = _vanChuyenService.Create(dto);
                return Ok(new
                {
                    success = true,
                    message = "Tạo vận chuyển thành công",
                    data = new { MaVanChuyen = maVanChuyen }
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
        /// Cập nhật trạng thái vận chuyển
        /// </summary>
        /// <param name="id">Mã vận chuyển</param>
        /// <param name="dto">Trạng thái mới</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("update-trang-thai/{id}")]
        public IActionResult UpdateTrangThai(int id, [FromBody] VanChuyenUpdateDTO dto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID vận chuyển không hợp lệ"
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

                if (dto.TrangThai == "hoan_thanh" && (!dto.MaKhoDich.HasValue || dto.MaKhoDich <= 0))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Vui lòng chọn kho đích (MaKhoDich) khi hoàn thành vận chuyển"
                    });
                }

                var result = _vanChuyenService.UpdateTrangThai(id, dto.TrangThai, dto.NgayKetThuc, dto.MaKhoDich);
                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Cập nhật trạng thái vận chuyển thành công"
                    });
                }

                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy vận chuyển để cập nhật"
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
        /// Hoàn thành vận chuyển
        /// </summary>
        /// <param name="id">Mã vận chuyển</param>
        /// <returns>Kết quả hoàn thành</returns>
        [HttpPut("hoan-thanh/{id}")]
        public IActionResult HoanThanh(int id, [FromQuery] int maKhoDich)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID vận chuyển không hợp lệ"
                    });
                }

                if (maKhoDich <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Vui lòng chọn kho đích (maKhoDich)"
                    });
                }

                var result = _vanChuyenService.UpdateTrangThai(id, "hoan_thanh", DateTime.Now, maKhoDich);
                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Hoàn thành vận chuyển thành công"
                    });
                }

                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy vận chuyển để hoàn thành"
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
        /// Xóa vận chuyển
        /// </summary>
        /// <param name="id">Mã vận chuyển</param>
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
                        message = "ID vận chuyển không hợp lệ"
                    });
                }

                var result = _vanChuyenService.Delete(id);
                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Xóa vận chuyển thành công"
                    });
                }

                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy vận chuyển để xóa"
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
        /// Thống kê vận chuyển theo đại lý
        /// </summary>
        /// <param name="maDaiLy">Mã đại lý</param>
        /// <returns>Thống kê vận chuyển</returns>
        [HttpGet("stats/{maDaiLy}")]
        public IActionResult GetStats(int maDaiLy)
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

                var stats = _vanChuyenService.GetStatsByDaiLy(maDaiLy);
                return Ok(new
                {
                    success = true,
                    message = "Lấy thống kê vận chuyển thành công",
                    data = stats
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
        /// Thống kê vận chuyển theo trạng thái
        /// </summary>
        /// <returns>Thống kê vận chuyển</returns>
        [HttpGet("thong-ke")]
        public IActionResult ThongKe()
        {
            try
            {
                var dangVanChuyen = _vanChuyenService.CountByTrangThai("dang_van_chuyen");
                var hoanThanh = _vanChuyenService.CountByTrangThai("hoan_thanh");

                return Ok(new
                {
                    success = true,
                    message = "Lấy thống kê vận chuyển thành công",
                    data = new
                    {
                        DangVanChuyen = dangVanChuyen,
                        HoanThanh = hoanThanh,
                        Tong = dangVanChuyen + hoanThanh
                    }
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