namespace DaiLyService.Models.DTOs
{
    public class VanChuyenDTO
    {
        public int MaVanChuyen { get; set; }
        public int MaLo { get; set; }
        public string DiemDi { get; set; } = string.Empty;
        public string DiemDen { get; set; } = string.Empty;
        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string TrangThai { get; set; } = string.Empty; // 'dang_van_chuyen', 'hoan_thanh'
        
        // Thông tin liên quan
        public string? TenSanPham { get; set; }
        public string? DonViTinh { get; set; }
        public string? MaQR { get; set; }
        public decimal? SoLuongLo { get; set; }
        public DateTime? NgayThuHoach { get; set; }
        public DateTime? HanSuDung { get; set; }
    }
}