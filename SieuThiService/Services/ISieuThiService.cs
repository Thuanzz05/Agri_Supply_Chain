using SieuThiService.Models.DTOs;

namespace SieuThiService.Services
{
    public interface ISieuThiService
    {
        List<SieuThiDTO> GetAll();
        SieuThiDTO? GetById(int maSieuThi);
        SieuThiDTO? GetByTaiKhoan(int maTaiKhoan);
        bool Create(SieuThiCreateDTO sieuThiDto);
        bool Update(int maSieuThi, SieuThiUpdateDTO sieuThiDto);
        bool Delete(int maSieuThi);
        bool ExistsByTenDangNhap(string tenDangNhap);
        bool ExistsByEmail(string email);
    }
}