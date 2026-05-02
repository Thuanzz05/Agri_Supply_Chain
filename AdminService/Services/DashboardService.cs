using AdminService.Data;

namespace AdminService.Services
{
    public class DashboardService
    {
        private readonly DashboardRepository _repository;

        public DashboardService(DashboardRepository repository)
        {
            _repository = repository;
        }

        public (bool success, string message, object? data) GetDashboardStats()
        {
            try
            {
                var stats = new
                {
                    TongNongDan = _repository.GetTotalNongDan(),
                    TongDaiLy = _repository.GetTotalDaiLy(),
                    TongSieuThi = _repository.GetTotalSieuThi(),
                    TongTaiKhoan = _repository.GetTotalTaiKhoan(),
                    TongLoNongSan = _repository.GetTotalLoNongSan(),
                    TongDonHang = _repository.GetTotalDonHang(),
                    TongKiemDinh = _repository.GetTotalKiemDinh(),
                    ThongKeTheoLoai = _repository.GetUserStatsByType()
                };

                return (true, "Lấy thống kê thành công", stats);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}", null);
            }
        }
    }
}
