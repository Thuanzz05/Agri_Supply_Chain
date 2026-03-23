using System.ComponentModel.DataAnnotations;

namespace DaiLyService.Models.DTOs
{
    public class VanChuyenUpdateDTO
    {
        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public string TrangThai { get; set; } = string.Empty; // 'dang_van_chuyen', 'hoan_thanh'

        public DateTime? NgayKetThuc { get; set; } // Nếu null và trạng thái = 'hoan_thanh' sẽ dùng DateTime.Now
    }
}