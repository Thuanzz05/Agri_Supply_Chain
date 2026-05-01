using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DaiLyService.Models;

public partial class CnwebAgriSupplyChainContext : DbContext
{
    public CnwebAgriSupplyChainContext()
    {
    }

    public CnwebAgriSupplyChainContext(DbContextOptions<CnwebAgriSupplyChainContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<DaiLy> DaiLies { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<Kho> Khos { get; set; }

    public virtual DbSet<KiemDinh> KiemDinhs { get; set; }

    public virtual DbSet<LoNongSan> LoNongSans { get; set; }

    public virtual DbSet<NongDan> NongDans { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<SieuThi> SieuThis { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<TonKho> TonKhos { get; set; }

    public virtual DbSet<TrangTrai> TrangTrais { get; set; }

    public virtual DbSet<VanChuyen> VanChuyens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DuyThuanzz;Database=CNWEB_Agri_Supply_Chain;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.MaAdmin).HasName("PK__Admin__49341E38194A29F5");

            entity.ToTable("Admin");

            entity.HasIndex(e => e.MaTaiKhoan, "UQ__Admin__AD7C65281D333F18").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithOne(p => p.Admin)
                .HasForeignKey<Admin>(d => d.MaTaiKhoan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Admin__MaTaiKhoa__3D5E1FD2");
        });

        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => new { e.MaDonHang, e.MaLo }).HasName("PK__ChiTietD__60E7D8D8B51C60F8");

            entity.ToTable("ChiTietDonHang");

            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.SoLuong).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaDonHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDo__MaDon__6754599E");

