using DaiLyService.Models.DTOs;

namespace DaiLyService.Data
{
    public interface IVanChuyenRepository
    {
        // Lấy vận chuyển
        List<VanChuyenDTO> GetAll(); // Lấy tất cả vận chuyển
        List<VanChuyenDTO> GetByTrangThai(string trangThai); // Lấy theo trạng thái
        List<VanChuyenDTO> GetByLo(int maLo); // Lấy theo lô nông sản
        List<VanChuyenDTO> GetByDaiLy(int maDaiLy); // Lấy theo đại lý
        VanChuyenDTO? GetById(int maVanChuyen);
        
        // CRUD vận chuyển
        int Create(VanChuyenCreateDTO dto);
        bool UpdateTrangThai(int maVanChuyen, string trangThai, DateTime? ngayKetThuc = null);
        bool Delete(int maVanChuyen);
        
        // Thống kê
        int CountByTrangThai(string trangThai);
        object GetStatsByDaiLy(int maDaiLy);
    }
}