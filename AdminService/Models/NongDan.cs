using System;
using System.Collections.Generic;

namespace AdminService.Models;

public partial class NongDan
{
    public int MaNongDan { get; set; }

    public int MaTaiKhoan { get; set; }

    public string HoTen { get; set; } = null!;

    public string? SoDienThoai { get; set; }

    public string? DiaChi { get; set; }

    public string? Facebook { get; set; }

    public string? TikTok { get; set; }

    public virtual TaiKhoan MaTaiKhoanNavigation { get; set; } = null!;

    public virtual ICollection<TrangTrai> TrangTrais { get; set; } = new List<TrangTrai>();
}
