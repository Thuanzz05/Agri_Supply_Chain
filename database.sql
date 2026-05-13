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
    Facebook NVARCHAR(255) NULL,
    TikTok NVARCHAR(255) NULL,
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

-- 6. SẢN PHẨM
INSERT INTO SanPham (TenSanPham, DonViTinh, MoTa) VALUES
(N'Cà chua', N'kg', N'Cà chua tươi, màu đỏ tự nhiên'),
(N'Rau muống', N'kg', N'Rau muống tươi, lá xanh'),
(N'Cải thảo', N'kg', N'Cải thảo tươi, giòn ngọt'),
(N'Dưa chuột', N'kg', N'Dưa chuột xanh, giòn ngọt'),
(N'Rau xà lách', N'kg', N'Xà lách tươi, lá xanh'),
(N'Rau dền', N'kg', N'Rau dền đỏ/xanh'),
(N'Rau ngót', N'kg', N'Rau ngót tươi, mát'),
(N'Rau cần', N'kg', N'Rau cần thơm'),
(N'Bắp cải', N'kg', N'Bắp cải tươi'),
(N'Cải bó xôi', N'kg', N'Cải bó xôi'),
(N'Cà rốt', N'kg', N'Cà rốt cam, giòn ngọt'),
(N'Khoai tây', N'kg', N'Khoai tây Đà Lạt'),
(N'Hành tây', N'kg', N'Hành tây tím'),
(N'Bí đao', N'kg', N'Bí đao tươi'),
(N'Bí ngô', N'kg', N'Bí ngô vàng'),
(N'Tỏi', N'kg', N'Tỏi Lý Sơn'),
(N'Gừng', N'kg', N'Gừng già'),
(N'Ớt', N'kg', N'Ớt hiểm, cay'),
(N'Hành lá', N'kg', N'Hành lá tươi'),
(N'Húng quế', N'kg', N'Húng quế thơm'),
(N'Cam', N'kg', N'Cam Vinh, ngọt'),
(N'Chuối', N'kg', N'Chuối tiêu'),
(N'Bưởi', N'kg', N'Bưởi da xanh'),
(N'Táo', N'kg', N'Táo Fuji'),
(N'Thanh long', N'kg', N'Thanh long ruột đỏ');

-- 7. TRANG TRẠI
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

-- 8. LÔ NÔNG SẢN (Thu hoạch 11/05/2026)
INSERT INTO LoNongSan (MaTrangTrai, MaSanPham, SoLuongBanDau, SoLuongHienTai, NgayThuHoach, HanSuDung, MaQR, TrangThai) VALUES
(1, 1, 500, 500, '2026-05-11', '2026-05-20', 'QR001', N'san_sang'),
(1, 2, 300, 300, '2026-05-11', '2026-05-20', 'QR002', N'san_sang'),
(1, 3, 400, 400, '2026-05-11', '2026-05-22', 'QR003', N'san_sang'),
(1, 4, 250, 250, '2026-05-11', '2026-05-21', 'QR004', N'san_sang'),
(2, 5, 200, 200, '2026-05-11', '2026-05-20', 'QR005', N'san_sang'),
(2, 6, 180, 180, '2026-05-11', '2026-05-20', 'QR006', N'san_sang'),
(2, 7, 160, 160, '2026-05-11', '2026-05-20', 'QR007', N'san_sang'),
(2, 8, 150, 150, '2026-05-11', '2026-05-21', 'QR008', N'san_sang'),
(3, 9, 300, 300, '2026-05-11', '2026-05-22', 'QR009', N'san_sang'),
(3, 10, 280, 280, '2026-05-11', '2026-05-20', 'QR010', N'san_sang'),
(3, 2, 250, 250, '2026-05-11', '2026-05-20', 'QR011', N'san_sang'),
(4, 11, 400, 400, '2026-05-11', '2026-05-23', 'QR012', N'san_sang'),
(4, 12, 500, 500, '2026-05-11', '2026-05-25', 'QR013', N'san_sang'),
(4, 13, 350, 350, '2026-05-11', '2026-05-25', 'QR014', N'san_sang'),
(4, 14, 300, 300, '2026-05-11', '2026-05-24', 'QR015', N'san_sang'),
(5, 21, 600, 600, '2026-05-11', '2026-05-25', 'QR016', N'san_sang'),
(5, 22, 300, 300, '2026-05-11', '2026-05-22', 'QR017', N'san_sang'),
(5, 23, 400, 400, '2026-05-11', '2026-05-23', 'QR018', N'san_sang'),
(6, 16, 280, 280, '2026-05-11', '2026-05-25', 'QR019', N'san_sang'),
(6, 17, 200, 200, '2026-05-11', '2026-05-25', 'QR020', N'san_sang'),
(6, 18, 150, 150, '2026-05-11', '2026-05-23', 'QR021', N'san_sang'),
(6, 19, 120, 120, '2026-05-11', '2026-05-22', 'QR022', N'san_sang'),
(7, 1, 350, 350, '2026-05-11', '2026-05-20', 'QR023', N'san_sang'),
(7, 11, 300, 300, '2026-05-11', '2026-05-23', 'QR024', N'san_sang'),
(7, 15, 250, 250, '2026-05-11', '2026-05-25', 'QR025', N'san_sang'),
(8, 3, 280, 280, '2026-05-11', '2026-05-22', 'QR026', N'san_sang'),
(8, 5, 200, 200, '2026-05-11', '2026-05-20', 'QR027', N'san_sang'),
(8, 9, 220, 220, '2026-05-11', '2026-05-22', 'QR028', N'san_sang'),
(9, 12, 400, 400, '2026-05-11', '2026-05-25', 'QR029', N'san_sang'),
(9, 14, 350, 350, '2026-05-11', '2026-05-24', 'QR030', N'san_sang'),
(9, 15, 300, 300, '2026-05-11', '2026-05-25', 'QR031', N'san_sang'),
(10, 21, 500, 500, '2026-05-11', '2026-05-25', 'QR032', N'san_sang'),
(10, 24, 200, 200, '2026-05-11', '2026-05-24', 'QR033', N'san_sang'),
(10, 25, 250, 250, '2026-05-11', '2026-05-22', 'QR034', N'san_sang');