            entity.HasOne(d => d.MaLoNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaLo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDon__MaLo__68487DD7");
        });

        modelBuilder.Entity<DaiLy>(entity =>
        {
            entity.HasKey(e => e.MaDaiLy).HasName("PK__DaiLy__069B00B3BD69D2C2");

            entity.ToTable("DaiLy");

            entity.HasIndex(e => e.MaTaiKhoan, "UQ__DaiLy__AD7C652881403A90").IsUnique();

            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Facebook).HasMaxLength(255);
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
            entity.Property(e => e.TenDaiLy).HasMaxLength(100);
            entity.Property(e => e.TikTok).HasMaxLength(255);

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithOne(p => p.DaiLy)
                .HasForeignKey<DaiLy>(d => d.MaTaiKhoan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DaiLy__MaTaiKhoa__44FF419A");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.MaDonHang).HasName("PK__DonHang__129584AD526E3BB7");

            entity.ToTable("DonHang");

            entity.Property(e => e.LoaiDon).HasMaxLength(30);
            entity.Property(e => e.LoaiNguoiBan).HasMaxLength(20);
            entity.Property(e => e.LoaiNguoiMua).HasMaxLength(20);
            entity.Property(e => e.NgayDat).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.TongGiaTri)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .HasDefaultValue("cho_xac_nhan");
        });

        modelBuilder.Entity<Kho>(entity =>
        {
            entity.HasKey(e => e.MaKho).HasName("PK__Kho__3BDA93509AF2CC21");

            entity.ToTable("Kho");

            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.LoaiChuSoHuu).HasMaxLength(20);
            entity.Property(e => e.LoaiKho).HasMaxLength(20);
            entity.Property(e => e.TenKho).HasMaxLength(100);
        });

        modelBuilder.Entity<KiemDinh>(entity =>
        {
            entity.HasKey(e => e.MaKiemDinh).HasName("PK__KiemDinh__5C6E5701E7CC4DD2");

            entity.ToTable("KiemDinh");

            entity.Property(e => e.ChuKySo).HasMaxLength(255);
            entity.Property(e => e.KetQua).HasMaxLength(20);
            entity.Property(e => e.NgayKiemDinh).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.NguoiKiemDinh).HasMaxLength(100);

            entity.HasOne(d => d.MaLoNavigation).WithMany(p => p.KiemDinhs)
                .HasForeignKey(d => d.MaLo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KiemDinh__MaLo__6C190EBB");
        });

        modelBuilder.Entity<LoNongSan>(entity =>
        {
            entity.HasKey(e => e.MaLo).HasName("PK__LoNongSa__2725C7569ABAA19B");

            entity.ToTable("LoNongSan");

            entity.HasIndex(e => e.MaQr, "UQ__LoNongSa__2725F85D607B8E69").IsUnique();

            entity.Property(e => e.MaQr)
                .HasMaxLength(255)
                .HasColumnName("MaQR");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.SoLuongBanDau).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.SoLuongHienTai).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .HasDefaultValue("san_sang");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.LoNongSans)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LoNongSan__MaSan__5441852A");

            entity.HasOne(d => d.MaTrangTraiNavigation).WithMany(p => p.LoNongSans)
                .HasForeignKey(d => d.MaTrangTrai)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LoNongSan__MaTra__534D60F1");
        });

        modelBuilder.Entity<NongDan>(entity =>
        {
            entity.HasKey(e => e.MaNongDan).HasName("PK__NongDan__A4CC49E6302A5B1E");

            entity.ToTable("NongDan");

            entity.HasIndex(e => e.MaTaiKhoan, "UQ__NongDan__AD7C65287F2DCDA0").IsUnique();

            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Facebook).HasMaxLength(255);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
            entity.Property(e => e.TikTok).HasMaxLength(255);

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithOne(p => p.NongDan)
                .HasForeignKey<NongDan>(d => d.MaTaiKhoan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NongDan__MaTaiKh__412EB0B6");
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSanPham).HasName("PK__SanPham__FAC7442DA17C45BB");

            entity.ToTable("SanPham");

            entity.Property(e => e.DonViTinh).HasMaxLength(20);
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.TenSanPham).HasMaxLength(100);
        });

        modelBuilder.Entity<SieuThi>(entity =>
        {
            entity.HasKey(e => e.MaSieuThi).HasName("PK__SieuThi__7CF72B9F67DB9C6D");

            entity.ToTable("SieuThi");

            entity.HasIndex(e => e.MaTaiKhoan, "UQ__SieuThi__AD7C65280D681795").IsUnique();

            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Facebook).HasMaxLength(255);
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
            entity.Property(e => e.TenSieuThi).HasMaxLength(100);
            entity.Property(e => e.TikTok).HasMaxLength(255);

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithOne(p => p.SieuThi)
                .HasForeignKey<SieuThi>(d => d.MaTaiKhoan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SieuThi__MaTaiKh__48CFD27E");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTaiKhoan).HasName("PK__TaiKhoan__AD7C65294B5E2E71");

            entity.ToTable("TaiKhoan");

            entity.HasIndex(e => e.TenDangNhap, "UQ__TaiKhoan__55F68FC0A948683A").IsUnique();

            entity.Property(e => e.LoaiTaiKhoan).HasMaxLength(20);
            entity.Property(e => e.MatKhau).HasMaxLength(255);
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.TenDangNhap).HasMaxLength(50);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("hoat_dong");
        });

        modelBuilder.Entity<TonKho>(entity =>
        {
            entity.HasKey(e => new { e.MaKho, e.MaLo }).HasName("PK__TonKho__49A8CF25F1B21874");

            entity.ToTable("TonKho");

            entity.Property(e => e.NgayCapNhat).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.SoLuong).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.MaKhoNavigation).WithMany(p => p.TonKhos)
                .HasForeignKey(d => d.MaKho)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TonKho__MaKho__59FA5E80");

            entity.HasOne(d => d.MaLoNavigation).WithMany(p => p.TonKhos)
                .HasForeignKey(d => d.MaLo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TonKho__MaLo__5AEE82B9");
        });

        modelBuilder.Entity<TrangTrai>(entity =>
        {
            entity.HasKey(e => e.MaTrangTrai).HasName("PK__TrangTra__5C7F7908A1625F46");

            entity.ToTable("TrangTrai");

            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.SoChungNhan).HasMaxLength(50);
            entity.Property(e => e.TenTrangTrai).HasMaxLength(100);

            entity.HasOne(d => d.MaNongDanNavigation).WithMany(p => p.TrangTrais)
                .HasForeignKey(d => d.MaNongDan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TrangTrai__MaNon__4D94879B");
        });

        modelBuilder.Entity<VanChuyen>(entity =>
        {
            entity.HasKey(e => e.MaVanChuyen).HasName("PK__VanChuye__4B22972DD5B59C47");

            entity.ToTable("VanChuyen");

            entity.Property(e => e.DiemDen).HasMaxLength(255);
            entity.Property(e => e.DiemDi).HasMaxLength(255);
            entity.Property(e => e.NgayBatDau).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .HasDefaultValue("dang_van_chuyen");

            entity.HasOne(d => d.MaLoNavigation).WithMany(p => p.VanChuyens)
                .HasForeignKey(d => d.MaLo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__VanChuyen__MaLo__5FB337D6");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
