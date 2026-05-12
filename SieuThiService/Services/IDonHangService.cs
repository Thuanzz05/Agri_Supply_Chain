namespace SieuThiService.Services
{
    public interface IDonHangService
    {
        List<Models.DTOs.DonHangDTO> GetBySieuThi(int maSieuThi);
        Models.DTOs.DonHangDTO? GetById(int maDonHang);
        bool UpdateTrangThai(int maDonHang, string trangThai);
    }
}