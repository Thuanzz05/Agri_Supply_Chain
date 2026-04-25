namespace DaiLyService.Models.DTOs
{
    public class LoHangKiemDinhDTO
    {
        public int MaLo { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public int MaNongDan { get; set; }
        public string TenNongDan { get; set; } = string.Empty;
        public decimal SoLuong { get; set; }
        public string DonViTinh { get; set; } = string.Empty;
        public DateTime? NgayThuHoach { get; set; }
        public string TrangThaiKiemDinh { get; set; } = "cho_kiem_dinh"; // cho_kiem_dinh, dat, khong_dat
        public string? KetQuaKiemDinh { get; set; }
        public DateTime? NgayKiemDinh { get; set; }
        public int? MaKiemDinh { get; set; }
    }
}
