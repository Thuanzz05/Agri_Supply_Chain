using System.ComponentModel.DataAnnotations;

namespace SieuThiService.Models.DTOs
{
    public class TonKhoUpdateDTO
    {
        [Required(ErrorMessage = "Số lượng mới là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public decimal SoLuongMoi { get; set; }
    }
}