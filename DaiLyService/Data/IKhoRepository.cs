using DaiLyService.Models.DTOs;

namespace DaiLyService.Data
{
    public interface IKhoRepository
    {
        List<KhoDTO> GetAll();
        List<KhoDTO> GetByDaiLy(int maDaiLy);
        KhoDTO? GetById(int id);
        int Create(KhoCreateDTO dto);
        bool Update(int id, KhoUpdateDTO dto);
        bool Delete(int id);
    }
}