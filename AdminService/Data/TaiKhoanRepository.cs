using Microsoft.Data.SqlClient;

namespace AdminService.Data
{
    public class TaiKhoanRepository
    {
        private readonly string _connectionString;

        public TaiKhoanRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<object> GetAll(string? loaiTaiKhoan)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var whereClause = string.IsNullOrEmpty(loaiTaiKhoan) ? "" : "WHERE LoaiTaiKhoan = @LoaiTaiKhoan";

            using var cmd = new SqlCommand($@"
                SELECT MaTaiKhoan, TenDangNhap, Email, LoaiTaiKhoan, TrangThai, NgayTao
                FROM TaiKhoan 
                {whereClause}
                ORDER BY NgayTao DESC", conn);

            if (!string.IsNullOrEmpty(loaiTaiKhoan))
                cmd.Parameters.AddWithValue("@LoaiTaiKhoan", loaiTaiKhoan);

            var result = new List<object>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new
                {
                    MaTaiKhoan = (int)reader["MaTaiKhoan"],
                    TenDangNhap = reader["TenDangNhap"].ToString(),
                    Email = reader["Email"].ToString(),
                    LoaiTaiKhoan = reader["LoaiTaiKhoan"].ToString(),
                    TrangThai = reader["TrangThai"].ToString(),
                    NgayTao = reader["NgayTao"]
                });
            }

