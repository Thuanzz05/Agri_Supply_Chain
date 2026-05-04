namespace SieuThiService.Models.DTOs
{
    public class DashboardStatsDTO
    {
        public int TongSanPhamTrongKho { get; set; }
        public int TongDonHangThang { get; set; }
        public int SoDonChoXacNhan { get; set; }
        public int SoDonHoanThanh { get; set; }
        public List<DonHangGanDayDTO>? DonHangGanDay { get; set; }
    }

    public class DonHangGanDayDTO
    {
        public int MaDonHang { get; set; }
        public DateTime NgayDat { get; set; }
        public string? TrangThai { get; set; }
        public decimal TongGiaTri { get; set; }
        public string? TenNguoiBan { get; set; }
    }
}
