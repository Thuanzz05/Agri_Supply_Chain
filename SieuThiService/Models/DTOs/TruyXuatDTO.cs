using System.Text.Json.Serialization;

namespace SieuThiService.Models.DTOs
{
    public class TruyXuatLoInfoDTO
    {
        public int MaLo { get; set; }
        public string? MaQR { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public string DonViTinh { get; set; } = string.Empty;
        public decimal SoLuongBanDau { get; set; }
        public decimal SoLuongHienTai { get; set; }
        public string? NgayThuHoach { get; set; }
        public string? HanSuDung { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string? NgayTao { get; set; }
        public string TenTrangTrai { get; set; } = string.Empty;
        public string? DiaChiTrangTrai { get; set; }
        public string? SoChungNhan { get; set; }
        public string TenNongDan { get; set; } = string.Empty;
        public string? SoDienThoaiNongDan { get; set; }
        public string? DiaChiNongDan { get; set; }
        
        [JsonPropertyName("facebookNongDan")]
        public string? FacebookNongDan { get; set; }
        
        [JsonPropertyName("tiktokNongDan")]
        public string? TiktokNongDan { get; set; }
    }

    public class TruyXuatKiemDinhDTO
    {
        public int MaKiemDinh { get; set; }
        public string NguoiKiemDinh { get; set; } = string.Empty;
        public string? NgayKiemDinh { get; set; }
        public string KetQua { get; set; } = string.Empty;
        public string? BienBanKiemTra { get; set; }
        public string? ChuKySo { get; set; }
    }

    public class TruyXuatVanChuyenDTO
    {
        public int MaVanChuyen { get; set; }
        public string DiemDi { get; set; } = string.Empty;
        public string DiemDen { get; set; } = string.Empty;
        public string? NgayBatDau { get; set; }
        public string? NgayKetThuc { get; set; }
        public string TrangThai { get; set; } = string.Empty;
    }

    public class TruyXuatResultDTO : TruyXuatLoInfoDTO
    {
        public TruyXuatKiemDinhDTO? KiemDinh { get; set; }
        public List<TruyXuatVanChuyenDTO> VanChuyen { get; set; } = new List<TruyXuatVanChuyenDTO>();
    }
}