            return result;
        }

        public bool ChangePassword(int id, string matKhauMoi)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(@"
                UPDATE TaiKhoan 
                SET MatKhau = @MatKhauMoi
                WHERE MaTaiKhoan = @Id", conn);

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@MatKhauMoi", matKhauMoi);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool ToggleStatus(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(@"
                UPDATE TaiKhoan 
                SET TrangThai = CASE 
                    WHEN TrangThai = N'hoat_dong' THEN N'khoa'
                    ELSE N'hoat_dong'
                END
                WHERE MaTaiKhoan = @Id", conn);

            cmd.Parameters.AddWithValue("@Id", id);

            return cmd.ExecuteNonQuery() > 0;
        }

        public string? GetLoaiTaiKhoan(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand("SELECT LoaiTaiKhoan FROM TaiKhoan WHERE MaTaiKhoan = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);

            return cmd.ExecuteScalar()?.ToString();
        }

        public int? GetMaNongDan(int maTaiKhoan)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand("SELECT MaNongDan FROM NongDan WHERE MaTaiKhoan = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", maTaiKhoan);

            var result = cmd.ExecuteScalar();
            return result != null ? (int)result : null;
        }

        public int? GetMaDaiLy(int maTaiKhoan)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand("SELECT MaDaiLy FROM DaiLy WHERE MaTaiKhoan = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", maTaiKhoan);

            var result = cmd.ExecuteScalar();
            return result != null ? (int)result : null;
        }

        public int? GetMaSieuThi(int maTaiKhoan)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand("SELECT MaSieuThi FROM SieuThi WHERE MaTaiKhoan = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", maTaiKhoan);

            var result = cmd.ExecuteScalar();
            return result != null ? (int)result : null;
        }

        public string? GetTenDangNhap(int maTaiKhoan)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand("SELECT TenDangNhap FROM TaiKhoan WHERE MaTaiKhoan = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", maTaiKhoan);

            return cmd.ExecuteScalar()?.ToString();
        }

        public void DeleteNongDanRelatedData(int maNongDan)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(@"
                DELETE ct FROM ChiTietDonHang ct
                INNER JOIN LoNongSan lo ON ct.MaLo = lo.MaLo
                INNER JOIN TrangTrai tt ON lo.MaTrangTrai = tt.MaTrangTrai
                WHERE tt.MaNongDan = @MaNongDan;

                DELETE FROM DonHang WHERE MaNguoiBan = @MaNongDan AND LoaiNguoiBan = 'nongdan';

                DELETE kd FROM KiemDinh kd
                INNER JOIN LoNongSan lo ON kd.MaLo = lo.MaLo
                INNER JOIN TrangTrai tt ON lo.MaTrangTrai = tt.MaTrangTrai
                WHERE tt.MaNongDan = @MaNongDan;

                DELETE vc FROM VanChuyen vc
                INNER JOIN LoNongSan lo ON vc.MaLo = lo.MaLo
                INNER JOIN TrangTrai tt ON lo.MaTrangTrai = tt.MaTrangTrai
                WHERE tt.MaNongDan = @MaNongDan;

                DELETE tk FROM TonKho tk
                INNER JOIN LoNongSan lo ON tk.MaLo = lo.MaLo
                INNER JOIN TrangTrai tt ON lo.MaTrangTrai = tt.MaTrangTrai
                WHERE tt.MaNongDan = @MaNongDan;

                DELETE lo FROM LoNongSan lo
                INNER JOIN TrangTrai tt ON lo.MaTrangTrai = tt.MaTrangTrai
                WHERE tt.MaNongDan = @MaNongDan;

                DELETE FROM TrangTrai WHERE MaNongDan = @MaNongDan;
                DELETE FROM NongDan WHERE MaNongDan = @MaNongDan;", conn);

            cmd.Parameters.AddWithValue("@MaNongDan", maNongDan);
            cmd.ExecuteNonQuery();
        }

        public void DeleteDaiLyRelatedData(int maDaiLy, string tenDangNhap)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(@"
                DELETE ct FROM ChiTietDonHang ct
                INNER JOIN DonHang dh ON ct.MaDonHang = dh.MaDonHang
                WHERE (dh.MaNguoiBan = @MaDaiLy AND dh.LoaiNguoiBan = 'daily')
                   OR (dh.MaNguoiMua = @MaDaiLy AND dh.LoaiNguoiMua = 'daily');

                DELETE FROM DonHang 
                WHERE (MaNguoiBan = @MaDaiLy AND LoaiNguoiBan = 'daily')
                   OR (MaNguoiMua = @MaDaiLy AND LoaiNguoiMua = 'daily');

                DELETE FROM KiemDinh WHERE NguoiKiemDinh = @TenDangNhap;

                DELETE tk FROM TonKho tk
                INNER JOIN Kho k ON tk.MaKho = k.MaKho
                WHERE k.MaChuSoHuu = @MaDaiLy AND k.LoaiChuSoHuu = 'daily';

                DELETE FROM Kho WHERE MaChuSoHuu = @MaDaiLy AND LoaiChuSoHuu = 'daily';
                DELETE FROM DaiLy WHERE MaDaiLy = @MaDaiLy;", conn);

            cmd.Parameters.AddWithValue("@MaDaiLy", maDaiLy);
            cmd.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
            cmd.ExecuteNonQuery();
        }

        public void DeleteSieuThiRelatedData(int maSieuThi)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(@"
                DELETE ct FROM ChiTietDonHang ct
                INNER JOIN DonHang dh ON ct.MaDonHang = dh.MaDonHang
                WHERE dh.MaNguoiMua = @MaSieuThi AND dh.LoaiNguoiMua = 'sieuthi';

                DELETE FROM DonHang 
                WHERE MaNguoiMua = @MaSieuThi AND LoaiNguoiMua = 'sieuthi';

                DELETE tk FROM TonKho tk
                INNER JOIN Kho k ON tk.MaKho = k.MaKho
                WHERE k.MaChuSoHuu = @MaSieuThi AND k.LoaiChuSoHuu = 'sieuthi';

                DELETE FROM Kho WHERE MaChuSoHuu = @MaSieuThi AND LoaiChuSoHuu = 'sieuthi';
                DELETE FROM SieuThi WHERE MaSieuThi = @MaSieuThi;", conn);

            cmd.Parameters.AddWithValue("@MaSieuThi", maSieuThi);
            cmd.ExecuteNonQuery();
        }

        public void DeleteAdmin(int maTaiKhoan)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand("DELETE FROM Admin WHERE MaTaiKhoan = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", maTaiKhoan);
            cmd.ExecuteNonQuery();
        }

        public bool DeleteTaiKhoan(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand("DELETE FROM TaiKhoan WHERE MaTaiKhoan = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);

            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
