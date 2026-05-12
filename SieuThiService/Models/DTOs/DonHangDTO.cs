namespace SieuThiService.Models.DTOs
{
    public class DonHangDTO
    {
        public int MaDonHang { get; set; }
        public string LoaiDon { get; set; } = string.Empty;
        public int MaNguoiBan { get; set; }
        public string LoaiNguoiBan { get; set; } = string.Empty;
        public int MaNguoiMua { get; set; }
        public string LoaiNguoiMua { get; set; } = string.Empty;
        public DateTime NgayDat { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public decimal TongGiaTri { get; set; }
        public string? TenNguoiBan { get; set; }
        public string? TenNguoiMua { get; set; }
    }
}
