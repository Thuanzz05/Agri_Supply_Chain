using Microsoft.AspNetCore.Mvc;
using SieuThiService.Models.DTOs;
using SieuThiService.Services;

namespace SieuThiService.Controllers
{
    [Route("api/kho")]
    [ApiController]
    public class KhoController : ControllerBase
    {
        private readonly IKhoService _khoService;

        public KhoController(IKhoService khoService)
        {
            _khoService = khoService;
        }

        /// <summary>
        /// Lấy tất cả kho của siêu thị
        /// </summary>
        /// <returns>Danh sách kho siêu thị</returns>
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            try
            {
                // Chỉ lấy kho của siêu thị
                var data = _khoService.GetBySieuThi();
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách kho siêu thị thành công",
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
        /// Lấy kho theo siêu thị
        /// </summary>
        /// <param name="maSieuThi">Mã siêu thị</param>
        /// <returns>Danh sách kho của siêu thị</returns>
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

                var data = _khoService.GetBySieuThi(maSieuThi);
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách kho thành công",
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
        /// Lấy kho theo ID
        /// </summary>
        /// <param name="id">Mã kho</param>
        /// <returns>Thông tin kho</returns>
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
                        message = "ID kho không hợp lệ"
                    });
                }

                var data = _khoService.GetById(id);
                if (data == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy kho"
                    });
                }

                // Kiểm tra xem kho có thuộc siêu thị không
                if (data.LoaiChuSoHuu != "sieuthi")
                {
                    return Forbid("Kho này không thuộc quyền quản lý của siêu thị");
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin kho thành công",
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
    }
}