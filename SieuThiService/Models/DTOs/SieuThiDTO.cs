namespace SieuThiService.Models.DTOs
{
    public class SieuThiDTO
    {
        public int MaSieuThi { get; set; }
        public int MaTaiKhoan { get; set; }
        public string TenSieuThi { get; set; } = string.Empty;
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }
        
        // Thông tin từ bảng TaiKhoan
        public string? TenDangNhap { get; set; }
        public string? Email { get; set; }
        public DateTime? NgayTao { get; set; }
    }
}