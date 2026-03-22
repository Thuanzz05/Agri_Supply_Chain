using SieuThiService.Data;
using SieuThiService.Models.DTOs;

namespace SieuThiService.Services
{
    public class KhoService : IKhoService
    {
        private readonly IKhoRepository _repo;

        public KhoService(IKhoRepository repo)
        {
            _repo = repo;
        }

        public List<KhoDTO> GetBySieuThi() => _repo.GetBySieuThi();

        public List<KhoDTO> GetBySieuThi(int maSieuThi) => _repo.GetBySieuThi(maSieuThi);

        public KhoDTO? GetById(int id) => _repo.GetById(id);
    }
}