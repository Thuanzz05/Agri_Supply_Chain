using NongDanService.Models.DTOs;

namespace NongDanService.Services
{
    public interface IDonHangService
    {
        List<DonHangDTO> GetByNongDan(int maNongDan);
        DonHangDTO? GetById(int maDonHang);
        bool UpdateTrangThai(int maDonHang, string trangThai);
    }
}
