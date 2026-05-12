using Microsoft.Data.SqlClient;
using SieuThiService.Models.DTOs;

namespace SieuThiService.Data
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

        public List<DonHangDTO> GetBySieuThi(int maSieuThi)
        {
            var list = new List<DonHangDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT dh.MaDonHang, dh.LoaiDon, dh.MaNguoiBan, dh.LoaiNguoiBan, 
                           dh.MaNguoiMua, dh.LoaiNguoiMua, dh.NgayDat, dh.TrangThai, dh.TongGiaTri,
                           dl.TenDaiLy AS TenNguoiBan,
                           st.TenSieuThi AS TenNguoiMua
                    FROM DonHang dh
                    LEFT JOIN DaiLy dl ON dh.MaNguoiBan = dl.MaDaiLy AND dh.LoaiNguoiBan = 'daily'
                    LEFT JOIN SieuThi st ON dh.MaNguoiMua = st.MaSieuThi AND dh.LoaiNguoiMua = 'sieuthi'
                    WHERE dh.MaNguoiMua = @MaSieuThi 
                      AND dh.LoaiNguoiMua = 'sieuthi'
                      AND dh.LoaiDon = 'daily_to_sieuthi'
                    ORDER BY dh.NgayDat DESC", conn);
                
                cmd.Parameters.AddWithValue("@MaSieuThi", maSieuThi);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new DonHangDTO
                    {
                        MaDonHang = reader.GetInt32(reader.GetOrdinal("MaDonHang")),
                        LoaiDon = reader.GetString(reader.GetOrdinal("LoaiDon")),
                        MaNguoiBan = reader.GetInt32(reader.GetOrdinal("MaNguoiBan")),
                        LoaiNguoiBan = reader.GetString(reader.GetOrdinal("LoaiNguoiBan")),
                        MaNguoiMua = reader.GetInt32(reader.GetOrdinal("MaNguoiMua")),
                        LoaiNguoiMua = reader.GetString(reader.GetOrdinal("LoaiNguoiMua")),
                        NgayDat = reader.GetDateTime(reader.GetOrdinal("NgayDat")),
                        TrangThai = reader.GetString(reader.GetOrdinal("TrangThai")),
                        TongGiaTri = reader.GetDecimal(reader.GetOrdinal("TongGiaTri")),
                        TenNguoiBan = reader.IsDBNull(reader.GetOrdinal("TenNguoiBan")) ? null : reader.GetString(reader.GetOrdinal("TenNguoiBan")),
                        TenNguoiMua = reader.IsDBNull(reader.GetOrdinal("TenNguoiMua")) ? null : reader.GetString(reader.GetOrdinal("TenNguoiMua"))
                    });
                }
                _logger.LogInformation("Retrieved {Count} orders for supermarket {SupermarketId}", list.Count, maSieuThi);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting orders for supermarket");
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
                           dl.TenDaiLy AS TenNguoiBan,
                           st.TenSieuThi AS TenNguoiMua
                    FROM DonHang dh
                    LEFT JOIN DaiLy dl ON dh.MaNguoiBan = dl.MaDaiLy AND dh.LoaiNguoiBan = 'daily'
                    LEFT JOIN SieuThi st ON dh.MaNguoiMua = st.MaSieuThi AND dh.LoaiNguoiMua = 'sieuthi'
                    WHERE dh.MaDonHang = @MaDonHang 
                      AND dh.LoaiDon = 'daily_to_sieuthi'", conn);
                
                cmd.Parameters.AddWithValue("@MaDonHang", maDonHang);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                
                return new DonHangDTO
                {
                    MaDonHang = reader.GetInt32(reader.GetOrdinal("MaDonHang")),
                    LoaiDon = reader.GetString(reader.GetOrdinal("LoaiDon")),
                    MaNguoiBan = reader.GetInt32(reader.GetOrdinal("MaNguoiBan")),
                    LoaiNguoiBan = reader.GetString(reader.GetOrdinal("LoaiNguoiBan")),
                    MaNguoiMua = reader.GetInt32(reader.GetOrdinal("MaNguoiMua")),
                    LoaiNguoiMua = reader.GetString(reader.GetOrdinal("LoaiNguoiMua")),
                    NgayDat = reader.GetDateTime(reader.GetOrdinal("NgayDat")),
                    TrangThai = reader.GetString(reader.GetOrdinal("TrangThai")),
                    TongGiaTri = reader.GetDecimal(reader.GetOrdinal("TongGiaTri")),
                    TenNguoiBan = reader.IsDBNull(reader.GetOrdinal("TenNguoiBan")) ? null : reader.GetString(reader.GetOrdinal("TenNguoiBan")),
                    TenNguoiMua = reader.IsDBNull(reader.GetOrdinal("TenNguoiMua")) ? null : reader.GetString(reader.GetOrdinal("TenNguoiMua"))
                };
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting order by ID {OrderId}", maDonHang);
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
                    // Lấy thông tin đơn hàng
                    using var orderCmd = new SqlCommand(@"
                        SELECT LoaiDon, TrangThai, MaNguoiBan, LoaiNguoiBan, MaNguoiMua, LoaiNguoiMua
                        FROM DonHang
                        WHERE MaDonHang = @MaDonHang
                          AND LoaiDon = 'daily_to_sieuthi'", conn, transaction);
                    orderCmd.Parameters.AddWithValue("@MaDonHang", maDonHang);

                    string loaiDon;
                    string currentStatus;
                    int maNguoiBan;
                    int maNguoiMua;
                    
                    using (var orderReader = orderCmd.ExecuteReader())
                    {
                        if (!orderReader.Read())
                        {
                            transaction.Rollback();
                            return false;
                        }
                        loaiDon = orderReader.GetString(orderReader.GetOrdinal("LoaiDon"));
                        currentStatus = orderReader.GetString(orderReader.GetOrdinal("TrangThai"));
                        maNguoiBan = orderReader.GetInt32(orderReader.GetOrdinal("MaNguoiBan"));
                        maNguoiMua = orderReader.GetInt32(orderReader.GetOrdinal("MaNguoiMua"));
                    }

                    // Validate transition
                    bool validTransition = 
                        (currentStatus == "cho_xac_nhan" && (trangThai == "dang_van_chuyen" || trangThai == "da_huy")) ||
                        (currentStatus == "dang_van_chuyen" && trangThai == "hoan_thanh");

                    if (!validTransition)
                    {
                        throw new Exception($"Không thể chuyển trạng thái từ '{currentStatus}' sang '{trangThai}'");
                    }

                    // Nếu siêu thị xác nhận (chuyển sang dang_van_chuyen), tạo vận chuyển
                    if (currentStatus == "cho_xac_nhan" && trangThai == "dang_van_chuyen")
                    {
                        _logger.LogInformation("Creating transports for order {OrderId}", maDonHang);
                        
                        // Lấy địa chỉ
                        string diemDi = "";
                        string diemDen = "";
                        
                        using (var getDaiLyCmd = new SqlCommand(@"
                            SELECT DiaChi FROM DaiLy WHERE MaDaiLy = @MaDaiLy", conn, transaction))
                        {
                            getDaiLyCmd.Parameters.AddWithValue("@MaDaiLy", maNguoiBan);
                            var result = getDaiLyCmd.ExecuteScalar();
                            diemDi = result != null && result != DBNull.Value ? result.ToString()! : $"Đại lý {maNguoiBan}";
                        }
                        
                        using (var getSieuThiCmd = new SqlCommand(@"
                            SELECT DiaChi FROM SieuThi WHERE MaSieuThi = @MaSieuThi", conn, transaction))
                        {
                            getSieuThiCmd.Parameters.AddWithValue("@MaSieuThi", maNguoiMua);
                            var result = getSieuThiCmd.ExecuteScalar();
                            diemDen = result != null && result != DBNull.Value ? result.ToString()! : $"Siêu thị {maNguoiMua}";
                        }
                        
                        // Lấy danh sách lô
                        var chiTietList = new List<int>();
                        using (var getChiTietCmd = new SqlCommand(@"
                            SELECT MaLo FROM ChiTietDonHang WHERE MaDonHang = @MaDonHang", conn, transaction))
                        {
                            getChiTietCmd.Parameters.AddWithValue("@MaDonHang", maDonHang);
                            using var reader = getChiTietCmd.ExecuteReader();
                            while (reader.Read())
                            {
                                chiTietList.Add(reader.GetInt32(0));
                            }
                        }
                        
                        if (chiTietList.Count == 0)
                        {
                            throw new Exception($"Không tìm thấy chi tiết đơn hàng {maDonHang}");
                        }
                        
                        // Tạo vận chuyển cho từng lô
                        foreach (var maLo in chiTietList)
                        {
                            using var createVanChuyenCmd = new SqlCommand(@"
                                INSERT INTO VanChuyen (MaLo, MaDonHang, DiemDi, DiemDen, NgayBatDau, TrangThai)
                                VALUES (@MaLo, @MaDonHang, @DiemDi, @DiemDen, @NgayBatDau, @TrangThai)", conn, transaction);
                            
                            createVanChuyenCmd.Parameters.AddWithValue("@MaLo", maLo);
                            createVanChuyenCmd.Parameters.AddWithValue("@MaDonHang", maDonHang);
                            createVanChuyenCmd.Parameters.AddWithValue("@DiemDi", diemDi);
                            createVanChuyenCmd.Parameters.AddWithValue("@DiemDen", diemDen);
                            createVanChuyenCmd.Parameters.AddWithValue("@NgayBatDau", DateTime.Now);
                            createVanChuyenCmd.Parameters.AddWithValue("@TrangThai", "dang_van_chuyen");
                            
                            createVanChuyenCmd.ExecuteNonQuery();
                            _logger.LogInformation("Created transport for lot {LotId} in order {OrderId}", maLo, maDonHang);
                        }
                    }

                    // Cập nhật trạng thái đơn hàng
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
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while updating order status");
                throw new Exception("Lỗi cập nhật trạng thái đơn hàng", ex);
            }
        }
    }
}