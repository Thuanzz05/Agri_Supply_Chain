using Microsoft.Data.SqlClient;
using SieuThiService.Models.DTOs;

namespace SieuThiService.Data
{
    public interface IDonHangRepository
    {
        List<DonHangDTO> GetBySieuThi(int maSieuThi);
        DonHangDTO? GetById(int maDonHang);
        bool UpdateTrangThai(int maDonHang, string trangThai);
    }
}