namespace SieuThiService.Models.DTOs
{
    public class KhoStatsDTO
    {
        public int TongSanPham { get; set; }
        public int SoLuongSanPhamSapHet { get; set; }
        public decimal TongSoLuongTonKho { get; set; }
        public List<SanPhamTonKhoDTO>? DanhSachSanPhamTonKho { get; set; }
        public List<SanPhamSapHetDTO>? DanhSachSanPhamSapHet { get; set; }
    }

    public class SanPhamTonKhoDTO
    {
        public string? TenSanPham { get; set; }
        public decimal SoLuong { get; set; }
        public string? DonViTinh { get; set; }
        public DateTime? HanSuDung { get; set; }
    }

    public class SanPhamSapHetDTO
    {
        public string? TenSanPham { get; set; }
        public decimal SoLuong { get; set; }
        public string? DonViTinh { get; set; }
        public string? TenKho { get; set; }
    }
}
