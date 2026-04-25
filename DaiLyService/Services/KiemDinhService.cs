using DaiLyService.Data;
using DaiLyService.Models.DTOs;

namespace DaiLyService.Services
{
    public class KiemDinhService : IKiemDinhService
    {
        private readonly IKiemDinhRepository _repo;

        public KiemDinhService(IKiemDinhRepository repo)
        {
            _repo = repo;
        }

        public List<LoHangKiemDinhDTO> GetLoHangByDaiLy(int maDaiLy) => _repo.GetLoHangByDaiLy(maDaiLy);

        public List<KiemDinhDTO> GetAll() => _repo.GetAll();

        public List<KiemDinhDTO> GetByLo(int maLo) => _repo.GetByLo(maLo);

        public List<KiemDinhDTO> GetByKetQua(string ketQua) => _repo.GetByKetQua(ketQua);

        public KiemDinhDTO? GetById(int maKiemDinh) => _repo.GetById(maKiemDinh);

        public int Create(KiemDinhCreateDTO dto) => _repo.Create(dto);

        public bool Update(int maKiemDinh, KiemDinhUpdateDTO dto) => _repo.Update(maKiemDinh, dto);

        public bool Delete(int maKiemDinh) => _repo.Delete(maKiemDinh);

        public int CountByKetQua(string ketQua) => _repo.CountByKetQua(ketQua);
    }
}