-- 9. KHO
INSERT INTO Kho (TenKho, LoaiKho, MaChuSoHuu, LoaiChuSoHuu, DiaChi) VALUES
(N'Kho Đại lý Hà Nội', 'daily', 1, 'daily', N'Số 25 Trần Đại Nghĩa, Hai Bà Trưng, Hà Nội'),
(N'Kho Đại lý Bắc Ninh', 'daily', 2, 'daily', N'Số 15 Lý Thái Tổ, TP Bắc Ninh, Bắc Ninh'),
(N'Kho Đại lý Hải Dương', 'daily', 3, 'daily', N'Số 30 Nguyễn Lương Bằng, TP Hải Dương, Hải Dương'),
(N'Kho Đại lý Hưng Yên', 'daily', 4, 'daily', N'Số 45 Phạm Văn Đồng, TP Hưng Yên, Hưng Yên'),
(N'Kho Đại lý Hải Phòng', 'daily', 5, 'daily', N'Số 50 Lạch Tray, Ngô Quyền, Hải Phòng'),
(N'Kho Đại lý Nam Định', 'daily', 6, 'daily', N'Số 60 Trần Hưng Đạo, TP Nam Định, Nam Định'),
(N'Kho VinMart Hà Nội', 'sieuthi', 1, 'sieuthi', N'Tầng B1, Vincom Center, 191 Bà Triệu, Hà Nội'),
(N'Kho BigC Hà Nội', 'sieuthi', 2, 'sieuthi', N'Tầng B2, BigC, 222 Trần Duy Hưng, Hà Nội'),
(N'Kho Aeon Hải Phòng', 'sieuthi', 3, 'sieuthi', N'Tầng B1, Aeon Mall, 10 Lê Hồng Phong, Hải Phòng'),
(N'Kho Lotte Bắc Ninh', 'sieuthi', 4, 'sieuthi', N'Tầng B1, Lotte Mart, 88 Lý Thái Tổ, Bắc Ninh'),
(N'Kho Co.opmart Hải Dương', 'sieuthi', 5, 'sieuthi', N'Tầng B1, Co.opmart, 100 Nguyễn Lương Bằng, Hải Dương'),
(N'Kho Mega Hưng Yên', 'sieuthi', 6, 'sieuthi', N'Tầng B1, Mega Market, 120 Phạm Văn Đồng, Hưng Yên');

-- 10. TỒN KHO
INSERT INTO TonKho (MaKho, MaLo, SoLuong, NgayCapNhat) VALUES
(1, 1, 200, '2026-05-11'),
(1, 2, 150, '2026-05-11'),
(1, 3, 180, '2026-05-11'),
(1, 5, 100, '2026-05-11'),
(2, 12, 250, '2026-05-11'),
(2, 13, 200, '2026-05-11'),
(2, 14, 150, '2026-05-11'),
(2, 16, 300, '2026-05-11'),
(3, 19, 140, '2026-05-11'),
(3, 20, 100, '2026-05-11'),
(3, 21, 80, '2026-05-11'),
(4, 23, 180, '2026-05-11'),
(4, 24, 150, '2026-05-11'),
(4, 25, 120, '2026-05-11'),
(7, 1, 100, '2026-05-12'),
(7, 2, 80, '2026-05-12'),
(7, 5, 50, '2026-05-12'),
(8, 3, 90, '2026-05-12'),
(8, 12, 120, '2026-05-12'),
(9, 16, 150, '2026-05-12'),
(9, 19, 70, '2026-05-12'),
(10, 13, 100, '2026-05-12'),
(10, 14, 80, '2026-05-12');

