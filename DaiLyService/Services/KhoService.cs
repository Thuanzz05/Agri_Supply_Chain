using DaiLyService.Data;
using DaiLyService.Models.DTOs;

namespace DaiLyService.Services
{
    public class KhoService : IKhoService
    {
        private readonly IKhoRepository _repo;

        public KhoService(IKhoRepository repo)
        {
            _repo = repo;
        }

        public List<KhoDTO> GetAll() => _repo.GetAll();

        public List<KhoDTO> GetByDaiLy(int maDaiLy) => _repo.GetByDaiLy(maDaiLy);

        public KhoDTO? GetById(int id) => _repo.GetById(id);

        public List<KhoDTO> GetByTrangThai(string trangThai) => _repo.GetByTrangThai(trangThai);

        public int Create(KhoCreateDTO dto) => _repo.Create(dto);

        public bool Update(int id, KhoUpdateDTO dto) => _repo.Update(id, dto);

        public bool Delete(int id) => _repo.Delete(id);
    }
}