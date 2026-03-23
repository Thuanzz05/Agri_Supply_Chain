Create database CNWEB_Agri_Supply_Chain
Go

USE CNWEB_Agri_Supply_Chain;
GO

-- HỆ THỐNG QUẢN LÝ NÔNG SẢN & CHUỖI CUNG ỨNG


-- 1. TÀI KHOẢN
CREATE TABLE TaiKhoan (
    MaTaiKhoan INT IDENTITY(1,1) PRIMARY KEY,
    TenDangNhap NVARCHAR(50) UNIQUE NOT NULL,
    MatKhau NVARCHAR(255) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    LoaiTaiKhoan NVARCHAR(20) NOT NULL, -- 'admin', 'nongdan', 'daily', 'sieuthi'
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
    FOREIGN KEY (MaTaiKhoan) REFERENCES TaiKhoan(MaTaiKhoan)
);

-- 4. ĐẠI LÝ
CREATE TABLE DaiLy (
    MaDaiLy INT IDENTITY(1,1) PRIMARY KEY,
    MaTaiKhoan INT UNIQUE NOT NULL,
    TenDaiLy NVARCHAR(100) NOT NULL,
    SoDienThoai NVARCHAR(20),
    DiaChi NVARCHAR(255),
    FOREIGN KEY (MaTaiKhoan) REFERENCES TaiKhoan(MaTaiKhoan)
);

-- 5. SIÊU THỊ
CREATE TABLE SieuThi (
    MaSieuThi INT IDENTITY(1,1) PRIMARY KEY,
    MaTaiKhoan INT UNIQUE NOT NULL,
    TenSieuThi NVARCHAR(100) NOT NULL,
    SoDienThoai NVARCHAR(20),
    DiaChi NVARCHAR(255),
    FOREIGN KEY (MaTaiKhoan) REFERENCES TaiKhoan(MaTaiKhoan)
);

-- 6. SẢN PHẨM
CREATE TABLE SanPham (
    MaSanPham INT IDENTITY(1,1) PRIMARY KEY,
    TenSanPham NVARCHAR(100) NOT NULL,
    DonViTinh NVARCHAR(20) NOT NULL,
    MoTa NVARCHAR(255)
);

-- 7. TRANG TRẠI
CREATE TABLE TrangTrai (
    MaTrangTrai INT IDENTITY(1,1) PRIMARY KEY,
    MaNongDan INT NOT NULL,
    TenTrangTrai NVARCHAR(100) NOT NULL,
    DiaChi NVARCHAR(255),
    SoChungNhan NVARCHAR(50), -- Số chứng nhận VietGAP, Organic...
    FOREIGN KEY (MaNongDan) REFERENCES NongDan(MaNongDan)
);

-- 8. LÔ NÔNG SẢN
CREATE TABLE LoNongSan (
    MaLo INT IDENTITY(1,1) PRIMARY KEY,
    MaTrangTrai INT NOT NULL,
    MaSanPham INT NOT NULL,
    SoLuongBanDau DECIMAL(18) NOT NULL,
    SoLuongHienTai DECIMAL(18) NOT NULL,
    NgayThuHoach DATE,
    HanSuDung DATE,
    MaQR NVARCHAR(255) UNIQUE, -- QR Code truy xuất nguồn gốc
    TrangThai NVARCHAR(30) DEFAULT N'san_sang', -- san_sang, da_ban, het_han
    NgayTao DATETIME2 DEFAULT SYSDATETIME(),
    FOREIGN KEY (MaTrangTrai) REFERENCES TrangTrai(MaTrangTrai),
    FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham)
);

-- 9. KHO
CREATE TABLE Kho (
    MaKho INT IDENTITY(1,1) PRIMARY KEY,
    TenKho NVARCHAR(100) NOT NULL,
    LoaiKho NVARCHAR(20) NOT NULL, -- 'daily', 'sieuthi', 'trung_gian'
    MaChuSoHuu INT NOT NULL,
    LoaiChuSoHuu NVARCHAR(20) NOT NULL, -- 'daily', 'sieuthi'
    DiaChi NVARCHAR(255)
);

