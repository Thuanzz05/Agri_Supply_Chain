using System.ComponentModel.DataAnnotations;

namespace DaiLyService.Models.DTOs
{
    public class VanChuyenCreateDTO
    {
        [Required(ErrorMessage = "Mã lô là bắt buộc")]
        public int MaLo { get; set; }

        [Required(ErrorMessage = "Điểm đi là bắt buộc")]
        [StringLength(255, ErrorMessage = "Điểm đi không được vượt quá 255 ký tự")]
        public string DiemDi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Điểm đến là bắt buộc")]
        [StringLength(255, ErrorMessage = "Điểm đến không được vượt quá 255 ký tự")]
        public string DiemDen { get; set; } = string.Empty;

        public DateTime? NgayBatDau { get; set; } // Nếu null sẽ dùng DateTime.Now
    }
}