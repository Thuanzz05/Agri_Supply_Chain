using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NongDanService.Models.DTOs
{
    public class NongDanUpdateDTO
    {
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public string? HoTen { get; set; }

        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        [RegularExpression(@"^[0-9+\-\s()]*$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? SoDienThoai { get; set; }

        [StringLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [StringLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự")]
        public string? DiaChi { get; set; }

        [StringLength(255, ErrorMessage = "Facebook khong duoc vuot qua 255 ky tu")]
        [JsonPropertyName("facebook")]
        public string? Facebook { get; set; }

        [StringLength(255, ErrorMessage = "TikTok khong duoc vuot qua 255 ky tu")]
        [JsonPropertyName("tiktok")]
        public string? TikTok { get; set; }

        public string? AnhDaiDien { get; set; }
    }
}