-- 10. TỒN KHO
CREATE TABLE TonKho (
    MaKho INT NOT NULL,
    MaLo INT NOT NULL,
    SoLuong DECIMAL(18) NOT NULL,
    NgayCapNhat DATETIME2 DEFAULT SYSDATETIME(),
    PRIMARY KEY (MaKho, MaLo),
    FOREIGN KEY (MaKho) REFERENCES Kho(MaKho),
    FOREIGN KEY (MaLo) REFERENCES LoNongSan(MaLo)
);

-- 11. VẬN CHUYỂN
CREATE TABLE VanChuyen (
    MaVanChuyen INT IDENTITY(1,1) PRIMARY KEY,
    MaLo INT NOT NULL,
    DiemDi NVARCHAR(255) NOT NULL,
    DiemDen NVARCHAR(255) NOT NULL,
    NgayBatDau DATETIME2 DEFAULT SYSDATETIME(),
    NgayKetThuc DATETIME2 NULL,
    TrangThai NVARCHAR(30) DEFAULT N'dang_van_chuyen', -- dang_van_chuyen, hoan_thanh
    FOREIGN KEY (MaLo) REFERENCES LoNongSan(MaLo)
);

-- 12. ĐƠN HÀNG
CREATE TABLE DonHang (
    MaDonHang INT IDENTITY(1,1) PRIMARY KEY,
    LoaiDon NVARCHAR(30) NOT NULL, -- 'nongdan_to_daily', 'daily_to_sieuthi'
    MaNguoiBan INT NOT NULL,
    LoaiNguoiBan NVARCHAR(20) NOT NULL, -- 'nongdan', 'daily'
    MaNguoiMua INT NOT NULL,
    LoaiNguoiMua NVARCHAR(20) NOT NULL, -- 'daily', 'sieuthi'
    NgayDat DATETIME2 DEFAULT SYSDATETIME(),
    TrangThai NVARCHAR(30) DEFAULT N'cho_xac_nhan', -- cho_xac_nhan, hoan_thanh, da_huy
    TongGiaTri DECIMAL(18) DEFAULT 0
);

-- 13. CHI TIẾT ĐƠN HÀNG
CREATE TABLE ChiTietDonHang (
    MaDonHang INT NOT NULL,
    MaLo INT NOT NULL,
    SoLuong DECIMAL(18) NOT NULL,
    DonGia DECIMAL(18) NOT NULL,
    ThanhTien DECIMAL(18) NOT NULL,
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
    KetQua NVARCHAR(20) NOT NULL, -- 'dat', 'khong_dat'
    BienBanKiemTra NVARCHAR(MAX), -- Biên bản kiểm tra
    ChuKySo NVARCHAR(255), -- Chữ ký số
    FOREIGN KEY (MaLo) REFERENCES LoNongSan(MaLo)
);


-- 1. TÀI KHOẢN
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, Email, LoaiTaiKhoan) VALUES
('admin01', 'admin123', 'admin@agrisupply.com', 'admin'),
('nongdan01', 'nongdan123', 'nongdan01@gmail.com', 'nongdan'),
('nongdan02', 'nongdan123', 'nongdan02@gmail.com', 'nongdan'),
('nongdan03', 'nongdan123', 'nongdan03@gmail.com', 'nongdan'),
('daily01', 'daily123', 'daily01@gmail.com', 'daily'),
('daily02', 'daily123', 'daily02@gmail.com', 'daily'),
('sieuthi01', 'sieuthi123', 'sieuthi01@gmail.com', 'sieuthi'),
('sieuthi02', 'sieuthi123', 'sieuthi02@gmail.com', 'sieuthi');

-- 2. ADMIN
INSERT INTO Admin (MaTaiKhoan, HoTen) VALUES
(1, N'Nguyễn Văn Admin');

