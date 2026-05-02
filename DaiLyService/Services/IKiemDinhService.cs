using DaiLyService.Models.DTOs;

namespace DaiLyService.Services
{
    public interface IKiemDinhService
    {
        List<LoHangKiemDinhDTO> GetLoHangByDaiLy(int maDaiLy);
        List<LoHangKiemDinhDTO> GetAllLoHangAvailable();
        List<KiemDinhDTO> GetAll();
        List<KiemDinhDTO> GetByLo(int maLo);
        List<KiemDinhDTO> GetByKetQua(string ketQua);
        KiemDinhDTO? GetById(int maKiemDinh);
        int Create(KiemDinhCreateDTO dto);
        bool Update(int maKiemDinh, KiemDinhUpdateDTO dto);
        bool Delete(int maKiemDinh);
        int CountByKetQua(string ketQua);
        object GetStatsByDaiLy(int maDaiLy);
    }
}