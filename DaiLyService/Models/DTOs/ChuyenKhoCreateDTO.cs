using System.ComponentModel.DataAnnotations;

namespace DaiLyService.Models.DTOs
{
    public class ChuyenKhoCreateDTO
    {
        [Required(ErrorMessage = "Mã kho nguồn là bắt buộc")]
        public int MaKhoNguon { get; set; }

        [Required(ErrorMessage = "Mã kho đích là bắt buộc")]
        public int MaKhoDich { get; set; }

        [Required(ErrorMessage = "Mã lô là bắt buộc")]
        public int MaLo { get; set; }

        [Required(ErrorMessage = "Số lượng chuyển là bắt buộc")]
        [Range(0.0000001, double.MaxValue, ErrorMessage = "Số lượng chuyển phải lớn hơn 0")]
        public decimal SoLuong { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string? GhiChu { get; set; }
    }
}

