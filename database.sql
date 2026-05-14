USE master;
GO

-- Drop database if exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'CNWEB_Agri_Supply_Chain')
BEGIN
    ALTER DATABASE CNWEB_Agri_Supply_Chain SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE CNWEB_Agri_Supply_Chain;
    PRINT N'Đã xóa database cũ';
END
GO

CREATE DATABASE CNWEB_Agri_Supply_Chain;
GO

USE CNWEB_Agri_Supply_Chain;
GO

-- =============================================
-- SCHEMA DEFINITION
-- =============================================

-- 1. TÀI KHOẢN
CREATE TABLE TaiKhoan (
    MaTaiKhoan INT IDENTITY(1,1) PRIMARY KEY,
    TenDangNhap NVARCHAR(50) UNIQUE NOT NULL,
    MatKhau NVARCHAR(255) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    LoaiTaiKhoan NVARCHAR(20) NOT NULL,
    TrangThai NVARCHAR(20) DEFAULT N'hoat_dong',
    NgayTao DATETIME2 DEFAULT SYSDATETIME()
);

-- 2. ADMIN
CREATE TABLE Admin (
    MaAdmin INT IDENTITY(1,1) PRIMARY KEY,
    MaTaiKhoan INT UNIQUE NOT NULL,
    HoTen NVARCHAR(100),
    FOREIGN KEY (MaTaiKhoan) REFERENCES TaiKhoan(MaTaiKhoan)
);

-- 3. NÔNG DÂN
CREATE TABLE NongDan (
    MaNongDan INT IDENTITY(1,1) PRIMARY KEY,
    MaTaiKhoan INT UNIQUE NOT NULL,
    HoTen NVARCHAR(100) NOT NULL,
    SoDienThoai NVARCHAR(20),
    DiaChi NVARCHAR(255),
    Facebook NVARCHAR(255) NULL,
    TikTok NVARCHAR(255) NULL,
    AnhDaiDien NVARCHAR(MAX) NULL,
    FOREIGN KEY (MaTaiKhoan) REFERENCES TaiKhoan(MaTaiKhoan)
);

-- 4. ĐẠI LÝ
CREATE TABLE DaiLy (
    MaDaiLy INT IDENTITY(1,1) PRIMARY KEY,
    MaTaiKhoan INT UNIQUE NOT NULL,
    TenDaiLy NVARCHAR(100) NOT NULL,
    SoDienThoai NVARCHAR(20),
    DiaChi NVARCHAR(255),
    Facebook NVARCHAR(255) NULL,
    TikTok NVARCHAR(255) NULL,
    AnhDaiDien NVARCHAR(MAX) NULL,
    FOREIGN KEY (MaTaiKhoan) REFERENCES TaiKhoan(MaTaiKhoan)
);

-- 5. SIÊU THỊ
CREATE TABLE SieuThi (
    MaSieuThi INT IDENTITY(1,1) PRIMARY KEY,
    MaTaiKhoan INT UNIQUE NOT NULL,
    TenSieuThi NVARCHAR(100) NOT NULL,
    SoDienThoai NVARCHAR(20),
    DiaChi NVARCHAR(255),
    Facebook NVARCHAR(255) NULL,
    TikTok NVARCHAR(255) NULL,
    AnhDaiDien NVARCHAR(MAX) NULL,
    FOREIGN KEY (MaTaiKhoan) REFERENCES TaiKhoan(MaTaiKhoan)
);

-- 6. TRANG TRẠI (Must be created BEFORE SanPham)
CREATE TABLE TrangTrai (
    MaTrangTrai INT IDENTITY(1,1) PRIMARY KEY,
    MaNongDan INT NOT NULL,
    TenTrangTrai NVARCHAR(100) NOT NULL,
    DiaChi NVARCHAR(255),
    SoChungNhan NVARCHAR(50),
    HinhAnh NVARCHAR(500) NULL,
    FOREIGN KEY (MaNongDan) REFERENCES NongDan(MaNongDan)
);

-- 7. SẢN PHẨM (Products belong to specific farms)
CREATE TABLE SanPham (
    MaSanPham INT IDENTITY(1,1) PRIMARY KEY,
    TenSanPham NVARCHAR(100) NOT NULL,
    DonViTinh NVARCHAR(20) NOT NULL,
    MoTa NVARCHAR(255),
    HinhAnh NVARCHAR(500) NULL,
    MaTrangTrai INT NOT NULL,
    FOREIGN KEY (MaTrangTrai) REFERENCES TrangTrai(MaTrangTrai)
);

CREATE INDEX IX_SanPham_MaTrangTrai ON SanPham(MaTrangTrai);

-- 8. LÔ NÔNG SẢN
CREATE TABLE LoNongSan (
    MaLo INT IDENTITY(1,1) PRIMARY KEY,
    MaTrangTrai INT NOT NULL,
    MaSanPham INT NOT NULL,
    SoLuongBanDau DECIMAL(18,2) NOT NULL,
    SoLuongHienTai DECIMAL(18,2) NOT NULL,
    NgayThuHoach DATE,
    HanSuDung DATE,
    MaQR NVARCHAR(255) UNIQUE,
    TrangThai NVARCHAR(30) DEFAULT N'san_sang',
    NgayTao DATETIME2 DEFAULT SYSDATETIME(),
    FOREIGN KEY (MaTrangTrai) REFERENCES TrangTrai(MaTrangTrai),
    FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham)
);

-- 9. KHO
CREATE TABLE Kho (
    MaKho INT IDENTITY(1,1) PRIMARY KEY,
    TenKho NVARCHAR(100) NOT NULL,
    LoaiKho NVARCHAR(20) NOT NULL,
    MaChuSoHuu INT NOT NULL,
    LoaiChuSoHuu NVARCHAR(20) NOT NULL,
    DiaChi NVARCHAR(255)
);

-- 10. TỒN KHO
CREATE TABLE TonKho (
    MaKho INT NOT NULL,
    MaLo INT NOT NULL,
    SoLuong DECIMAL(18,2) NOT NULL,
    NgayCapNhat DATETIME2 DEFAULT SYSDATETIME(),
    PRIMARY KEY (MaKho, MaLo),
    FOREIGN KEY (MaKho) REFERENCES Kho(MaKho),
    FOREIGN KEY (MaLo) REFERENCES LoNongSan(MaLo)
);

