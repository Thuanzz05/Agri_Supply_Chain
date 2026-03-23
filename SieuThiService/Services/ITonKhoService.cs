using SieuThiService.Models.DTOs;

namespace SieuThiService.Services
{
    public interface ITonKhoService
    {
        List<TonKhoDTO> GetAll();
        List<TonKhoDTO> GetByKho(int maKho);
        List<TonKhoDTO> GetBySieuThi(int maSieuThi);
        TonKhoDTO? GetByKhoAndLo(int maKho, int maLo);
        bool UpdateSoLuong(int maKho, int maLo, decimal soLuongMoi);
        bool Delete(int maKho, int maLo);
    }
}