namespace Gateway.Models
{
    public class UserInfo
    {
        public int MaTaiKhoan { get; set; }
        public string TenDangNhap { get; set; } = string.Empty;
        public string LoaiTaiKhoan { get; set; } = string.Empty;
        public int? MaNongDan { get; set; }
        public int? MaDaiLy { get; set; }
        public int? MaSieuThi { get; set; }
    }
}
