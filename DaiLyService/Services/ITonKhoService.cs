using DaiLyService.Models.DTOs;

namespace DaiLyService.Services
{
    public interface ITonKhoService
    {
        List<TonKhoDTO> GetAll();
        List<TonKhoDTO> GetByKho(int maKho);
        List<TonKhoDTO> GetByDaiLy(int maDaiLy);
        TonKhoDTO? GetById(int id);
        TonKhoDTO? GetByLoNongSan(int maLoNongSan);
        List<TonKhoDTO> GetByTrangThai(string trangThai);
        List<TonKhoDTO> GetSapHetHang(int maDaiLy);
        int Create(TonKhoCreateDTO dto);
        bool Update(int id, TonKhoUpdateDTO dto);
        bool UpdateSoLuong(int id, decimal soLuongMoi);
        bool Delete(int id);
    }
}