-- 11. ĐƠN HÀNG
CREATE TABLE DonHang (
    MaDonHang INT IDENTITY(1,1) PRIMARY KEY,
    LoaiDon NVARCHAR(30) NOT NULL,
    MaNguoiBan INT NOT NULL,
    LoaiNguoiBan NVARCHAR(20) NOT NULL,
    MaNguoiMua INT NOT NULL,
    LoaiNguoiMua NVARCHAR(20) NOT NULL,
    NgayDat DATETIME2 DEFAULT SYSDATETIME(),
    TrangThai NVARCHAR(30) DEFAULT N'cho_xac_nhan',
    TongGiaTri DECIMAL(18,2) DEFAULT 0
);

-- 12. VẬN CHUYỂN (with MaDonHang integrated)
CREATE TABLE VanChuyen (
    MaVanChuyen INT IDENTITY(1,1) PRIMARY KEY,
    MaLo INT NOT NULL,
    DiemDi NVARCHAR(255) NOT NULL,
    DiemDen NVARCHAR(255) NOT NULL,
    NgayBatDau DATETIME2 DEFAULT SYSDATETIME(),
    NgayKetThuc DATETIME2 NULL,
    TrangThai NVARCHAR(30) DEFAULT N'dang_van_chuyen',
    MaDonHang INT NULL,
    FOREIGN KEY (MaLo) REFERENCES LoNongSan(MaLo),
    FOREIGN KEY (MaDonHang) REFERENCES DonHang(MaDonHang)
);

CREATE INDEX IX_VanChuyen_MaDonHang ON VanChuyen(MaDonHang);

-- 13. CHI TIẾT ĐƠN HÀNG
CREATE TABLE ChiTietDonHang (
    MaDonHang INT NOT NULL,
    MaLo INT NOT NULL,
    SoLuong DECIMAL(18,2) NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL,
    ThanhTien DECIMAL(18,2) NOT NULL,
    PRIMARY KEY (MaDonHang, MaLo),
    FOREIGN KEY (MaDonHang) REFERENCES DonHang(MaDonHang),
    FOREIGN KEY (MaLo) REFERENCES LoNongSan(MaLo)
);

-- 14. KIỂM ĐỊNH
CREATE TABLE KiemDinh (
    MaKiemDinh INT IDENTITY(1,1) PRIMARY KEY,
    MaLo INT NOT NULL,
    NguoiKiemDinh NVARCHAR(100) NOT NULL,
    NgayKiemDinh DATETIME2 DEFAULT SYSDATETIME(),
    KetQua NVARCHAR(20) NOT NULL,
    BienBanKiemTra NVARCHAR(MAX),
    ChuKySo NVARCHAR(255),
    FOREIGN KEY (MaLo) REFERENCES LoNongSan(MaLo)
);

-- 15. CUỘC TRÒ CHUYỆN
CREATE TABLE CuocTroChuyen (
    MaCuocTroChuyen INT IDENTITY(1,1) PRIMARY KEY,
    MaNguoi1 INT NOT NULL,
    LoaiNguoi1 NVARCHAR(20) NOT NULL,
    MaNguoi2 INT NOT NULL,
    LoaiNguoi2 NVARCHAR(20) NOT NULL,
    TinNhanCuoi NVARCHAR(MAX) NULL,
    NgayCapNhat DATETIME2 DEFAULT SYSDATETIME(),
    NgayTao DATETIME2 DEFAULT SYSDATETIME()
);

CREATE INDEX IX_CuocTroChuyen_Nguoi1 ON CuocTroChuyen(MaNguoi1, LoaiNguoi1);
CREATE INDEX IX_CuocTroChuyen_Nguoi2 ON CuocTroChuyen(MaNguoi2, LoaiNguoi2);

-- 16. TIN NHẮN
CREATE TABLE TinNhan (
    MaTinNhan INT IDENTITY(1,1) PRIMARY KEY,
    MaCuocTroChuyen INT NOT NULL,
    MaNguoiGui INT NOT NULL,
    LoaiNguoiGui NVARCHAR(20) NOT NULL,
    NoiDung NVARCHAR(MAX) NOT NULL,
    DaDoc BIT DEFAULT 0,
    NgayGui DATETIME2 DEFAULT SYSDATETIME(),
    FOREIGN KEY (MaCuocTroChuyen) REFERENCES CuocTroChuyen(MaCuocTroChuyen) ON DELETE CASCADE
);

CREATE INDEX IX_TinNhan_CuocTroChuyen ON TinNhan(MaCuocTroChuyen);
CREATE INDEX IX_TinNhan_DaDoc ON TinNhan(DaDoc);

-- 17. PHIẾU CHUYỂN KHO
CREATE TABLE PhieuChuyenKho (
    MaPhieu INT IDENTITY(1,1) PRIMARY KEY,
    MaKhoNguon INT NOT NULL,
    MaKhoDich INT NOT NULL,
    MaLo INT NOT NULL,
    SoLuong DECIMAL(18,2) NOT NULL,
    NgayChuyen DATETIME2 DEFAULT SYSDATETIME(),
    GhiChu NVARCHAR(500) NULL,
    FOREIGN KEY (MaKhoNguon) REFERENCES Kho(MaKho),
    FOREIGN KEY (MaKhoDich) REFERENCES Kho(MaKho),
    FOREIGN KEY (MaLo) REFERENCES LoNongSan(MaLo)
);

CREATE INDEX IX_PhieuChuyenKho_NgayChuyen ON PhieuChuyenKho(NgayChuyen DESC);
CREATE INDEX IX_PhieuChuyenKho_KhoNguon ON PhieuChuyenKho(MaKhoNguon);

GO

-- =============================================
-- SAMPLE DATA
-- =============================================

