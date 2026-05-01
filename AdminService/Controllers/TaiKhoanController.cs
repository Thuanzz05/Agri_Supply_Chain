using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdminService.Services;

namespace AdminService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaiKhoanController : ControllerBase
    {
        private readonly TaiKhoanService _service;

        public TaiKhoanController(TaiKhoanService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy danh sách tất cả tài khoản - Cần token admin
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult GetAll([FromQuery] string? loaiTaiKhoan = null)
        {
            var (success, message, data, total) = _service.GetAll(loaiTaiKhoan);

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
        /// Đổi mật khẩu tài khoản - Cần token admin
        /// </summary>
        [HttpPut("{id}/change-password")]
        [Authorize(Roles = "admin")]
        public IActionResult ChangePassword(int id, [FromBody] ChangePasswordRequest request)
        {
            var (success, message) = _service.ChangePassword(id, request.MatKhauMoi);

            if (!success)
            {
                if (message.Contains("không được để trống"))
                {
                    return BadRequest(new { success = false, message });
                }
                if (message.Contains("Không tìm thấy"))
                {
                    return NotFound(new { success = false, message });
                }
                return StatusCode(500, new { success = false, message = "Lỗi server: " + message });
            }

            return Ok(new { success = true, message });
        }

        /// <summary>
        /// Khóa/Mở khóa tài khoản - Cần token admin
        /// </summary>
        [HttpPut("{id}/toggle-status")]
        [Authorize(Roles = "admin")]
        public IActionResult ToggleStatus(int id)
        {
            var (success, message) = _service.ToggleStatus(id);

            if (!success)
            {
                if (message.Contains("Không tìm thấy"))
                {
                    return NotFound(new { success = false, message });
                }
                return StatusCode(500, new { success = false, message = "Lỗi server: " + message });
            }

            return Ok(new { success = true, message });
        }

        /// <summary>
        /// Xóa tài khoản - Cần token admin
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            var (success, message) = _service.Delete(id);

            if (!success)
            {
                if (message.Contains("Không tìm thấy") || message.Contains("Không thể xóa"))
                {
                    return NotFound(new { success = false, message });
                }
                return StatusCode(500, new { success = false, message = "Lỗi server: " + message });
            }

            return Ok(new { success = true, message });
        }
    }

    // DTOs
    public class ChangePasswordRequest
    {
        public string MatKhauMoi { get; set; } = string.Empty;
    }
}