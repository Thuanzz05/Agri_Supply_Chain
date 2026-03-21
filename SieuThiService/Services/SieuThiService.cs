using SieuThiService.Data;
using SieuThiService.Models.DTOs;

namespace SieuThiService.Services
{
    public class SieuThiBusinessService : ISieuThiService
    {
        private readonly ISieuThiRepository _repo;

        public SieuThiBusinessService(ISieuThiRepository repo)
        {
            _repo = repo;
        }

        public List<SieuThiDTO> GetAll() => _repo.GetAll();

        public SieuThiDTO? GetById(int maSieuThi) => _repo.GetById(maSieuThi);

        public SieuThiDTO? GetByTaiKhoan(int maTaiKhoan) => _repo.GetByTaiKhoan(maTaiKhoan);

        public bool Create(SieuThiCreateDTO sieuThiDto) => _repo.Create(sieuThiDto);

        public bool Update(int maSieuThi, SieuThiUpdateDTO sieuThiDto) => _repo.Update(maSieuThi, sieuThiDto);

        public bool Delete(int maSieuThi) => _repo.Delete(maSieuThi);

        public bool ExistsByTenDangNhap(string tenDangNhap) => _repo.ExistsByTenDangNhap(tenDangNhap);

        public bool ExistsByEmail(string email) => _repo.ExistsByEmail(email);
    }
}