-- 1. TÀI KHOẢN
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, Email, LoaiTaiKhoan, TrangThai, NgayTao) VALUES
('admin01', 'admin123', 'admin01@agrisupply.vn', 'admin', N'hoat_dong', '2026-01-15'),
('admin02', 'admin123', 'admin02@agrisupply.vn', 'admin', N'hoat_dong', '2026-02-20'),
('nongdan01', 'nongdan123', 'nguyenvantuan@gmail.com', 'nongdan', N'hoat_dong', '2026-02-01'),
('nongdan02', 'nongdan123', 'tranthiha@gmail.com', 'nongdan', N'hoat_dong', '2026-02-05'),
('nongdan03', 'nongdan123', 'phungthanhdo@gmail.com', 'nongdan', N'hoat_dong', '2026-02-10'),
('nongdan04', 'nongdan123', 'phamthid@gmail.com', 'nongdan', N'hoat_dong', '2026-02-15'),
('nongdan05', 'nongdan123', 'hoangvane@gmail.com', 'nongdan', N'hoat_dong', '2026-02-20'),
('nongdan06', 'nongdan123', 'vovanf@gmail.com', 'nongdan', N'hoat_dong', '2026-02-25'),
('nongdan07', 'nongdan123', 'dangthig@gmail.com', 'nongdan', N'hoat_dong', '2026-03-01'),
('nongdan08', 'nongdan123', 'buivanh@gmail.com', 'nongdan', N'hoat_dong', '2026-03-05'),
('nongdan09', 'nongdan123', 'lythii@gmail.com', 'nongdan', N'hoat_dong', '2026-03-10'),
('nongdan10', 'nongdan123', 'phanvank@gmail.com', 'nongdan', N'hoat_dong', '2026-03-15'),
('daily01', 'daily123', 'daily.hanoi@gmail.com', 'daily', N'hoat_dong', '2026-02-25'),
('daily02', 'daily123', 'daily.bacninh@gmail.com', 'daily', N'hoat_dong', '2026-03-01'),
('daily03', 'daily123', 'daily.haiduong@gmail.com', 'daily', N'hoat_dong', '2026-03-05'),
('daily04', 'daily123', 'daily.hungyen@gmail.com', 'daily', N'hoat_dong', '2026-03-10'),
('daily05', 'daily123', 'daily.haiphong@gmail.com', 'daily', N'hoat_dong', '2026-03-15'),
('daily06', 'daily123', 'daily.namdinh@gmail.com', 'daily', N'hoat_dong', '2026-03-20'),
('sieuthi01', 'sieuthi123', 'vinmart.hanoi@gmail.com', 'sieuthi', N'hoat_dong', '2026-03-15'),
('sieuthi02', 'sieuthi123', 'bigc.hanoi@gmail.com', 'sieuthi', N'hoat_dong', '2026-03-20'),
('sieuthi03', 'sieuthi123', 'aeon.haiphong@gmail.com', 'sieuthi', N'hoat_dong', '2026-03-25'),
('sieuthi04', 'sieuthi123', 'lotte.bacninh@gmail.com', 'sieuthi', N'hoat_dong', '2026-03-30'),
('sieuthi05', 'sieuthi123', 'coopmart.haiduong@gmail.com', 'sieuthi', N'hoat_dong', '2026-04-05'),
('sieuthi06', 'sieuthi123', 'mega.hungyen@gmail.com', 'sieuthi', N'hoat_dong', '2026-04-10');

-- 2. ADMIN
INSERT INTO Admin (MaTaiKhoan, HoTen) VALUES
(1, N'Nguyễn Duy Thuấn'),
(2, N'Nguyễn Duy Thuấnn');

-- 3. NÔNG DÂN
INSERT INTO NongDan (MaTaiKhoan, HoTen, SoDienThoai, DiaChi) VALUES
(3, N'Nguyễn Văn Tuấn', '0912345678', N'Xã Đông Anh, Huyện Đông Anh, Hà Nội'),
(4, N'Trần Thị Hà', '0923456789', N'Xã Gia Lâm, Huyện Gia Lâm, Hà Nội'),
(5, N'Phùng Thanh Độ', '0934567890', N'Xã Thuận Thành, Huyện Thuận Thành, Bắc Ninh'),
(6, N'Trương Tuấn Tú', '0945678901', N'Xã Chí Linh, Thành phố Chí Linh, Hải Dương'),
(7, N'Hoàng Văn Em', '0956789012', N'Xã Khoái Châu, Huyện Khoái Châu, Hưng Yên'),
(8, N'Võ Văn Phát', '0967890123', N'Xã Văn Giang, Huyện Văn Giang, Hưng Yên'),
(9, N'Đặng Thị Giang', '0978901234', N'Xã Tiên Du, Huyện Tiên Du, Bắc Ninh'),
(10, N'Bùi Văn Hải', '0989012345', N'Xã Thanh Hà, Huyện Thanh Hà, Hải Dương'),
(11, N'Lý Thị Ích', '0990123456', N'Xã Sóc Sơn, Huyện Sóc Sơn, Hà Nội'),
(12, N'Phan Văn Khoa', '0901234567', N'Xã Mê Linh, Huyện Mê Linh, Hà Nội');

-- 4. ĐẠI LÝ
INSERT INTO DaiLy (MaTaiKhoan, TenDaiLy, SoDienThoai, DiaChi) VALUES
(13, N'Đại lý Nông sản Hà Nội', '0967890123', N'Số 25 Trần Đại Nghĩa, Hai Bà Trưng, Hà Nội'),
(14, N'Đại lý Thực phẩm Bắc Ninh', '0978901234', N'Số 15 Lý Thái Tổ, TP Bắc Ninh, Bắc Ninh'),
(15, N'Đại lý Rau sạch Hải Dương', '0989012345', N'Số 30 Nguyễn Lương Bằng, TP Hải Dương, Hải Dương'),
(16, N'Đại lý Nông sản Hưng Yên', '0990123456', N'Số 45 Phạm Văn Đồng, TP Hưng Yên, Hưng Yên'),
(17, N'Đại lý Thực phẩm Hải Phòng', '0901234568', N'Số 50 Lạch Tray, Ngô Quyền, Hải Phòng'),
(18, N'Đại lý Rau củ Nam Định', '0912345679', N'Số 60 Trần Hưng Đạo, TP Nam Định, Nam Định');

-- 5. SIÊU THỊ
INSERT INTO SieuThi (MaTaiKhoan, TenSieuThi, SoDienThoai, DiaChi) VALUES
(19, N'VinMart Hà Nội', '0901234568', N'Tầng 1, Vincom Center, 191 Bà Triệu, Hai Bà Trưng, Hà Nội'),
(20, N'BigC Hà Nội', '0902345679', N'Số 222 Trần Duy Hưng, Cầu Giấy, Hà Nội'),
(21, N'Aeon Mall Hải Phòng', '0903456780', N'Số 10 Lê Hồng Phong, Ngô Quyền, Hải Phòng'),
(22, N'Lotte Mart Bắc Ninh', '0904567891', N'Số 88 Lý Thái Tổ, TP Bắc Ninh, Bắc Ninh'),
(23, N'Co.opmart Hải Dương', '0905678902', N'Số 100 Nguyễn Lương Bằng, TP Hải Dương, Hải Dương'),
(24, N'Mega Market Hưng Yên', '0906789013', N'Số 120 Phạm Văn Đồng, TP Hưng Yên, Hưng Yên');

