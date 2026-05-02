using Microsoft.Data.SqlClient;

namespace DaiLyService.Data
{
    public interface IDashboardRepository
    {
        int GetTotalKho(int maDaiLy);
        int GetTotalWarehouses(int maDaiLy);
        int GetTotalDonHangMua(int maDaiLy);
        int GetTotalDonHangBan(int maDaiLy);
        object GetOrderStats(int maDaiLy);
        List<object> GetRecentOrders(int maDaiLy, int limit = 5);
    }

    public class DashboardRepository : IDashboardRepository
    {
        private readonly string _connectionString;

        public DashboardRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int GetTotalKho(int maDaiLy)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(@"
                SELECT COALESCE(SUM(CAST(tk.SoLuong AS INT)), 0)
                FROM TonKho tk
                INNER JOIN Kho k ON tk.MaKho = k.MaKho
                WHERE k.MaChuSoHuu = @MaDaiLy AND k.LoaiChuSoHuu = N'daily'", conn);
            cmd.Parameters.AddWithValue("@MaDaiLy", maDaiLy);
            var result = cmd.ExecuteScalar();
            return result != DBNull.Value ? (int)result : 0;
        }

        public int GetTotalWarehouses(int maDaiLy)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(@"
                SELECT COUNT(*)
                FROM Kho
                WHERE MaChuSoHuu = @MaDaiLy AND LoaiChuSoHuu = N'daily'", conn);
            cmd.Parameters.AddWithValue("@MaDaiLy", maDaiLy);
            return (int)cmd.ExecuteScalar();
        }

        public int GetTotalDonHangMua(int maDaiLy)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(@"
                SELECT COUNT(*) 
                FROM DonHang 
                WHERE MaNguoiMua = @MaDaiLy AND LoaiNguoiMua = N'daily'", conn);
            cmd.Parameters.AddWithValue("@MaDaiLy", maDaiLy);
            return (int)cmd.ExecuteScalar();
        }

        public int GetTotalDonHangBan(int maDaiLy)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(@"
                SELECT COUNT(*) 
                FROM DonHang 
                WHERE MaNguoiBan = @MaDaiLy AND LoaiNguoiBan = N'daily'", conn);
            cmd.Parameters.AddWithValue("@MaDaiLy", maDaiLy);
            return (int)cmd.ExecuteScalar();
        }

        public object GetOrderStats(int maDaiLy)
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
                WHERE MaNguoiBan = @MaDaiLy AND LoaiNguoiBan = N'daily'", conn);
            cmd.Parameters.AddWithValue("@MaDaiLy", maDaiLy);

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

        public List<object> GetRecentOrders(int maDaiLy, int limit = 5)
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
                WHERE dh.MaNguoiBan = @MaDaiLy AND dh.LoaiNguoiBan = N'daily'
                ORDER BY dh.NgayDat DESC", conn);
            cmd.Parameters.AddWithValue("@MaDaiLy", maDaiLy);
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
    }
}
