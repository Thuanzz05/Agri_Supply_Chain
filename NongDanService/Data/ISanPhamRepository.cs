using NongDanService.Models.DTOs;

namespace NongDanService.Data
{
    public interface ISanPhamRepository
    {
        List<SanPhamDTO> GetAll();
        SanPhamDTO? GetById(int id);
        List<SanPhamDTO> SearchByName(string tenSanPham);
        List<SanPhamDTO> GetByNongDan(int maNongDan);
        List<SanPhamDTO> GetByTrangTrai(int maTrangTrai);
        int Create(SanPhamCreateDTO dto);
        bool Update(int id, SanPhamUpdateDTO dto);
        bool Delete(int id);
    }
}