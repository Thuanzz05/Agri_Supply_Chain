namespace NongDanService.Models.DTOs
{
    public class TrangTraiDTO
    {
        public int MaTrangTrai { get; set; }
        public int MaNongDan { get; set; }
        public string TenTrangTrai { get; set; } = string.Empty;
        public string? DiaChi { get; set; }
        public string? SoChungNhan { get; set; }
        public string? TenNongDan { get; set; } // Thông tin nông dân
        public string? HinhAnh { get; set; }
    }
}