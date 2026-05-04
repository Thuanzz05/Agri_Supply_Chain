using Microsoft.Data.SqlClient;
using SieuThiService.Models.DTOs;
using System.Data;

namespace SieuThiService.Data
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<DashboardRepository> _logger;

        public DashboardRepository(IConfiguration config, ILogger<DashboardRepository> logger)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
            _logger = logger;
        }

        public async Task<DashboardStatsDTO> GetDashboardStats(int maSieuThi)
        {
            var stats = new DashboardStatsDTO
            {
                DonHangGanDay = new List<DonHangGanDayDTO>()
            };

            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                // 1. Tổng sản phẩm trong kho
                using (var cmd = new SqlCommand(@"
                    SELECT COUNT(DISTINCT tk.MaLo) as TongSanPham
                    FROM TonKho tk
                    INNER JOIN Kho k ON tk.MaKho = k.MaKho
                    WHERE k.LoaiChuSoHuu = 'sieuthi' AND k.MaChuSoHuu = @MaSieuThi
                    AND tk.SoLuong > 0", conn))
                {
                    cmd.Parameters.AddWithValue("@MaSieuThi", maSieuThi);
                    var result = await cmd.ExecuteScalarAsync();
                    stats.TongSanPhamTrongKho = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }

                // 2. Đơn hàng tháng này
                using (var cmd = new SqlCommand(@"
                    SELECT COUNT(*) as TongDonHang
                    FROM DonHang
                    WHERE LoaiNguoiMua = 'sieuthi' AND MaNguoiMua = @MaSieuThi
                    AND MONTH(NgayDat) = MONTH(GETDATE())
                    AND YEAR(NgayDat) = YEAR(GETDATE())", conn))
                {
                    cmd.Parameters.AddWithValue("@MaSieuThi", maSieuThi);
                    var result = await cmd.ExecuteScalarAsync();
                    stats.TongDonHangThang = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }

                // 3. Số đơn chờ xác nhận
                using (var cmd = new SqlCommand(@"
                    SELECT COUNT(*) as SoDon
                    FROM DonHang
                    WHERE LoaiNguoiMua = 'sieuthi' AND MaNguoiMua = @MaSieuThi
                    AND TrangThai = N'cho_xac_nhan'", conn))
                {
                    cmd.Parameters.AddWithValue("@MaSieuThi", maSieuThi);
                    var result = await cmd.ExecuteScalarAsync();
                    stats.SoDonChoXacNhan = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }

                // 4. Số đơn hoàn thành
                using (var cmd = new SqlCommand(@"
                    SELECT COUNT(*) as SoDon
                    FROM DonHang
                    WHERE LoaiNguoiMua = 'sieuthi' AND MaNguoiMua = @MaSieuThi
                    AND TrangThai = N'hoan_thanh'", conn))
                {
                    cmd.Parameters.AddWithValue("@MaSieuThi", maSieuThi);
                    var result = await cmd.ExecuteScalarAsync();
                    stats.SoDonHoanThanh = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }

                // 5. Đơn hàng gần đây (5 đơn mới nhất)
                using (var cmd = new SqlCommand(@"
                    SELECT TOP 5
                        dh.MaDonHang,
                        dh.NgayDat,
                        dh.TrangThai,
                        dh.TongGiaTri,
                        dl.TenDaiLy as TenNguoiBan
                    FROM DonHang dh
                    LEFT JOIN DaiLy dl ON dh.MaNguoiBan = dl.MaDaiLy AND dh.LoaiNguoiBan = 'daily'
                    WHERE dh.LoaiNguoiMua = 'sieuthi' AND dh.MaNguoiMua = @MaSieuThi
                    ORDER BY dh.NgayDat DESC", conn))
                {
                    cmd.Parameters.AddWithValue("@MaSieuThi", maSieuThi);
                    using var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        stats.DonHangGanDay!.Add(new DonHangGanDayDTO
                        {
                            MaDonHang = reader.GetInt32("MaDonHang"),
                            NgayDat = reader.GetDateTime("NgayDat"),
                            TrangThai = reader.IsDBNull("TrangThai") ? null : reader.GetString("TrangThai"),
                            TongGiaTri = reader.GetDecimal("TongGiaTri"),
                            TenNguoiBan = reader.IsDBNull("TenNguoiBan") ? null : reader.GetString("TenNguoiBan")
                        });
                    }
                }

                _logger.LogInformation("Retrieved dashboard stats for supermarket {SupermarketId}", maSieuThi);
                return stats;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting dashboard stats for supermarket {SupermarketId}", maSieuThi);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting dashboard stats for supermarket {SupermarketId}", maSieuThi);
                throw;
            }
        }

        public async Task<DonHangStatsDTO> GetDonHangStats(int maSieuThi)
        {
            var stats = new DonHangStatsDTO
            {
                DonHangTheoThang = new List<DonHangTheoThangDTO>(),
                DonHangGanDay = new List<DonHangGanDayDTO>()
            };

            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                // 1. Tổng đơn hàng
                using (var cmd = new SqlCommand(@"
                    SELECT COUNT(*) as TongDonHang
                    FROM DonHang
                    WHERE LoaiNguoiMua = 'sieuthi' AND MaNguoiMua = @MaSieuThi", conn))
                {
                    cmd.Parameters.AddWithValue("@MaSieuThi", maSieuThi);
                    var result = await cmd.ExecuteScalarAsync();
                    stats.TongDonHang = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }

                // 2. Đơn chờ xác nhận
                using (var cmd = new SqlCommand(@"
                    SELECT COUNT(*) as SoDon
                    FROM DonHang
                    WHERE LoaiNguoiMua = 'sieuthi' AND MaNguoiMua = @MaSieuThi
                    AND TrangThai = N'cho_xac_nhan'", conn))
                {
                    cmd.Parameters.AddWithValue("@MaSieuThi", maSieuThi);
                    var result = await cmd.ExecuteScalarAsync();
                    stats.DonChoXacNhan = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }

                // 3. Đơn hoàn thành
                using (var cmd = new SqlCommand(@"
                    SELECT COUNT(*) as SoDon
                    FROM DonHang
                    WHERE LoaiNguoiMua = 'sieuthi' AND MaNguoiMua = @MaSieuThi
                    AND TrangThai = N'hoan_thanh'", conn))
                {
                    cmd.Parameters.AddWithValue("@MaSieuThi", maSieuThi);
                    var result = await cmd.ExecuteScalarAsync();
                    stats.DonHoanThanh = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }

                // 4. Đơn đã hủy
                using (var cmd = new SqlCommand(@"
                    SELECT COUNT(*) as SoDon
                    FROM DonHang
                    WHERE LoaiNguoiMua = 'sieuthi' AND MaNguoiMua = @MaSieuThi
                    AND TrangThai = N'da_huy'", conn))
                {
                    cmd.Parameters.AddWithValue("@MaSieuThi", maSieuThi);
                    var result = await cmd.ExecuteScalarAsync();
                    stats.DonDaHuy = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }

                // 5. Tổng giá trị đơn hàng
                using (var cmd = new SqlCommand(@"
                    SELECT ISNULL(SUM(TongGiaTri), 0) as TongGiaTri
                    FROM DonHang
                    WHERE LoaiNguoiMua = 'sieuthi' AND MaNguoiMua = @MaSieuThi
                    AND TrangThai = N'hoan_thanh'", conn))
                {
                    cmd.Parameters.AddWithValue("@MaSieuThi", maSieuThi);
                    var result = await cmd.ExecuteScalarAsync();
                    stats.TongGiaTriDonHang = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                }

                // 6. Đơn hàng theo tháng (6 tháng gần nhất)
                using (var cmd = new SqlCommand(@"
                    SELECT 
                        MONTH(NgayDat) as Thang,
                        YEAR(NgayDat) as Nam,
                        COUNT(*) as SoDonHang,
                        SUM(TongGiaTri) as TongGiaTri
                    FROM DonHang
                    WHERE LoaiNguoiMua = 'sieuthi' AND MaNguoiMua = @MaSieuThi
                    AND NgayDat >= DATEADD(MONTH, -6, GETDATE())
                    GROUP BY YEAR(NgayDat), MONTH(NgayDat)
                    ORDER BY Nam DESC, Thang DESC", conn))
                {
                    cmd.Parameters.AddWithValue("@MaSieuThi", maSieuThi);
                    using var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        stats.DonHangTheoThang!.Add(new DonHangTheoThangDTO
                        {
                            Thang = reader.GetInt32("Thang"),
                            Nam = reader.GetInt32("Nam"),
                            SoDonHang = reader.GetInt32("SoDonHang"),
                            TongGiaTri = reader.GetDecimal("TongGiaTri")
                        });
                    }
                }

                // 7. Đơn hàng gần đây (10 đơn mới nhất)
                using (var cmd = new SqlCommand(@"
                    SELECT TOP 10
                        dh.MaDonHang,
                        dh.NgayDat,
                        dh.TrangThai,
                        dh.TongGiaTri,
                        dl.TenDaiLy as TenNguoiBan
                    FROM DonHang dh
                    LEFT JOIN DaiLy dl ON dh.MaNguoiBan = dl.MaDaiLy AND dh.LoaiNguoiBan = 'daily'
                    WHERE dh.LoaiNguoiMua = 'sieuthi' AND dh.MaNguoiMua = @MaSieuThi
                    ORDER BY dh.NgayDat DESC", conn))
                {
                    cmd.Parameters.AddWithValue("@MaSieuThi", maSieuThi);
                    using var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        stats.DonHangGanDay!.Add(new DonHangGanDayDTO
                        {
                            MaDonHang = reader.GetInt32("MaDonHang"),
                            NgayDat = reader.GetDateTime("NgayDat"),
                            TrangThai = reader.IsDBNull("TrangThai") ? null : reader.GetString("TrangThai"),
                            TongGiaTri = reader.GetDecimal("TongGiaTri"),
                            TenNguoiBan = reader.IsDBNull("TenNguoiBan") ? null : reader.GetString("TenNguoiBan")
                        });
                    }
                }

                _logger.LogInformation("Retrieved order stats for supermarket {SupermarketId}", maSieuThi);
                return stats;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting order stats for supermarket {SupermarketId}", maSieuThi);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting order stats for supermarket {SupermarketId}", maSieuThi);
                throw;
            }
        }

        public async Task<KhoStatsDTO> GetKhoStats(int maSieuThi)
        {
            var stats = new KhoStatsDTO
            {
                DanhSachSanPhamTonKho = new List<SanPhamTonKhoDTO>(),
                DanhSachSanPhamSapHet = new List<SanPhamSapHetDTO>()
            };

            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                // 1. Tổng sản phẩm
                using (var cmd = new SqlCommand(@"
                    SELECT COUNT(DISTINCT tk.MaLo) as TongSanPham
                    FROM TonKho tk
                    INNER JOIN Kho k ON tk.MaKho = k.MaKho
                    WHERE k.LoaiChuSoHuu = 'sieuthi' AND k.MaChuSoHuu = @MaSieuThi
                    AND tk.SoLuong > 0", conn))
                {
                    cmd.Parameters.AddWithValue("@MaSieuThi", maSieuThi);
                    var result = await cmd.ExecuteScalarAsync();
                    stats.TongSanPham = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }

                // 2. Sản phẩm sắp hết (số lượng < 50)
                using (var cmd = new SqlCommand(@"
                    SELECT COUNT(DISTINCT tk.MaLo) as SanPhamSapHet
                    FROM TonKho tk
                    INNER JOIN Kho k ON tk.MaKho = k.MaKho
                    WHERE k.LoaiChuSoHuu = 'sieuthi' AND k.MaChuSoHuu = @MaSieuThi
                    AND tk.SoLuong > 0 AND tk.SoLuong < 50", conn))
                {
                    cmd.Parameters.AddWithValue("@MaSieuThi", maSieuThi);
                    var result = await cmd.ExecuteScalarAsync();
                    stats.SoLuongSanPhamSapHet = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }

                // 3. Tổng số lượng tồn kho
                using (var cmd = new SqlCommand(@"
                    SELECT ISNULL(SUM(tk.SoLuong), 0) as TongSoLuong
                    FROM TonKho tk
                    INNER JOIN Kho k ON tk.MaKho = k.MaKho
                    WHERE k.LoaiChuSoHuu = 'sieuthi' AND k.MaChuSoHuu = @MaSieuThi", conn))
                {
                    cmd.Parameters.AddWithValue("@MaSieuThi", maSieuThi);
                    var result = await cmd.ExecuteScalarAsync();
                    stats.TongSoLuongTonKho = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                }

                // 4. Danh sách sản phẩm tồn kho
                using (var cmd = new SqlCommand(@"
                    SELECT 
                        sp.TenSanPham,
                        SUM(tk.SoLuong) as SoLuong,
                        sp.DonViTinh,
                        MIN(lo.HanSuDung) as HanSuDung
                    FROM TonKho tk
                    INNER JOIN Kho k ON tk.MaKho = k.MaKho
                    INNER JOIN LoNongSan lo ON tk.MaLo = lo.MaLo
                    INNER JOIN SanPham sp ON lo.MaSanPham = sp.MaSanPham
                    WHERE k.LoaiChuSoHuu = 'sieuthi' AND k.MaChuSoHuu = @MaSieuThi
                    AND tk.SoLuong > 0
                    GROUP BY sp.TenSanPham, sp.DonViTinh
                    ORDER BY SoLuong DESC", conn))
                {
                    cmd.Parameters.AddWithValue("@MaSieuThi", maSieuThi);
                    using var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        stats.DanhSachSanPhamTonKho!.Add(new SanPhamTonKhoDTO
                        {
                            TenSanPham = reader.GetString("TenSanPham"),
                            SoLuong = reader.GetDecimal("SoLuong"),
                            DonViTinh = reader.GetString("DonViTinh"),
                            HanSuDung = reader.IsDBNull("HanSuDung") ? null : reader.GetDateTime("HanSuDung")
                        });
                    }
                }

                // 5. Sản phẩm sắp hết (số lượng < 50)
                using (var cmd = new SqlCommand(@"
                    SELECT 
                        sp.TenSanPham,
                        tk.SoLuong,
                        sp.DonViTinh,
                        k.TenKho
                    FROM TonKho tk
                    INNER JOIN Kho k ON tk.MaKho = k.MaKho
                    INNER JOIN LoNongSan lo ON tk.MaLo = lo.MaLo
                    INNER JOIN SanPham sp ON lo.MaSanPham = sp.MaSanPham
                    WHERE k.LoaiChuSoHuu = 'sieuthi' AND k.MaChuSoHuu = @MaSieuThi
                    AND tk.SoLuong > 0 AND tk.SoLuong < 50
                    ORDER BY tk.SoLuong ASC", conn))
                {
                    cmd.Parameters.AddWithValue("@MaSieuThi", maSieuThi);
                    using var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        stats.DanhSachSanPhamSapHet!.Add(new SanPhamSapHetDTO
                        {
                            TenSanPham = reader.GetString("TenSanPham"),
                            SoLuong = reader.GetDecimal("SoLuong"),
                            DonViTinh = reader.GetString("DonViTinh"),
                            TenKho = reader.GetString("TenKho")
                        });
                    }
                }

                _logger.LogInformation("Retrieved warehouse stats for supermarket {SupermarketId}", maSieuThi);
                return stats;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting warehouse stats for supermarket {SupermarketId}", maSieuThi);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting warehouse stats for supermarket {SupermarketId}", maSieuThi);
                throw;
            }
        }
    }
}
