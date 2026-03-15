using System.ComponentModel.DataAnnotations;

namespace DaiLyService.Models.DTOs
{
    public class KhoCreateDTO
    {
        [Required(ErrorMessage = "Mã đại lý là bắt buộc")]
        public int MaDaiLy { get; set; }

        [Required(ErrorMessage = "Tên kho là bắt buộc")]
        [StringLength(255, ErrorMessage = "Tên kho không được vượt quá 255 ký tự")]
        public string TenKho { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [StringLength(500, ErrorMessage = "Địa chỉ không được vượt quá 500 ký tự")]
        public string DiaChi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Diện tích là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Diện tích phải lớn hơn 0")]
        public decimal DienTich { get; set; }

        [Required(ErrorMessage = "Sức chứa là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Sức chứa phải lớn hơn 0")]
        public decimal SucChua { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string? MoTa { get; set; }
    }
}