-- 11. VẬN CHUYỂN (với MaDonHang)
-- Vận chuyển từ Nông dân đến Đại lý (liên kết với đơn hàng 1-6)
INSERT INTO VanChuyen (MaLo, DiemDi, DiemDen, NgayBatDau, NgayKetThuc, TrangThai, MaDonHang) VALUES
-- Đơn hàng 1: Nông dân 1 -> Đại lý 1 (Lô 1,2,3,4)
(1, N'Trang trại Đông Anh', N'Kho Đại lý Hà Nội', '2026-05-11 06:00', '2026-05-11 08:30', N'hoan_thanh', 1),
(2, N'Trang trại Đông Anh', N'Kho Đại lý Hà Nội', '2026-05-11 06:00', '2026-05-11 08:30', N'hoan_thanh', 1),
(3, N'Trang trại Đông Anh', N'Kho Đại lý Hà Nội', '2026-05-11 06:00', '2026-05-11 08:30', N'hoan_thanh', 1),
(4, N'Trang trại Đông Anh', N'Kho Đại lý Hà Nội', '2026-05-11 06:00', '2026-05-11 08:30', N'hoan_thanh', 1),
-- Đơn hàng 2: Nông dân 2 -> Đại lý 1 (Lô 5,6,7)
(5, N'Vườn rau An Nhiên', N'Kho Đại lý Hà Nội', '2026-05-11 06:30', '2026-05-11 09:00', N'hoan_thanh', 2),
(6, N'Vườn rau An Nhiên', N'Kho Đại lý Hà Nội', '2026-05-11 06:30', '2026-05-11 09:00', N'hoan_thanh', 2),
(7, N'Vườn rau An Nhiên', N'Kho Đại lý Hà Nội', '2026-05-11 06:30', '2026-05-11 09:00', N'hoan_thanh', 2),
-- Đơn hàng 3: Nông dân 3 -> Đại lý 2 (Lô 12,13,14,15)
(12, N'Trang trại Bắc Ninh', N'Kho Đại lý Bắc Ninh', '2026-05-11 07:00', '2026-05-11 09:30', N'hoan_thanh', 3),
(13, N'Trang trại Bắc Ninh', N'Kho Đại lý Bắc Ninh', '2026-05-11 07:00', '2026-05-11 09:30', N'hoan_thanh', 3),
(14, N'Trang trại Bắc Ninh', N'Kho Đại lý Bắc Ninh', '2026-05-11 07:00', '2026-05-11 09:30', N'hoan_thanh', 3),
(15, N'Trang trại Bắc Ninh', N'Kho Đại lý Bắc Ninh', '2026-05-11 07:00', '2026-05-11 09:30', N'hoan_thanh', 3),
-- Đơn hàng 4: Nông dân 5 -> Đại lý 2 (Lô 16,17,18)
(16, N'Vườn trái cây Cường', N'Kho Đại lý Bắc Ninh', '2026-05-11 07:30', '2026-05-11 10:00', N'hoan_thanh', 4),
(17, N'Vườn trái cây Cường', N'Kho Đại lý Bắc Ninh', '2026-05-11 07:30', '2026-05-11 10:00', N'hoan_thanh', 4),
(18, N'Vườn trái cây Cường', N'Kho Đại lý Bắc Ninh', '2026-05-11 07:30', '2026-05-11 10:00', N'hoan_thanh', 4),
-- Đơn hàng 5: Nông dân 4 -> Đại lý 3 (Lô 19,20,21)
(19, N'Trang trại Hải Dương', N'Kho Đại lý Hải Dương', '2026-05-11 06:30', '2026-05-11 09:00', N'hoan_thanh', 5),
(20, N'Trang trại Hải Dương', N'Kho Đại lý Hải Dương', '2026-05-11 06:30', '2026-05-11 09:00', N'hoan_thanh', 5),
(21, N'Trang trại Hải Dương', N'Kho Đại lý Hải Dương', '2026-05-11 06:30', '2026-05-11 09:00', N'hoan_thanh', 5),
-- Đơn hàng 6: Nông dân 7 -> Đại lý 4 (Lô 23,24,25)
(23, N'Trang trại Hưng Yên', N'Kho Đại lý Hưng Yên', '2026-05-11 07:00', '2026-05-11 09:30', N'hoan_thanh', 6),
(24, N'Trang trại Hưng Yên', N'Kho Đại lý Hưng Yên', '2026-05-11 07:00', '2026-05-11 09:30', N'hoan_thanh', 6),
(25, N'Trang trại Hưng Yên', N'Kho Đại lý Hưng Yên', '2026-05-11 07:00', '2026-05-11 09:30', N'hoan_thanh', 6),

-- Vận chuyển từ Đại lý đến Siêu thị (liên kết với đơn hàng 7-10)
-- Đơn hàng 7: Đại lý 1 -> Siêu thị 1 (Lô 1,2,5)
(1, N'Kho Đại lý Hà Nội', N'Kho VinMart Hà Nội', '2026-05-12 08:00', '2026-05-12 10:00', N'hoan_thanh', 7),
(2, N'Kho Đại lý Hà Nội', N'Kho VinMart Hà Nội', '2026-05-12 08:00', '2026-05-12 10:00', N'hoan_thanh', 7),
(5, N'Kho Đại lý Hà Nội', N'Kho VinMart Hà Nội', '2026-05-12 08:00', '2026-05-12 10:00', N'hoan_thanh', 7),
-- Đơn hàng 8: Đại lý 1 -> Siêu thị 2 (Lô 3,12)
(3, N'Kho Đại lý Hà Nội', N'Kho BigC Hà Nội', '2026-05-12 08:30', '2026-05-12 10:30', N'hoan_thanh', 8),
(12, N'Kho Đại lý Bắc Ninh', N'Kho BigC Hà Nội', '2026-05-12 09:00', '2026-05-12 11:30', N'hoan_thanh', 8),
-- Đơn hàng 9: Đại lý 2 -> Siêu thị 3 (Lô 16,19)
(16, N'Kho Đại lý Bắc Ninh', N'Kho Aeon Hải Phòng', '2026-05-12 08:00', '2026-05-12 11:00', N'hoan_thanh', 9),
(19, N'Kho Đại lý Hải Dương', N'Kho Aeon Hải Phòng', '2026-05-12 08:30', '2026-05-12 10:30', N'hoan_thanh', 9),
-- Đơn hàng 10: Đại lý 2 -> Siêu thị 4 (Lô 13,14)
(13, N'Kho Đại lý Bắc Ninh', N'Kho Lotte Bắc Ninh', '2026-05-12 09:00', '2026-05-12 10:30', N'hoan_thanh', 10),
(14, N'Kho Đại lý Bắc Ninh', N'Kho Lotte Bắc Ninh', '2026-05-12 09:00', '2026-05-12 10:30', N'hoan_thanh', 10);

