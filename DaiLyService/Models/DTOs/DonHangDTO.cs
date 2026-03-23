namespace DaiLyService.Models.DTOs
{
    public class DonHangDTO
    {
        public int MaDonHang { get; set; }
        public string LoaiDon { get; set; } = string.Empty; // 'nongdan_to_daily', 'daily_to_sieuthi'
        public int MaNguoiBan { get; set; }
        public string LoaiNguoiBan { get; set; } = string.Empty; // 'nongdan', 'daily'
        public int MaNguoiMua { get; set; }
        public string LoaiNguoiMua { get; set; } = string.Empty; // 'daily', 'sieuthi'
        public DateTime NgayDat { get; set; }
        public string TrangThai { get; set; } = string.Empty; // 'cho_xac_nhan', 'hoan_thanh', 'da_huy'
        public decimal TongGiaTri { get; set; }
        
        // Thông tin liên quan
        public string? TenNguoiBan { get; set; }
        public string? TenNguoiMua { get; set; }
        public List<ChiTietDonHangDTO>? ChiTietDonHang { get; set; }
    }
}