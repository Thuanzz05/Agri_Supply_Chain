using Microsoft.AspNetCore.Mvc;
using DaiLyService.Models.DTOs;
using DaiLyService.Services;

namespace DaiLyService.Controllers
{
    [Route("api/kiem-dinh")]
    [ApiController]
    public class KiemDinhController : ControllerBase
    {
        private readonly IKiemDinhService _kiemDinhService;
        private readonly ILogger<KiemDinhController> _logger;

        public KiemDinhController(IKiemDinhService kiemDinhService, ILogger<KiemDinhController> logger)
        {
            _kiemDinhService = kiemDinhService;
            _logger = logger;
        }

        /// <summary>
        /// Test endpoint - Lấy danh sách lô hàng cần kiểm định theo đại lý
        /// </summary>
        /// <param name="maDaiLy">Mã đại lý</param>
        /// <returns>Danh sách lô hàng với trạng thái kiểm định</returns>
        [HttpGet("get-lo-hang-by-dai-ly/{maDaiLy}")]
        public IActionResult GetLoHangByDaiLy(int maDaiLy)
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

                var data = _kiemDinhService.GetLoHangByDaiLy(maDaiLy);
                
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách lô hàng kiểm định thành công",
                    data = data,
                    count = data.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetLoHangByDaiLy for agent {MaDaiLy}", maDaiLy);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy tất cả lô hàng available (để tạo đơn hàng)
        /// </summary>
        /// <returns>Danh sách tất cả lô hàng có sẵn</returns>
        [HttpGet("get-all-lo-hang-available")]
        public IActionResult GetAllLoHangAvailable()
        {
            try
            {
                var data = _kiemDinhService.GetAllLoHangAvailable();
                
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách lô hàng thành công",
                    data = data,
                    count = data.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllLoHangAvailable");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy tất cả kiểm định
        /// </summary>
        /// <returns>Danh sách kiểm định</returns>
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                var data = _kiemDinhService.GetAll();
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách kiểm định thành công",
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
        /// Lấy kiểm định theo lô nông sản
        /// </summary>
        /// <param name="maLo">Mã lô nông sản</param>
        /// <returns>Danh sách kiểm định của lô</returns>
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

                var data = _kiemDinhService.GetByLo(maLo);
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách kiểm định theo lô thành công",
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
        /// Lấy kiểm định theo kết quả
        /// </summary>
        /// <param name="ketQua">Kết quả kiểm định (dat, khong_dat)</param>
        /// <returns>Danh sách kiểm định theo kết quả</returns>
        [HttpGet("get-by-ket-qua/{ketQua}")]
        public IActionResult GetByKetQua(string ketQua)
        {
            try
            {
                if (string.IsNullOrEmpty(ketQua) || (ketQua != "dat" && ketQua != "khong_dat"))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Kết quả phải là 'dat' hoặc 'khong_dat'"
                    });
                }

                var data = _kiemDinhService.GetByKetQua(ketQua);
                return Ok(new
                {
                    success = true,
                    message = $"Lấy danh sách kiểm định kết quả '{ketQua}' thành công",
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
        /// Lấy kiểm định theo ID
        /// </summary>
        /// <param name="id">Mã kiểm định</param>
        /// <returns>Thông tin kiểm định</returns>
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
                        message = "ID kiểm định không hợp lệ"
                    });
                }

                var data = _kiemDinhService.GetById(id);
                if (data == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy kiểm định"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin kiểm định thành công",
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
        /// Tạo kiểm định mới
        /// </summary>
        /// <param name="dto">Thông tin kiểm định</param>
        /// <returns>Kết quả tạo kiểm định</returns>
        [HttpPost("create")]
        public IActionResult Create([FromBody] KiemDinhCreateDTO dto)
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

                var maKiemDinh = _kiemDinhService.Create(dto);
                return Ok(new
                {
                    success = true,
                    message = "Tạo kiểm định thành công",
                    data = new { MaKiemDinh = maKiemDinh }
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
        /// Cập nhật kiểm định
        /// </summary>
        /// <param name="id">Mã kiểm định</param>
        /// <param name="dto">Thông tin cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] KiemDinhUpdateDTO dto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID kiểm định không hợp lệ"
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

                var result = _kiemDinhService.Update(id, dto);
                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Cập nhật kiểm định thành công"
                    });
                }

                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy kiểm định để cập nhật"
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
        /// Xóa kiểm định
        /// </summary>
        /// <param name="id">Mã kiểm định</param>
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
                        message = "ID kiểm định không hợp lệ"
                    });
                }

                var result = _kiemDinhService.Delete(id);
                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Xóa kiểm định thành công"
                    });
                }

                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy kiểm định để xóa"
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
        /// Thống kê kiểm định theo đại lý
        /// </summary>
        /// <param name="maDaiLy">Mã đại lý</param>
        /// <returns>Thống kê kiểm định</returns>
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

                var stats = _kiemDinhService.GetStatsByDaiLy(maDaiLy);
                return Ok(new
                {
                    success = true,
                    message = "Lấy thống kê kiểm định thành công",
                    data = stats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetStats for agent {MaDaiLy}", maDaiLy);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Thống kê kiểm định theo kết quả
        /// </summary>
        /// <returns>Thống kê kiểm định</returns>
        [HttpGet("thong-ke")]
        public IActionResult ThongKe()
        {
            try
            {
                var dat = _kiemDinhService.CountByKetQua("dat");
                var khongDat = _kiemDinhService.CountByKetQua("khong_dat");

                return Ok(new
                {
                    success = true,
                    message = "Lấy thống kê kiểm định thành công",
                    data = new
                    {
                        Dat = dat,
                        KhongDat = khongDat,
                        Tong = dat + khongDat,
                        TyLeDat = dat + khongDat > 0 ? Math.Round((double)dat / (dat + khongDat) * 100, 2) : 0
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