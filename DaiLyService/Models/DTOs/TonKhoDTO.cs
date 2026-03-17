namespace DaiLyService.Models.DTOs
{
    public class TonKhoDTO
    {
        public int MaKho { get; set; }
        public int MaLo { get; set; }
        public decimal SoLuong { get; set; }
        public DateTime NgayCapNhat { get; set; }
        
        // Thông tin liên quan
        public string? TenKho { get; set; }
        public string? TenSanPham { get; set; }
        public string? DonViTinh { get; set; }
        public string? MaQR { get; set; }
    }
}