-- 3. NÔNG DÂN
INSERT INTO NongDan (MaTaiKhoan, HoTen, SoDienThoai, DiaChi) VALUES
(2, N'Trần Văn Nông', '0901234567', N'Xã Tân Phú, Huyện Củ Chi, TP.HCM'),
(3, N'Lê Thị Hoa', '0912345678', N'Xã Phước Vĩnh An, Huyện Củ Chi, TP.HCM'),
(4, N'Phạm Minh Tâm', '0923456789', N'Xã Trung An, Huyện Củ Chi, TP.HCM');

-- 4. ĐẠI LÝ
INSERT INTO DaiLy (MaTaiKhoan, TenDaiLy, SoDienThoai, DiaChi) VALUES
(5, N'Đại lý Nông sản Miền Nam', '0934567890', N'123 Đường Lê Văn Việt, Quận 9, TP.HCM'),
(6, N'Đại lý Thực phẩm Sạch', '0945678901', N'456 Đường Nguyễn Duy Trinh, Quận 2, TP.HCM');

-- 5. SIÊU THỊ
INSERT INTO SieuThi (MaTaiKhoan, TenSieuThi, SoDienThoai, DiaChi) VALUES
(7, N'Siêu thị Co.opmart', '0956789012', N'789 Đường Võ Văn Ngân, Thủ Đức, TP.HCM'),
(8, N'Siêu thị BigC', '0967890123', N'321 Đường Phạm Văn Đồng, Gò Vấp, TP.HCM');

-- 6. SẢN PHẨM
INSERT INTO SanPham (TenSanPham, DonViTinh, MoTa) VALUES
(N'Cà chua', N'kg', N'Cà chua tươi, màu đỏ, chín tự nhiên'),
(N'Rau muống', N'kg', N'Rau muống tươi, lá xanh, không thuốc trừ sâu'),
(N'Cải thảo', N'kg', N'Cải thảo tươi, lá xanh, giòn ngọt'),
(N'Dưa chuột', N'kg', N'Dưa chuột tươi, xanh, giòn ngọt'),
(N'Ớt', N'kg', N'Ớt tươi, cay nồng, màu đỏ'),
(N'Gạo ST25', N'kg', N'Gạo thơm ST25, hạt dài, thơm ngon');

-- 7. TRANG TRẠI
INSERT INTO TrangTrai (MaNongDan, TenTrangTrai, DiaChi, SoChungNhan) VALUES
(1, N'Trang trại Xanh', N'Ấp 3, Xã Tân Phú, Huyện Củ Chi', 'VG001234'),
(1, N'Trang trại Organic', N'Ấp 5, Xã Tân Phú, Huyện Củ Chi', 'ORG001235'),
(2, N'Trang trại Hoa Sen', N'Ấp 2, Xã Phước Vĩnh An, Huyện Củ Chi', 'VG001236'),
(3, N'Trang trại Minh Tâm', N'Ấp 1, Xã Trung An, Huyện Củ Chi', 'VG001237');

-- 8. LÔ NÔNG SẢN
INSERT INTO LoNongSan (MaTrangTrai, MaSanPham, SoLuongBanDau, SoLuongHienTai, NgayThuHoach, HanSuDung, MaQR, TrangThai) VALUES
(1, 1, 500, 450, '2024-03-01', '2024-03-15', 'QR001_CACHUA_240301', N'san_sang'),
(1, 2, 200, 180, '2024-03-02', '2024-03-09', 'QR002_RAUMUONG_240302', N'san_sang'),
(2, 3, 300, 300, '2024-03-03', '2024-03-17', 'QR003_CAITHAO_240303', N'san_sang'),
(2, 4, 150, 120, '2024-03-04', '2024-03-18', 'QR004_DUACHUOT_240304', N'san_sang'),
(3, 5, 100, 80, '2024-03-05', '2024-04-05', 'QR005_OT_240305', N'san_sang'),
(4, 6, 1000, 800, '2024-02-15', '2024-08-15', 'QR006_GAO_240215', N'san_sang');

