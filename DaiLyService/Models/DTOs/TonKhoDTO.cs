namespace DaiLyService.Models.DTOs
{
    public class TonKhoDTO
    {
        public int MaTonKho { get; set; }
        public int MaKho { get; set; }
        public int MaLoNongSan { get; set; }
        public decimal SoLuongTon { get; set; }
        public decimal SoLuongToiThieu { get; set; }
        public decimal SoLuongToiDa { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string? GhiChu { get; set; }
        
        // Thông tin liên quan
        public string? TenKho { get; set; }
        public string? TenSanPham { get; set; }
        public string? DonViTinh { get; set; }
        public string? MaQR { get; set; }
    }
}