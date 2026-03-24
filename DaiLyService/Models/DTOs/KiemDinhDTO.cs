using System.ComponentModel.DataAnnotations;

namespace DaiLyService.Models.DTOs
{
    public class KiemDinhDTO
    {
        public int MaKiemDinh { get; set; }
        public int MaLo { get; set; }
        public string NguoiKiemDinh { get; set; } = string.Empty;
        public DateTime NgayKiemDinh { get; set; }
        public string KetQua { get; set; } = string.Empty; // 'dat', 'khong_dat'
        public string? BienBanKiemTra { get; set; }
        public string? ChuKySo { get; set; }
        
        // Thông tin bổ sung từ JOIN
        public string? TenSanPham { get; set; }
        public string? DonViTinh { get; set; }
        public string? MaQR { get; set; }
        public DateTime? NgayThuHoach { get; set; }
        public DateTime? HanSuDung { get; set; }
        public decimal? SoLuongLo { get; set; }
    }
}