using System.ComponentModel.DataAnnotations;

namespace NongDanService.Models.DTOs
{
    public class DonHangDTO
    {
        public int MaDonHang { get; set; }
        public string LoaiDon { get; set; } = string.Empty;
        public int MaNguoiBan { get; set; }
        public string LoaiNguoiBan { get; set; } = string.Empty;
        public int MaNguoiMua { get; set; }
        public string LoaiNguoiMua { get; set; } = string.Empty;
        public DateTime NgayDat { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public decimal TongGiaTri { get; set; }
        public string? TenNguoiBan { get; set; }
        public string? TenNguoiMua { get; set; }
        public List<ChiTietDonHangDTO>? ChiTietDonHang { get; set; }
    }

    public class ChiTietDonHangDTO
    {
        public int MaDonHang { get; set; }
        public int MaLo { get; set; }
        public decimal SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
        public string? TenSanPham { get; set; }
        public string? DonViTinh { get; set; }
        public string? MaQR { get; set; }
        public DateTime? NgayThuHoach { get; set; }
        public DateTime? HanSuDung { get; set; }
    }

    public class UpdateTrangThaiDTO
    {
        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        [RegularExpression("^(cho_kiem_dinh|da_huy|hoan_thanh)$", ErrorMessage = "Trạng thái không hợp lệ")]
        public string TrangThai { get; set; } = string.Empty;
    }
}
