using SieuThiService.Models.DTOs;

namespace SieuThiService.Services
{
    public interface IChuyenKhoService
    {
        int Create(ChuyenKhoCreateDTO dto);
        List<ChuyenKhoDTO> GetBySieuThi(int maSieuThi);
    }
}
