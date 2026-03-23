using DaiLyService.Models.DTOs;

namespace DaiLyService.Data
{
    public interface IDonHangRepository
    {
        // Lấy đơn hàng
        List<DonHangDTO> GetAll(); // Lấy tất cả đơn hàng liên quan đến đại lý
        List<DonHangDTO> GetByDaiLy(int maDaiLy); // Lấy đơn hàng của đại lý cụ thể
        List<DonHangDTO> GetByNguoiBan(int maNguoiBan, string loaiNguoiBan); // Đơn hàng bán ra
        List<DonHangDTO> GetByNguoiMua(int maNguoiMua, string loaiNguoiMua); // Đơn hàng mua vào
        DonHangDTO? GetById(int maDonHang);
        
        // CRUD đơn hàng
        int Create(DonHangCreateDTO dto);
        bool UpdateTrangThai(int maDonHang, string trangThai);
        bool Delete(int maDonHang);
        
        // Chi tiết đơn hàng
        List<ChiTietDonHangDTO> GetChiTietDonHang(int maDonHang);
    }
}