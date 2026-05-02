using NongDanService.Data;

namespace NongDanService.Services
{
    public interface IDashboardService
    {
        object GetDashboardStats(int maNongDan);
        List<object> GetRecentOrders(int maNongDan, int limit = 5);
        object GetOrderStats(int maNongDan);
    }

    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _repository;

        public DashboardService(IDashboardRepository repository)
        {
            _repository = repository;
        }

        public object GetDashboardStats(int maNongDan)
        {
            return new
            {
                tongSanPham = _repository.GetTotalSanPham(maNongDan),
                tongTrangTrai = _repository.GetTotalTrangTrai(maNongDan),
                tongLoNongSan = _repository.GetTotalLoNongSan(maNongDan),
                tongDonHang = _repository.GetTotalDonHang(maNongDan)
            };
        }

        public List<object> GetRecentOrders(int maNongDan, int limit = 5)
        {
            return _repository.GetRecentOrders(maNongDan, limit);
        }

        public object GetOrderStats(int maNongDan)
        {
            return _repository.GetOrderStats(maNongDan);
        }
    }
}
