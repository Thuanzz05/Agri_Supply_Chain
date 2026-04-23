using NongDanService.Data;
using NongDanService.Models.DTOs;

namespace NongDanService.Services
{
    public class DonHangService : IDonHangService
    {
        private readonly IDonHangRepository _repository;

        public DonHangService(IDonHangRepository repository)
        {
            _repository = repository;
        }

        public List<DonHangDTO> GetByNongDan(int maNongDan)
        {
            return _repository.GetByNongDan(maNongDan);
        }

        public DonHangDTO? GetById(int maDonHang)
        {
            return _repository.GetById(maDonHang);
        }

        public bool UpdateTrangThai(int maDonHang, string trangThai)
        {
            return _repository.UpdateTrangThai(maDonHang, trangThai);
        }
    }
}
