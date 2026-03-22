using DaiLyService.Models.DTOs;

namespace DaiLyService.Data
{
    public interface IKhoRepository
    {
        List<KhoDTO> GetAll();
        List<KhoDTO> GetByDaiLy(int maDaiLy);
        List<KhoDTO> GetBySieuThi(); // Lấy tất cả kho siêu thị
        List<KhoDTO> GetBySieuThi(int maSieuThi); // Lấy kho theo siêu thị cụ thể
        KhoDTO? GetById(int id);
        int Create(KhoCreateDTO dto);
        bool Update(int id, KhoUpdateDTO dto);
        bool Delete(int id);
    }
}