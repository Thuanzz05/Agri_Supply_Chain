namespace AdminService.Models.DTOs
{
    public class CuocTroChuyenDTO
    {
        public int MaCuocTroChuyen { get; set; }
        public int MaNguoi1 { get; set; }
        public string LoaiNguoi1 { get; set; } = string.Empty;
        public int MaNguoi2 { get; set; }
        public string LoaiNguoi2 { get; set; } = string.Empty;
        public string? TinNhanCuoi { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public DateTime NgayTao { get; set; }
        
        // Thông tin bổ sung
        public string? TenNguoiKia { get; set; }
        public string? AnhDaiDienNguoiKia { get; set; }
        public int SoTinNhanChuaDoc { get; set; }
    }

    public class TinNhanDTO
    {
        public int MaTinNhan { get; set; }
        public int MaCuocTroChuyen { get; set; }
        public int MaNguoiGui { get; set; }
        public string LoaiNguoiGui { get; set; } = string.Empty;
        public string NoiDung { get; set; } = string.Empty;
        public bool DaDoc { get; set; }
        public DateTime NgayGui { get; set; }
        
        // Thông tin bổ sung
        public string? TenNguoiGui { get; set; }
        public string? AnhDaiDienNguoiGui { get; set; }
    }

    public class GuiTinNhanRequest
    {
        public int? MaCuocTroChuyen { get; set; }
        public int MaNguoiNhan { get; set; }
        public string LoaiNguoiNhan { get; set; } = string.Empty;
        public string NoiDung { get; set; } = string.Empty;
    }

    public class DanhDauDaDocRequest
    {
        public int MaCuocTroChuyen { get; set; }
    }

    public class UserListDTO
    {
        public int MaNguoi { get; set; }
        public string LoaiNguoi { get; set; } = string.Empty;
        public string Ten { get; set; } = string.Empty;
        public string? AnhDaiDien { get; set; }
        public string? DiaChi { get; set; }
        public string? SoDienThoai { get; set; }
    }
}
