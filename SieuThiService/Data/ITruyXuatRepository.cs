using SieuThiService.Models.DTOs;

namespace SieuThiService.Data
{
    public interface ITruyXuatRepository
    {
        TruyXuatLoInfoDTO? GetLoNongSanByQR(string maQR);
        TruyXuatKiemDinhDTO? GetKiemDinh(int maLo);
        List<TruyXuatVanChuyenDTO> GetVanChuyen(int maLo);
    }
}
