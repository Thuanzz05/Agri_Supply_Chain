using SieuThiService.Models.DTOs;

namespace SieuThiService.Data
{
    public interface IKhoRepository
    {
        List<KhoDTO> GetBySieuThi(); // Lấy tất cả kho siêu thị
        List<KhoDTO> GetBySieuThi(int maSieuThi); // Lấy kho theo siêu thị cụ thể
        KhoDTO? GetById(int id);
    }
}