using DaiLyService.Models.DTOs;

namespace DaiLyService.Services
{
    public interface IDaiLyService
    {
        List<DaiLyDTO> GetAll();
        DaiLyDTO? GetById(int id);
        DaiLyDTO? GetByTaiKhoan(int maTaiKhoan);
        int Create(DaiLyCreateDTO dto);
        bool Update(int id, DaiLyUpdateDTO dto);
        bool Delete(int id);
    }
}