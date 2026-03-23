using SieuThiService.Models.DTOs;

namespace SieuThiService.Data
{
    public interface ITonKhoRepository
    {
        List<TonKhoDTO> GetAll(); // Lấy tất cả tồn kho của siêu thị
        List<TonKhoDTO> GetByKho(int maKho); // Lấy tồn kho theo kho cụ thể
        List<TonKhoDTO> GetBySieuThi(int maSieuThi); // Lấy tồn kho theo siêu thị
        TonKhoDTO? GetByKhoAndLo(int maKho, int maLo); // Lấy tồn kho cụ thể
        bool UpdateSoLuong(int maKho, int maLo, decimal soLuongMoi); // Cập nhật số lượng
        bool Delete(int maKho, int maLo); // Xóa tồn kho
    }
}