using System;
using System.Collections.Generic;

namespace AdminService.Models;

public partial class SieuThi
{
    public int MaSieuThi { get; set; }

    public int MaTaiKhoan { get; set; }

    public string TenSieuThi { get; set; } = null!;

    public string? SoDienThoai { get; set; }

    public string? DiaChi { get; set; }

    public string? Facebook { get; set; }

    public string? TikTok { get; set; }

    public virtual TaiKhoan MaTaiKhoanNavigation { get; set; } = null!;
}