-- 12. ĐƠN HÀNG
INSERT INTO DonHang (LoaiDon, MaNguoiBan, LoaiNguoiBan, MaNguoiMua, LoaiNguoiMua, NgayDat, TrangThai, TongGiaTri) VALUES
('nongdan_to_daily', 1, 'nongdan', 1, 'daily', '2026-05-10 14:00', N'hoan_thanh', 26500000),
('nongdan_to_daily', 2, 'nongdan', 1, 'daily', '2026-05-10 14:30', N'hoan_thanh', 8000000),
('nongdan_to_daily', 3, 'nongdan', 2, 'daily', '2026-05-10 15:00', N'hoan_thanh', 42000000),
('nongdan_to_daily', 5, 'nongdan', 2, 'daily', '2026-05-10 15:30', N'hoan_thanh', 21000000),
('nongdan_to_daily', 4, 'nongdan', 3, 'daily', '2026-05-10 16:00', N'hoan_thanh', 18900000),
('nongdan_to_daily', 7, 'nongdan', 4, 'daily', '2026-05-10 16:30', N'hoan_thanh', 23500000),
('daily_to_sieuthi', 1, 'daily', 1, 'sieuthi', '2026-05-11 10:00', N'hoan_thanh', 16500000),
('daily_to_sieuthi', 1, 'daily', 2, 'sieuthi', '2026-05-11 10:30', N'hoan_thanh', 10800000),
('daily_to_sieuthi', 2, 'daily', 3, 'sieuthi', '2026-05-11 11:00', N'hoan_thanh', 28500000),
('daily_to_sieuthi', 2, 'daily', 4, 'sieuthi', '2026-05-11 11:30', N'hoan_thanh', 18000000),
('nongdan_to_daily', 6, 'nongdan', 5, 'daily', '2026-05-12 09:00', N'cho_xac_nhan', 15000000),
('nongdan_to_daily', 8, 'nongdan', 6, 'daily', '2026-05-12 09:30', N'cho_xac_nhan', 12500000),
('daily_to_sieuthi', 3, 'daily', 5, 'sieuthi', '2026-05-12 14:00', N'cho_xac_nhan', 9500000),
('daily_to_sieuthi', 4, 'daily', 6, 'sieuthi', '2026-05-12 14:30', N'cho_xac_nhan', 11000000);

-- 13. CHI TIẾT ĐƠN HÀNG
INSERT INTO ChiTietDonHang (MaDonHang, MaLo, SoLuong, DonGia, ThanhTien) VALUES
(1, 1, 200, 50000, 10000000),
(1, 2, 150, 30000, 4500000),
(1, 3, 180, 40000, 7200000),
(1, 4, 100, 35000, 3500000),
(2, 5, 100, 45000, 4500000),
(2, 6, 80, 25000, 2000000),
(2, 7, 60, 22000, 1320000),
(3, 12, 250, 60000, 15000000),
(3, 13, 200, 70000, 14000000),
(3, 14, 150, 55000, 8250000),
(3, 15, 80, 58000, 4640000),
(4, 16, 300, 35000, 10500000),
(4, 17, 150, 40000, 6000000),
(4, 18, 100, 45000, 4500000),
(5, 19, 140, 80000, 11200000),
(5, 20, 100, 45000, 4500000),
(5, 21, 80, 40000, 3200000),
(6, 23, 180, 50000, 9000000),
(6, 24, 150, 60000, 9000000),
(6, 25, 120, 45000, 5400000),
(7, 1, 100, 65000, 6500000),
(7, 2, 80, 45000, 3600000),
(7, 5, 50, 60000, 3000000),
(8, 3, 90, 55000, 4950000),
(8, 12, 120, 75000, 9000000),
(9, 16, 150, 50000, 7500000),
(9, 19, 70, 95000, 6650000),
(10, 13, 100, 85000, 8500000),
(10, 14, 80, 70000, 5600000);

