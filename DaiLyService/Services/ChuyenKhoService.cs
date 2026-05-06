using DaiLyService.Data;
using DaiLyService.Models.DTOs;

namespace DaiLyService.Services
{
    public class ChuyenKhoService : IChuyenKhoService
    {
        private readonly IChuyenKhoRepository _repo;

        public ChuyenKhoService(IChuyenKhoRepository repo)
        {
            _repo = repo;
        }

        public int Create(ChuyenKhoCreateDTO dto) => _repo.Create(dto);

        public List<PhieuChuyenKhoDTO> GetByDaiLy(int maDaiLy) => _repo.GetByDaiLy(maDaiLy);
    }
}

