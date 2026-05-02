using DaiLyService.Data;

namespace DaiLyService.Services
{
    public interface IDashboardService
    {
        object GetDashboardStats(int maDaiLy);
        List<object> GetRecentOrders(int maDaiLy, int limit = 5);
        object GetOrderStats(int maDaiLy);
    }

    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _repository;

        public DashboardService(IDashboardRepository repository)
        {
            _repository = repository;
        }

        public object GetDashboardStats(int maDaiLy)
        {
            return new
            {
                tongKho = _repository.GetTotalKho(maDaiLy),
                tongKhoHang = _repository.GetTotalWarehouses(maDaiLy),
                tongDonHangMua = _repository.GetTotalDonHangMua(maDaiLy),
                tongDonHangBan = _repository.GetTotalDonHangBan(maDaiLy)
            };
        }

        public List<object> GetRecentOrders(int maDaiLy, int limit = 5)
        {
            return _repository.GetRecentOrders(maDaiLy, limit);
        }

        public object GetOrderStats(int maDaiLy)
        {
            return _repository.GetOrderStats(maDaiLy);
        }
    }
}
