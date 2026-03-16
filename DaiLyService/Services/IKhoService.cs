using DaiLyService.Models.DTOs;

namespace DaiLyService.Services
{
    public interface IKhoService
    {
        List<KhoDTO> GetAll();
        List<KhoDTO> GetByDaiLy(int maDaiLy);
        KhoDTO? GetById(int id);
        int Create(KhoCreateDTO dto);
        bool Update(int id, KhoUpdateDTO dto);
        bool Delete(int id);
    }
}