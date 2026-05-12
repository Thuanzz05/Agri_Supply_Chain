using SieuThiService.Data;
using SieuThiService.Models.DTOs;

namespace SieuThiService.Services
{
    public class DonHangService : IDonHangService
    {
        private readonly IDonHangRepository _repo;

        public DonHangService(IDonHangRepository repo)
        {
            _repo = repo;
        }

        public List<DonHangDTO> GetBySieuThi(int maSieuThi) => _repo.GetBySieuThi(maSieuThi);

        public DonHangDTO? GetById(int maDonHang) => _repo.GetById(maDonHang);

        public bool UpdateTrangThai(int maDonHang, string trangThai) => _repo.UpdateTrangThai(maDonHang, trangThai);
    }
}