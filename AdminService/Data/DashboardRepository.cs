using Microsoft.Data.SqlClient;

namespace AdminService.Data
{
    public class DashboardRepository
    {
        private readonly string _connectionString;

        public DashboardRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int GetTotalNongDan()
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand("SELECT COUNT(*) FROM NongDan", conn);
            return (int)cmd.ExecuteScalar();
        }

        public int GetTotalDaiLy()
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand("SELECT COUNT(*) FROM DaiLy", conn);
            return (int)cmd.ExecuteScalar();
        }

        public int GetTotalSieuThi()
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand("SELECT COUNT(*) FROM SieuThi", conn);
            return (int)cmd.ExecuteScalar();
        }

        public int GetTotalTaiKhoan()
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand("SELECT COUNT(*) FROM TaiKhoan", conn);
            return (int)cmd.ExecuteScalar();
        }

        public int GetTotalLoNongSan()
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand("SELECT COUNT(*) FROM LoNongSan", conn);
            return (int)cmd.ExecuteScalar();
        }

        public int GetTotalDonHang()
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand("SELECT COUNT(*) FROM DonHang", conn);
            return (int)cmd.ExecuteScalar();
        }

        public int GetTotalKiemDinh()
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand("SELECT COUNT(*) FROM KiemDinh", conn);
            return (int)cmd.ExecuteScalar();
        }

        public object GetUserStatsByType()
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(@"
                SELECT 
                    LoaiTaiKhoan,
                    COUNT(*) as SoLuong,
                    SUM(CASE WHEN TrangThai = 'hoat_dong' THEN 1 ELSE 0 END) as HoatDong,
                    SUM(CASE WHEN TrangThai = 'khoa' THEN 1 ELSE 0 END) as Khoa
                FROM TaiKhoan
                GROUP BY LoaiTaiKhoan", conn);

            var result = new Dictionary<string, object>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var loai = reader["LoaiTaiKhoan"].ToString();
                result[loai!] = new
                {
                    SoLuong = (int)reader["SoLuong"],
                    HoatDong = (int)reader["HoatDong"],
                    Khoa = (int)reader["Khoa"]
                };
            }

            return result;
        }
    }
}