-- 14. KIỂM ĐỊNH
INSERT INTO KiemDinh (MaLo, NguoiKiemDinh, NgayKiemDinh, KetQua, BienBanKiemTra, ChuKySo) VALUES
(1, N'Nguyễn Văn Kiểm', '2026-05-11 05:00', 'dat', N'Cà chua đạt tiêu chuẩn VietGAP, không dư lượng thuốc BVTV', 'SIGN001'),
(2, N'Nguyễn Văn Kiểm', '2026-05-11 05:00', 'dat', N'Rau muống đạt tiêu chuẩn VietGAP, tươi xanh', 'SIGN002'),
(3, N'Nguyễn Văn Kiểm', '2026-05-11 05:00', 'dat', N'Cải thảo đạt tiêu chuẩn VietGAP, không sâu bệnh', 'SIGN003'),
(4, N'Nguyễn Văn Kiểm', '2026-05-11 05:00', 'dat', N'Dưa chuột đạt tiêu chuẩn VietGAP, giòn ngọt', 'SIGN004'),
(5, N'Trần Thị Định', '2026-05-11 05:15', 'dat', N'Xà lách hữu cơ đạt tiêu chuẩn Organic, không hóa chất', 'SIGN005'),
(6, N'Trần Thị Định', '2026-05-11 05:15', 'dat', N'Rau dền đạt tiêu chuẩn VietGAP', 'SIGN006'),
(7, N'Trần Thị Định', '2026-05-11 05:15', 'dat', N'Rau ngót đạt tiêu chuẩn VietGAP', 'SIGN007'),
(8, N'Trần Thị Định', '2026-05-11 05:15', 'dat', N'Rau cần đạt tiêu chuẩn VietGAP, thơm tự nhiên', 'SIGN008'),
(12, N'Lê Văn Nghiệm', '2026-05-11 05:30', 'dat', N'Khoai tây Đà Lạt đạt tiêu chuẩn VietGAP, củ to đều', 'SIGN009'),
(13, N'Lê Văn Nghiệm', '2026-05-11 05:30', 'dat', N'Hành tây đạt tiêu chuẩn VietGAP', 'SIGN010'),
(14, N'Lê Văn Nghiệm', '2026-05-11 05:30', 'dat', N'Bí đao đạt tiêu chuẩn VietGAP, tươi ngon', 'SIGN011'),
(16, N'Phạm Thị Thẩm', '2026-05-11 05:45', 'dat', N'Cam Vinh đạt tiêu chuẩn VietGAP, ngọt tự nhiên', 'SIGN012'),
(17, N'Phạm Thị Thẩm', '2026-05-11 05:45', 'dat', N'Chuối tiêu đạt tiêu chuẩn VietGAP, chín vàng', 'SIGN013'),
(18, N'Phạm Thị Thẩm', '2026-05-11 05:45', 'dat', N'Bưởi da xanh đạt tiêu chuẩn VietGAP', 'SIGN014'),
(19, N'Hoàng Văn Tra', '2026-05-11 06:00', 'dat', N'Tỏi Lý Sơn đạt tiêu chuẩn VietGAP, cay thơm', 'SIGN015'),
(20, N'Hoàng Văn Tra', '2026-05-11 06:00', 'dat', N'Gừng già đạt tiêu chuẩn VietGAP, cay nồng', 'SIGN016'),
(21, N'Hoàng Văn Tra', '2026-05-11 06:00', 'dat', N'Ớt hiểm đạt tiêu chuẩn VietGAP, cay đậm', 'SIGN017'),
(23, N'Võ Thị Xét', '2026-05-11 06:15', 'dat', N'Cà chua đạt tiêu chuẩn VietGAP', 'SIGN018'),
(24, N'Võ Thị Xét', '2026-05-11 06:15', 'dat', N'Cà rốt đạt tiêu chuẩn VietGAP, giòn ngọt', 'SIGN019'),
(25, N'Võ Thị Xét', '2026-05-11 06:15', 'dat', N'Bí ngô đạt tiêu chuẩn VietGAP, ngọt tự nhiên', 'SIGN020');

GO



DELETE FROM KiemDinh;
DBCC CHECKIDENT ('KiemDinh', RESEED, 0);
PRINT N'Đã xóa dữ liệu bảng KiemDinh';

DELETE FROM ChiTietDonHang;
PRINT N'Đã xóa dữ liệu bảng ChiTietDonHang';

DELETE FROM DonHang;
DBCC CHECKIDENT ('DonHang', RESEED, 0);
PRINT N'Đã xóa dữ liệu bảng DonHang';

DELETE FROM VanChuyen;
DBCC CHECKIDENT ('VanChuyen', RESEED, 0);
PRINT N'Đã xóa dữ liệu bảng VanChuyen';

DELETE FROM TonKho;
PRINT N'Đã xóa dữ liệu bảng TonKho';

DELETE FROM Kho;
DBCC CHECKIDENT ('Kho', RESEED, 0);
PRINT N'Đã xóa dữ liệu bảng Kho';

DELETE FROM LoNongSan;
DBCC CHECKIDENT ('LoNongSan', RESEED, 0);
PRINT N'Đã xóa dữ liệu bảng LoNongSan';

DELETE FROM TrangTrai;
DBCC CHECKIDENT ('TrangTrai', RESEED, 0);
PRINT N'Đã xóa dữ liệu bảng TrangTrai';

DELETE FROM SanPham;
DBCC CHECKIDENT ('SanPham', RESEED, 0);
PRINT N'Đã xóa dữ liệu bảng SanPham';

