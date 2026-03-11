using NongDanService.Models.DTOs;

namespace NongDanService.Services
{
    public interface INongDanService
    {
        List<NongDanDTO> GetAll();
        NongDanDTO? GetById(int id);
        NongDanDTO? GetByAccount(int maTaiKhoan);
        int Create(NongDanCreateDTO dto);
        bool Update(int id, NongDanUpdateDTO dto);
        bool Delete(int id);
    }
}