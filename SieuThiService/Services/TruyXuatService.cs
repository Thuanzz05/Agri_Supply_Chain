using SieuThiService.Data;
using SieuThiService.Models.DTOs;

namespace SieuThiService.Services
{
    public class TruyXuatService : ITruyXuatService
    {
        private readonly ITruyXuatRepository _truyXuatRepository;

        public TruyXuatService(ITruyXuatRepository truyXuatRepository)
        {
            _truyXuatRepository = truyXuatRepository;
        }

        public TruyXuatResultDTO? TraceProductByQR(string maQR)
        {
            var loInfo = _truyXuatRepository.GetLoNongSanByQR(maQR);
            if (loInfo == null)
            {
                return null;
            }

            var result = new TruyXuatResultDTO
            {
                MaLo = loInfo.MaLo,
                MaQR = loInfo.MaQR,
                TenSanPham = loInfo.TenSanPham,
                DonViTinh = loInfo.DonViTinh,
                SoLuongBanDau = loInfo.SoLuongBanDau,
                SoLuongHienTai = loInfo.SoLuongHienTai,
                NgayThuHoach = loInfo.NgayThuHoach,
                HanSuDung = loInfo.HanSuDung,
                TrangThai = loInfo.TrangThai,
                NgayTao = loInfo.NgayTao,
                TenTrangTrai = loInfo.TenTrangTrai,
                DiaChiTrangTrai = loInfo.DiaChiTrangTrai,
                SoChungNhan = loInfo.SoChungNhan,
                TenNongDan = loInfo.TenNongDan,
                SoDienThoaiNongDan = loInfo.SoDienThoaiNongDan,
                DiaChiNongDan = loInfo.DiaChiNongDan,
                FacebookNongDan = loInfo.FacebookNongDan,
                TiktokNongDan = loInfo.TiktokNongDan
            };

            result.KiemDinh = _truyXuatRepository.GetKiemDinh(loInfo.MaLo);
            result.VanChuyen = _truyXuatRepository.GetVanChuyen(loInfo.MaLo);

            return result;
        }
    }
}
