using DaiLyService.Models.DTOs;

namespace DaiLyService.Services
{
    public interface ITonKhoService
    {
        List<TonKhoDTO> GetAll();
        List<TonKhoDTO> GetByKho(int maKho);
        TonKhoDTO? GetByKhoAndLo(int maKho, int maLo);
        bool Create(int maKho, int maLo, decimal soLuong);
        bool UpdateSoLuong(int maKho, int maLo, decimal soLuongMoi);
        bool Delete(int maKho, int maLo);
    }
}