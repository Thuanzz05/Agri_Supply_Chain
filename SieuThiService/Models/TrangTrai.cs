using System;
using System.Collections.Generic;

namespace SieuThiService.Models;

public partial class TrangTrai
{
    public int MaTrangTrai { get; set; }

    public int MaNongDan { get; set; }

    public string TenTrangTrai { get; set; } = null!;

    public string? DiaChi { get; set; }

    public string? SoChungNhan { get; set; }

    public string? HinhAnh { get; set; }

    public virtual ICollection<LoNongSan> LoNongSans { get; set; } = new List<LoNongSan>();

    public virtual NongDan MaNongDanNavigation { get; set; } = null!;
}
