using System.ComponentModel.DataAnnotations;

namespace DaiLyService.Models.DTOs
{
    public class DaiLyCreateDTO
    {
        [Required(ErrorMessage = "Mã tài khoản là bắt buộc")]
        public int MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Tên đại lý là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên đại lý không được vượt quá 100 ký tự")]
        public string TenDaiLy { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự")]
        public string? DiaChi { get; set; }

        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public string? SoDienThoai { get; set; }
    }
}