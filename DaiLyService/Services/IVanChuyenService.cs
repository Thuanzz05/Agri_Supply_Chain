using DaiLyService.Models.DTOs;

namespace DaiLyService.Services
{
    public interface IVanChuyenService
    {
        List<VanChuyenDTO> GetAll();
        List<VanChuyenDTO> GetByTrangThai(string trangThai);
        List<VanChuyenDTO> GetByLo(int maLo);
        VanChuyenDTO? GetById(int maVanChuyen);
        int Create(VanChuyenCreateDTO dto);
        bool UpdateTrangThai(int maVanChuyen, string trangThai, DateTime? ngayKetThuc = null);
        bool Delete(int maVanChuyen);
        int CountByTrangThai(string trangThai);
    }
}