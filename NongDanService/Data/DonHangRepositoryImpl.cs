using Microsoft.Data.SqlClient;
using Dapper;
using NongDanService.Models.DTOs;

namespace NongDanService.Data
{
    public class DonHangRepositoryImpl : IDonHangRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<DonHangRepositoryImpl> _logger;

        public DonHangRepositoryImpl(IConfiguration configuration, ILogger<DonHangRepositoryImpl> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            _logger = logger;
        }

        public List<DonHangDTO> GetByNongDan(int maNongDan)
        {
            var list = new List<DonHangDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                var sql = @"
                    SELECT dh.MaDonHang, dh.LoaiDon, dh.MaNguoiBan, dh.LoaiNguoiBan, 
                           dh.MaNguoiMua, dh.LoaiNguoiMua, dh.NgayDat, dh.TrangThai, dh.TongGiaTri,
                           nd.HoTen AS TenNguoiBan,
                           dl.TenDaiLy AS TenNguoiMua
                    FROM DonHang dh
                    LEFT JOIN NongDan nd ON dh.MaNguoiBan = nd.MaNongDan AND dh.LoaiNguoiBan = 'nongdan'
                    LEFT JOIN DaiLy dl ON dh.MaNguoiMua = dl.MaDaiLy AND dh.LoaiNguoiMua = 'daily'
                    WHERE dh.MaNguoiBan = @MaNongDan 
                      AND dh.LoaiNguoiBan = 'nongdan'
                      AND dh.LoaiDon = 'nongdan_to_daily'
                    ORDER BY dh.NgayDat DESC";

                list = conn.Query<DonHangDTO>(sql, new { MaNongDan = maNongDan }).ToList();
                _logger.LogInformation("Retrieved {Count} orders for farmer {FarmerId}", list.Count, maNongDan);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting orders for farmer");
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public DonHangDTO? GetById(int maDonHang)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);

                // Lấy thông tin đơn hàng
                var sqlDonHang = @"
                    SELECT dh.MaDonHang, dh.LoaiDon, dh.MaNguoiBan, dh.LoaiNguoiBan, 
                           dh.MaNguoiMua, dh.LoaiNguoiMua, dh.NgayDat, dh.TrangThai, dh.TongGiaTri,
                           nd.HoTen AS TenNguoiBan,
                           dl.TenDaiLy AS TenNguoiMua
                    FROM DonHang dh
                    LEFT JOIN NongDan nd ON dh.MaNguoiBan = nd.MaNongDan AND dh.LoaiNguoiBan = 'nongdan'
                    LEFT JOIN DaiLy dl ON dh.MaNguoiMua = dl.MaDaiLy AND dh.LoaiNguoiMua = 'daily'
                    WHERE dh.MaDonHang = @MaDonHang";

                var donHang = conn.QueryFirstOrDefault<DonHangDTO>(sqlDonHang, new { MaDonHang = maDonHang });

                if (donHang == null)
                {
                    return null;
                }

                // Lấy chi tiết đơn hàng
                var sqlChiTiet = @"
                    SELECT ct.MaDonHang, ct.MaLo, ct.SoLuong, ct.DonGia, ct.ThanhTien,
                           sp.TenSanPham, sp.DonViTinh,
                           ln.MaQR, ln.NgayThuHoach, ln.HanSuDung
                    FROM ChiTietDonHang ct
                    LEFT JOIN LoNongSan ln ON ct.MaLo = ln.MaLo
                    LEFT JOIN SanPham sp ON ln.MaSanPham = sp.MaSanPham
                    WHERE ct.MaDonHang = @MaDonHang";

                donHang.ChiTietDonHang = conn.Query<ChiTietDonHangDTO>(sqlChiTiet, new { MaDonHang = maDonHang }).ToList();

                _logger.LogInformation("Retrieved order {OrderId} with {ItemCount} items", maDonHang, donHang.ChiTietDonHang.Count);
                return donHang;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting order by id");
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
        }

        public bool UpdateTrangThai(int maDonHang, string trangThai)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                var sql = "UPDATE DonHang SET TrangThai = @TrangThai WHERE MaDonHang = @MaDonHang";
                var rowsAffected = conn.Execute(sql, new { TrangThai = trangThai, MaDonHang = maDonHang });

                _logger.LogInformation("Updated order {OrderId} status to {Status}", maDonHang, trangThai);
                return rowsAffected > 0;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while updating order status");
                throw new Exception("Lỗi cập nhật cơ sở dữ liệu", ex);
            }
        }
    }
}
