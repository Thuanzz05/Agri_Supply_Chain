using DaiLyService.Data;
using DaiLyService.Models.DTOs;

namespace DaiLyService.Services
{
    public class VanChuyenService : IVanChuyenService
    {
        private readonly IVanChuyenRepository _repo;

        public VanChuyenService(IVanChuyenRepository repo)
        {
            _repo = repo;
        }

        public List<VanChuyenDTO> GetAll() => _repo.GetAll();

        public List<VanChuyenDTO> GetByTrangThai(string trangThai) => _repo.GetByTrangThai(trangThai);

        public List<VanChuyenDTO> GetByLo(int maLo) => _repo.GetByLo(maLo);

        public VanChuyenDTO? GetById(int maVanChuyen) => _repo.GetById(maVanChuyen);

        public int Create(VanChuyenCreateDTO dto) => _repo.Create(dto);

        public bool UpdateTrangThai(int maVanChuyen, string trangThai, DateTime? ngayKetThuc = null) => 
            _repo.UpdateTrangThai(maVanChuyen, trangThai, ngayKetThuc);

        public bool Delete(int maVanChuyen) => _repo.Delete(maVanChuyen);

        public int CountByTrangThai(string trangThai) => _repo.CountByTrangThai(trangThai);
    }
}