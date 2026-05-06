using DaiLyService.Models.DTOs;

namespace DaiLyService.Data
{
    public interface IChuyenKhoRepository
    {
        int Create(ChuyenKhoCreateDTO dto);
        List<PhieuChuyenKhoDTO> GetByDaiLy(int maDaiLy);
    }
}

