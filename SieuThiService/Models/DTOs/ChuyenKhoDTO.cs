namespace SieuThiService.Models.DTOs
{
    public class ChuyenKhoDTO
    {
        public int MaPhieu { get; set; }
        public int MaKhoNguon { get; set; }
        public int MaKhoDich { get; set; }
        public int MaLo { get; set; }
        public decimal SoLuong { get; set; }
        public DateTime NgayTao { get; set; }
        public string? NguoiTao { get; set; }
        public string? GhiChu { get; set; }
        
        // Thông tin bổ sung
        public string? TenKhoNguon { get; set; }
        public string? TenKhoDich { get; set; }
        public string? TenSanPham { get; set; }
    }
}
