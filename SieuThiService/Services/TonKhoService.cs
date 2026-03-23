using SieuThiService.Data;
using SieuThiService.Models.DTOs;

namespace SieuThiService.Services
{
    public class TonKhoService : ITonKhoService
    {
        private readonly ITonKhoRepository _repo;

        public TonKhoService(ITonKhoRepository repo)
        {
            _repo = repo;
        }

        public List<TonKhoDTO> GetAll() => _repo.GetAll();

        public List<TonKhoDTO> GetByKho(int maKho) => _repo.GetByKho(maKho);

        public List<TonKhoDTO> GetBySieuThi(int maSieuThi) => _repo.GetBySieuThi(maSieuThi);

        public TonKhoDTO? GetByKhoAndLo(int maKho, int maLo) => _repo.GetByKhoAndLo(maKho, maLo);

        public bool Create(int maKho, int maLo, decimal soLuong) => _repo.Create(maKho, maLo, soLuong);

        public bool UpdateSoLuong(int maKho, int maLo, decimal soLuongMoi) => _repo.UpdateSoLuong(maKho, maLo, soLuongMoi);

        public bool Delete(int maKho, int maLo) => _repo.Delete(maKho, maLo);
    }
}