using SieuThiService.Models.DTOs;

namespace SieuThiService.Data
{
    public interface IChuyenKhoRepository
    {
        int Create(ChuyenKhoCreateDTO dto);
        List<ChuyenKhoDTO> GetBySieuThi(int maSieuThi);
    }
}
