using DaiLyService.Data;
using DaiLyService.Models.DTOs;

namespace DaiLyService.Services
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

        public List<TonKhoDTO> GetByDaiLy(int maDaiLy) => _repo.GetByDaiLy(maDaiLy);

        public TonKhoDTO? GetById(int id) => _repo.GetById(id);

        public TonKhoDTO? GetByLoNongSan(int maLoNongSan) => _repo.GetByLoNongSan(maLoNongSan);

        public List<TonKhoDTO> GetByTrangThai(string trangThai) => _repo.GetByTrangThai(trangThai);

        public List<TonKhoDTO> GetSapHetHang(int maDaiLy) => _repo.GetSapHetHang(maDaiLy);

        public int Create(TonKhoCreateDTO dto) => _repo.Create(dto);

        public bool Update(int id, TonKhoUpdateDTO dto) => _repo.Update(id, dto);

        public bool UpdateSoLuong(int id, decimal soLuongMoi) => _repo.UpdateSoLuong(id, soLuongMoi);

        public bool Delete(int id) => _repo.Delete(id);
    }
}