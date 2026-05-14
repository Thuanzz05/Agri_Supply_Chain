using SieuThiService.Data;
using SieuThiService.Models.DTOs;

namespace SieuThiService.Services
{
    public class ChuyenKhoService : IChuyenKhoService
    {
        private readonly IChuyenKhoRepository _repo;

        public ChuyenKhoService(IChuyenKhoRepository repo)
        {
            _repo = repo;
        }

        public int Create(ChuyenKhoCreateDTO dto) => _repo.Create(dto);

        public List<ChuyenKhoDTO> GetBySieuThi(int maSieuThi) => _repo.GetBySieuThi(maSieuThi);
    }
}
