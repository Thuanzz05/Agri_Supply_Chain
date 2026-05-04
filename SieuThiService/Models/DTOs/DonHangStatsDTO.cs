namespace SieuThiService.Models.DTOs
{
    public class DonHangStatsDTO
    {
        public int TongDonHang { get; set; }
        public int DonChoXacNhan { get; set; }
        public int DonHoanThanh { get; set; }
        public int DonDaHuy { get; set; }
        public decimal TongGiaTriDonHang { get; set; }
        public List<DonHangTheoThangDTO>? DonHangTheoThang { get; set; }
        public List<DonHangGanDayDTO>? DonHangGanDay { get; set; }
    }

    public class DonHangTheoThangDTO
    {
        public int Thang { get; set; }
        public int Nam { get; set; }
        public int SoDonHang { get; set; }
        public decimal TongGiaTri { get; set; }
    }
}
