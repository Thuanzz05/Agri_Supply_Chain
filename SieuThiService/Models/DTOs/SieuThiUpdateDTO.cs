using System.ComponentModel.DataAnnotations;

namespace SieuThiService.Models.DTOs
{
    public class SieuThiUpdateDTO
    {
        [Required(ErrorMessage = "Tên siêu thị là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên siêu thị không được vượt quá 100 ký tự")]
        public string TenSieuThi { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public string? SoDienThoai { get; set; }

        [StringLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự")]
        public string? DiaChi { get; set; }
    }
}