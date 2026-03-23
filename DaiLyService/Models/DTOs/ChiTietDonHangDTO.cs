namespace DaiLyService.Models.DTOs
{
    public class ChiTietDonHangDTO
    {
        public int MaDonHang { get; set; }
        public int MaLo { get; set; }
        public decimal SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
        
        // Thông tin liên quan
        public string? TenSanPham { get; set; }
        public string? DonViTinh { get; set; }
        public string? MaQR { get; set; }
        public DateTime? NgayThuHoach { get; set; }
        public DateTime? HanSuDung { get; set; }
    }
}