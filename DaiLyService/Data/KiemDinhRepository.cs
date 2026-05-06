using Microsoft.Data.SqlClient;
using DaiLyService.Models.DTOs;
using System.Data;

namespace DaiLyService.Data
{
    public class KiemDinhRepository : IKiemDinhRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<KiemDinhRepository> _logger;

        public KiemDinhRepository(IConfiguration config, ILogger<KiemDinhRepository> logger)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
            _logger = logger;
        }

        public List<LoHangKiemDinhDTO> GetLoHangByDaiLy(int maDaiLy)
        {
            var list = new List<LoHangKiemDinhDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    WITH LatestKiemDinh AS (
                        SELECT
                            kd.*,
                            ROW_NUMBER() OVER (PARTITION BY kd.MaLo ORDER BY kd.NgayKiemDinh DESC, kd.MaKiemDinh DESC) AS rn
                        FROM KiemDinh kd
                    ),
                    PendingOrders AS (
                        SELECT ct.MaLo, COUNT(*) AS PendingCount
                        FROM DonHang dh
                        INNER JOIN ChiTietDonHang ct ON dh.MaDonHang = ct.MaDonHang
                        WHERE dh.MaNguoiMua = @MaDaiLy
                          AND dh.LoaiNguoiMua = 'daily'
                          AND dh.LoaiNguoiBan = 'nongdan'
                          AND dh.TrangThai = 'cho_kiem_dinh'
                        GROUP BY ct.MaLo
                    )
                    SELECT DISTINCT
                        ln.MaLo,
                        sp.TenSanPham,
                        nd.MaNongDan,
                        nd.HoTen AS TenNongDan,
                        ln.SoLuongHienTai AS SoLuong,
                        sp.DonViTinh,
                        ln.NgayThuHoach,
                        CASE 
                            WHEN po.PendingCount > 0 THEN 'cho_kiem_dinh'
                            WHEN kd.KetQua IS NULL THEN 'cho_kiem_dinh'
                            ELSE kd.KetQua
                        END AS TrangThaiKiemDinh,
                        kd.BienBanKiemTra AS KetQuaKiemDinh,
                        kd.NgayKiemDinh,
                        kd.MaKiemDinh,
                        CASE 
                            WHEN kd.KetQua IS NULL THEN 0
                            ELSE 1
                        END AS SortOrder
                    FROM LoNongSan ln
                    INNER JOIN SanPham sp ON ln.MaSanPham = sp.MaSanPham
                    INNER JOIN TrangTrai tt ON ln.MaTrangTrai = tt.MaTrangTrai
                    INNER JOIN NongDan nd ON tt.MaNongDan = nd.MaNongDan
                    -- Chỉ lấy lô trong đơn hàng của đại lý này
                    INNER JOIN ChiTietDonHang ct ON ln.MaLo = ct.MaLo
                    INNER JOIN DonHang dh ON ct.MaDonHang = dh.MaDonHang
                    -- Lấy kết quả kiểm định (nếu có)
                    LEFT JOIN LatestKiemDinh kd ON ln.MaLo = kd.MaLo AND kd.rn = 1
                    LEFT JOIN PendingOrders po ON ln.MaLo = po.MaLo
                    WHERE dh.MaNguoiMua = @MaDaiLy
                        AND dh.LoaiNguoiMua = 'daily'
                        AND dh.LoaiNguoiBan = 'nongdan'
                        AND ln.SoLuongHienTai > 0
                        AND dh.TrangThai IN ('cho_kiem_dinh', 'dang_van_chuyen', 'tra_hang')
                    ORDER BY SortOrder, ln.NgayThuHoach DESC", conn);

                cmd.Parameters.AddWithValue("@MaDaiLy", maDaiLy);
                conn.Open();
                
                using var reader = cmd.ExecuteReader();
                
                while (reader.Read())
                {
                    list.Add(new LoHangKiemDinhDTO
                    {
                        MaLo = reader.GetInt32(reader.GetOrdinal("MaLo")),
                        TenSanPham = reader.GetString(reader.GetOrdinal("TenSanPham")),
                        MaNongDan = reader.GetInt32(reader.GetOrdinal("MaNongDan")),
                        TenNongDan = reader.GetString(reader.GetOrdinal("TenNongDan")),
                        SoLuong = reader.GetDecimal(reader.GetOrdinal("SoLuong")),
                        DonViTinh = reader.GetString(reader.GetOrdinal("DonViTinh")),
                        NgayThuHoach = reader.IsDBNull(reader.GetOrdinal("NgayThuHoach")) 
                            ? null 
                            : reader.GetDateTime(reader.GetOrdinal("NgayThuHoach")),
                        TrangThaiKiemDinh = reader.GetString(reader.GetOrdinal("TrangThaiKiemDinh")),
                        KetQuaKiemDinh = reader.IsDBNull(reader.GetOrdinal("KetQuaKiemDinh")) 
                            ? null 
                            : reader.GetString(reader.GetOrdinal("KetQuaKiemDinh")),
                        NgayKiemDinh = reader.IsDBNull(reader.GetOrdinal("NgayKiemDinh")) 
                            ? null 
                            : reader.GetDateTime(reader.GetOrdinal("NgayKiemDinh")),
                        MaKiemDinh = reader.IsDBNull(reader.GetOrdinal("MaKiemDinh")) 
                            ? null 
                            : reader.GetInt32(reader.GetOrdinal("MaKiemDinh"))
                    });
                }
                _logger.LogInformation("Retrieved {Count} lots in orders for agent {AgentId}", list.Count, maDaiLy);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting lots for quality check. Error: {ErrorMessage}", ex.Message);
                throw new Exception($"Lỗi truy vấn cơ sở dữ liệu: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetLoHangByDaiLy");
                throw;
            }
            return list;
        }

        public List<LoHangKiemDinhDTO> GetAllLoHangAvailable()
        {
            var list = new List<LoHangKiemDinhDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    WITH LatestKiemDinh AS (
                        SELECT
                            kd.*,
                            ROW_NUMBER() OVER (PARTITION BY kd.MaLo ORDER BY kd.NgayKiemDinh DESC, kd.MaKiemDinh DESC) AS rn
                        FROM KiemDinh kd
                    )
                    SELECT 
                        ln.MaLo,
                        sp.TenSanPham,
                        nd.MaNongDan,
                        nd.HoTen AS TenNongDan,
                        ln.SoLuongHienTai AS SoLuong,
                        sp.DonViTinh,
                        ln.NgayThuHoach,
                        ln.HanSuDung,
                        CASE 
                            WHEN kd.KetQua IS NULL THEN 'cho_kiem_dinh'
                            ELSE kd.KetQua
                        END AS TrangThaiKiemDinh,
                        kd.BienBanKiemTra AS KetQuaKiemDinh,
                        kd.NgayKiemDinh,
                        kd.MaKiemDinh,
                        CASE 
                            WHEN kd.KetQua IS NULL THEN 0
                            ELSE 1
                        END AS SortOrder
                    FROM LoNongSan ln
                    INNER JOIN SanPham sp ON ln.MaSanPham = sp.MaSanPham
                    INNER JOIN TrangTrai tt ON ln.MaTrangTrai = tt.MaTrangTrai
                    INNER JOIN NongDan nd ON tt.MaNongDan = nd.MaNongDan
                    LEFT JOIN LatestKiemDinh kd ON ln.MaLo = kd.MaLo AND kd.rn = 1
                    WHERE ln.SoLuongHienTai > 0
                    ORDER BY SortOrder, ln.NgayThuHoach DESC", conn);

                conn.Open();
                
                using var reader = cmd.ExecuteReader();
                
                while (reader.Read())
                {
                    list.Add(new LoHangKiemDinhDTO
                    {
                        MaLo = reader.GetInt32(reader.GetOrdinal("MaLo")),
                        TenSanPham = reader.GetString(reader.GetOrdinal("TenSanPham")),
                        MaNongDan = reader.GetInt32(reader.GetOrdinal("MaNongDan")),
                        TenNongDan = reader.GetString(reader.GetOrdinal("TenNongDan")),
                        SoLuong = reader.GetDecimal(reader.GetOrdinal("SoLuong")),
                        DonViTinh = reader.GetString(reader.GetOrdinal("DonViTinh")),
                        NgayThuHoach = reader.IsDBNull(reader.GetOrdinal("NgayThuHoach")) 
                            ? null 
                            : reader.GetDateTime(reader.GetOrdinal("NgayThuHoach")),
                        TrangThaiKiemDinh = reader.GetString(reader.GetOrdinal("TrangThaiKiemDinh")),
                        KetQuaKiemDinh = reader.IsDBNull(reader.GetOrdinal("KetQuaKiemDinh")) 
                            ? null 
                            : reader.GetString(reader.GetOrdinal("KetQuaKiemDinh")),
                        NgayKiemDinh = reader.IsDBNull(reader.GetOrdinal("NgayKiemDinh")) 
                            ? null 
                            : reader.GetDateTime(reader.GetOrdinal("NgayKiemDinh")),
                        MaKiemDinh = reader.IsDBNull(reader.GetOrdinal("MaKiemDinh")) 
                            ? null 
                            : reader.GetInt32(reader.GetOrdinal("MaKiemDinh"))
                    });
                }
                _logger.LogInformation("Retrieved {Count} available lots", list.Count);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting available lots. Error: {ErrorMessage}", ex.Message);
                throw new Exception($"Lỗi truy vấn cơ sở dữ liệu: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetAllLoHangAvailable");
                throw;
            }
            return list;
        }

        public List<KiemDinhDTO> GetAll()
        {
            var list = new List<KiemDinhDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT kd.MaKiemDinh, kd.MaLo, kd.NguoiKiemDinh, kd.NgayKiemDinh, 
                           kd.KetQua, kd.BienBanKiemTra, kd.ChuKySo,
                           sp.TenSanPham, sp.DonViTinh, ln.MaQR, ln.NgayThuHoach, 
                           ln.HanSuDung, ln.SoLuongHienTai
                    FROM KiemDinh kd
                    LEFT JOIN LoNongSan ln ON kd.MaLo = ln.MaLo
                    LEFT JOIN SanPham sp ON ln.MaSanPham = sp.MaSanPham
                    LEFT JOIN TrangTrai tt ON ln.MaTrangTrai = tt.MaTrangTrai
                    LEFT JOIN NongDan nd ON tt.MaNongDan = nd.MaNongDan
                    LEFT JOIN DonHang dh ON EXISTS (
                        SELECT 1 FROM ChiTietDonHang ct 
                        WHERE ct.MaDonHang = dh.MaDonHang 
                        AND ct.MaLo = kd.MaLo 
                        AND dh.LoaiNguoiMua = 'daily'
                    )
                    ORDER BY kd.NgayKiemDinh DESC", conn);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} inspection records", list.Count);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting all inspection records");
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public List<KiemDinhDTO> GetByLo(int maLo)
        {
            var list = new List<KiemDinhDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT kd.MaKiemDinh, kd.MaLo, kd.NguoiKiemDinh, kd.NgayKiemDinh, 
                           kd.KetQua, kd.BienBanKiemTra, kd.ChuKySo,
                           sp.TenSanPham, sp.DonViTinh, ln.MaQR, ln.NgayThuHoach, 
                           ln.HanSuDung, ln.SoLuongHienTai
                    FROM KiemDinh kd
                    LEFT JOIN LoNongSan ln ON kd.MaLo = ln.MaLo
                    LEFT JOIN SanPham sp ON ln.MaSanPham = sp.MaSanPham
                    WHERE kd.MaLo = @MaLo
                    ORDER BY kd.NgayKiemDinh DESC", conn);
                
                cmd.Parameters.AddWithValue("@MaLo", maLo);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} inspection records for lot {LotId}", list.Count, maLo);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting inspection records for lot {LotId}", maLo);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public List<KiemDinhDTO> GetByKetQua(string ketQua)
        {
            var list = new List<KiemDinhDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT kd.MaKiemDinh, kd.MaLo, kd.NguoiKiemDinh, kd.NgayKiemDinh, 
                           kd.KetQua, kd.BienBanKiemTra, kd.ChuKySo,
                           sp.TenSanPham, sp.DonViTinh, ln.MaQR, ln.NgayThuHoach, 
                           ln.HanSuDung, ln.SoLuongHienTai
                    FROM KiemDinh kd
                    LEFT JOIN LoNongSan ln ON kd.MaLo = ln.MaLo
                    LEFT JOIN SanPham sp ON ln.MaSanPham = sp.MaSanPham
                    WHERE kd.KetQua = @KetQua
                    ORDER BY kd.NgayKiemDinh DESC", conn);
                
                cmd.Parameters.AddWithValue("@KetQua", ketQua);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} inspection records with result {Result}", list.Count, ketQua);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting inspection records by result");
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public KiemDinhDTO? GetById(int maKiemDinh)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT kd.MaKiemDinh, kd.MaLo, kd.NguoiKiemDinh, kd.NgayKiemDinh, 
                           kd.KetQua, kd.BienBanKiemTra, kd.ChuKySo,
                           sp.TenSanPham, sp.DonViTinh, ln.MaQR, ln.NgayThuHoach, 
                           ln.HanSuDung, ln.SoLuongHienTai
                    FROM KiemDinh kd
                    LEFT JOIN LoNongSan ln ON kd.MaLo = ln.MaLo
                    LEFT JOIN SanPham sp ON ln.MaSanPham = sp.MaSanPham
                    WHERE kd.MaKiemDinh = @MaKiemDinh", conn);
                
                cmd.Parameters.AddWithValue("@MaKiemDinh", maKiemDinh);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                return MapToDTO(reader);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting inspection record by ID {InspectionId}", maKiemDinh);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
        }

        public int Create(KiemDinhCreateDTO dto)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                using var transaction = conn.BeginTransaction();

                try
                {
                    using var cmd = new SqlCommand(@"
                        INSERT INTO KiemDinh (MaLo, NguoiKiemDinh, NgayKiemDinh, KetQua, BienBanKiemTra, ChuKySo) 
                        OUTPUT INSERTED.MaKiemDinh 
                        VALUES (@MaLo, @NguoiKiemDinh, @NgayKiemDinh, @KetQua, @BienBanKiemTra, @ChuKySo)", conn, transaction);

                    cmd.Parameters.AddWithValue("@MaLo", dto.MaLo);
                    cmd.Parameters.AddWithValue("@NguoiKiemDinh", dto.NguoiKiemDinh);
                    cmd.Parameters.AddWithValue("@NgayKiemDinh", dto.NgayKiemDinh ?? DateTime.Now);
                    cmd.Parameters.AddWithValue("@KetQua", dto.KetQua);
                    cmd.Parameters.AddWithValue("@BienBanKiemTra", (object?)dto.BienBanKiemTra ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ChuKySo", (object?)dto.ChuKySo ?? DBNull.Value);

                    var maKiemDinh = (int)cmd.ExecuteScalar();

                    var targetOrderStatus = dto.KetQua == "dat" ? "dang_van_chuyen" : "tra_hang";
                    using var orderStatusCmd = new SqlCommand(@"
                        UPDATE dh
                        SET dh.TrangThai = @TrangThai
                        FROM DonHang dh
                        INNER JOIN ChiTietDonHang ct ON dh.MaDonHang = ct.MaDonHang
                        WHERE ct.MaLo = @MaLo
                          AND dh.LoaiDon = 'nongdan_to_daily'
                          AND dh.TrangThai = 'cho_kiem_dinh'", conn, transaction);
                    orderStatusCmd.Parameters.AddWithValue("@MaLo", dto.MaLo);
                    orderStatusCmd.Parameters.AddWithValue("@TrangThai", targetOrderStatus);
                    orderStatusCmd.ExecuteNonQuery();

                    if (dto.KetQua == "dat")
                    {
                        using var checkTransportCmd = new SqlCommand(@"
                            SELECT COUNT(*) FROM VanChuyen WHERE MaLo = @MaLo AND TrangThai = 'dang_van_chuyen'", conn, transaction);
                        checkTransportCmd.Parameters.AddWithValue("@MaLo", dto.MaLo);
                        var existingTransport = (int)checkTransportCmd.ExecuteScalar();

                        if (existingTransport == 0)
                        {
                            using var getOrderInfoCmd = new SqlCommand(@"
                                SELECT TOP 1 dh.MaNguoiMua
                                FROM DonHang dh
                                INNER JOIN ChiTietDonHang ct ON dh.MaDonHang = ct.MaDonHang
                                WHERE ct.MaLo = @MaLo
                                  AND dh.LoaiDon = 'nongdan_to_daily'
                                  AND dh.TrangThai = 'dang_van_chuyen'
                                ORDER BY dh.NgayDat DESC", conn, transaction);
                            getOrderInfoCmd.Parameters.AddWithValue("@MaLo", dto.MaLo);
                            var maDaiLyObj = getOrderInfoCmd.ExecuteScalar();
                            var maDaiLy = maDaiLyObj == null ? 0 : (int)maDaiLyObj;

                            using var getAddrCmd = new SqlCommand(@"
                                SELECT 
                                    (SELECT TOP 1 ISNULL(tt.DiaChi, nd.DiaChi) FROM LoNongSan ln 
                                     LEFT JOIN TrangTrai tt ON ln.MaTrangTrai = tt.MaTrangTrai
                                     LEFT JOIN NongDan nd ON tt.MaNongDan = nd.MaNongDan
                                     WHERE ln.MaLo = @MaLo) AS DiemDi,
                                    (SELECT TOP 1 ISNULL(k.DiaChi, dl.DiaChi) FROM DaiLy dl 
                                     LEFT JOIN Kho k ON k.MaChuSoHuu = dl.MaDaiLy AND k.LoaiChuSoHuu = 'daily'
                                     WHERE dl.MaDaiLy = @MaDaiLy) AS DiemDen", conn, transaction);
                            getAddrCmd.Parameters.AddWithValue("@MaLo", dto.MaLo);
                            getAddrCmd.Parameters.AddWithValue("@MaDaiLy", maDaiLy);

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
                            insertTransportCmd.Parameters.AddWithValue("@MaLo", dto.MaLo);
                            insertTransportCmd.Parameters.AddWithValue("@DiemDi", diemDi);
                            insertTransportCmd.Parameters.AddWithValue("@DiemDen", diemDen);
                            insertTransportCmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                
                    _logger.LogInformation("Created inspection record {InspectionId} for lot {LotId} with result {Result}", maKiemDinh, dto.MaLo, dto.KetQua);
                    return maKiemDinh;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while creating inspection record");
                if (ex.Number == 547) // Foreign key constraint
                    throw new Exception("Lô nông sản không tồn tại trong hệ thống", ex);
                throw new Exception("Lỗi tạo kiểm định trong cơ sở dữ liệu: " + ex.Message, ex);
            }
        }

        public bool Update(int maKiemDinh, KiemDinhUpdateDTO dto)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    UPDATE KiemDinh 
                    SET NguoiKiemDinh = @NguoiKiemDinh, 
                        KetQua = @KetQua, 
                        BienBanKiemTra = @BienBanKiemTra, 
                        ChuKySo = @ChuKySo
                    WHERE MaKiemDinh = @MaKiemDinh", conn);

                cmd.Parameters.AddWithValue("@MaKiemDinh", maKiemDinh);
                cmd.Parameters.AddWithValue("@NguoiKiemDinh", dto.NguoiKiemDinh);
                cmd.Parameters.AddWithValue("@KetQua", dto.KetQua);
                cmd.Parameters.AddWithValue("@BienBanKiemTra", (object?)dto.BienBanKiemTra ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ChuKySo", (object?)dto.ChuKySo ?? DBNull.Value);

                conn.Open();
                var rowsAffected = cmd.ExecuteNonQuery();
                
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Updated inspection record {InspectionId}", maKiemDinh);
                    return true;
                }
                
                return false;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while updating inspection record");
                throw new Exception("Lỗi cập nhật kiểm định", ex);
            }
        }

        public bool Delete(int maKiemDinh)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("DELETE FROM KiemDinh WHERE MaKiemDinh = @MaKiemDinh", conn);
                cmd.Parameters.AddWithValue("@MaKiemDinh", maKiemDinh);

                conn.Open();
                var rowsAffected = cmd.ExecuteNonQuery();
                
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Deleted inspection record {InspectionId}", maKiemDinh);
                    return true;
                }
                
                return false;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while deleting inspection record");
                throw new Exception("Lỗi xóa kiểm định trong cơ sở dữ liệu", ex);
            }
        }

        public int CountByKetQua(string ketQua)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SELECT COUNT(*) FROM KiemDinh WHERE KetQua = @KetQua", conn);
                cmd.Parameters.AddWithValue("@KetQua", ketQua);

                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while counting inspection records by result");
                throw new Exception("Lỗi đếm số lượng kiểm định", ex);
            }
        }

        public object GetStatsByDaiLy(int maDaiLy)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    WITH LatestKiemDinh AS (
                        SELECT
                            kd.*,
                            ROW_NUMBER() OVER (PARTITION BY kd.MaLo ORDER BY kd.NgayKiemDinh DESC, kd.MaKiemDinh DESC) AS rn
                        FROM KiemDinh kd
                    ),
                    PendingOrders AS (
                        SELECT ct.MaLo
                        FROM DonHang dh
                        INNER JOIN ChiTietDonHang ct ON dh.MaDonHang = ct.MaDonHang
                        WHERE dh.MaNguoiMua = @MaDaiLy
                          AND dh.LoaiNguoiMua = 'daily'
                          AND dh.LoaiNguoiBan = 'nongdan'
                          AND dh.TrangThai = 'cho_kiem_dinh'
                        GROUP BY ct.MaLo
                    )
                    SELECT 
                        COUNT(DISTINCT CASE WHEN po.MaLo IS NOT NULL THEN ln.MaLo END) AS ChoKiemDinh,
                        COUNT(DISTINCT CASE WHEN po.MaLo IS NULL AND kd.KetQua = 'dat' THEN ln.MaLo END) AS DatChuanCount,
                        COUNT(DISTINCT CASE WHEN po.MaLo IS NULL AND kd.KetQua = 'khong_dat' THEN ln.MaLo END) AS KhongDatCount
                    FROM LoNongSan ln
                    INNER JOIN ChiTietDonHang ct ON ln.MaLo = ct.MaLo
                    INNER JOIN DonHang dh ON ct.MaDonHang = dh.MaDonHang
                    LEFT JOIN LatestKiemDinh kd ON ln.MaLo = kd.MaLo AND kd.rn = 1
                    LEFT JOIN PendingOrders po ON ln.MaLo = po.MaLo
                    WHERE dh.MaNguoiMua = @MaDaiLy
                        AND dh.LoaiNguoiMua = 'daily'
                        AND dh.LoaiNguoiBan = 'nongdan'
                        AND ln.SoLuongHienTai > 0
                        AND dh.TrangThai IN ('cho_kiem_dinh', 'dang_van_chuyen', 'tra_hang')", conn);

                cmd.Parameters.AddWithValue("@MaDaiLy", maDaiLy);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new
                    {
                        choKiemDinh = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                        datChuanCount = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                        khongDatCount = reader.IsDBNull(2) ? 0 : reader.GetInt32(2)
                    };
                }

                return new { choKiemDinh = 0, datChuanCount = 0, khongDatCount = 0 };
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting inspection stats for agent {AgentId}", maDaiLy);
                throw new Exception("Lỗi truy vấn thống kê kiểm định", ex);
            }
        }

        private static KiemDinhDTO MapToDTO(SqlDataReader reader)
        {
            return new KiemDinhDTO
            {
                MaKiemDinh = reader.GetInt32("MaKiemDinh"),
                MaLo = reader.GetInt32("MaLo"),
                NguoiKiemDinh = reader.GetString("NguoiKiemDinh"),
                NgayKiemDinh = reader.GetDateTime("NgayKiemDinh"),
                KetQua = reader.GetString("KetQua"),
                BienBanKiemTra = reader.IsDBNull("BienBanKiemTra") ? null : reader.GetString("BienBanKiemTra"),
                ChuKySo = reader.IsDBNull("ChuKySo") ? null : reader.GetString("ChuKySo"),
                TenSanPham = reader.IsDBNull("TenSanPham") ? null : reader.GetString("TenSanPham"),
                DonViTinh = reader.IsDBNull("DonViTinh") ? null : reader.GetString("DonViTinh"),
                MaQR = reader.IsDBNull("MaQR") ? null : reader.GetString("MaQR"),
                NgayThuHoach = reader.IsDBNull("NgayThuHoach") ? null : reader.GetDateTime("NgayThuHoach"),
                HanSuDung = reader.IsDBNull("HanSuDung") ? null : reader.GetDateTime("HanSuDung"),
                SoLuongLo = reader.IsDBNull("SoLuongHienTai") ? null : reader.GetDecimal("SoLuongHienTai")
            };
        }
    }
}