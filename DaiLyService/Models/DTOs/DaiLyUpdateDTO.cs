using System.ComponentModel.DataAnnotations;

namespace DaiLyService.Models.DTOs
{
    public class DaiLyUpdateDTO
    {
        [Required(ErrorMessage = "Tên đại lý là bắt buộc")]
        [StringLength(255, ErrorMessage = "Tên đại lý không được vượt quá 255 ký tự")]
        public string TenDaiLy { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [StringLength(500, ErrorMessage = "Địa chỉ không được vượt quá 500 ký tự")]
        public string DiaChi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [StringLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string? MoTa { get; set; }
    }
}