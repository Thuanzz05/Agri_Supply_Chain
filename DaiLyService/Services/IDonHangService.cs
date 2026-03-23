using DaiLyService.Models.DTOs;

namespace DaiLyService.Services
{
    public interface IDonHangService
    {
        List<DonHangDTO> GetAll();
        List<DonHangDTO> GetByDaiLy(int maDaiLy);
        List<DonHangDTO> GetByNguoiBan(int maNguoiBan, string loaiNguoiBan);
        List<DonHangDTO> GetByNguoiMua(int maNguoiMua, string loaiNguoiMua);
        DonHangDTO? GetById(int maDonHang);
        int Create(DonHangCreateDTO dto);
        bool UpdateTrangThai(int maDonHang, string trangThai);
        bool Delete(int maDonHang);
        List<ChiTietDonHangDTO> GetChiTietDonHang(int maDonHang);
    }
}