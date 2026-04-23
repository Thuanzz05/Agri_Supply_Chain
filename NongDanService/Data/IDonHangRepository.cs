using NongDanService.Models.DTOs;

namespace NongDanService.Data
{
    public interface IDonHangRepository
    {
        List<DonHangDTO> GetByNongDan(int maNongDan);
        DonHangDTO? GetById(int maDonHang);
        bool UpdateTrangThai(int maDonHang, string trangThai);
    }
}
