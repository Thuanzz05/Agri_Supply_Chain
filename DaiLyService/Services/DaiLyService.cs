using DaiLyService.Data;
using DaiLyService.Models.DTOs;

namespace DaiLyService.Services
{
    public class DaiLyService : IDaiLyService
    {
        private readonly IDaiLyRepository _repo;

        public DaiLyService(IDaiLyRepository repo)
        {
            _repo = repo;
        }

        public List<DaiLyDTO> GetAll() => _repo.GetAll();

        public DaiLyDTO? GetById(int id) => _repo.GetById(id);

        public DaiLyDTO? GetByTaiKhoan(int maTaiKhoan) => _repo.GetByTaiKhoan(maTaiKhoan);

        public int Create(DaiLyCreateDTO dto) => _repo.Create(dto);

        public bool Update(int id, DaiLyUpdateDTO dto) => _repo.Update(id, dto);

        public bool Delete(int id) => _repo.Delete(id);
    }
}