namespace DaiLyService.Models.DTOs
{
    public class DaiLyDTO
    {
        public int MaDaiLy { get; set; }
        public int MaTaiKhoan { get; set; }
        public string TenDaiLy { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public DateTime NgayTao { get; set; }
        
        // Thông tin liên quan
        public string? TenDangNhap { get; set; }
        public string? HoTen { get; set; }
    }
}