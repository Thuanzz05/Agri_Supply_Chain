using SieuThiService.Models.DTOs;

namespace SieuThiService.Data
{
    public interface IDashboardRepository
    {
        Task<DashboardStatsDTO> GetDashboardStats(int maSieuThi);
        Task<DonHangStatsDTO> GetDonHangStats(int maSieuThi);
        Task<KhoStatsDTO> GetKhoStats(int maSieuThi);
    }
}
