using SieuThiService.Data;
using SieuThiService.Models.DTOs;

namespace SieuThiService.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _repository;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(IDashboardRepository repository, ILogger<DashboardService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<DashboardStatsDTO> GetDashboardStats(int maSieuThi)
        {
            try
            {
                _logger.LogInformation("Getting dashboard stats for supermarket {SupermarketId}", maSieuThi);
                return await _repository.GetDashboardStats(maSieuThi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DashboardService.GetDashboardStats for supermarket {SupermarketId}", maSieuThi);
                throw;
            }
        }

        public async Task<DonHangStatsDTO> GetDonHangStats(int maSieuThi)
        {
            try
            {
                _logger.LogInformation("Getting order stats for supermarket {SupermarketId}", maSieuThi);
                return await _repository.GetDonHangStats(maSieuThi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DashboardService.GetDonHangStats for supermarket {SupermarketId}", maSieuThi);
                throw;
            }
        }

        public async Task<KhoStatsDTO> GetKhoStats(int maSieuThi)
        {
            try
            {
                _logger.LogInformation("Getting warehouse stats for supermarket {SupermarketId}", maSieuThi);
                return await _repository.GetKhoStats(maSieuThi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DashboardService.GetKhoStats for supermarket {SupermarketId}", maSieuThi);
                throw;
            }
        }
    }
}
