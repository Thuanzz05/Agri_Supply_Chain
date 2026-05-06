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
                    using var orderCmd = new SqlCommand(@"
                        SELECT LoaiDon, TrangThai
                        FROM DonHang
                        WHERE MaDonHang = @MaDonHang
                          AND (LoaiNguoiBan = 'daily' OR LoaiNguoiMua = 'daily')", conn, transaction);
                    orderCmd.Parameters.AddWithValue("@MaDonHang", maDonHang);

                    string loaiDon;
                    string currentStatus;
                    using (var orderReader = orderCmd.ExecuteReader())
                    {
                        if (!orderReader.Read())
                        {
                            transaction.Rollback();
                            return false;
                        }
                        loaiDon = orderReader.GetString(orderReader.GetOrdinal("LoaiDon"));
                        currentStatus = orderReader.GetString(orderReader.GetOrdinal("TrangThai"));
                    }

                    bool validTransition = false;
                    if (loaiDon == "nongdan_to_daily")
                    {
                        validTransition =
                            (currentStatus == "cho_xac_nhan" && (trangThai == "cho_kiem_dinh" || trangThai == "da_huy")) ||
                            (currentStatus == "cho_kiem_dinh" && (trangThai == "dang_van_chuyen" || trangThai == "tra_hang")) ||
                            (currentStatus == "dang_van_chuyen" && trangThai == "hoan_thanh");
                    }
                    else if (loaiDon == "daily_to_sieuthi")
                    {
                        validTransition =
                            (currentStatus == "cho_xac_nhan" && (trangThai == "hoan_thanh" || trangThai == "da_huy"));
                    }

                    if (!validTransition)
                    {
                        throw new Exception($"Không thể chuyển trạng thái đơn {loaiDon} từ '{currentStatus}' sang '{trangThai}'");
                    }

                    using var cmd = new SqlCommand(@"
                        UPDATE DonHang 
                        SET TrangThai = @TrangThai
                        WHERE MaDonHang = @MaDonHang", conn, transaction);

                    cmd.Parameters.AddWithValue("@MaDonHang", maDonHang);
                    cmd.Parameters.AddWithValue("@TrangThai", trangThai);

                    var rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        transaction.Rollback();
                        return false;
                    }

                    transaction.Commit();
                    _logger.LogInformation("Updated order {OrderId} status {FromStatus} -> {ToStatus}", maDonHang, currentStatus, trangThai);
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