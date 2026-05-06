using System.ComponentModel.DataAnnotations;

namespace DaiLyService.Models.DTOs
{
    public class DonHangUpdateDTO
    {
        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        [RegularExpression("^(cho_xac_nhan|cho_kiem_dinh|dang_van_chuyen|hoan_thanh|tra_hang|da_huy)$",
            ErrorMessage = "Trạng thái không hợp lệ")]
        public string TrangThai { get; set; } = string.Empty;
    }
}