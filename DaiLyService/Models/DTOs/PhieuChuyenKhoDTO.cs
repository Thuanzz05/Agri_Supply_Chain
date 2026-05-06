namespace DaiLyService.Models.DTOs
{
    public class PhieuChuyenKhoDTO
    {
        public int MaPhieu { get; set; }
        public int MaKhoNguon { get; set; }
        public int MaKhoDich { get; set; }
        public int MaLo { get; set; }
        public decimal SoLuong { get; set; }
        public DateTime NgayChuyen { get; set; }
        public string? GhiChu { get; set; }

        public string? TenKhoNguon { get; set; }
        public string? TenKhoDich { get; set; }
        public string? TenSanPham { get; set; }
        public string? DonViTinh { get; set; }
        public string? MaQR { get; set; }
    }
}