-- 6. TRANG TRẠI (Must insert BEFORE SanPham)
INSERT INTO TrangTrai (MaNongDan, TenTrangTrai, DiaChi, SoChungNhan) VALUES
(1, N'Trang trại Đông Anh', N'Xã Đông Anh, Huyện Đông Anh, Hà Nội', 'VG001234'),
(1, N'Vườn rau An Nhiên', N'Xã Xuân Nộn, Huyện Đông Anh, Hà Nội', 'ORG001235'),
(2, N'Trang trại Gia Lâm', N'Xã Gia Lâm, Huyện Gia Lâm, Hà Nội', 'VG001236'),
(3, N'Trang trại Bắc Ninh', N'Xã Thuận Thành, Huyện Thuận Thành, Bắc Ninh', 'VG001237'),
(3, N'Vườn trái cây Cường', N'Xã Võ Cường, TP Bắc Ninh, Bắc Ninh', 'VG001238'),
(4, N'Trang trại Hải Dương', N'Xã Chí Linh, Thành phố Chí Linh, Hải Dương', 'VG001239'),
(5, N'Trang trại Hưng Yên', N'Xã Khoái Châu, Huyện Khoái Châu, Hưng Yên', 'VG001240'),
(6, N'Trang trại Văn Giang', N'Xã Văn Giang, Huyện Văn Giang, Hưng Yên', 'VG001241'),
(7, N'Trang trại Tiên Du', N'Xã Tiên Du, Huyện Tiên Du, Bắc Ninh', 'VG001242'),
(8, N'Trang trại Thanh Hà', N'Xã Thanh Hà, Huyện Thanh Hà, Hải Dương', 'VG001243'),
(9, N'Trang trại Sóc Sơn', N'Xã Sóc Sơn, Huyện Sóc Sơn, Hà Nội', 'VG001244'),
(10, N'Trang trại Mê Linh', N'Xã Mê Linh, Huyện Mê Linh, Hà Nội', 'VG001245');

