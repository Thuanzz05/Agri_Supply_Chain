using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DaiLyService.Models.DTOs
{
    public class DaiLyUpdateDTO
    {
        [Required(ErrorMessage = "Tên đại lý là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên đại lý không được vượt quá 100 ký tự")]
        public string TenDaiLy { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự")]
        public string? DiaChi { get; set; }

        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public string? SoDienThoai { get; set; }

        [StringLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [StringLength(255, ErrorMessage = "Facebook khong duoc vuot qua 255 ky tu")]
        [JsonPropertyName("facebook")]
        public string? Facebook { get; set; }

        [StringLength(255, ErrorMessage = "TikTok khong duoc vuot qua 255 ky tu")]
        [JsonPropertyName("tiktok")]
        public string? TikTok { get; set; }

        public string? AnhDaiDien { get; set; }
    }
}
