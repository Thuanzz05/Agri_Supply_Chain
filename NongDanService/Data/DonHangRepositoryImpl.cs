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
                conn.Open();
                
                using var transaction = conn.BeginTransaction();
                
                try
                {
                    // 1. Cập nhật trạng thái đơn hàng
                    var sqlUpdateOrder = "UPDATE DonHang SET TrangThai = @TrangThai WHERE MaDonHang = @MaDonHang";
                    conn.Execute(sqlUpdateOrder, new { TrangThai = trangThai, MaDonHang = maDonHang }, transaction);

                    // 2. Nếu xác nhận đơn hàng (hoàn thành), cập nhật số lượng lô và tạo tồn kho
                    if (trangThai == "hoan_thanh")
                    {
                        // Lấy thông tin đơn hàng
                        var sqlGetOrder = @"
                            SELECT dh.MaNguoiMua, dh.LoaiNguoiMua
                            FROM DonHang dh
                            WHERE dh.MaDonHang = @MaDonHang";
                        
                        var orderInfo = conn.QueryFirstOrDefault<dynamic>(sqlGetOrder, new { MaDonHang = maDonHang }, transaction);
                        
                        if (orderInfo == null)
                        {
                            throw new Exception("Không tìm thấy đơn hàng");
                        }

                        // Lấy chi tiết đơn hàng
                        var sqlGetDetails = @"
                            SELECT ct.MaLo, ct.SoLuong
                            FROM ChiTietDonHang ct
                            WHERE ct.MaDonHang = @MaDonHang";
                        
                        var details = conn.Query<dynamic>(sqlGetDetails, new { MaDonHang = maDonHang }, transaction).ToList();

                        foreach (var detail in details)
                        {
                            int maLo = detail.MaLo;
                            decimal soLuong = detail.SoLuong;

                            // Giảm số lượng hiện tại của lô
                            var sqlUpdateLot = @"
                                UPDATE LoNongSan 
                                SET SoLuongHienTai = SoLuongHienTai - @SoLuong
                                WHERE MaLo = @MaLo AND SoLuongHienTai >= @SoLuong";
                            
                            var rowsAffected = conn.Execute(sqlUpdateLot, new { MaLo = maLo, SoLuong = soLuong }, transaction);
                            
                            if (rowsAffected == 0)
                            {
                                throw new Exception($"Lô {maLo} không đủ số lượng hoặc không tồn tại");
                            }

                            // Cập nhật trạng thái lô nếu hết hàng
                            var sqlUpdateLotStatus = @"
                                UPDATE LoNongSan 
                                SET TrangThai = N'da_ban'
                                WHERE MaLo = @MaLo AND SoLuongHienTai = 0";
                            
                            conn.Execute(sqlUpdateLotStatus, new { MaLo = maLo }, transaction);

                            // Tạo tồn kho và vận chuyển cho đại lý (nếu người mua là đại lý)
                            if (orderInfo.LoaiNguoiMua == "daily")
                            {
                                int maDaiLy = orderInfo.MaNguoiMua;
                                
                                // Lấy kho của đại lý (lấy kho đầu tiên)
                                var sqlGetWarehouse = @"
                                    SELECT TOP 1 MaKho 
                                    FROM Kho 
                                    WHERE MaChuSoHuu = @MaDaiLy AND LoaiChuSoHuu = 'daily'";
                                
                                var maKho = conn.QueryFirstOrDefault<int?>(sqlGetWarehouse, new { MaDaiLy = maDaiLy }, transaction);
                                
                                if (maKho.HasValue)
                                {
                                    // Kiểm tra xem đã có tồn kho chưa
                                    var sqlCheckInventory = @"
                                        SELECT SoLuong 
                                        FROM TonKho 
                                        WHERE MaKho = @MaKho AND MaLo = @MaLo";
                                    
                                    var existingQty = conn.QueryFirstOrDefault<decimal?>(sqlCheckInventory, 
                                        new { MaKho = maKho.Value, MaLo = maLo }, transaction);

                                    if (existingQty.HasValue)
                                    {
                                        // Cập nhật số lượng tồn kho
                                        var sqlUpdateInventory = @"
                                            UPDATE TonKho 
                                            SET SoLuong = SoLuong + @SoLuong, NgayCapNhat = GETDATE()
                                            WHERE MaKho = @MaKho AND MaLo = @MaLo";
                                        
                                        conn.Execute(sqlUpdateInventory, 
                                            new { MaKho = maKho.Value, MaLo = maLo, SoLuong = soLuong }, transaction);
                                    }
                                    else
                                    {
                                        // Tạo mới tồn kho
                                        var sqlInsertInventory = @"
                                            INSERT INTO TonKho (MaKho, MaLo, SoLuong, NgayCapNhat)
                                            VALUES (@MaKho, @MaLo, @SoLuong, GETDATE())";
                                        
                                        conn.Execute(sqlInsertInventory, 
                                            new { MaKho = maKho.Value, MaLo = maLo, SoLuong = soLuong }, transaction);
                                    }
                                }

                                // Tạo đơn vận chuyển tự động
                                // Lấy địa chỉ nông dân (điểm đi) và đại lý (điểm đến)
                                var sqlGetAddresses = @"
                                    SELECT 
                                        (SELECT TOP 1 ISNULL(tt.DiaChi, nd.DiaChi) 
                                         FROM LoNongSan ln 
                                         LEFT JOIN TrangTrai tt ON ln.MaTrangTrai = tt.MaTrangTrai
                                         LEFT JOIN NongDan nd ON tt.MaNongDan = nd.MaNongDan
                                         WHERE ln.MaLo = @MaLo) AS DiemDi,
                                        (SELECT TOP 1 ISNULL(k.DiaChi, dl.DiaChi) 
                                         FROM DaiLy dl 
                                         LEFT JOIN Kho k ON k.MaChuSoHuu = dl.MaDaiLy AND k.LoaiChuSoHuu = 'daily'
                                         WHERE dl.MaDaiLy = @MaDaiLy) AS DiemDen";
                                
                                var addresses = conn.QueryFirstOrDefault<dynamic>(sqlGetAddresses, 
                                    new { MaLo = maLo, MaDaiLy = maDaiLy }, transaction);

                                string diemDi = addresses?.DiemDi ?? "Trang trại nông dân";
                                string diemDen = addresses?.DiemDen ?? "Kho đại lý";

                                // Kiểm tra chưa có vận chuyển cho lô này
                                var sqlCheckTransport = @"
                                    SELECT COUNT(*) FROM VanChuyen 
                                    WHERE MaLo = @MaLo AND TrangThai = 'dang_van_chuyen'";
                                var existingTransport = conn.QueryFirstOrDefault<int>(sqlCheckTransport, 
                                    new { MaLo = maLo }, transaction);

                                if (existingTransport == 0)
                                {
                                    var sqlInsertTransport = @"
                                        INSERT INTO VanChuyen (MaLo, DiemDi, DiemDen, NgayBatDau, TrangThai)
                                        VALUES (@MaLo, @DiemDi, @DiemDen, GETDATE(), N'dang_van_chuyen')";
                                    
                                    conn.Execute(sqlInsertTransport, 
                                        new { MaLo = maLo, DiemDi = diemDi, DiemDen = diemDen }, transaction);
                                    
                                    _logger.LogInformation("Created transport for lot {MaLo}: {DiemDi} -> {DiemDen}", maLo, diemDi, diemDen);
                                }
                            }
                        }
                    }

                    transaction.Commit();
                    _logger.LogInformation("Updated order {OrderId} status to {Status} with inventory management", maDonHang, trangThai);
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError(ex, "Error in transaction while updating order status");
                    throw;
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while updating order status");
                throw new Exception("Lỗi cập nhật cơ sở dữ liệu: " + ex.Message, ex);
            }
        }
    }
}