DELETE FROM SieuThi;
DBCC CHECKIDENT ('SieuThi', RESEED, 0);
PRINT N'Đã xóa dữ liệu bảng SieuThi';

DELETE FROM DaiLy;
DBCC CHECKIDENT ('DaiLy', RESEED, 0);
PRINT N'Đã xóa dữ liệu bảng DaiLy';

DELETE FROM NongDan;
DBCC CHECKIDENT ('NongDan', RESEED, 0);
PRINT N'Đã xóa dữ liệu bảng NongDan';

DELETE FROM Admin;
DBCC CHECKIDENT ('Admin', RESEED, 0);
PRINT N'Đã xóa dữ liệu bảng Admin';

DELETE FROM TaiKhoan;
DBCC CHECKIDENT ('TaiKhoan', RESEED, 0);
PRINT N'Đã xóa dữ liệu bảng TaiKhoan';

DELETE FROM PhieuChuyenKho;
DBCC CHECKIDENT ('PhieuChuyenKho', RESEED, 0);



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



USE CNWEB_Agri_Supply_Chain;
GO

IF COL_LENGTH('dbo.NongDan', 'Facebook') IS NULL
BEGIN
    ALTER TABLE dbo.NongDan ADD Facebook NVARCHAR(255) NULL;
END
GO

IF COL_LENGTH('dbo.NongDan', 'TikTok') IS NULL
BEGIN
    ALTER TABLE dbo.NongDan ADD TikTok NVARCHAR(255) NULL;
END
GO

IF COL_LENGTH('dbo.DaiLy', 'Facebook') IS NULL
BEGIN
    ALTER TABLE dbo.DaiLy ADD Facebook NVARCHAR(255) NULL;
END
GO

IF COL_LENGTH('dbo.DaiLy', 'TikTok') IS NULL
BEGIN
    ALTER TABLE dbo.DaiLy ADD TikTok NVARCHAR(255) NULL;
END
GO

IF COL_LENGTH('dbo.SieuThi', 'Facebook') IS NULL
BEGIN
    ALTER TABLE dbo.SieuThi ADD Facebook NVARCHAR(255) NULL;
END
GO

IF COL_LENGTH('dbo.SieuThi', 'TikTok') IS NULL
BEGIN
    ALTER TABLE dbo.SieuThi ADD TikTok NVARCHAR(255) NULL;
END
GO




-- Thêm cột HinhAnh vào bảng SanPham
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SanPham]') AND name = 'HinhAnh')
BEGIN
    ALTER TABLE [dbo].[SanPham]
    ADD HinhAnh NVARCHAR(500) NULL;
    PRINT 'Đã thêm cột HinhAnh vào bảng SanPham';
END
ELSE
BEGIN
    PRINT 'Cột HinhAnh đã tồn tại trong bảng SanPham';
END
GO

-- Thêm cột HinhAnh vào bảng TrangTrai
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[TrangTrai]') AND name = 'HinhAnh')
BEGIN
    ALTER TABLE [dbo].[TrangTrai]
    ADD HinhAnh NVARCHAR(500) NULL;
    PRINT 'Đã thêm cột HinhAnh vào bảng TrangTrai';
END
ELSE
BEGIN
    PRINT 'Cột HinhAnh đã tồn tại trong bảng TrangTrai';
END
GO

PRINT 'Hoàn thành cập nhật database!';




-- Thêm cột AnhDaiDien vào bảng NongDan
ALTER TABLE [dbo].[NongDan]
ADD AnhDaiDien NVARCHAR(MAX) NULL;

-- Thêm cột AnhDaiDien vào bảng DaiLy
ALTER TABLE [dbo].[DaiLy]
ADD AnhDaiDien NVARCHAR(MAX) NULL;

-- Thêm cột AnhDaiDien vào bảng SieuThi
ALTER TABLE [dbo].[SieuThi]
ADD AnhDaiDien NVARCHAR(MAX) NULL;

PRINT 'Đã thêm cột AnhDaiDien vào các bảng NongDan, DaiLy, SieuThi!';



-- =============================================
-- CHỨC NĂNG CHAT/TIN NHẮN
-- =============================================

-- Bảng CuocTroChuyen (Conversations)
CREATE TABLE CuocTroChuyen (
    MaCuocTroChuyen INT IDENTITY(1,1) PRIMARY KEY,
    MaNguoi1 INT NOT NULL,
    LoaiNguoi1 NVARCHAR(20) NOT NULL, -- 'nongdan', 'daily', 'sieuthi'
    MaNguoi2 INT NOT NULL,
    LoaiNguoi2 NVARCHAR(20) NOT NULL,
    TinNhanCuoi NVARCHAR(MAX) NULL,
    NgayCapNhat DATETIME2 DEFAULT SYSDATETIME(),
    NgayTao DATETIME2 DEFAULT SYSDATETIME()
);

