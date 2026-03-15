using System.ComponentModel.DataAnnotations;

namespace DaiLyService.Models.DTOs
{
    public class TonKhoCreateDTO
    {
        [Required(ErrorMessage = "Mã kho là bắt buộc")]
        public int MaKho { get; set; }

        [Required(ErrorMessage = "Mã lô nông sản là bắt buộc")]
        public int MaLoNongSan { get; set; }

        [Required(ErrorMessage = "Số lượng tồn là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Số lượng tồn không được âm")]
        public decimal SoLuongTon { get; set; }

        [Required(ErrorMessage = "Số lượng tối thiểu là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Số lượng tối thiểu không được âm")]
        public decimal SoLuongToiThieu { get; set; }

        [Required(ErrorMessage = "Số lượng tối đa là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số lượng tối đa phải lớn hơn 0")]
        public decimal SoLuongToiDa { get; set; }

        [StringLength(1000, ErrorMessage = "Ghi chú không được vượt quá 1000 ký tự")]
        public string? GhiChu { get; set; }
    }
}