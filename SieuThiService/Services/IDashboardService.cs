using SieuThiService.Models.DTOs;

namespace SieuThiService.Services
{
    public interface IDashboardService
    {
        Task<DashboardStatsDTO> GetDashboardStats(int maSieuThi);
        Task<DonHangStatsDTO> GetDonHangStats(int maSieuThi);
        Task<KhoStatsDTO> GetKhoStats(int maSieuThi);
    }
}
