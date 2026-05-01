using Microsoft.Data.SqlClient;
using SieuThiService.Models.DTOs;

namespace SieuThiService.Data
{
    public class TruyXuatRepository : ITruyXuatRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<TruyXuatRepository> _logger;

        public TruyXuatRepository(IConfiguration config, ILogger<TruyXuatRepository> logger)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
            _logger = logger;
        }

        public TruyXuatLoInfoDTO? GetLoNongSanByQR(string maQR)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT l.MaLo, l.MaQR, sp.TenSanPham, sp.DonViTinh,
                           l.SoLuongBanDau, l.SoLuongHienTai, l.NgayThuHoach, l.HanSuDung,
                           l.TrangThai, l.NgayTao,
                           tt.TenTrangTrai, tt.DiaChi AS DiaChiTrangTrai, tt.SoChungNhan,
                           nd.HoTen AS TenNongDan, nd.SoDienThoai AS SoDienThoaiNongDan, nd.DiaChi AS DiaChiNongDan
                    FROM LoNongSan l
                    INNER JOIN SanPham sp ON l.MaSanPham = sp.MaSanPham
                    INNER JOIN TrangTrai tt ON l.MaTrangTrai = tt.MaTrangTrai
                    INNER JOIN NongDan nd ON tt.MaNongDan = nd.MaNongDan
                    WHERE l.MaQR = @MaQR", conn);

                cmd.Parameters.AddWithValue("@MaQR", maQR);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                if (!reader.Read()) return null;

                return new TruyXuatLoInfoDTO
                {
                    MaLo = reader.GetInt32(reader.GetOrdinal("MaLo")),
                    MaQR = reader.IsDBNull(reader.GetOrdinal("MaQR")) ? null : reader.GetString(reader.GetOrdinal("MaQR")),
                    TenSanPham = reader.GetString(reader.GetOrdinal("TenSanPham")),
                    DonViTinh = reader.GetString(reader.GetOrdinal("DonViTinh")),
                    SoLuongBanDau = reader.GetDecimal(reader.GetOrdinal("SoLuongBanDau")),
                    SoLuongHienTai = reader.GetDecimal(reader.GetOrdinal("SoLuongHienTai")),
                    NgayThuHoach = reader.IsDBNull(reader.GetOrdinal("NgayThuHoach")) ? null : reader.GetDateTime(reader.GetOrdinal("NgayThuHoach")).ToString("yyyy-MM-dd"),
                    HanSuDung = reader.IsDBNull(reader.GetOrdinal("HanSuDung")) ? null : reader.GetDateTime(reader.GetOrdinal("HanSuDung")).ToString("yyyy-MM-dd"),
                    TrangThai = reader.GetString(reader.GetOrdinal("TrangThai")),
                    NgayTao = reader.IsDBNull(reader.GetOrdinal("NgayTao")) ? null : reader.GetDateTime(reader.GetOrdinal("NgayTao")).ToString("yyyy-MM-ddTHH:mm:ss"),
                    TenTrangTrai = reader.GetString(reader.GetOrdinal("TenTrangTrai")),
                    DiaChiTrangTrai = reader.IsDBNull(reader.GetOrdinal("DiaChiTrangTrai")) ? null : reader.GetString(reader.GetOrdinal("DiaChiTrangTrai")),
                    SoChungNhan = reader.IsDBNull(reader.GetOrdinal("SoChungNhan")) ? null : reader.GetString(reader.GetOrdinal("SoChungNhan")),
                    TenNongDan = reader.GetString(reader.GetOrdinal("TenNongDan")),
                    SoDienThoaiNongDan = reader.IsDBNull(reader.GetOrdinal("SoDienThoaiNongDan")) ? null : reader.GetString(reader.GetOrdinal("SoDienThoaiNongDan")),
                    DiaChiNongDan = reader.IsDBNull(reader.GetOrdinal("DiaChiNongDan")) ? null : reader.GetString(reader.GetOrdinal("DiaChiNongDan")),
                };
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting LoNongSan by QR {QR}", maQR);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
        }

        public TruyXuatKiemDinhDTO? GetKiemDinh(int maLo)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT TOP 1 MaKiemDinh, NguoiKiemDinh, NgayKiemDinh, KetQua, BienBanKiemTra, ChuKySo
                    FROM KiemDinh
                    WHERE MaLo = @MaLo
                    ORDER BY NgayKiemDinh DESC", conn);

                cmd.Parameters.AddWithValue("@MaLo", maLo);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                if (!reader.Read()) return null;

                return new TruyXuatKiemDinhDTO
                {
                    MaKiemDinh = reader.GetInt32(reader.GetOrdinal("MaKiemDinh")),
                    NguoiKiemDinh = reader.GetString(reader.GetOrdinal("NguoiKiemDinh")),
                    NgayKiemDinh = reader.IsDBNull(reader.GetOrdinal("NgayKiemDinh")) ? null : reader.GetDateTime(reader.GetOrdinal("NgayKiemDinh")).ToString("yyyy-MM-ddTHH:mm:ss"),
                    KetQua = reader.GetString(reader.GetOrdinal("KetQua")),
                    BienBanKiemTra = reader.IsDBNull(reader.GetOrdinal("BienBanKiemTra")) ? null : reader.GetString(reader.GetOrdinal("BienBanKiemTra")),
                    ChuKySo = reader.IsDBNull(reader.GetOrdinal("ChuKySo")) ? null : reader.GetString(reader.GetOrdinal("ChuKySo"))
                };
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting KiemDinh for maLo {MaLo}", maLo);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
        }

        public List<TruyXuatVanChuyenDTO> GetVanChuyen(int maLo)
        {
            var list = new List<TruyXuatVanChuyenDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT MaVanChuyen, DiemDi, DiemDen, NgayBatDau, NgayKetThuc, TrangThai
                    FROM VanChuyen
                    WHERE MaLo = @MaLo
                    ORDER BY NgayBatDau ASC", conn);

                cmd.Parameters.AddWithValue("@MaLo", maLo);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new TruyXuatVanChuyenDTO
                    {
                        MaVanChuyen = reader.GetInt32(reader.GetOrdinal("MaVanChuyen")),
                        DiemDi = reader.GetString(reader.GetOrdinal("DiemDi")),
                        DiemDen = reader.GetString(reader.GetOrdinal("DiemDen")),
                        NgayBatDau = reader.IsDBNull(reader.GetOrdinal("NgayBatDau")) ? null : reader.GetDateTime(reader.GetOrdinal("NgayBatDau")).ToString("yyyy-MM-ddTHH:mm:ss"),
                        NgayKetThuc = reader.IsDBNull(reader.GetOrdinal("NgayKetThuc")) ? null : reader.GetDateTime(reader.GetOrdinal("NgayKetThuc")).ToString("yyyy-MM-ddTHH:mm:ss"),
                        TrangThai = reader.GetString(reader.GetOrdinal("TrangThai"))
                    });
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting VanChuyen for maLo {MaLo}", maLo);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }
    }
}
