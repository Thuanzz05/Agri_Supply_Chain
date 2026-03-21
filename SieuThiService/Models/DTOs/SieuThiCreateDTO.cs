using System.ComponentModel.DataAnnotations;

namespace SieuThiService.Models.DTOs
{
    public class SieuThiCreateDTO
    {
        [Required(ErrorMessage = "Tên siêu thị là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên siêu thị không được vượt quá 100 ký tự")]
        public string TenSieuThi { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public string? SoDienThoai { get; set; }

        [StringLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự")]
        public string? DiaChi { get; set; }

        // Thông tin tài khoản
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6-255 ký tự")]
        public string MatKhau { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string? Email { get; set; }
    }
}