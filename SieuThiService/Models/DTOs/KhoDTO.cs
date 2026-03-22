namespace SieuThiService.Models.DTOs
{
    public class KhoDTO
    {
        public int MaKho { get; set; }
        public string TenKho { get; set; } = string.Empty;
        public string LoaiKho { get; set; } = string.Empty; // 'daily', 'sieuthi', 'trung_gian'
        public int MaChuSoHuu { get; set; }
        public string LoaiChuSoHuu { get; set; } = string.Empty; // 'daily', 'sieuthi'
        public string? DiaChi { get; set; }
        
        // Thông tin liên quan
        public string? TenChuSoHuu { get; set; } // Tên đại lý hoặc siêu thị
    }
}