-- Bảng TinNhan (Messages)
CREATE TABLE TinNhan (
    MaTinNhan INT IDENTITY(1,1) PRIMARY KEY,
    MaCuocTroChuyen INT NOT NULL,
    MaNguoiGui INT NOT NULL,
    LoaiNguoiGui NVARCHAR(20) NOT NULL, -- 'nongdan', 'daily', 'sieuthi'
    NoiDung NVARCHAR(MAX) NOT NULL,
    DaDoc BIT DEFAULT 0,
    NgayGui DATETIME2 DEFAULT SYSDATETIME(),
    FOREIGN KEY (MaCuocTroChuyen) REFERENCES CuocTroChuyen(MaCuocTroChuyen) ON DELETE CASCADE
);

-- Index để tăng tốc truy vấn
CREATE INDEX IX_CuocTroChuyen_Nguoi1 ON CuocTroChuyen(MaNguoi1, LoaiNguoi1);
CREATE INDEX IX_CuocTroChuyen_Nguoi2 ON CuocTroChuyen(MaNguoi2, LoaiNguoi2);
CREATE INDEX IX_TinNhan_CuocTroChuyen ON TinNhan(MaCuocTroChuyen);
CREATE INDEX IX_TinNhan_DaDoc ON TinNhan(DaDoc);

PRINT 'Đã tạo bảng CuocTroChuyen và TinNhan cho chức năng chat!';
GO

-- Dữ liệu mẫu cho chat
INSERT INTO CuocTroChuyen (MaNguoi1, LoaiNguoi1, MaNguoi2, LoaiNguoi2, TinNhanCuoi, NgayCapNhat) VALUES
(1, 'nongdan', 1, 'daily', N'Vâng, tôi sẽ giao hàng vào sáng mai', '2026-05-12 10:00'),
(1, 'daily', 1, 'sieuthi', N'Đơn hàng đã được xác nhận', '2026-05-12 11:00'),
(2, 'nongdan', 2, 'daily', N'Cảm ơn bạn đã đặt hàng', '2026-05-12 12:00'),
(3, 'nongdan', 3, 'daily', N'Hàng đã sẵn sàng giao', '2026-05-12 13:00'),
(4, 'nongdan', 2, 'daily', N'Cam Vinh chất lượng cao', '2026-05-12 14:00');

INSERT INTO TinNhan (MaCuocTroChuyen, MaNguoiGui, LoaiNguoiGui, NoiDung, DaDoc, NgayGui) VALUES
-- Cuộc trò chuyện 1: Nông dân 1 - Đại lý 1
(1, 1, 'daily', N'Chào bạn, tôi muốn đặt 100kg cà chua', 1, '2026-05-12 08:00'),
(1, 1, 'nongdan', N'Chào bạn, hiện tại tôi có sẵn hàng. Giá 50.000đ/kg', 1, '2026-05-12 08:30'),
(1, 1, 'daily', N'Được, khi nào có thể giao hàng?', 1, '2026-05-12 09:00'),
(1, 1, 'nongdan', N'Vâng, tôi sẽ giao hàng vào sáng mai', 0, '2026-05-12 10:00'),

-- Cuộc trò chuyện 2: Đại lý 1 - Siêu thị 1
(2, 1, 'sieuthi', N'Tôi cần đặt 200kg rau muống', 1, '2026-05-12 09:00'),
(2, 1, 'daily', N'Dạ, hiện tại shop có sẵn. Giá 45.000đ/kg', 1, '2026-05-12 09:30'),
(2, 1, 'sieuthi', N'OK, tôi đặt luôn nhé', 1, '2026-05-12 10:30'),
(2, 1, 'daily', N'Đơn hàng đã được xác nhận', 0, '2026-05-12 11:00'),

-- Cuộc trò chuyện 3: Nông dân 2 - Đại lý 2
(3, 2, 'daily', N'Bạn có rau xà lách không?', 1, '2026-05-12 11:00'),
(3, 2, 'nongdan', N'Có bạn, tôi có 100kg rau xà lách tươi', 1, '2026-05-12 11:30'),
(3, 2, 'daily', N'Cảm ơn bạn đã đặt hàng', 0, '2026-05-12 12:00'),

-- Cuộc trò chuyện 4: Nông dân 3 - Đại lý 3
(4, 3, 'daily', N'Khoai tây Đà Lạt còn hàng không?', 1, '2026-05-12 12:00'),
(4, 3, 'nongdan', N'Còn nhiều bạn, giá 70.000đ/kg', 1, '2026-05-12 12:30'),
(4, 3, 'daily', N'Hàng đã sẵn sàng giao', 0, '2026-05-12 13:00'),

-- Cuộc trò chuyện 5: Nông dân 4 - Đại lý 2
(5, 4, 'daily', N'Cam Vinh có sẵn không bạn?', 1, '2026-05-12 13:00'),
(5, 4, 'nongdan', N'Có bạn, cam rất ngọt và tươi', 1, '2026-05-12 13:30'),
(5, 4, 'daily', N'Cam Vinh chất lượng cao', 0, '2026-05-12 14:00');

PRINT 'Đã thêm dữ liệu mẫu cho chức năng chat!';
GO

SELECT * FROM CuocTroChuyen;
SELECT * FROM TinNhan;



