using Microsoft.Data.SqlClient;

namespace AdminService.Data
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<object> GetAllNongDan()
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(@"
                SELECT 
                    nd.MaNongDan as Id,
                    tk.TenDangNhap,
                    tk.Email,
                    nd.HoTen,
                    nd.SoDienThoai,
                    nd.DiaChi,
                    tk.NgayTao,
                    tk.TrangThai,
                    'nongdan' as LoaiNguoiDung
                FROM NongDan nd
                INNER JOIN TaiKhoan tk ON nd.MaTaiKhoan = tk.MaTaiKhoan
                ORDER BY tk.NgayTao DESC", conn);

            var result = new List<object>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new
                {
                    Id = (int)reader["Id"],
                    TenDangNhap = reader["TenDangNhap"].ToString(),
                    Email = reader["Email"].ToString(),
                    HoTen = reader["HoTen"].ToString(),
                    SoDienThoai = reader["SoDienThoai"].ToString(),
                    DiaChi = reader["DiaChi"].ToString(),
                    NgayTao = reader["NgayTao"],
                    TrangThai = reader["TrangThai"].ToString(),
                    LoaiNguoiDung = reader["LoaiNguoiDung"].ToString()
                });
            }

            return result;
        }

        public List<object> GetAllDaiLy()
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(@"
                SELECT 
                    dl.MaDaiLy as Id,
                    tk.TenDangNhap,
                    tk.Email,
                    dl.TenDaiLy as HoTen,
                    dl.SoDienThoai,
                    dl.DiaChi,
                    tk.NgayTao,
                    tk.TrangThai,
                    'daily' as LoaiNguoiDung
                FROM DaiLy dl
                INNER JOIN TaiKhoan tk ON dl.MaTaiKhoan = tk.MaTaiKhoan
                ORDER BY tk.NgayTao DESC", conn);

            var result = new List<object>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new
                {
                    Id = (int)reader["Id"],
                    TenDangNhap = reader["TenDangNhap"].ToString(),
                    Email = reader["Email"].ToString(),
                    HoTen = reader["HoTen"].ToString(),
                    SoDienThoai = reader["SoDienThoai"].ToString(),
                    DiaChi = reader["DiaChi"].ToString(),
                    NgayTao = reader["NgayTao"],
                    TrangThai = reader["TrangThai"].ToString(),
                    LoaiNguoiDung = reader["LoaiNguoiDung"].ToString()
                });
            }

            return result;
        }

        public List<object> GetAllSieuThi()
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(@"
                SELECT 
                    st.MaSieuThi as Id,
                    tk.TenDangNhap,
                    tk.Email,
                    st.TenSieuThi as HoTen,
                    st.SoDienThoai,
                    st.DiaChi,
                    tk.NgayTao,
                    tk.TrangThai,
                    'sieuthi' as LoaiNguoiDung
                FROM SieuThi st
                INNER JOIN TaiKhoan tk ON st.MaTaiKhoan = tk.MaTaiKhoan
                ORDER BY tk.NgayTao DESC", conn);

            var result = new List<object>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new
                {
                    Id = (int)reader["Id"],
                    TenDangNhap = reader["TenDangNhap"].ToString(),
                    Email = reader["Email"].ToString(),
                    HoTen = reader["HoTen"].ToString(),
                    SoDienThoai = reader["SoDienThoai"].ToString(),
                    DiaChi = reader["DiaChi"].ToString(),
                    NgayTao = reader["NgayTao"],
                    TrangThai = reader["TrangThai"].ToString(),
                    LoaiNguoiDung = reader["LoaiNguoiDung"].ToString()
                });
            }

            return result;
        }

        public object? GetNongDanDetail(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(@"
                SELECT 
                    nd.MaNongDan,
                    nd.MaTaiKhoan,
                    tk.TenDangNhap,
                    tk.Email,
                    tk.TrangThai,
                    tk.NgayTao,
                    nd.HoTen,
                    nd.SoDienThoai,
                    nd.DiaChi,
                    nd.Facebook,
                    nd.TikTok,
                    COUNT(DISTINCT tt.MaTrangTrai) as SoTrangTrai,
                    COUNT(DISTINCT lo.MaLo) as SoLoNongSan
                FROM NongDan nd
                INNER JOIN TaiKhoan tk ON nd.MaTaiKhoan = tk.MaTaiKhoan
                LEFT JOIN TrangTrai tt ON nd.MaNongDan = tt.MaNongDan
                LEFT JOIN LoNongSan lo ON tt.MaTrangTrai = lo.MaTrangTrai
                WHERE nd.MaNongDan = @Id
                GROUP BY nd.MaNongDan, nd.MaTaiKhoan, tk.TenDangNhap, tk.Email, tk.TrangThai, 
                         tk.NgayTao, nd.HoTen, nd.SoDienThoai, nd.DiaChi, nd.Facebook, nd.TikTok", conn);

            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new
            {
                MaNongDan = (int)reader["MaNongDan"],
                MaTaiKhoan = (int)reader["MaTaiKhoan"],
                TenDangNhap = reader["TenDangNhap"].ToString(),
                Email = reader["Email"].ToString(),
                TrangThai = reader["TrangThai"].ToString(),
                HoTen = reader["HoTen"].ToString(),
                SoDienThoai = reader["SoDienThoai"].ToString(),
                DiaChi = reader["DiaChi"].ToString(),
                Facebook = reader["Facebook"] != DBNull.Value ? reader["Facebook"].ToString() : null,
                TikTok = reader["TikTok"] != DBNull.Value ? reader["TikTok"].ToString() : null,
                NgayTao = reader["NgayTao"],
                SoTrangTrai = (int)reader["SoTrangTrai"],
                SoLoNongSan = (int)reader["SoLoNongSan"]
            };
        }

        public object? GetDaiLyDetail(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(@"
                SELECT 
                    dl.MaDaiLy,
                    dl.MaTaiKhoan,
                    tk.TenDangNhap,
                    tk.Email,
                    tk.TrangThai,
                    tk.NgayTao,
                    dl.TenDaiLy,
                    dl.SoDienThoai,
                    dl.DiaChi,
                    dl.Facebook,
                    dl.TikTok,
                    COUNT(DISTINCT dh1.MaDonHang) as SoDonHangNhan,
                    COUNT(DISTINCT dh2.MaDonHang) as SoDonHangBan,
                    COUNT(DISTINCT kd.MaKiemDinh) as SoKiemDinh
                FROM DaiLy dl
                INNER JOIN TaiKhoan tk ON dl.MaTaiKhoan = tk.MaTaiKhoan
                LEFT JOIN DonHang dh1 ON dl.MaDaiLy = dh1.MaNguoiMua AND dh1.LoaiDon = 'nongdan_to_daily'
                LEFT JOIN DonHang dh2 ON dl.MaDaiLy = dh2.MaNguoiBan AND dh2.LoaiDon = 'daily_to_sieuthi'
                LEFT JOIN KiemDinh kd ON kd.NguoiKiemDinh = tk.TenDangNhap
                WHERE dl.MaDaiLy = @Id
                GROUP BY dl.MaDaiLy, dl.MaTaiKhoan, tk.TenDangNhap, tk.Email, tk.TrangThai,
                         tk.NgayTao, dl.TenDaiLy, dl.SoDienThoai, dl.DiaChi, dl.Facebook, dl.TikTok", conn);

            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new
            {
                MaDaiLy = (int)reader["MaDaiLy"],
                MaTaiKhoan = (int)reader["MaTaiKhoan"],
                TenDangNhap = reader["TenDangNhap"].ToString(),
                Email = reader["Email"].ToString(),
                TrangThai = reader["TrangThai"].ToString(),
                NgayTao = reader["NgayTao"],
                TenDaiLy = reader["TenDaiLy"].ToString(),
                SoDienThoai = reader["SoDienThoai"].ToString(),
                DiaChi = reader["DiaChi"].ToString(),
                Facebook = reader["Facebook"] != DBNull.Value ? reader["Facebook"].ToString() : null,
                TikTok = reader["TikTok"] != DBNull.Value ? reader["TikTok"].ToString() : null,
                SoDonHangNhan = (int)reader["SoDonHangNhan"],
                SoDonHangBan = (int)reader["SoDonHangBan"],
                SoKiemDinh = (int)reader["SoKiemDinh"]
            };
        }

        public object? GetSieuThiDetail(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(@"
                SELECT 
                    st.MaSieuThi,
                    st.MaTaiKhoan,
                    tk.TenDangNhap,
                    tk.Email,
                    tk.TrangThai,
                    tk.NgayTao,
                    st.TenSieuThi,
                    st.SoDienThoai,
                    st.DiaChi,
                    st.Facebook,
                    st.TikTok,
                    COUNT(DISTINCT dh.MaDonHang) as SoDonHangMua,
                    COUNT(DISTINCT k.MaKho) as SoKho
                FROM SieuThi st
                INNER JOIN TaiKhoan tk ON st.MaTaiKhoan = tk.MaTaiKhoan
                LEFT JOIN DonHang dh ON st.MaSieuThi = dh.MaNguoiMua AND dh.LoaiDon = 'daily_to_sieuthi'
                LEFT JOIN Kho k ON st.MaSieuThi = k.MaChuSoHuu AND k.LoaiChuSoHuu = 'sieuthi'
                WHERE st.MaSieuThi = @Id
                GROUP BY st.MaSieuThi, st.MaTaiKhoan, tk.TenDangNhap, tk.Email, tk.TrangThai,
                         tk.NgayTao, st.TenSieuThi, st.SoDienThoai, st.DiaChi, st.Facebook, st.TikTok", conn);

            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new
            {
                MaSieuThi = (int)reader["MaSieuThi"],
                MaTaiKhoan = (int)reader["MaTaiKhoan"],
                TenDangNhap = reader["TenDangNhap"].ToString(),
                Email = reader["Email"].ToString(),
                TrangThai = reader["TrangThai"].ToString(),
                NgayTao = reader["NgayTao"],
                TenSieuThi = reader["TenSieuThi"].ToString(),
                SoDienThoai = reader["SoDienThoai"].ToString(),
                DiaChi = reader["DiaChi"].ToString(),
                Facebook = reader["Facebook"] != DBNull.Value ? reader["Facebook"].ToString() : null,
                TikTok = reader["TikTok"] != DBNull.Value ? reader["TikTok"].ToString() : null,
                SoDonHangMua = (int)reader["SoDonHangMua"],
                SoKho = (int)reader["SoKho"]
            };
        }

        public List<object> SearchNongDan(string keyword)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(@"
                SELECT 
                    nd.MaNongDan as Id,
                    tk.TenDangNhap,
                    nd.HoTen,
                    nd.SoDienThoai,
                    nd.DiaChi,
                    'nongdan' as LoaiNguoiDung
                FROM NongDan nd
                INNER JOIN TaiKhoan tk ON nd.MaTaiKhoan = tk.MaTaiKhoan
                WHERE nd.HoTen LIKE @Keyword 
                   OR tk.TenDangNhap LIKE @Keyword 
                   OR nd.SoDienThoai LIKE @Keyword", conn);

            cmd.Parameters.AddWithValue("@Keyword", $"%{keyword}%");

            var result = new List<object>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new
                {
                    Id = (int)reader["Id"],
                    TenDangNhap = reader["TenDangNhap"].ToString(),
                    HoTen = reader["HoTen"].ToString(),
                    SoDienThoai = reader["SoDienThoai"].ToString(),
                    DiaChi = reader["DiaChi"].ToString(),
                    LoaiNguoiDung = reader["LoaiNguoiDung"].ToString()
                });
            }

            return result;
        }
    }
}