-- 9. KHO
INSERT INTO Kho (TenKho, LoaiKho, MaChuSoHuu, LoaiChuSoHuu, DiaChi) VALUES
(N'Kho Đại lý Miền Nam', 'daily', 1, 'daily', N'123 Đường Lê Văn Việt, Quận 9, TP.HCM'),
(N'Kho Thực phẩm Sạch', 'daily', 2, 'daily', N'456 Đường Nguyễn Duy Trinh, Quận 2, TP.HCM'),
(N'Kho Co.opmart', 'sieuthi', 1, 'sieuthi', N'789 Đường Võ Văn Ngân, Thủ Đức, TP.HCM'),
(N'Kho BigC', 'sieuthi', 2, 'sieuthi', N'321 Đường Phạm Văn Đồng, Gò Vấp, TP.HCM');

-- 10. TỒN KHO
INSERT INTO TonKho (MaKho, MaLo, SoLuong) VALUES
(1, 1, 100),
(1, 2, 50),
(2, 3, 80),
(2, 4, 30),
(3, 1, 150),
(3, 5, 20),
(4, 6, 200);

-- 11. VẬN CHUYỂN
INSERT INTO VanChuyen (MaLo, DiemDi, DiemDen, NgayBatDau, NgayKetThuc, TrangThai) VALUES
(1, N'Trang trại Xanh', N'Kho Đại lý Miền Nam', '2024-03-01 08:00:00', '2024-03-01 10:00:00', N'hoan_thanh'),
(2, N'Trang trại Xanh', N'Kho Đại lý Miền Nam', '2024-03-02 08:00:00', '2024-03-02 10:00:00', N'hoan_thanh'),
(3, N'Trang trại Organic', N'Kho Thực phẩm Sạch', '2024-03-03 09:00:00', NULL, N'dang_van_chuyen');

-- 12. ĐƠN HÀNG
INSERT INTO DonHang (LoaiDon, MaNguoiBan, LoaiNguoiBan, MaNguoiMua, LoaiNguoiMua, NgayDat, TrangThai, TongGiaTri) VALUES
('nongdan_to_daily', 1, 'nongdan', 1, 'daily', '2024-03-01 07:00:00', N'hoan_thanh', 1250000),
('nongdan_to_daily', 2, 'nongdan', 2, 'daily', '2024-03-02 08:00:00', N'hoan_thanh', 800000),
('daily_to_sieuthi', 1, 'daily', 1, 'sieuthi', '2024-03-03 09:00:00', N'cho_xac_nhan', 2000000),
('daily_to_sieuthi', 2, 'daily', 2, 'sieuthi', '2024-03-04 10:00:00', N'hoan_thanh', 1500000);

-- 13. CHI TIẾT ĐƠN HÀNG
INSERT INTO ChiTietDonHang (MaDonHang, MaLo, SoLuong, DonGia, ThanhTien) VALUES
(1, 1, 50, 25000, 1250000),
(2, 2, 20, 15000, 300000),
(2, 3, 25, 20000, 500000),
(3, 1, 100, 20000, 2000000),
(4, 4, 30, 18000, 540000),
(4, 5, 20, 48000, 960000);

-- 14. KIỂM ĐỊNH
INSERT INTO KiemDinh (MaLo, NguoiKiemDinh, NgayKiemDinh, KetQua, BienBanKiemTra, ChuKySo) VALUES
(1, N'Trung tâm Kiểm định Nông sản TP.HCM', '2024-03-01 06:00:00', 'dat', N'Sản phẩm đạt tiêu chuẩn VietGAP, không có dư lượng thuốc bảo vệ thực vật', 'SIGN001'),
(2, N'Trung tâm Kiểm định Nông sản TP.HCM', '2024-03-02 06:00:00', 'dat', N'Sản phẩm đạt tiêu chuẩn an toàn thực phẩm', 'SIGN002'),
(3, N'Viện Kiểm nghiệm An toàn vệ sinh thực phẩm', '2024-03-03 06:00:00', 'dat', N'Sản phẩm organic đạt tiêu chuẩn quốc tế', 'SIGN003');

