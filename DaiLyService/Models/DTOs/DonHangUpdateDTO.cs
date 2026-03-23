using System.ComponentModel.DataAnnotations;

namespace DaiLyService.Models.DTOs
{
    public class DonHangUpdateDTO
    {
        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public string TrangThai { get; set; } = string.Empty; // 'cho_xac_nhan', 'hoan_thanh', 'da_huy'
    }
}