using DaiLyService.Models.DTOs;

namespace DaiLyService.Services
{
    public interface IChuyenKhoService
    {
        int Create(ChuyenKhoCreateDTO dto);
        List<PhieuChuyenKhoDTO> GetByDaiLy(int maDaiLy);
    }
}

