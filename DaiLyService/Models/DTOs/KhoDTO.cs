namespace DaiLyService.Models.DTOs
{
    public class KhoDTO
    {
        public int MaKho { get; set; }
        public int MaDaiLy { get; set; }
        public string TenKho { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;
        public decimal DienTich { get; set; }
        public decimal SucChua { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public DateTime NgayTao { get; set; }
        
        // Thông tin liên quan
        public string? TenDaiLy { get; set; }
    }
}