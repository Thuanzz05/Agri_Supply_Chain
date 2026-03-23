using DaiLyService.Data;
using DaiLyService.Models.DTOs;

namespace DaiLyService.Services
{
    public class DonHangService : IDonHangService
    {
        private readonly IDonHangRepository _repo;

        public DonHangService(IDonHangRepository repo)
        {
            _repo = repo;
        }

        public List<DonHangDTO> GetAll() => _repo.GetAll();

        public List<DonHangDTO> GetByDaiLy(int maDaiLy) => _repo.GetByDaiLy(maDaiLy);

        public List<DonHangDTO> GetByNguoiBan(int maNguoiBan, string loaiNguoiBan) => _repo.GetByNguoiBan(maNguoiBan, loaiNguoiBan);

        public List<DonHangDTO> GetByNguoiMua(int maNguoiMua, string loaiNguoiMua) => _repo.GetByNguoiMua(maNguoiMua, loaiNguoiMua);

        public DonHangDTO? GetById(int maDonHang) => _repo.GetById(maDonHang);

        public int Create(DonHangCreateDTO dto) => _repo.Create(dto);

        public bool UpdateTrangThai(int maDonHang, string trangThai) => _repo.UpdateTrangThai(maDonHang, trangThai);

        public bool Delete(int maDonHang) => _repo.Delete(maDonHang);

        public List<ChiTietDonHangDTO> GetChiTietDonHang(int maDonHang) => _repo.GetChiTietDonHang(maDonHang);
    }
}