IF OBJECT_ID(N'dbo.PhieuChuyenKho', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PhieuChuyenKho
    (
        MaPhieu      INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        MaKhoNguon   INT NOT NULL,
        MaKhoDich    INT NOT NULL,
        MaLo         INT NOT NULL,
        SoLuong      DECIMAL(18,2) NOT NULL,
        NgayChuyen   DATETIME NOT NULL CONSTRAINT DF_PhieuChuyenKho_NgayChuyen DEFAULT (GETDATE()),
        GhiChu       NVARCHAR(500) NULL
    );

    ALTER TABLE dbo.PhieuChuyenKho
      ADD CONSTRAINT FK_PhieuChuyenKho_KhoNguon FOREIGN KEY (MaKhoNguon) REFERENCES dbo.Kho(MaKho);

    ALTER TABLE dbo.PhieuChuyenKho
      ADD CONSTRAINT FK_PhieuChuyenKho_KhoDich FOREIGN KEY (MaKhoDich) REFERENCES dbo.Kho(MaKho);

    ALTER TABLE dbo.PhieuChuyenKho
      ADD CONSTRAINT FK_PhieuChuyenKho_LoNongSan FOREIGN KEY (MaLo) REFERENCES dbo.LoNongSan(MaLo);

    CREATE INDEX IX_PhieuChuyenKho_NgayChuyen ON dbo.PhieuChuyenKho(NgayChuyen DESC);
    CREATE INDEX IX_PhieuChuyenKho_KhoNguon ON dbo.PhieuChuyenKho(MaKhoNguon);
END
GO




-- 1. Tạo stored procedure để cập nhật trạng thái hết hạn
CREATE OR ALTER PROCEDURE sp_UpdateExpiredBatchStatus
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Chỉ cập nhật trạng thái hết hạn cho các lô:
    -- - Đã quá hạn sử dụng
    -- - Đang ở trạng thái 'san_sang' hoặc 'dang_van_chuyen'
    -- - Còn số lượng (SoLuongHienTai > 0)
    UPDATE LoNongSan
    SET TrangThai = 'het_han'
    WHERE HanSuDung < CAST(GETDATE() AS DATE)
      AND TrangThai IN ('san_sang', 'dang_van_chuyen')
      AND SoLuongHienTai > 0;
    
    -- Log số lượng bản ghi đã cập nhật
    DECLARE @UpdatedCount INT = @@ROWCOUNT;
    IF @UpdatedCount > 0
    BEGIN
        PRINT CONCAT('Đã cập nhật ', @UpdatedCount, ' lô nông sản sang trạng thái hết hạn');
    END
END;
GO


EXEC sp_UpdateExpiredBatchStatus;
GO



-- 3. Cập nhật ngay lập tức tất cả lô đã hết hạn
EXEC sp_UpdateExpiredBatchStatus;
GO

-- 4. Kiểm tra kết quả
SELECT 
    MaLo,
    TenSanPham = sp.TenSanPham,
    HanSuDung,
    TrangThai,
    DaysExpired = DATEDIFF(DAY, HanSuDung, GETDATE())
FROM LoNongSan ln
LEFT JOIN SanPham sp ON ln.MaSanPham = sp.MaSanPham
WHERE HanSuDung < CAST(GETDATE() AS DATE)
ORDER BY HanSuDung DESC;
GO






-- Thêm cột MaDonHang (nullable vì các bản ghi cũ không có)
ALTER TABLE VanChuyen
ADD MaDonHang INT NULL;

-- Thêm foreign key constraint
ALTER TABLE VanChuyen
ADD CONSTRAINT FK_VanChuyen_DonHang 
FOREIGN KEY (MaDonHang) REFERENCES DonHang(MaDonHang);

-- Thêm index để tăng tốc query theo MaDonHang
CREATE INDEX IX_VanChuyen_MaDonHang ON VanChuyen(MaDonHang);

PRINT 'Migration completed: Added MaDonHang column to VanChuyen table';









-- 1: Thêm cột MaTrangTrai 
ALTER TABLE SanPham
ADD MaTrangTrai INT NULL;

-- 2: Cập nhật MaTrangTrai cho các sản phẩm hiện có dựa trên lô nông sản
-- Lấy MaTrangTrai từ LoNongSan (lô nông sản đầu tiên của sản phẩm đó)
UPDATE sp
SET sp.MaTrangTrai = (
    SELECT TOP 1 ln.MaTrangTrai
    FROM LoNongSan ln
    WHERE ln.MaSanPham = sp.MaSanPham
    ORDER BY ln.NgayTao DESC
)
FROM SanPham sp
WHERE EXISTS (
    SELECT 1 FROM LoNongSan ln WHERE ln.MaSanPham = sp.MaSanPham
);

--  3: Xóa các sản phẩm không có trang trại (nếu có)
DELETE FROM SanPham WHERE MaTrangTrai IS NULL;

--  4: Đặt cột MaTrangTrai thành NOT NULL
ALTER TABLE SanPham
ALTER COLUMN MaTrangTrai INT NOT NULL;

-- 5: Thêm foreign key constraint
ALTER TABLE SanPham
ADD CONSTRAINT FK_SanPham_TrangTrai 
FOREIGN KEY (MaTrangTrai) REFERENCES TrangTrai(MaTrangTrai);

--  6: Thêm index để tăng tốc query
CREATE INDEX IX_SanPham_MaTrangTrai ON SanPham(MaTrangTrai);

