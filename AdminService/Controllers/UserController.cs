using Microsoft.AspNetCore.Mvc;
using AdminService.Services;

namespace AdminService.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _service;

        public UserController(UserService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy danh sách tất cả người dùng - Cần token admin
        /// </summary>
        [HttpGet]
        public IActionResult GetAllUsers([FromQuery] string? loaiNguoiDung = null)
        {
            var (success, message, data, total) = _service.GetAllUsers(loaiNguoiDung);

            if (!success)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server: " + message
                });
            }

            return Ok(new
            {
                success = true,
                message,
                data,
                total
            });
        }

        /// <summary>
        /// Lấy thông tin chi tiết nông dân - Cần token admin
        /// </summary>
        [HttpGet("nongdan/{id}")]
        public IActionResult GetNongDanDetail(int id)
        {
            var (success, message, data) = _service.GetNongDanDetail(id);

            if (!success)
            {
                if (message.Contains("Không tìm thấy"))
                {
                    return NotFound(new { success = false, message });
                }
                return StatusCode(500, new { success = false, message = "Lỗi server: " + message });
            }

            return Ok(new { success = true, message, data });
        }

        /// <summary>
        /// Lấy thông tin chi tiết đại lý - Cần token admin
        /// </summary>
        [HttpGet("daily/{id}")]
        public IActionResult GetDaiLyDetail(int id)
        {
            var (success, message, data) = _service.GetDaiLyDetail(id);

            if (!success)
            {
                if (message.Contains("Không tìm thấy"))
                {
                    return NotFound(new { success = false, message });
                }
                return StatusCode(500, new { success = false, message = "Lỗi server: " + message });
            }

            return Ok(new { success = true, message, data });
        }

        /// <summary>
        /// Lấy thông tin chi tiết siêu thị - Cần token admin
        /// </summary>
        [HttpGet("sieuthi/{id}")]
        public IActionResult GetSieuThiDetail(int id)
        {
            var (success, message, data) = _service.GetSieuThiDetail(id);

            if (!success)
            {
                if (message.Contains("Không tìm thấy"))
                {
                    return NotFound(new { success = false, message });
                }
                return StatusCode(500, new { success = false, message = "Lỗi server: " + message });
            }

            return Ok(new { success = true, message, data });
        }

        /// <summary>
        /// Tìm kiếm người dùng - Cần token admin
        /// </summary>
        [HttpGet("search")]
        public IActionResult SearchUsers([FromQuery] string keyword, [FromQuery] string? loaiNguoiDung = null)
        {
            var (success, message, data) = _service.SearchUsers(keyword, loaiNguoiDung);

            if (!success)
            {
                if (message.Contains("không được để trống"))
                {
                    return BadRequest(new { success = false, message });
                }
                return StatusCode(500, new { success = false, message = "Lỗi server: " + message });
            }

            return Ok(new { success = true, message, data });
        }
    }
}
