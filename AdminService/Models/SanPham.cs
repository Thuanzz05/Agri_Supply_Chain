using System;
using System.Collections.Generic;

namespace AdminService.Models;

public partial class SanPham
{
    public int MaSanPham { get; set; }

    public string TenSanPham { get; set; } = null!;

    public string DonViTinh { get; set; } = null!;

    public string? MoTa { get; set; }

    public string? HinhAnh { get; set; }

    public virtual ICollection<LoNongSan> LoNongSans { get; set; } = new List<LoNongSan>();
}
