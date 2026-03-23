using System.ComponentModel.DataAnnotations;

namespace DaiLyService.Models.DTOs
{
    public class DonHangCreateDTO
    {
        [Required(ErrorMessage = "Loại đơn là bắt buộc")]
        public string LoaiDon { get; set; } = string.Empty; // 'nongdan_to_daily', 'daily_to_sieuthi'

        [Required(ErrorMessage = "Mã người bán là bắt buộc")]
        public int MaNguoiBan { get; set; }

        [Required(ErrorMessage = "Loại người bán là bắt buộc")]
        public string LoaiNguoiBan { get; set; } = string.Empty; // 'nongdan', 'daily'

        [Required(ErrorMessage = "Mã người mua là bắt buộc")]
        public int MaNguoiMua { get; set; }

        [Required(ErrorMessage = "Loại người mua là bắt buộc")]
        public string LoaiNguoiMua { get; set; } = string.Empty; // 'daily', 'sieuthi'

        [Required(ErrorMessage = "Chi tiết đơn hàng là bắt buộc")]
        [MinLength(1, ErrorMessage = "Đơn hàng phải có ít nhất 1 sản phẩm")]
        public List<ChiTietDonHangCreateDTO> ChiTietDonHang { get; set; } = new();
    }

    public class ChiTietDonHangCreateDTO
    {
        [Required(ErrorMessage = "Mã lô là bắt buộc")]
        public int MaLo { get; set; }

        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public decimal SoLuong { get; set; }

        [Required(ErrorMessage = "Đơn giá là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Đơn giá phải lớn hơn 0")]
        public decimal DonGia { get; set; }
    }
}