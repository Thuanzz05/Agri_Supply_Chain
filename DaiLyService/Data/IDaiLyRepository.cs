using DaiLyService.Models.DTOs;

namespace DaiLyService.Data
{
    public interface IDaiLyRepository
    {
        List<DaiLyDTO> GetAll();
        DaiLyDTO? GetById(int id);
        DaiLyDTO? GetByTaiKhoan(int maTaiKhoan);
        int Create(DaiLyCreateDTO dto);
        bool Update(int id, DaiLyUpdateDTO dto);
        bool Delete(int id);
    }
}