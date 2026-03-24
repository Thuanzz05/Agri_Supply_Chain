using DaiLyService.Models.DTOs;

namespace DaiLyService.Data
{
    public interface IKiemDinhRepository
    {
        List<KiemDinhDTO> GetAll(); // Lấy tất cả kiểm định liên quan đến đại lý
        List<KiemDinhDTO> GetByLo(int maLo); // Lấy kiểm định theo lô
        List<KiemDinhDTO> GetByKetQua(string ketQua); // Lấy kiểm định theo kết quả
        KiemDinhDTO? GetById(int maKiemDinh); // Lấy kiểm định theo ID
        int Create(KiemDinhCreateDTO dto); // Tạo kiểm định mới
        bool Update(int maKiemDinh, KiemDinhUpdateDTO dto); // Cập nhật kiểm định
        bool Delete(int maKiemDinh); // Xóa kiểm định
        int CountByKetQua(string ketQua); // Đếm số lượng theo kết quả
    }
}