-- 7. SẢN PHẨM (Assign products to Farmer 1's farms)
INSERT INTO SanPham (TenSanPham, DonViTinh, MoTa, MaTrangTrai) VALUES
-- Farmer 1 - Trang trại 1 (vegetables)
(N'Cà chua', N'kg', N'Cà chua tươi, màu đỏ tự nhiên', 1),
(N'Rau muống', N'kg', N'Rau muống tươi, lá xanh', 1),
(N'Cải thảo', N'kg', N'Cải thảo tươi, giòn ngọt', 1),
(N'Dưa chuột', N'kg', N'Dưa chuột xanh, giòn ngọt', 1),
(N'Rau xà lách', N'kg', N'Xà lách tươi, lá xanh', 1),
(N'Rau dền', N'kg', N'Rau dền đỏ/xanh', 1),
(N'Rau ngót', N'kg', N'Rau ngót tươi, mát', 1),
(N'Rau cần', N'kg', N'Rau cần thơm', 1),
(N'Bắp cải', N'kg', N'Bắp cải tươi', 1),
(N'Cải bó xôi', N'kg', N'Cải bó xôi', 1),
(N'Cà rốt', N'kg', N'Cà rốt cam, giòn ngọt', 1),
-- Farmer 1 - Trang trại 2 (more vegetables)
(N'Khoai tây', N'kg', N'Khoai tây Đà Lạt', 2),
(N'Hành tây', N'kg', N'Hành tây tím', 2),
(N'Bí đao', N'kg', N'Bí đao tươi', 2),
(N'Bí ngô', N'kg', N'Bí ngô vàng', 2),
(N'Tỏi', N'kg', N'Tỏi Lý Sơn', 2),
(N'Gừng', N'kg', N'Gừng già', 2),
(N'Ớt', N'kg', N'Ớt hiểm, cay', 2),
(N'Hành lá', N'kg', N'Hành lá tươi', 2),
(N'Húng quế', N'kg', N'Húng quế thơm', 2),
-- Other farmers
(N'Cam', N'kg', N'Cam Vinh, ngọt', 5),
(N'Chuối', N'kg', N'Chuối tiêu', 5),
(N'Bưởi', N'kg', N'Bưởi da xanh', 5),
(N'Táo', N'kg', N'Táo Fuji', 6),
(N'Thanh long', N'kg', N'Thanh long ruột đỏ', 6);

-- 8. LÔ NÔNG SẢN (Focus on Farmer 1 with multiple batches)
INSERT INTO LoNongSan (MaTrangTrai, MaSanPham, SoLuongBanDau, SoLuongHienTai, NgayThuHoach, HanSuDung, MaQR, TrangThai) VALUES
-- Farmer 1 - Trang trại 1 (Main batches for 6 completed orders)
(1, 1, 1500, 0, '2026-04-10', '2026-05-20', 'QR001', N'da_ban'),  -- Cà chua
(1, 3, 1200, 0, '2026-04-10', '2026-05-22', 'QR002', N'da_ban'),  -- Cải thảo
(1, 4, 800, 0, '2026-04-10', '2026-05-21', 'QR003', N'da_ban'),   -- Dưa chuột
(1, 11, 1000, 0, '2026-04-10', '2026-05-23', 'QR004', N'da_ban'), -- Cà rốt
(1, 8, 600, 0, '2026-04-10', '2026-05-21', 'QR005', N'da_ban'),   -- Rau cần
-- Farmer 1 - Trang trại 2 (Additional batches)
(2, 9, 500, 100, '2026-05-01', '2026-05-22', 'QR006', N'san_sang'),  -- Bắp cải
(2, 10, 400, 80, '2026-05-01', '2026-05-20', 'QR007', N'san_sang'),  -- Cải bó xôi
(2, 19, 300, 60, '2026-05-01', '2026-05-22', 'QR008', N'san_sang'),  -- Hành lá
(2, 14, 450, 90, '2026-05-01', '2026-05-24', 'QR009', N'san_sang'),  -- Bí đao
-- Farmer 2 - Trang trại 3 (Fewer batches)
(3, 2, 300, 50, '2026-05-05', '2026-05-20', 'QR010', N'san_sang'),
(3, 5, 350, 70, '2026-05-05', '2026-05-20', 'QR011', N'san_sang'),
-- Farmer 3 - Trang trại 4 (Fewer batches)
(4, 12, 400, 150, '2026-05-08', '2026-05-25', 'QR012', N'san_sang'),
(4, 13, 500, 300, '2026-05-08', '2026-05-25', 'QR013', N'san_sang'),
-- Other farmers (minimal data)
(6, 16, 280, 140, '2026-05-10', '2026-05-25', 'QR014', N'san_sang'),
(7, 17, 350, 170, '2026-05-10', '2026-05-25', 'QR015', N'san_sang');

-- 9. KHO
INSERT INTO Kho (TenKho, LoaiKho, MaChuSoHuu, LoaiChuSoHuu, DiaChi) VALUES
(N'Kho Đại lý Hà Nội', 'daily', 1, 'daily', N'Số 25 Trần Đại Nghĩa, Hai Bà Trưng, Hà Nội'),
(N'Kho Đại lý Bắc Ninh', 'daily', 2, 'daily', N'Số 15 Lý Thái Tổ, TP Bắc Ninh, Bắc Ninh'),
(N'Kho Đại lý Hải Dương', 'daily', 3, 'daily', N'Số 30 Nguyễn Lương Bằng, TP Hải Dương, Hải Dương'),
(N'Kho VinMart Hà Nội', 'sieuthi', 1, 'sieuthi', N'Tầng B1, Vincom Center, 191 Bà Triệu, Hà Nội'),
(N'Kho BigC Hà Nội', 'sieuthi', 2, 'sieuthi', N'Tầng B2, BigC, 222 Trần Duy Hưng, Hà Nội'),
(N'Kho Aeon Hải Phòng', 'sieuthi', 3, 'sieuthi', N'Tầng B1, Aeon Mall, 10 Lê Hồng Phong, Hải Phòng');

-- 10. ĐƠN HÀNG (Focus on Farmer 1 with 6 completed orders)
INSERT INTO DonHang (LoaiDon, MaNguoiBan, LoaiNguoiBan, MaNguoiMua, LoaiNguoiMua, NgayDat, TrangThai, TongGiaTri) VALUES
-- Farmer 1 completed orders (6 orders)
('nongdan_to_daily', 1, 'nongdan', 1, 'daily', '2026-04-15 14:00', N'hoan_thanh', 8490000),
('nongdan_to_daily', 1, 'nongdan', 1, 'daily', '2026-04-20 14:00', N'hoan_thanh', 8130000),
('nongdan_to_daily', 1, 'nongdan', 2, 'daily', '2026-04-25 14:00', N'hoan_thanh', 7260000),
('nongdan_to_daily', 1, 'nongdan', 1, 'daily', '2026-05-01 14:00', N'hoan_thanh', 9700000),
('nongdan_to_daily', 1, 'nongdan', 2, 'daily', '2026-05-05 14:00', N'hoan_thanh', 9480000),
('nongdan_to_daily', 1, 'nongdan', 1, 'daily', '2026-05-10 14:00', N'hoan_thanh', 8720000),
-- Farmer 1 pending order
('nongdan_to_daily', 1, 'nongdan', 3, 'daily', '2026-05-12 09:00', N'cho_xac_nhan', 3280000),
-- Farmer 2 orders (fewer)
('nongdan_to_daily', 2, 'nongdan', 1, 'daily', '2026-05-08 14:30', N'hoan_thanh', 2460000),
('nongdan_to_daily', 2, 'nongdan', 2, 'daily', '2026-05-12 09:30', N'cho_xac_nhan', 3000000),
-- Farmer 3 order (fewer)
('nongdan_to_daily', 3, 'nongdan', 2, 'daily', '2026-05-09 15:00', N'hoan_thanh', 10250000),
-- Other farmers
('nongdan_to_daily', 4, 'nongdan', 3, 'daily', '2026-05-10 16:00', N'hoan_thanh', 4200000),
-- Đại lý to Siêu thị
('daily_to_sieuthi', 1, 'daily', 1, 'sieuthi', '2026-05-11 10:00', N'hoan_thanh', 3700000),
('daily_to_sieuthi', 1, 'daily', 2, 'sieuthi', '2026-05-11 10:30', N'hoan_thanh', 5820000),
('daily_to_sieuthi', 2, 'daily', 3, 'sieuthi', '2026-05-11 11:00', N'hoan_thanh', 8050000);

-- 11. CHI TIẾT ĐƠN HÀNG (Farmer 1 sells mainly tomatoes, cabbage, cucumber, carrot, celery)
INSERT INTO ChiTietDonHang (MaDonHang, MaLo, SoLuong, DonGia, ThanhTien) VALUES
-- Order 1: Farmer 1 (Cà chua, Cải thảo, Dưa chuột, Cà rốt)
(1, 1, 200, 18000, 3600000),   -- Cà chua
(1, 2, 150, 10000, 1500000),   -- Cải thảo
(1, 3, 120, 12000, 1440000),   -- Dưa chuột
(1, 4, 130, 15000, 1950000),   -- Cà rốt
-- Order 2: Farmer 1 (Cà chua, Dưa chuột, Cà rốt, Rau cần)
(2, 1, 180, 18000, 3240000),   -- Cà chua
(2, 3, 100, 12000, 1200000),   -- Dưa chuột
(2, 4, 150, 15000, 2250000),   -- Cà rốt
(2, 5, 80, 18000, 1440000),    -- Rau cần
-- Order 3: Farmer 1 (Cà chua, Cải thảo, Cà rốt)
(3, 1, 220, 18000, 3960000),   -- Cà chua
(3, 2, 180, 10000, 1800000),   -- Cải thảo
(3, 4, 100, 15000, 1500000),   -- Cà rốt
-- Order 4: Farmer 1 (Nhiều Cà chua, Cải thảo)
(4, 1, 300, 18000, 5400000),   -- Cà chua
(4, 2, 250, 10000, 2500000),   -- Cải thảo
(4, 3, 150, 12000, 1800000),   -- Dưa chuột
-- Order 5: Farmer 1 (Cà chua, Cà rốt, Dưa chuột)
(5, 1, 280, 18000, 5040000),   -- Cà chua
(5, 4, 200, 15000, 3000000),   -- Cà rốt
(5, 3, 120, 12000, 1440000),   -- Dưa chuột
-- Order 6: Farmer 1 (Cà chua, Cải thảo, Rau cần)
(6, 1, 220, 18000, 3960000),   -- Cà chua
(6, 2, 200, 10000, 2000000),   -- Cải thảo
(6, 5, 100, 18000, 1800000),   -- Rau cần
(6, 3, 80, 12000, 960000),     -- Dưa chuột
-- Order 7: Farmer 1 (pending - Bắp cải, Cải bó xôi, Hành lá)
(7, 6, 100, 10000, 1000000),   -- Bắp cải
(7, 7, 80, 12000, 960000),     -- Cải bó xôi
(7, 8, 60, 22000, 1320000),    -- Hành lá
-- Order 8: Farmer 2
(8, 10, 100, 15000, 1500000),
(8, 11, 80, 12000, 960000),
-- Order 9: Farmer 2 (pending)
(9, 10, 120, 15000, 1800000),
(9, 11, 100, 12000, 1200000),
-- Order 10: Farmer 3
(10, 12, 250, 25000, 6250000),
(10, 13, 200, 20000, 4000000),
-- Order 11: Farmer 4
(11, 14, 140, 30000, 4200000),
-- Orders 12-14: Đại lý to Siêu thị (giá bán lẻ cao hơn)
(12, 1, 100, 25000, 2500000),
(12, 2, 80, 15000, 1200000),
(13, 3, 90, 18000, 1620000),
(13, 12, 120, 35000, 4200000),
(14, 12, 150, 35000, 5250000),
(14, 14, 70, 40000, 2800000);

-- 12. VẬN CHUYỂN (with MaDonHang)
INSERT INTO VanChuyen (MaLo, DiemDi, DiemDen, NgayBatDau, NgayKetThuc, TrangThai, MaDonHang) VALUES
-- Order 1 transportation (Cà chua, Cải thảo, Dưa chuột, Cà rốt)
(1, N'Trang trại Đông Anh', N'Kho Đại lý Hà Nội', '2026-04-15 06:00', '2026-04-15 08:30', N'hoan_thanh', 1),
(2, N'Trang trại Đông Anh', N'Kho Đại lý Hà Nội', '2026-04-15 06:00', '2026-04-15 08:30', N'hoan_thanh', 1),
(3, N'Trang trại Đông Anh', N'Kho Đại lý Hà Nội', '2026-04-15 06:00', '2026-04-15 08:30', N'hoan_thanh', 1),
(4, N'Trang trại Đông Anh', N'Kho Đại lý Hà Nội', '2026-04-15 06:00', '2026-04-15 08:30', N'hoan_thanh', 1),
-- Order 2 transportation (Cà chua, Dưa chuột, Cà rốt, Rau cần)
(1, N'Trang trại Đông Anh', N'Kho Đại lý Hà Nội', '2026-04-20 06:00', '2026-04-20 08:30', N'hoan_thanh', 2),
(3, N'Trang trại Đông Anh', N'Kho Đại lý Hà Nội', '2026-04-20 06:00', '2026-04-20 08:30', N'hoan_thanh', 2),
(4, N'Trang trại Đông Anh', N'Kho Đại lý Hà Nội', '2026-04-20 06:00', '2026-04-20 08:30', N'hoan_thanh', 2),
(5, N'Trang trại Đông Anh', N'Kho Đại lý Hà Nội', '2026-04-20 06:00', '2026-04-20 08:30', N'hoan_thanh', 2),
-- Order 3 transportation (Cà chua, Cải thảo, Cà rốt)
(1, N'Trang trại Đông Anh', N'Kho Đại lý Bắc Ninh', '2026-04-25 06:00', '2026-04-25 09:00', N'hoan_thanh', 3),
(2, N'Trang trại Đông Anh', N'Kho Đại lý Bắc Ninh', '2026-04-25 06:00', '2026-04-25 09:00', N'hoan_thanh', 3),
(4, N'Trang trại Đông Anh', N'Kho Đại lý Bắc Ninh', '2026-04-25 06:00', '2026-04-25 09:00', N'hoan_thanh', 3),
-- Order 4 transportation (Cà chua, Cải thảo, Dưa chuột)
(1, N'Trang trại Đông Anh', N'Kho Đại lý Hà Nội', '2026-05-01 06:00', '2026-05-01 08:30', N'hoan_thanh', 4),
(2, N'Trang trại Đông Anh', N'Kho Đại lý Hà Nội', '2026-05-01 06:00', '2026-05-01 08:30', N'hoan_thanh', 4),
(3, N'Trang trại Đông Anh', N'Kho Đại lý Hà Nội', '2026-05-01 06:00', '2026-05-01 08:30', N'hoan_thanh', 4),
-- Order 5 transportation (Cà chua, Cà rốt, Dưa chuột)
(1, N'Trang trại Đông Anh', N'Kho Đại lý Bắc Ninh', '2026-05-05 06:00', '2026-05-05 09:00', N'hoan_thanh', 5),
(4, N'Trang trại Đông Anh', N'Kho Đại lý Bắc Ninh', '2026-05-05 06:00', '2026-05-05 09:00', N'hoan_thanh', 5),
(3, N'Trang trại Đông Anh', N'Kho Đại lý Bắc Ninh', '2026-05-05 06:00', '2026-05-05 09:00', N'hoan_thanh', 5),
-- Order 6 transportation (Cà chua, Cải thảo, Rau cần, Dưa chuột)
(1, N'Trang trại Đông Anh', N'Kho Đại lý Hà Nội', '2026-05-10 06:00', '2026-05-10 08:30', N'hoan_thanh', 6),
(2, N'Trang trại Đông Anh', N'Kho Đại lý Hà Nội', '2026-05-10 06:00', '2026-05-10 08:30', N'hoan_thanh', 6),
(5, N'Trang trại Đông Anh', N'Kho Đại lý Hà Nội', '2026-05-10 06:00', '2026-05-10 08:30', N'hoan_thanh', 6),
(3, N'Trang trại Đông Anh', N'Kho Đại lý Hà Nội', '2026-05-10 06:00', '2026-05-10 08:30', N'hoan_thanh', 6);

-- 13. KIỂM ĐỊNH
INSERT INTO KiemDinh (MaLo, NguoiKiemDinh, NgayKiemDinh, KetQua, BienBanKiemTra, ChuKySo) VALUES
(1, N'Nguyễn Văn Kiểm', '2026-04-10 05:00', 'dat', N'Cà chua đạt tiêu chuẩn VietGAP', 'SIGN001'),
(2, N'Nguyễn Văn Kiểm', '2026-04-10 05:00', 'dat', N'Cải thảo đạt tiêu chuẩn VietGAP', 'SIGN002'),
(3, N'Nguyễn Văn Kiểm', '2026-04-10 05:00', 'dat', N'Dưa chuột đạt tiêu chuẩn VietGAP', 'SIGN003'),
(4, N'Nguyễn Văn Kiểm', '2026-04-10 05:00', 'dat', N'Cà rốt đạt tiêu chuẩn VietGAP', 'SIGN004'),
(5, N'Nguyễn Văn Kiểm', '2026-04-10 05:00', 'dat', N'Rau cần đạt tiêu chuẩn VietGAP', 'SIGN005'),
(6, N'Trần Thị Định', '2026-05-01 05:15', 'dat', N'Bắp cải đạt tiêu chuẩn VietGAP', 'SIGN006'),
(7, N'Trần Thị Định', '2026-05-01 05:15', 'dat', N'Cải bó xôi đạt tiêu chuẩn VietGAP', 'SIGN007'),
(8, N'Trần Thị Định', '2026-05-01 05:15', 'dat', N'Hành lá đạt tiêu chuẩn VietGAP', 'SIGN008'),
(9, N'Trần Thị Định', '2026-05-01 05:15', 'dat', N'Bí đao đạt tiêu chuẩn VietGAP', 'SIGN009'),
(10, N'Lê Văn Nghiệm', '2026-05-05 05:30', 'dat', N'Rau muống đạt tiêu chuẩn VietGAP', 'SIGN010'),
(11, N'Lê Văn Nghiệm', '2026-05-05 05:30', 'dat', N'Rau xà lách đạt tiêu chuẩn VietGAP', 'SIGN011'),
(12, N'Phạm Thị Thẩm', '2026-05-08 05:45', 'dat', N'Khoai tây Đà Lạt đạt tiêu chuẩn VietGAP', 'SIGN012'),
(13, N'Phạm Thị Thẩm', '2026-05-08 05:45', 'dat', N'Hành tây đạt tiêu chuẩn VietGAP', 'SIGN013'),
(14, N'Hoàng Văn Tra', '2026-05-10 06:00', 'dat', N'Tỏi Lý Sơn đạt tiêu chuẩn VietGAP', 'SIGN014'),
(15, N'Võ Thị Xét', '2026-05-10 06:15', 'dat', N'Gừng già đạt tiêu chuẩn VietGAP', 'SIGN015');


-- Tồn kho Đại lý 1 (Kho Đại lý Hà Nội - MaKho = 1)
INSERT INTO TonKho (MaKho, MaLo, SoLuong, NgayCapNhat) VALUES
(1, 1, 800, '2026-05-13 08:00'),  -- Cà chua: 900 - 100 = 800kg
(1, 2, 520, '2026-05-13 08:00'),  -- Cải thảo: 600 - 80 = 520kg
(1, 3, 360, '2026-05-13 08:00'),  -- Dưa chuột: 450 - 90 = 360kg
(1, 4, 280, '2026-05-13 08:00'),  -- Cà rốt: 280kg (chưa bán)
(1, 5, 180, '2026-05-13 08:00'),  -- Rau cần: 180kg (chưa bán)
(1, 10, 100, '2026-05-13 08:00'), -- Rau muống: 100kg (chưa bán)
(1, 11, 80, '2026-05-13 08:00');  -- Rau xà lách: 80kg (chưa bán)

-- Tồn kho Đại lý 2 (Kho Đại lý Bắc Ninh - MaKho = 2)
INSERT INTO TonKho (MaKho, MaLo, SoLuong, NgayCapNhat) VALUES
(2, 1, 500, '2026-05-13 08:00'),  -- Cà chua: 500kg (chưa bán)
(2, 2, 180, '2026-05-13 08:00'),  -- Cải thảo: 180kg (chưa bán)
(2, 3, 120, '2026-05-13 08:00'),  -- Dưa chuột: 120kg (chưa bán)
(2, 4, 300, '2026-05-13 08:00'),  -- Cà rốt: 300kg (chưa bán)
(2, 12, 100, '2026-05-13 08:00'), -- Khoai tây: 250 - 150 = 100kg
(2, 13, 200, '2026-05-13 08:00'); -- Hành tây: 200kg (chưa bán)

-- Tồn kho Đại lý 3 (Kho Đại lý Hải Dương - MaKho = 3)
INSERT INTO TonKho (MaKho, MaLo, SoLuong, NgayCapNhat) VALUES
(3, 14, 70, '2026-05-13 08:00');  -- Tỏi Lý Sơn: 140 - 70 = 70kg

-- Tồn kho Siêu thị 1 (Kho VinMart Hà Nội - MaKho = 4) - Nhận từ Order 12
INSERT INTO TonKho (MaKho, MaLo, SoLuong, NgayCapNhat) VALUES
(4, 1, 100, '2026-05-13 09:00'),  -- Cà chua: 100kg
(4, 2, 80, '2026-05-13 09:00');   -- Cải thảo: 80kg

-- Tồn kho Siêu thị 2 (Kho BigC Hà Nội - MaKho = 5) - Nhận từ Order 13
INSERT INTO TonKho (MaKho, MaLo, SoLuong, NgayCapNhat) VALUES
(5, 3, 90, '2026-05-13 09:00'),   -- Dưa chuột: 90kg
(5, 12, 120, '2026-05-13 09:00'); -- Khoai tây: 120kg

-- Tồn kho Siêu thị 3 (Kho Aeon Hải Phòng - MaKho = 6) - Nhận từ Order 14
INSERT INTO TonKho (MaKho, MaLo, SoLuong, NgayCapNhat) VALUES
(6, 12, 150, '2026-05-13 09:00'), -- Khoai tây: 150kg
(6, 14, 70, '2026-05-13 09:00');  -- Tỏi Lý Sơn: 70kg

-- 14. CUỘC TRÒ CHUYỆN
INSERT INTO CuocTroChuyen (MaNguoi1, LoaiNguoi1, MaNguoi2, LoaiNguoi2, TinNhanCuoi, NgayCapNhat) VALUES
(1, 'nongdan', 1, 'daily', N'Vâng, tôi sẽ giao hàng vào sáng mai', '2026-05-12 10:00:00'),
(1, 'daily', 1, 'sieuthi', N'Đơn hàng đã được xác nhận', '2026-05-12 11:00:00'),
(2, 'nongdan', 2, 'daily', N'Cảm ơn bạn đã đặt hàng', '2026-05-12 12:00:00'),
(3, 'nongdan', 3, 'daily', N'Hàng đã sẵn sàng giao', '2026-05-12 13:00:00'),
(4, 'nongdan', 2, 'daily', N'Cam Vinh chất lượng cao', '2026-05-12 14:00:00');

-- 15. TIN NHẮN
INSERT INTO TinNhan (MaCuocTroChuyen, MaNguoiGui, LoaiNguoiGui, NoiDung, DaDoc, NgayGui) VALUES
(1, 1, 'daily', N'Chào bạn, tôi muốn đặt 100kg cà chua', 1, '2026-05-12 08:00:00'),
(1, 1, 'nongdan', N'Chào bạn, hiện tại tôi có sẵn hàng. Giá 50.000đ/kg', 1, '2026-05-12 08:30:00'),
(1, 1, 'daily', N'Được, khi nào có thể giao hàng?', 1, '2026-05-12 09:00:00'),
(1, 1, 'nongdan', N'Vâng, tôi sẽ giao hàng vào sáng mai', 0, '2026-05-12 10:00:00'),
(2, 1, 'sieuthi', N'Tôi cần đặt 200kg rau muống', 1, '2026-05-12 09:00:00'),
(2, 1, 'daily', N'Dạ, hiện tại shop có sẵn. Giá 45.000đ/kg', 1, '2026-05-12 09:30:00'),
(2, 1, 'sieuthi', N'OK, tôi đặt luôn nhé', 1, '2026-05-12 10:30:00'),
(2, 1, 'daily', N'Đơn hàng đã được xác nhận', 0, '2026-05-12 11:00:00'),
(3, 2, 'daily', N'Bạn có rau xà lách không?', 1, '2026-05-12 11:00:00'),
(3, 2, 'nongdan', N'Có bạn, tôi có 100kg rau xà lách tươi', 1, '2026-05-12 11:30:00'),
(3, 2, 'daily', N'Cảm ơn bạn đã đặt hàng', 0, '2026-05-12 12:00:00'),
(4, 3, 'daily', N'Khoai tây Đà Lạt còn hàng không?', 1, '2026-05-12 12:00:00'),
(4, 3, 'nongdan', N'Còn nhiều bạn, giá 70.000đ/kg', 1, '2026-05-12 12:30:00'),
(4, 3, 'daily', N'Hàng đã sẵn sàng giao', 0, '2026-05-12 13:00:00'),
(5, 4, 'daily', N'Cam Vinh có sẵn không bạn?', 1, '2026-05-12 13:00:00'),
(5, 4, 'nongdan', N'Có bạn, cam rất ngọt và tươi', 1, '2026-05-12 13:30:00'),
(5, 4, 'daily', N'Cam Vinh chất lượng cao', 0, '2026-05-12 14:00:00');

GO

-- =============================================
-- STORED PROCEDURES
-- =============================================

-- Stored procedure để tự động cập nhật trạng thái hết hạn cho lô nông sản
CREATE OR ALTER PROCEDURE sp_UpdateExpiredBatchStatus
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Cập nhật trạng thái hết hạn cho các lô:
    -- - Đã quá hạn sử dụng
    -- - Đang ở trạng thái 'san_sang' hoặc 'dang_van_chuyen'
    -- - Còn số lượng (SoLuongHienTai > 0)
    UPDATE LoNongSan
    SET TrangThai = N'het_han'
    WHERE HanSuDung < CAST(GETDATE() AS DATE)
      AND TrangThai IN (N'san_sang', N'dang_van_chuyen')
      AND SoLuongHienTai > 0;
    
    -- Log số lượng bản ghi đã cập nhật
    DECLARE @UpdatedCount INT = @@ROWCOUNT;
    IF @UpdatedCount > 0
    BEGIN
        PRINT CONCAT(N'Đã cập nhật ', @UpdatedCount, N' lô nông sản sang trạng thái hết hạn');
    END
END;
GO

-- Chạy stored procedure để cập nhật trạng thái hết hạn
EXEC sp_UpdateExpiredBatchStatus;
GO




--------------------

--DELETE FROM KiemDinh;
--DBCC CHECKIDENT ('KiemDinh', RESEED, 0);
--PRINT N'Đã xóa dữ liệu bảng KiemDinh';

--DELETE FROM ChiTietDonHang;
--PRINT N'Đã xóa dữ liệu bảng ChiTietDonHang';

--DELETE FROM DonHang;
--DBCC CHECKIDENT ('DonHang', RESEED, 0);
--PRINT N'Đã xóa dữ liệu bảng DonHang';

--DELETE FROM VanChuyen;
--DBCC CHECKIDENT ('VanChuyen', RESEED, 0);
--PRINT N'Đã xóa dữ liệu bảng VanChuyen';

--DELETE FROM TonKho;
--PRINT N'Đã xóa dữ liệu bảng TonKho';

--DELETE FROM Kho;
--DBCC CHECKIDENT ('Kho', RESEED, 0);
--PRINT N'Đã xóa dữ liệu bảng Kho';

--DELETE FROM LoNongSan;
--DBCC CHECKIDENT ('LoNongSan', RESEED, 0);
--PRINT N'Đã xóa dữ liệu bảng LoNongSan';

--DELETE FROM TrangTrai;
--DBCC CHECKIDENT ('TrangTrai', RESEED, 0);
--PRINT N'Đã xóa dữ liệu bảng TrangTrai';

--DELETE FROM SanPham;
--DBCC CHECKIDENT ('SanPham', RESEED, 0);
--PRINT N'Đã xóa dữ liệu bảng SanPham';

--DELETE FROM SieuThi;
--DBCC CHECKIDENT ('SieuThi', RESEED, 0);
--PRINT N'Đã xóa dữ liệu bảng SieuThi';

--DELETE FROM DaiLy;
--DBCC CHECKIDENT ('DaiLy', RESEED, 0);
--PRINT N'Đã xóa dữ liệu bảng DaiLy';

--DELETE FROM NongDan;
--DBCC CHECKIDENT ('NongDan', RESEED, 0);
--PRINT N'Đã xóa dữ liệu bảng NongDan';

--DELETE FROM Admin;
--DBCC CHECKIDENT ('Admin', RESEED, 0);
--PRINT N'Đã xóa dữ liệu bảng Admin';

--DELETE FROM TaiKhoan;
--DBCC CHECKIDENT ('TaiKhoan', RESEED, 0);
--PRINT N'Đã xóa dữ liệu bảng TaiKhoan';

--DELETE FROM PhieuChuyenKho;
--DBCC CHECKIDENT ('PhieuChuyenKho', RESEED, 0);



SELECT * FROM TaiKhoan;
SELECT * FROM Admin;
SELECT * FROM NongDan;
SELECT * FROM DaiLy;
SELECT * FROM SieuThi;
SELECT * FROM SanPham;
SELECT * FROM TrangTrai;
SELECT * FROM LoNongSan;
SELECT * FROM Kho;
SELECT * FROM TonKho;
SELECT * FROM VanChuyen;
SELECT * FROM DonHang;
SELECT * FROM ChiTietDonHang;
SELECT * FROM KiemDinh;
SELECT * FROM PhieuChuyenKho;
