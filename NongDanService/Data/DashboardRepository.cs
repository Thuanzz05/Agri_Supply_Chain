using Microsoft.Data.SqlClient;

namespace NongDanService.Data
{
    public interface IDashboardRepository
    {
        int GetTotalSanPham(int maNongDan);
        int GetTotalTrangTrai(int maNongDan);
        int GetTotalLoNongSan(int maNongDan);
        int GetTotalDonHang(int maNongDan);
        List<object> GetRecentOrders(int maNongDan, int limit = 5);
        object GetOrderStats(int maNongDan);
    }

    public class DashboardRepository : IDashboardRepository
    {
        private readonly string _connectionString;

        public DashboardRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int GetTotalSanPham(int maNongDan)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            // Đếm số sản phẩm khác nhau mà nông dân đang có trong lô nông sản
            using var cmd = new SqlCommand(@"
                SELECT COUNT(DISTINCT lo.MaSanPham) 
                FROM LoNongSan lo
                INNER JOIN TrangTrai tt ON lo.MaTrangTrai = tt.MaTrangTrai
                WHERE tt.MaNongDan = @MaNongDan", conn);
            cmd.Parameters.AddWithValue("@MaNongDan", maNongDan);
            return (int)cmd.ExecuteScalar();
        }

        public int GetTotalTrangTrai(int maNongDan)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM TrangTrai WHERE MaNongDan = @MaNongDan", conn);
            cmd.Parameters.AddWithValue("@MaNongDan", maNongDan);
            return (int)cmd.ExecuteScalar();
        }

        public int GetTotalLoNongSan(int maNongDan)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(@"
                SELECT COUNT(*) 
                FROM LoNongSan lo
                INNER JOIN TrangTrai tt ON lo.MaTrangTrai = tt.MaTrangTrai
                WHERE tt.MaNongDan = @MaNongDan", conn);
            cmd.Parameters.AddWithValue("@MaNongDan", maNongDan);
            return (int)cmd.ExecuteScalar();
        }

        public int GetTotalDonHang(int maNongDan)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(@"
                SELECT COUNT(*) 
                FROM DonHang 
                WHERE MaNguoiBan = @MaNongDan AND LoaiNguoiBan = N'nongdan'", conn);
            cmd.Parameters.AddWithValue("@MaNongDan", maNongDan);
            return (int)cmd.ExecuteScalar();
        }

        public List<object> GetRecentOrders(int maNongDan, int limit = 5)
        {
            var result = new List<object>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(@"
                SELECT TOP (@Limit)
                    dh.MaDonHang,
                    dh.NgayDat,
                    dh.TrangThai,
                    dh.TongGiaTri,
                    dh.MaNguoiMua,
                    dh.LoaiNguoiMua
                FROM DonHang dh
                WHERE dh.MaNguoiBan = @MaNongDan AND dh.LoaiNguoiBan = N'nongdan'
                ORDER BY dh.NgayDat DESC", conn);
            cmd.Parameters.AddWithValue("@MaNongDan", maNongDan);
            cmd.Parameters.AddWithValue("@Limit", limit);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new
                {
                    maDonHang = (int)reader["MaDonHang"],
                    ngayDat = ((DateTime)reader["NgayDat"]).ToString("dd/MM/yyyy"),
                    trangThai = reader["TrangThai"].ToString(),
                    tongGiaTri = reader["TongGiaTri"] != DBNull.Value ? (decimal)reader["TongGiaTri"] : 0,
                    maNguoiMua = (int)reader["MaNguoiMua"],
                    loaiNguoiMua = reader["LoaiNguoiMua"].ToString()
                });
            }

            return result;
        }

        public object GetOrderStats(int maNongDan)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(@"
                SELECT 
                    COUNT(*) as TongDonHang,
                    COUNT(CASE WHEN TrangThai = N'cho_xac_nhan' THEN 1 END) as ChoXacNhan,
                    COUNT(CASE WHEN TrangThai = N'hoan_thanh' THEN 1 END) as HoanThanh,
                    SUM(TongGiaTri) as TongGiaTri
                FROM DonHang 
                WHERE MaNguoiBan = @MaNongDan AND LoaiNguoiBan = N'nongdan'", conn);
            cmd.Parameters.AddWithValue("@MaNongDan", maNongDan);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new
                {
                    tongDonHang = (int)reader["TongDonHang"],
                    choXacNhan = (int)reader["ChoXacNhan"],
                    hoanThanh = (int)reader["HoanThanh"],
                    tongGiaTri = reader["TongGiaTri"] != DBNull.Value ? (decimal)reader["TongGiaTri"] : 0
                };
            }

            return new { tongDonHang = 0, choXacNhan = 0, hoanThanh = 0, tongGiaTri = 0 };
        }
    }
}
