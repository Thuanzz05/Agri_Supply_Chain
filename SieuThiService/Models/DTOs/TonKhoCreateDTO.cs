using System.ComponentModel.DataAnnotations;

namespace SieuThiService.Models.DTOs
{
    public class TonKhoCreateDTO
    {
        [Required(ErrorMessage = "Mã kho là bắt buộc")]
        public int MaKho { get; set; }

        [Required(ErrorMessage = "Mã lô là bắt buộc")]
        public int MaLo { get; set; }

        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public decimal SoLuong { get; set; }
    }
}