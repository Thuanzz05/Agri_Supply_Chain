using System.ComponentModel.DataAnnotations;

namespace DaiLyService.Models.DTOs
{
    public class KhoUpdateDTO
    {
        [Required(ErrorMessage = "Tên kho là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên kho không được vượt quá 100 ký tự")]
        public string TenKho { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại kho là bắt buộc")]
        [StringLength(20, ErrorMessage = "Loại kho không được vượt quá 20 ký tự")]
        public string LoaiKho { get; set; } = string.Empty; // 'daily', 'sieuthi', 'trung_gian'

        [StringLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự")]
        public string? DiaChi { get; set; }
    }
}