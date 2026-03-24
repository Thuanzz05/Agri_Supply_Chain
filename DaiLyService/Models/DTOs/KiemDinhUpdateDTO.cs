using System.ComponentModel.DataAnnotations;

namespace DaiLyService.Models.DTOs
{
    public class KiemDinhUpdateDTO
    {
        [Required(ErrorMessage = "Người kiểm định không được để trống")]
        [StringLength(100, ErrorMessage = "Tên người kiểm định không được vượt quá 100 ký tự")]
        public string NguoiKiemDinh { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kết quả kiểm định không được để trống")]
        [RegularExpression("^(dat|khong_dat)$", ErrorMessage = "Kết quả phải là 'dat' hoặc 'khong_dat'")]
        public string KetQua { get; set; } = string.Empty;

        [StringLength(4000, ErrorMessage = "Biên bản kiểm tra không được vượt quá 4000 ký tự")]
        public string? BienBanKiemTra { get; set; }

        [StringLength(255, ErrorMessage = "Chữ ký số không được vượt quá 255 ký tự")]
        public string? ChuKySo { get; set; }
    }
}