using Microsoft.Data.SqlClient;
using DaiLyService.Models.DTOs;
using System.Data;

namespace DaiLyService.Data
{
    public class DonHangRepository : IDonHangRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<DonHangRepository> _logger;

        public DonHangRepository(IConfiguration config, ILogger<DonHangRepository> logger)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
            _logger = logger;
        }

        public List<DonHangDTO> GetAll()
        {
            var list = new List<DonHangDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT dh.MaDonHang, dh.LoaiDon, dh.MaNguoiBan, dh.LoaiNguoiBan, 
                           dh.MaNguoiMua, dh.LoaiNguoiMua, dh.NgayDat, dh.TrangThai, dh.TongGiaTri,
                           CASE 
                               WHEN dh.LoaiNguoiBan = 'nongdan' THEN nd.HoTen
                               WHEN dh.LoaiNguoiBan = 'daily' THEN dl.TenDaiLy
                           END AS TenNguoiBan,
                           CASE 
                               WHEN dh.LoaiNguoiMua = 'daily' THEN dl2.TenDaiLy
                               WHEN dh.LoaiNguoiMua = 'sieuthi' THEN st.TenSieuThi
                           END AS TenNguoiMua
                    FROM DonHang dh
                    LEFT JOIN NongDan nd ON dh.MaNguoiBan = nd.MaNongDan AND dh.LoaiNguoiBan = 'nongdan'
                    LEFT JOIN DaiLy dl ON dh.MaNguoiBan = dl.MaDaiLy AND dh.LoaiNguoiBan = 'daily'
                    LEFT JOIN DaiLy dl2 ON dh.MaNguoiMua = dl2.MaDaiLy AND dh.LoaiNguoiMua = 'daily'
                    LEFT JOIN SieuThi st ON dh.MaNguoiMua = st.MaSieuThi AND dh.LoaiNguoiMua = 'sieuthi'
                    WHERE (dh.LoaiNguoiBan = 'daily' OR dh.LoaiNguoiMua = 'daily')
                    ORDER BY dh.NgayDat DESC", conn);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} orders related to distributors", list.Count);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting all distributor orders");
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public List<DonHangDTO> GetByDaiLy(int maDaiLy)
        {
            var list = new List<DonHangDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT dh.MaDonHang, dh.LoaiDon, dh.MaNguoiBan, dh.LoaiNguoiBan, 
                           dh.MaNguoiMua, dh.LoaiNguoiMua, dh.NgayDat, dh.TrangThai, dh.TongGiaTri,
                           CASE 
                               WHEN dh.LoaiNguoiBan = 'nongdan' THEN nd.HoTen
                               WHEN dh.LoaiNguoiBan = 'daily' THEN dl.TenDaiLy
                           END AS TenNguoiBan,
                           CASE 
                               WHEN dh.LoaiNguoiMua = 'daily' THEN dl2.TenDaiLy
                               WHEN dh.LoaiNguoiMua = 'sieuthi' THEN st.TenSieuThi
                           END AS TenNguoiMua
                    FROM DonHang dh
                    LEFT JOIN NongDan nd ON dh.MaNguoiBan = nd.MaNongDan AND dh.LoaiNguoiBan = 'nongdan'
                    LEFT JOIN DaiLy dl ON dh.MaNguoiBan = dl.MaDaiLy AND dh.LoaiNguoiBan = 'daily'
                    LEFT JOIN DaiLy dl2 ON dh.MaNguoiMua = dl2.MaDaiLy AND dh.LoaiNguoiMua = 'daily'
                    LEFT JOIN SieuThi st ON dh.MaNguoiMua = st.MaSieuThi AND dh.LoaiNguoiMua = 'sieuthi'
                    WHERE (dh.MaNguoiBan = @MaDaiLy AND dh.LoaiNguoiBan = 'daily') 
                       OR (dh.MaNguoiMua = @MaDaiLy AND dh.LoaiNguoiMua = 'daily')
                    ORDER BY dh.NgayDat DESC", conn);
                
                cmd.Parameters.AddWithValue("@MaDaiLy", maDaiLy);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} orders for distributor {DistributorId}", list.Count, maDaiLy);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting orders for distributor {DistributorId}", maDaiLy);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public List<DonHangDTO> GetByNguoiBan(int maNguoiBan, string loaiNguoiBan)
        {
            var list = new List<DonHangDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT dh.MaDonHang, dh.LoaiDon, dh.MaNguoiBan, dh.LoaiNguoiBan, 
                           dh.MaNguoiMua, dh.LoaiNguoiMua, dh.NgayDat, dh.TrangThai, dh.TongGiaTri,
                           CASE 
                               WHEN dh.LoaiNguoiBan = 'nongdan' THEN nd.HoTen
                               WHEN dh.LoaiNguoiBan = 'daily' THEN dl.TenDaiLy
                           END AS TenNguoiBan,
                           CASE 
                               WHEN dh.LoaiNguoiMua = 'daily' THEN dl2.TenDaiLy
                               WHEN dh.LoaiNguoiMua = 'sieuthi' THEN st.TenSieuThi
                           END AS TenNguoiMua
                    FROM DonHang dh
                    LEFT JOIN NongDan nd ON dh.MaNguoiBan = nd.MaNongDan AND dh.LoaiNguoiBan = 'nongdan'
                    LEFT JOIN DaiLy dl ON dh.MaNguoiBan = dl.MaDaiLy AND dh.LoaiNguoiBan = 'daily'
                    LEFT JOIN DaiLy dl2 ON dh.MaNguoiMua = dl2.MaDaiLy AND dh.LoaiNguoiMua = 'daily'
                    LEFT JOIN SieuThi st ON dh.MaNguoiMua = st.MaSieuThi AND dh.LoaiNguoiMua = 'sieuthi'
                    WHERE dh.MaNguoiBan = @MaNguoiBan AND dh.LoaiNguoiBan = @LoaiNguoiBan
                    ORDER BY dh.NgayDat DESC", conn);
                
                cmd.Parameters.AddWithValue("@MaNguoiBan", maNguoiBan);
                cmd.Parameters.AddWithValue("@LoaiNguoiBan", loaiNguoiBan);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} orders for seller {SellerId} type {SellerType}", list.Count, maNguoiBan, loaiNguoiBan);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting orders for seller");
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public List<DonHangDTO> GetByNguoiMua(int maNguoiMua, string loaiNguoiMua)
        {
            var list = new List<DonHangDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT dh.MaDonHang, dh.LoaiDon, dh.MaNguoiBan, dh.LoaiNguoiBan, 
                           dh.MaNguoiMua, dh.LoaiNguoiMua, dh.NgayDat, dh.TrangThai, dh.TongGiaTri,
                           CASE 
                               WHEN dh.LoaiNguoiBan = 'nongdan' THEN nd.HoTen
                               WHEN dh.LoaiNguoiBan = 'daily' THEN dl.TenDaiLy
                           END AS TenNguoiBan,
                           CASE 
                               WHEN dh.LoaiNguoiMua = 'daily' THEN dl2.TenDaiLy
                               WHEN dh.LoaiNguoiMua = 'sieuthi' THEN st.TenSieuThi
                           END AS TenNguoiMua
                    FROM DonHang dh
                    LEFT JOIN NongDan nd ON dh.MaNguoiBan = nd.MaNongDan AND dh.LoaiNguoiBan = 'nongdan'
                    LEFT JOIN DaiLy dl ON dh.MaNguoiBan = dl.MaDaiLy AND dh.LoaiNguoiBan = 'daily'
                    LEFT JOIN DaiLy dl2 ON dh.MaNguoiMua = dl2.MaDaiLy AND dh.LoaiNguoiMua = 'daily'
                    LEFT JOIN SieuThi st ON dh.MaNguoiMua = st.MaSieuThi AND dh.LoaiNguoiMua = 'sieuthi'
                    WHERE dh.MaNguoiMua = @MaNguoiMua AND dh.LoaiNguoiMua = @LoaiNguoiMua
                    ORDER BY dh.NgayDat DESC", conn);
                
                cmd.Parameters.AddWithValue("@MaNguoiMua", maNguoiMua);
                cmd.Parameters.AddWithValue("@LoaiNguoiMua", loaiNguoiMua);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} orders for buyer {BuyerId} type {BuyerType}", list.Count, maNguoiMua, loaiNguoiMua);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting orders for buyer");
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public DonHangDTO? GetById(int maDonHang)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT dh.MaDonHang, dh.LoaiDon, dh.MaNguoiBan, dh.LoaiNguoiBan, 
                           dh.MaNguoiMua, dh.LoaiNguoiMua, dh.NgayDat, dh.TrangThai, dh.TongGiaTri,
                           CASE 
                               WHEN dh.LoaiNguoiBan = 'nongdan' THEN nd.HoTen
                               WHEN dh.LoaiNguoiBan = 'daily' THEN dl.TenDaiLy
                           END AS TenNguoiBan,
                           CASE 
                               WHEN dh.LoaiNguoiMua = 'daily' THEN dl2.TenDaiLy
                               WHEN dh.LoaiNguoiMua = 'sieuthi' THEN st.TenSieuThi
                           END AS TenNguoiMua
                    FROM DonHang dh
                    LEFT JOIN NongDan nd ON dh.MaNguoiBan = nd.MaNongDan AND dh.LoaiNguoiBan = 'nongdan'
                    LEFT JOIN DaiLy dl ON dh.MaNguoiBan = dl.MaDaiLy AND dh.LoaiNguoiBan = 'daily'
                    LEFT JOIN DaiLy dl2 ON dh.MaNguoiMua = dl2.MaDaiLy AND dh.LoaiNguoiMua = 'daily'
                    LEFT JOIN SieuThi st ON dh.MaNguoiMua = st.MaSieuThi AND dh.LoaiNguoiMua = 'sieuthi'
                    WHERE dh.MaDonHang = @MaDonHang 
                      AND (dh.LoaiNguoiBan = 'daily' OR dh.LoaiNguoiMua = 'daily')", conn);
                
                cmd.Parameters.AddWithValue("@MaDonHang", maDonHang);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                
                var donHang = MapToDTO(reader);
                reader.Close();
                
                // Lấy chi tiết đơn hàng
                donHang.ChiTietDonHang = GetChiTietDonHang(maDonHang);
                
                return donHang;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting order by ID {OrderId}", maDonHang);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
        }
        public int Create(DonHangCreateDTO dto)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var transaction = conn.BeginTransaction();
            
            try
            {
                
                // Tạo đơn hàng
                using var cmd = new SqlCommand(@"
                    INSERT INTO DonHang (LoaiDon, MaNguoiBan, LoaiNguoiBan, MaNguoiMua, LoaiNguoiMua, NgayDat, TrangThai, TongGiaTri) 
                    OUTPUT INSERTED.MaDonHang 
                    VALUES (@LoaiDon, @MaNguoiBan, @LoaiNguoiBan, @MaNguoiMua, @LoaiNguoiMua, @NgayDat, @TrangThai, @TongGiaTri)", conn, transaction);

                cmd.Parameters.AddWithValue("@LoaiDon", dto.LoaiDon);
                cmd.Parameters.AddWithValue("@MaNguoiBan", dto.MaNguoiBan);
                cmd.Parameters.AddWithValue("@LoaiNguoiBan", dto.LoaiNguoiBan);
                cmd.Parameters.AddWithValue("@MaNguoiMua", dto.MaNguoiMua);
                cmd.Parameters.AddWithValue("@LoaiNguoiMua", dto.LoaiNguoiMua);
                cmd.Parameters.AddWithValue("@NgayDat", DateTime.Now);
                cmd.Parameters.AddWithValue("@TrangThai", "cho_xac_nhan");
                
                // Tính tổng giá trị
                decimal tongGiaTri = dto.ChiTietDonHang.Sum(ct => ct.SoLuong * ct.DonGia);
                cmd.Parameters.AddWithValue("@TongGiaTri", tongGiaTri);

                var maDonHang = (int)cmd.ExecuteScalar();
                
                // Tạo chi tiết đơn hàng
                foreach (var chiTiet in dto.ChiTietDonHang)
                {
                    using var detailCmd = new SqlCommand(@"
                        INSERT INTO ChiTietDonHang (MaDonHang, MaLo, SoLuong, DonGia, ThanhTien) 
                        VALUES (@MaDonHang, @MaLo, @SoLuong, @DonGia, @ThanhTien)", conn, transaction);

                    detailCmd.Parameters.AddWithValue("@MaDonHang", maDonHang);
                    detailCmd.Parameters.AddWithValue("@MaLo", chiTiet.MaLo);
                    detailCmd.Parameters.AddWithValue("@SoLuong", chiTiet.SoLuong);
                    detailCmd.Parameters.AddWithValue("@DonGia", chiTiet.DonGia);
                    detailCmd.Parameters.AddWithValue("@ThanhTien", chiTiet.SoLuong * chiTiet.DonGia);

                    detailCmd.ExecuteNonQuery();
                }
                
                transaction.Commit();
                _logger.LogInformation("Created order {OrderId} with {ItemCount} items", maDonHang, dto.ChiTietDonHang.Count);
                return maDonHang;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "SQL error occurred while creating order");
                throw new Exception("Lỗi tạo đơn hàng trong cơ sở dữ liệu: " + ex.Message, ex);
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
                    using var cmd = new SqlCommand(@"
                        UPDATE DonHang 
                        SET TrangThai = @TrangThai
                        WHERE MaDonHang = @MaDonHang 
                          AND (LoaiNguoiBan = 'daily' OR LoaiNguoiMua = 'daily')", conn, transaction);

                    cmd.Parameters.AddWithValue("@MaDonHang", maDonHang);
                    cmd.Parameters.AddWithValue("@TrangThai", trangThai);

                    var rowsAffected = cmd.ExecuteNonQuery();
                    
                    if (rowsAffected == 0)
                    {
                        transaction.Rollback();
                        return false;
                    }

                    // 2. Nếu xác nhận hoàn thành, xử lý tồn kho và vận chuyển
                    if (trangThai == "hoan_thanh")
                    {
                        // Lấy thông tin đơn hàng
                        using var getOrderCmd = new SqlCommand(@"
                            SELECT MaNguoiBan, LoaiNguoiBan, MaNguoiMua, LoaiNguoiMua, LoaiDon
                            FROM DonHang WHERE MaDonHang = @MaDonHang", conn, transaction);
                        getOrderCmd.Parameters.AddWithValue("@MaDonHang", maDonHang);
                        
                        using var orderReader = getOrderCmd.ExecuteReader();
                        if (!orderReader.Read())
                        {
                            transaction.Rollback();
                            return false;
                        }
                        
                        var loaiDon = orderReader.GetString(orderReader.GetOrdinal("LoaiDon"));
                        var maNguoiMua = orderReader.GetInt32(orderReader.GetOrdinal("MaNguoiMua"));
                        var loaiNguoiMua = orderReader.GetString(orderReader.GetOrdinal("LoaiNguoiMua"));
                        orderReader.Close();

                        // Lấy chi tiết đơn hàng
                        using var getDetailsCmd = new SqlCommand(@"
                            SELECT MaLo, SoLuong FROM ChiTietDonHang WHERE MaDonHang = @MaDonHang", conn, transaction);
                        getDetailsCmd.Parameters.AddWithValue("@MaDonHang", maDonHang);
                        
                        var details = new List<(int MaLo, decimal SoLuong)>();
                        using (var detailReader = getDetailsCmd.ExecuteReader())
                        {
                            while (detailReader.Read())
                            {
                                details.Add((detailReader.GetInt32(0), detailReader.GetDecimal(1)));
                            }
                        }

                        foreach (var (maLo, soLuong) in details)
                        {
                            // Giảm số lượng lô nông sản
                            if (loaiDon == "nongdan_to_daily")
                            {
                                using var updateLotCmd = new SqlCommand(@"
                                    UPDATE LoNongSan SET SoLuongHienTai = SoLuongHienTai - @SoLuong 
                                    WHERE MaLo = @MaLo AND SoLuongHienTai >= @SoLuong", conn, transaction);
                                updateLotCmd.Parameters.AddWithValue("@MaLo", maLo);
                                updateLotCmd.Parameters.AddWithValue("@SoLuong", soLuong);
                                updateLotCmd.ExecuteNonQuery();

                                // Cập nhật trạng thái lô nếu hết
                                using var updateStatusCmd = new SqlCommand(@"
                                    UPDATE LoNongSan SET TrangThai = N'da_ban' WHERE MaLo = @MaLo AND SoLuongHienTai = 0", conn, transaction);
                                updateStatusCmd.Parameters.AddWithValue("@MaLo", maLo);
                                updateStatusCmd.ExecuteNonQuery();
                            }

                            // Tạo tồn kho cho người mua
                            string loaiChuSoHuu = loaiNguoiMua;
                            using var getWarehouseCmd = new SqlCommand(@"
                                SELECT TOP 1 MaKho FROM Kho WHERE MaChuSoHuu = @MaChuSoHuu AND LoaiChuSoHuu = @LoaiChuSoHuu", conn, transaction);
                            getWarehouseCmd.Parameters.AddWithValue("@MaChuSoHuu", maNguoiMua);
                            getWarehouseCmd.Parameters.AddWithValue("@LoaiChuSoHuu", loaiChuSoHuu);
                            var maKhoObj = getWarehouseCmd.ExecuteScalar();

                            if (maKhoObj != null)
                            {
                                int maKho = (int)maKhoObj;

                                // Kiểm tra tồn kho hiện có
                                using var checkInvCmd = new SqlCommand(@"
                                    SELECT SoLuong FROM TonKho WHERE MaKho = @MaKho AND MaLo = @MaLo", conn, transaction);
                                checkInvCmd.Parameters.AddWithValue("@MaKho", maKho);
                                checkInvCmd.Parameters.AddWithValue("@MaLo", maLo);
                                var existingQty = checkInvCmd.ExecuteScalar();

                                if (existingQty != null)
                                {
                                    using var updateInvCmd = new SqlCommand(@"
                                        UPDATE TonKho SET SoLuong = SoLuong + @SoLuong, NgayCapNhat = GETDATE() WHERE MaKho = @MaKho AND MaLo = @MaLo", conn, transaction);
                                    updateInvCmd.Parameters.AddWithValue("@MaKho", maKho);
                                    updateInvCmd.Parameters.AddWithValue("@MaLo", maLo);
                                    updateInvCmd.Parameters.AddWithValue("@SoLuong", soLuong);
                                    updateInvCmd.ExecuteNonQuery();
                                }
                                else
                                {
                                    using var insertInvCmd = new SqlCommand(@"
                                        INSERT INTO TonKho (MaKho, MaLo, SoLuong, NgayCapNhat) VALUES (@MaKho, @MaLo, @SoLuong, GETDATE())", conn, transaction);
                                    insertInvCmd.Parameters.AddWithValue("@MaKho", maKho);
                                    insertInvCmd.Parameters.AddWithValue("@MaLo", maLo);
                                    insertInvCmd.Parameters.AddWithValue("@SoLuong", soLuong);
                                    insertInvCmd.ExecuteNonQuery();
                                }
                            }

                            // Tạo đơn vận chuyển tự động
                            using var checkTransportCmd = new SqlCommand(@"
                                SELECT COUNT(*) FROM VanChuyen WHERE MaLo = @MaLo AND TrangThai = 'dang_van_chuyen'", conn, transaction);
                            checkTransportCmd.Parameters.AddWithValue("@MaLo", maLo);
                            var existingTransport = (int)checkTransportCmd.ExecuteScalar();

                            if (existingTransport == 0)
                            {
                                // Lấy địa chỉ cho vận chuyển
                                using var getAddrCmd = new SqlCommand(@"
                                    SELECT 
                                        (SELECT TOP 1 ISNULL(tt.DiaChi, nd.DiaChi) FROM LoNongSan ln 
                                         LEFT JOIN TrangTrai tt ON ln.MaTrangTrai = tt.MaTrangTrai
                                         LEFT JOIN NongDan nd ON tt.MaNongDan = nd.MaNongDan
                                         WHERE ln.MaLo = @MaLo) AS DiemDi,
                                        (SELECT TOP 1 ISNULL(k.DiaChi, dl.DiaChi) FROM DaiLy dl 
                                         LEFT JOIN Kho k ON k.MaChuSoHuu = dl.MaDaiLy AND k.LoaiChuSoHuu = 'daily'
                                         WHERE dl.MaDaiLy = @MaNguoiMua) AS DiemDen", conn, transaction);
                                getAddrCmd.Parameters.AddWithValue("@MaLo", maLo);
                                getAddrCmd.Parameters.AddWithValue("@MaNguoiMua", maNguoiMua);

                                string diemDi = "Trang trại nông dân";
                                string diemDen = "Kho đại lý";
                                using (var addrReader = getAddrCmd.ExecuteReader())
                                {
                                    if (addrReader.Read())
                                    {
                                        if (!addrReader.IsDBNull(0)) diemDi = addrReader.GetString(0);
                                        if (!addrReader.IsDBNull(1)) diemDen = addrReader.GetString(1);
                                    }
                                }

                                using var insertTransportCmd = new SqlCommand(@"
                                    INSERT INTO VanChuyen (MaLo, DiemDi, DiemDen, NgayBatDau, TrangThai)
                                    VALUES (@MaLo, @DiemDi, @DiemDen, GETDATE(), N'dang_van_chuyen')", conn, transaction);
                                insertTransportCmd.Parameters.AddWithValue("@MaLo", maLo);
                                insertTransportCmd.Parameters.AddWithValue("@DiemDi", diemDi);
                                insertTransportCmd.Parameters.AddWithValue("@DiemDen", diemDen);
                                insertTransportCmd.ExecuteNonQuery();

                                _logger.LogInformation("Auto-created transport for lot {MaLo}: {DiemDi} -> {DiemDen}", maLo, diemDi, diemDen);
                            }
                        }
                    }

                    transaction.Commit();
                    _logger.LogInformation("Updated order {OrderId} status to {Status}", maDonHang, trangThai);
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
                throw new Exception("Lỗi cập nhật trạng thái đơn hàng", ex);
            }
        }

        public bool Delete(int maDonHang)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var transaction = conn.BeginTransaction();
            
            try
            {
                
                // Xóa chi tiết đơn hàng trước
                using var detailCmd = new SqlCommand(@"
                    DELETE FROM ChiTietDonHang WHERE MaDonHang = @MaDonHang", conn, transaction);
                detailCmd.Parameters.AddWithValue("@MaDonHang", maDonHang);
                detailCmd.ExecuteNonQuery();
                
                // Xóa đơn hàng
                using var cmd = new SqlCommand(@"
                    DELETE FROM DonHang 
                    WHERE MaDonHang = @MaDonHang 
                      AND (LoaiNguoiBan = 'daily' OR LoaiNguoiMua = 'daily')", conn, transaction);
                cmd.Parameters.AddWithValue("@MaDonHang", maDonHang);

                var rowsAffected = cmd.ExecuteNonQuery();
                
                transaction.Commit();
                
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Deleted order {OrderId}", maDonHang);
                    return true;
                }
                
                return false;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "SQL error occurred while deleting order");
                throw new Exception("Lỗi xóa đơn hàng trong cơ sở dữ liệu", ex);
            }
        }

        public List<ChiTietDonHangDTO> GetChiTietDonHang(int maDonHang)
        {
            var list = new List<ChiTietDonHangDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT ct.MaDonHang, ct.MaLo, ct.SoLuong, ct.DonGia, ct.ThanhTien,
                           sp.TenSanPham, sp.DonViTinh, ln.MaQR, ln.NgayThuHoach, ln.HanSuDung
                    FROM ChiTietDonHang ct
                    LEFT JOIN LoNongSan ln ON ct.MaLo = ln.MaLo
                    LEFT JOIN SanPham sp ON ln.MaSanPham = sp.MaSanPham
                    WHERE ct.MaDonHang = @MaDonHang
                    ORDER BY ct.MaLo", conn);
                
                cmd.Parameters.AddWithValue("@MaDonHang", maDonHang);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new ChiTietDonHangDTO
                    {
                        MaDonHang = reader.GetInt32("MaDonHang"),
                        MaLo = reader.GetInt32("MaLo"),
                        SoLuong = reader.GetDecimal("SoLuong"),
                        DonGia = reader.GetDecimal("DonGia"),
                        ThanhTien = reader.GetDecimal("ThanhTien"),
                        TenSanPham = reader.IsDBNull("TenSanPham") ? null : reader.GetString("TenSanPham"),
                        DonViTinh = reader.IsDBNull("DonViTinh") ? null : reader.GetString("DonViTinh"),
                        MaQR = reader.IsDBNull("MaQR") ? null : reader.GetString("MaQR"),
                        NgayThuHoach = reader.IsDBNull("NgayThuHoach") ? null : reader.GetDateTime("NgayThuHoach"),
                        HanSuDung = reader.IsDBNull("HanSuDung") ? null : reader.GetDateTime("HanSuDung")
                    });
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting order details");
                throw new Exception("Lỗi truy vấn chi tiết đơn hàng", ex);
            }
            return list;
        }

        private static DonHangDTO MapToDTO(SqlDataReader reader)
        {
            return new DonHangDTO
            {
                MaDonHang = reader.GetInt32("MaDonHang"),
                LoaiDon = reader.GetString("LoaiDon"),
                MaNguoiBan = reader.GetInt32("MaNguoiBan"),
                LoaiNguoiBan = reader.GetString("LoaiNguoiBan"),
                MaNguoiMua = reader.GetInt32("MaNguoiMua"),
                LoaiNguoiMua = reader.GetString("LoaiNguoiMua"),
                NgayDat = reader.GetDateTime("NgayDat"),
                TrangThai = reader.GetString("TrangThai"),
                TongGiaTri = reader.GetDecimal("TongGiaTri"),
                TenNguoiBan = reader.IsDBNull("TenNguoiBan") ? null : reader.GetString("TenNguoiBan"),
                TenNguoiMua = reader.IsDBNull("TenNguoiMua") ? null : reader.GetString("TenNguoiMua")
            };
        }
    }
}