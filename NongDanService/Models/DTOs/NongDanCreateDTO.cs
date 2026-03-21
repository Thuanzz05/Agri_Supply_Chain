using System.ComponentModel.DataAnnotations;

namespace NongDanService.Models.DTOs
{
    public class NongDanCreateDTO
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6-255 ký tự")]
        public string MatKhau { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string? Email { get; set; }

        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public string? HoTen { get; set; }

        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        [RegularExpression(@"^[0-9+\-\s()]*$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? SoDienThoai { get; set; }

        [StringLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự")]
        public string? DiaChi { get; set; }
    }
}