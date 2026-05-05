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
('admin01', 'admin123', 'admin01@agrisupply.com', 'admin', N'hoat_dong', '2024-01-15'),
('admin02', 'admin123', 'admin02@agrisupply.com', 'admin', N'hoat_dong', '2024-02-20'),
('nongdan01', 'nongdan123', 'tranvannong@gmail.com', 'nongdan', N'hoat_dong', '2024-03-10'),
('nongdan02', 'nongdan123', 'lethihoa@gmail.com', 'nongdan', N'hoat_dong', '2024-03-12'),
('nongdan03', 'nongdan123', 'phamminhtam@gmail.com', 'nongdan', N'hoat_dong', '2024-04-05'),
('nongdan04', 'nongdan123', 'nguyenvanbinh@gmail.com', 'nongdan', N'hoat_dong', '2024-04-18'),
('nongdan05', 'nongdan123', 'vothimai@gmail.com', 'nongdan', N'hoat_dong', '2024-05-22'),
('nongdan06', 'nongdan123', 'hoangvanduc@gmail.com', 'nongdan', N'hoat_dong', '2024-06-08'),
('nongdan07', 'nongdan123', 'dangthilan@gmail.com', 'nongdan', N'hoat_dong', '2024-07-14'),
('nongdan08', 'nongdan123', 'buivanhung@gmail.com', 'nongdan', N'hoat_dong', '2024-08-03'),
('nongdan09', 'nongdan123', 'lythinga@gmail.com', 'nongdan', N'hoat_dong', '2024-09-11'),
('nongdan10', 'nongdan123', 'phanvanson@gmail.com', 'nongdan', N'hoat_dong', '2024-10-25'),
('nongdan11', 'nongdan123', 'truongthihang@gmail.com', 'nongdan', N'hoat_dong', '2024-11-07'),
('nongdan12', 'nongdan123', 'dinhvantoan@gmail.com', 'nongdan', N'hoat_dong', '2024-12-19'),
('nongdan13', 'nongdan123', 'maithixuan@gmail.com', 'nongdan', N'hoat_dong', '2025-01-08'),
('nongdan14', 'nongdan123', 'duongvanphuc@gmail.com', 'nongdan', N'hoat_dong', '2025-02-14'),
('nongdan15', 'nongdan123', 'caothithao@gmail.com', 'nongdan', N'hoat_dong', '2025-03-22'),
('nongdan16', 'nongdan123', 'lamvankhoa@gmail.com', 'nongdan', N'hoat_dong', '2025-04-30'),
('nongdan17', 'nongdan123', 'huynhthidiem@gmail.com', 'nongdan', N'hoat_dong', '2025-05-17'),
('nongdan18', 'nongdan123', 'tovanlong@gmail.com', 'nongdan', N'hoat_dong', '2025-06-25'),
('nongdan19', 'nongdan123', 'vuthikim@gmail.com', 'nongdan', N'hoat_dong', '2025-07-09'),
('nongdan20', 'nongdan123', 'hovantai@gmail.com', 'nongdan', N'hoat_dong', '2025-08-13'),
('daily01', 'daily123', 'daily.miennam@gmail.com', 'daily', N'hoat_dong', '2024-02-10'),
('daily02', 'daily123', 'thucphamsach@gmail.com', 'daily', N'hoat_dong', '2024-03-15'),
('daily03', 'daily123', 'raucuquatuoi@gmail.com', 'daily', N'hoat_dong', '2024-04-20'),
('daily04', 'daily123', 'nongsanxanh@gmail.com', 'daily', N'hoat_dong', '2024-05-25'),
('daily05', 'daily123', 'thucphamantoan@gmail.com', 'daily', N'hoat_dong', '2024-06-30'),
('daily06', 'daily123', 'rausachorganic@gmail.com', 'daily', N'hoat_dong', '2024-08-05'),
('daily07', 'daily123', 'nongsanviet@gmail.com', 'daily', N'hoat_dong', '2024-09-10'),
('daily08', 'daily123', 'thucphamtuoingon@gmail.com', 'daily', N'hoat_dong', '2024-10-15'),
('daily09', 'daily123', 'raucusach@gmail.com', 'daily', N'hoat_dong', '2024-11-20'),
('daily10', 'daily123', 'nongsanhuuco@gmail.com', 'daily', N'hoat_dong', '2024-12-25'),
('daily11', 'daily123', 'thucphamxanh@gmail.com', 'daily', N'hoat_dong', '2025-01-30'),
('daily12', 'daily123', 'rauquatuoi@gmail.com', 'daily', N'hoat_dong', '2025-03-05'),
('daily13', 'daily123', 'nongsansach@gmail.com', 'daily', N'hoat_dong', '2025-04-10'),
('daily14', 'daily123', 'thucphamorganic@gmail.com', 'daily', N'hoat_dong', '2025-05-15'),
('sieuthi01', 'sieuthi123', 'coopmart@gmail.com', 'sieuthi', N'hoat_dong', '2024-01-20'),
('sieuthi02', 'sieuthi123', 'bigc@gmail.com', 'sieuthi', N'hoat_dong', '2024-02-25'),
('sieuthi03', 'sieuthi123', 'lottemart@gmail.com', 'sieuthi', N'hoat_dong', '2024-04-01'),
('sieuthi04', 'sieuthi123', 'aeonmall@gmail.com', 'sieuthi', N'hoat_dong', '2024-05-06'),
('sieuthi05', 'sieuthi123', 'vinmart@gmail.com', 'sieuthi', N'hoat_dong', '2024-06-11'),
('sieuthi06', 'sieuthi123', 'megamarket@gmail.com', 'sieuthi', N'hoat_dong', '2024-07-16'),
('sieuthi07', 'sieuthi123', 'emart@gmail.com', 'sieuthi', N'hoat_dong', '2024-08-21'),
('sieuthi08', 'sieuthi123', 'satra@gmail.com', 'sieuthi', N'hoat_dong', '2024-09-26'),
('sieuthi09', 'sieuthi123', 'citimart@gmail.com', 'sieuthi', N'hoat_dong', '2024-11-01'),
('sieuthi10', 'sieuthi123', 'topsmarket@gmail.com', 'sieuthi', N'hoat_dong', '2024-12-06'),
('sieuthi11', 'sieuthi123', 'maximark@gmail.com', 'sieuthi', N'hoat_dong', '2025-01-11'),
('sieuthi12', 'sieuthi123', 'fivimart@gmail.com', 'sieuthi', N'hoat_dong', '2025-02-16');

-- 2. ADMIN 
INSERT INTO Admin (MaTaiKhoan, HoTen) VALUES
(1, N'Nguyễn Duy Thuấn'),
(2, N'Nguyễn Thuấn Duy');

-- 3. NÔNG DÂN 
INSERT INTO NongDan (MaTaiKhoan, HoTen, SoDienThoai, DiaChi) VALUES
(3, N'Trần Văn Nông', '0901234567', N'Ấp 3, Xã Tân Phú, Củ Chi'),
(4, N'Lê Thị Hoa', '0912345678', N'Ấp 2, Xã Phước Vĩnh An, Củ Chi'),
(5, N'Phạm Minh Tâm', '0923456789', N'Ấp 1, Xã Trung An, Củ Chi'),
(6, N'Nguyễn Văn Bình', '0934567890', N'Ấp 4, Xã Phú Mỹ Hưng, Củ Chi'),
(7, N'Võ Thị Mai', '0945678901', N'Ấp 2, Xã Tân Thông Hội, Củ Chi'),
(8, N'Hoàng Văn Đức', '0956789012', N'Ấp 3, Xã Bình Mỹ, Củ Chi'),
(9, N'Đặng Thị Lan', '0967890123', N'Ấp 1, Xã Thái Mỹ, Củ Chi'),
(10, N'Bùi Văn Hùng', '0978901234', N'Ấp 5, Xã Phước Hiệp, Củ Chi'),
(11, N'Lý Thị Nga', '0989012345', N'Ấp 2, Xã An Nhơn Tây, Củ Chi'),
(12, N'Phan Văn Sơn', '0990123456', N'Ấp 4, Xã Nhuận Đức, Củ Chi'),
(13, N'Trương Thị Hằng', '0901234568', N'Ấp 1, Xã Phạm Văn Cội, Củ Chi'),
(14, N'Đinh Văn Toàn', '0912345679', N'Ấp 3, Xã Tân An Hội, Củ Chi'),
(15, N'Mai Thị Xuân', '0923456780', N'Ấp 2, Xã Trung Lập Hạ, Củ Chi'),
(16, N'Dương Văn Phúc', '0934567891', N'Ấp 5, Xã Trung Lập Thượng, Củ Chi'),
(17, N'Cao Thị Thảo', '0945678902', N'Ấp 1, Xã Hòa Phú, Củ Chi'),
(18, N'Lâm Văn Khoa', '0956789013', N'Ấp 4, Xã Tân Thạnh Đông, Củ Chi'),
(19, N'Huỳnh Thị Diễm', '0967890124', N'Ấp 2, Xã Tân Thạnh Tây, Củ Chi'),
(20, N'Tô Văn Long', '0978901235', N'Ấp 3, Xã Phước Thạnh, Củ Chi'),
(21, N'Vũ Thị Kim', '0989012346', N'Ấp 1, Xã Phước Vĩnh Tây, Củ Chi'),
(22, N'Hồ Văn Tài', '0990123457', N'Ấp 5, Xã Phước Vĩnh Đông, Củ Chi');

-- 4. ĐẠI LÝ
INSERT INTO DaiLy (MaTaiKhoan, TenDaiLy, SoDienThoai, DiaChi) VALUES
(23, N'Đại lý Nông sản Miền Nam', '0934567890', N'123 Lê Văn Việt, Q.9'),
(24, N'Đại lý Thực phẩm Sạch', '0945678901', N'456 Nguyễn Duy Trinh, Q.2'),
(25, N'Đại lý Rau Củ Quả Tươi', '0956789012', N'789 Võ Văn Ngân, Thủ Đức'),
(26, N'Đại lý Nông sản Xanh', '0967890123', N'321 Phạm Văn Đồng, Gò Vấp'),
(27, N'Đại lý Thực phẩm An Toàn', '0978901234', N'654 Quang Trung, Q.12'),
(28, N'Đại lý Rau Sạch Organic', '0989012345', N'987 Lê Đức Thọ, Gò Vấp'),
(29, N'Đại lý Nông sản Việt', '0990123456', N'147 Phan Văn Trị, Bình Thạnh'),
(30, N'Đại lý Thực phẩm Tươi Ngon', '0901234569', N'258 Cách Mạng Tháng 8, Q.10'),
(31, N'Đại lý Rau Củ Sạch', '0912345680', N'369 Lý Thường Kiệt, Q.11'),
(32, N'Đại lý Nông sản Hữu Cơ', '0923456781', N'741 Nguyễn Thị Minh Khai, Q.3'),
(33, N'Đại lý Thực phẩm Xanh', '0934567892', N'852 Điện Biên Phủ, Bình Thạnh'),
(34, N'Đại lý Rau Quả Tươi', '0945678903', N'963 Hoàng Văn Thụ, Tân Bình'),
(35, N'Đại lý Nông sản Sạch', '0956789014', N'159 Trường Chinh, Tân Bình'),
(36, N'Đại lý Thực phẩm Organic', '0967890125', N'357 Cộng Hòa, Tân Bình');

-- 5. SIÊU THỊ 
INSERT INTO SieuThi (MaTaiKhoan, TenSieuThi, SoDienThoai, DiaChi) VALUES
(37, N'Siêu thị Co.opmart', '0956789012', N'789 Võ Văn Ngân, Thủ Đức'),
(38, N'Siêu thị BigC', '0967890123', N'321 Phạm Văn Đồng, Gò Vấp'),
(39, N'Siêu thị Lotte Mart', '0978901234', N'469 Nguyễn Hữu Thọ, Q.7'),
(40, N'Siêu thị Aeon Mall', '0989012345', N'30 Bờ Bao Tân Thắng, Tân Phú'),
(41, N'Siêu thị Vinmart', '0990123456', N'72 Lê Thánh Tôn, Q.1'),
(42, N'Siêu thị Mega Market', '0901234570', N'1362 Kha Vạn Cân, Thủ Đức'),
(43, N'Siêu thị Emart', '0912345681', N'26 Lý Tự Trọng, Q.1'),
(44, N'Siêu thị Satra', '0923456782', N'268 Tô Hiến Thành, Q.10'),
(45, N'Siêu thị Citimart', '0934567893', N'268 Lê Hồng Phong, Q.10'),
(46, N'Siêu thị Tops Market', '0945678904', N'34 Lê Duẩn, Q.1'),
(47, N'Siêu thị Maximark', '0956789015', N'520 Cách Mạng Tháng 8, Q.3'),
(48, N'Siêu thị Fivimart', '0967890126', N'1 Nguyễn Văn Linh, Q.7');


-- 6. SẢN PHẨM 
INSERT INTO SanPham (TenSanPham, DonViTinh, MoTa) VALUES
(N'Cà chua', N'kg', N'Cà chua tươi, màu đỏ, chín tự nhiên'),
(N'Rau muống', N'kg', N'Rau muống tươi, lá xanh'),
(N'Cải thảo', N'kg', N'Cải thảo tươi, giòn ngọt'),
(N'Dưa chuột', N'kg', N'Dưa chuột tươi, xanh giòn'),
(N'Ớt', N'kg', N'Ớt tươi, cay nồng'),
(N'Gạo ST25', N'kg', N'Gạo thơm ST25, hạt dài'),
(N'Rau xà lách', N'kg', N'Xà lách tươi, lá xanh'),
(N'Cà rốt', N'kg', N'Cà rốt tươi, màu cam'),
(N'Khoai tây', N'kg', N'Khoai tây Đà Lạt'),
(N'Hành tây', N'kg', N'Hành tây tươi'),
(N'Tỏi', N'kg', N'Tỏi Lý Sơn'),
(N'Gừng', N'kg', N'Gừng tươi'),
(N'Ớt chuông', N'kg', N'Ớt chuông nhiều màu'),
(N'Bí đao', N'kg', N'Bí đao tươi'),
(N'Bí ngô', N'kg', N'Bí ngô Nhật'),
(N'Mướp', N'kg', N'Mướp tươi'),
(N'Đậu cove', N'kg', N'Đậu cove xanh'),
(N'Đậu đũa', N'kg', N'Đậu đũa tươi'),
(N'Bắp cải', N'kg', N'Bắp cải tươi'),
(N'Súp lơ xanh', N'kg', N'Súp lơ xanh'),
(N'Súp lơ trắng', N'kg', N'Súp lơ trắng'),
(N'Cải bó xôi', N'kg', N'Cải bó xôi'),
(N'Rau dền', N'kg', N'Rau dền đỏ/xanh'),
(N'Rau ngót', N'kg', N'Rau ngót tươi'),
(N'Rau cần', N'kg', N'Rau cần thơm'),
(N'Hành lá', N'kg', N'Hành lá tươi'),
(N'Ngò rí', N'kg', N'Ngò rí thơm'),
(N'Húng quế', N'kg', N'Húng quế tươi'),
(N'Rau thơm', N'kg', N'Rau thơm tổng hợp'),
(N'Chanh', N'kg', N'Chanh tươi'),
(N'Cam', N'kg', N'Cam Vinh/Cao Phong'),
(N'Quýt', N'kg', N'Quýt ngọt'),
(N'Bưởi', N'kg', N'Bưởi da xanh'),
(N'Xoài', N'kg', N'Xoài cát Hòa Lộc'),
(N'Chuối', N'kg', N'Chuối tiêu'),
(N'Đu đủ', N'kg', N'Đu đủ chín vàng'),
(N'Dưa hấu', N'kg', N'Dưa hấu không hạt'),
(N'Dưa lưới', N'kg', N'Dưa lưới Nhật'),
(N'Dâu tây', N'kg', N'Dâu tây Đà Lạt'),
(N'Nho', N'kg', N'Nho xanh/đỏ'),
(N'Táo', N'kg', N'Táo Fuji'),
(N'Lê', N'kg', N'Lê Hàn Quốc'),
(N'Thanh long', N'kg', N'Thanh long ruột đỏ'),
(N'Măng cụt', N'kg', N'Măng cụt tươi'),
(N'Chom chom', N'kg', N'Chom chom'),
(N'Vải', N'kg', N'Vải thiều'),
(N'Nhãn', N'kg', N'Nhãn lồng Hưng Yên'),
(N'Mận', N'kg', N'Mận Mộc Châu'),
(N'Dứa', N'kg', N'Dứa Cayenne'),
(N'Thơm', N'kg', N'Thơm Queen');


-- 7. TRANG TRẠI 
INSERT INTO TrangTrai (MaNongDan, TenTrangTrai, DiaChi, SoChungNhan) VALUES
(1, N'Trang trại Xanh', N'Ấp 3, Xã Tân Phú', 'VG001234'),
(1, N'Trang trại Organic 1', N'Ấp 5, Xã Tân Phú', 'ORG001235'),
(2, N'Trang trại Hoa Sen', N'Ấp 2, Xã Phước Vĩnh An', 'VG001236'),
(3, N'Trang trại Minh Tâm', N'Ấp 1, Xã Trung An', 'VG001237'),
(4, N'Trang trại Bình An', N'Ấp 4, Xã Phú Mỹ Hưng', 'VG001238'),
(5, N'Trang trại Mai Xanh', N'Ấp 2, Xã Tân Thông Hội', 'VG001239'),
(6, N'Trang trại Đức Phát', N'Ấp 3, Xã Bình Mỹ', 'VG001240'),
(7, N'Trang trại Lan Anh', N'Ấp 1, Xã Thái Mỹ', 'VG001241'),
(8, N'Trang trại Hùng Vương', N'Ấp 5, Xã Phước Hiệp', 'VG001242'),
(9, N'Trang trại Nga Xanh', N'Ấp 2, Xã An Nhơn Tây', 'VG001243'),
(10, N'Trang trại Sơn Thủy', N'Ấp 4, Xã Nhuận Đức', 'VG001244'),
(11, N'Trang trại Hằng Nga', N'Ấp 1, Xã Phạm Văn Cội', 'VG001245'),
(12, N'Trang trại Toàn Phát', N'Ấp 3, Xã Tân An Hội', 'VG001246'),
(13, N'Trang trại Xuân Mai', N'Ấp 2, Xã Trung Lập Hạ', 'VG001247'),
(14, N'Trang trại Phúc Lộc', N'Ấp 5, Xã Trung Lập Thượng', 'VG001248'),
(15, N'Trang trại Thảo Nguyên', N'Ấp 1, Xã Hòa Phú', 'VG001249'),
(16, N'Trang trại Khoa Học', N'Ấp 4, Xã Tân Thạnh Đông', 'VG001250'),
(17, N'Trang trại Diễm Phúc', N'Ấp 2, Xã Tân Thạnh Tây', 'VG001251'),
(18, N'Trang trại Long Thành', N'Ấp 3, Xã Phước Thạnh', 'VG001252'),
(19, N'Trang trại Kim Cương', N'Ấp 1, Xã Phước Vĩnh Tây', 'VG001253'),
(20, N'Trang trại Tài Lộc', N'Ấp 5, Xã Phước Vĩnh Đông', 'VG001254'),
(1, N'Trang trại Hữu Cơ 2', N'Ấp 6, Xã Tân Phú', 'ORG001255'),
(2, N'Trang trại Sạch', N'Ấp 3, Xã Phước Vĩnh An', 'VG001256'),
(3, N'Trang trại An Toàn', N'Ấp 2, Xã Trung An', 'VG001257'),
(4, N'Trang trại Xanh Sạch', N'Ấp 5, Xã Phú Mỹ Hưng', 'VG001258'),
(5, N'Trang trại Tươi Ngon', N'Ấp 3, Xã Tân Thông Hội', 'VG001259'),
(6, N'Trang trại Chất Lượng', N'Ấp 4, Xã Bình Mỹ', 'VG001260'),
(7, N'Trang trại Uy Tín', N'Ấp 2, Xã Thái Mỹ', 'VG001261'),
(8, N'Trang trại Phát Đạt', N'Ấp 6, Xã Phước Hiệp', 'VG001262'),
(9, N'Trang trại Thịnh Vượng', N'Ấp 3, Xã An Nhơn Tây', 'VG001263'),
(10, N'Trang trại Hòa Bình', N'Ấp 5, Xã Nhuận Đức', 'VG001264'),
(11, N'Trang trại Tân Tiến', N'Ấp 2, Xã Phạm Văn Cội', 'VG001265'),
(12, N'Trang trại Hiện Đại', N'Ấp 4, Xã Tân An Hội', 'VG001266'),
(13, N'Trang trại Công Nghệ', N'Ấp 3, Xã Trung Lập Hạ', 'VG001267'),
(14, N'Trang trại Thông Minh', N'Ấp 6, Xã Trung Lập Thượng', 'VG001268'),
(15, N'Trang trại Bền Vững', N'Ấp 2, Xã Hòa Phú', 'VG001269'),
(16, N'Trang trại Sinh Thái', N'Ấp 5, Xã Tân Thạnh Đông', 'VG001270'),
(17, N'Trang trại Môi Trường', N'Ấp 3, Xã Tân Thạnh Tây', 'VG001271'),
(18, N'Trang trại Xanh Đẹp', N'Ấp 4, Xã Phước Thạnh', 'VG001272'),
(19, N'Trang trại An Lành', N'Ấp 2, Xã Phước Vĩnh Tây', 'VG001273');


-- 8. LÔ NÔNG SẢN 
INSERT INTO LoNongSan (MaTrangTrai, MaSanPham, SoLuongBanDau, SoLuongHienTai, NgayThuHoach, HanSuDung, MaQR, TrangThai) VALUES
(1, 1, 500, 450, '2024-03-15', '2024-03-29', 'QR001', N'san_sang'),
(1, 2, 200, 180, '2024-04-20', '2024-04-27', 'QR002', N'san_sang'),
(2, 3, 300, 280, '2024-05-10', '2024-05-24', 'QR003', N'san_sang'),
(2, 4, 150, 120, '2024-06-05', '2024-06-19', 'QR004', N'san_sang'),
(3, 5, 100, 80, '2024-07-12', '2024-08-12', 'QR005', N'san_sang'),
(4, 6, 1000, 800, '2024-08-20', '2025-02-20', 'QR006', N'san_sang'),
(5, 7, 250, 200, '2024-09-08', '2024-09-15', 'QR007', N'san_sang'),
(6, 8, 400, 350, '2024-10-14', '2024-11-14', 'QR008', N'san_sang'),
(7, 9, 600, 500, '2024-11-22', '2024-12-22', 'QR009', N'san_sang'),
(8, 10, 300, 250, '2024-12-18', '2025-01-18', 'QR010', N'san_sang'),
(9, 11, 200, 180, '2025-01-10', '2025-02-10', 'QR011', N'san_sang'),
(10, 12, 150, 130, '2025-02-14', '2025-03-14', 'QR012', N'san_sang'),
(11, 13, 180, 150, '2025-03-20', '2025-04-03', 'QR013', N'san_sang'),
(12, 14, 500, 450, '2025-04-25', '2025-05-25', 'QR014', N'san_sang'),
(13, 15, 400, 350, '2025-05-30', '2025-06-30', 'QR015', N'san_sang'),
(14, 16, 250, 200, '2025-06-18', '2025-06-25', 'QR016', N'san_sang'),
(15, 17, 200, 180, '2025-07-22', '2025-07-29', 'QR017', N'san_sang'),
(16, 18, 220, 200, '2025-08-15', '2025-08-22', 'QR018', N'san_sang'),
(17, 19, 350, 300, '2025-09-10', '2025-09-24', 'QR019', N'san_sang'),
(18, 20, 180, 150, '2025-10-05', '2025-10-12', 'QR020', N'san_sang'),
(19, 21, 200, 180, '2025-11-12', '2025-11-19', 'QR021', N'san_sang'),
(20, 22, 150, 130, '2025-12-08', '2025-12-15', 'QR022', N'san_sang'),
(1, 23, 180, 160, '2026-01-14', '2026-01-21', 'QR023', N'san_sang'),
(2, 24, 160, 140, '2026-02-18', '2026-02-25', 'QR024', N'san_sang'),
(3, 25, 140, 120, '2026-03-10', '2026-03-17', 'QR025', N'san_sang'),
(4, 26, 120, 100, '2024-04-15', '2024-04-22', 'QR026', N'san_sang'),
(5, 27, 100, 80, '2024-05-20', '2024-05-27', 'QR027', N'san_sang'),
(6, 28, 90, 70, '2024-06-25', '2024-07-02', 'QR028', N'san_sang'),
(7, 29, 110, 90, '2024-07-30', '2024-08-06', 'QR029', N'san_sang'),
(8, 30, 300, 250, '2024-08-12', '2024-09-12', 'QR030', N'san_sang'),
(9, 31, 400, 350, '2024-09-18', '2024-10-18', 'QR031', N'san_sang'),
(10, 32, 350, 300, '2024-10-22', '2024-11-22', 'QR032', N'san_sang'),
(11, 33, 500, 450, '2024-11-28', '2024-12-28', 'QR033', N'san_sang'),
(12, 34, 300, 250, '2024-12-15', '2024-12-29', 'QR034', N'san_sang'),
(13, 35, 250, 200, '2025-01-20', '2025-01-27', 'QR035', N'san_sang'),
(14, 36, 200, 180, '2025-02-25', '2025-03-04', 'QR036', N'san_sang'),
(15, 37, 600, 500, '2025-03-18', '2025-04-01', 'QR037', N'san_sang'),
(16, 38, 180, 150, '2025-04-22', '2025-04-29', 'QR038', N'san_sang'),
(17, 39, 100, 80, '2025-05-15', '2025-05-22', 'QR039', N'san_sang'),
(18, 40, 150, 120, '2025-06-20', '2025-07-04', 'QR040', N'san_sang'),
(19, 41, 200, 180, '2025-07-25', '2025-08-25', 'QR041', N'san_sang'),
(20, 42, 180, 160, '2025-08-30', '2025-09-30', 'QR042', N'san_sang'),
(1, 43, 250, 200, '2025-09-12', '2025-09-26', 'QR043', N'san_sang'),
(2, 44, 120, 100, '2025-10-18', '2025-10-25', 'QR044', N'san_sang'),
(3, 45, 140, 120, '2025-11-22', '2025-11-29', 'QR045', N'san_sang'),
(4, 46, 160, 140, '2025-12-15', '2025-12-22', 'QR046', N'san_sang'),
(5, 47, 180, 160, '2026-01-20', '2026-01-27', 'QR047', N'san_sang'),
(6, 48, 130, 110, '2026-02-10', '2026-02-24', 'QR048', N'san_sang'),
(7, 49, 300, 250, '2026-03-15', '2026-04-15', 'QR049', N'san_sang'),
(8, 50, 280, 230, '2024-03-20', '2024-04-20', 'QR050', N'san_sang');


-- 9. KHO 
INSERT INTO Kho (TenKho, LoaiKho, MaChuSoHuu, LoaiChuSoHuu, DiaChi) VALUES
(N'Kho Đại lý Miền Nam', 'daily', 1, 'daily', N'123 Lê Văn Việt, Q.9'),
(N'Kho Thực phẩm Sạch', 'daily', 2, 'daily', N'456 Nguyễn Duy Trinh, Q.2'),
(N'Kho Rau Củ Quả Tươi', 'daily', 3, 'daily', N'789 Võ Văn Ngân, Thủ Đức'),
(N'Kho Nông sản Xanh', 'daily', 4, 'daily', N'321 Phạm Văn Đồng, Gò Vấp'),
(N'Kho Thực phẩm An Toàn', 'daily', 5, 'daily', N'654 Quang Trung, Q.12'),
(N'Kho Rau Sạch Organic', 'daily', 6, 'daily', N'987 Lê Đức Thọ, Gò Vấp'),
(N'Kho Nông sản Việt', 'daily', 7, 'daily', N'147 Phan Văn Trị, Bình Thạnh'),
(N'Kho Thực phẩm Tươi Ngon', 'daily', 8, 'daily', N'258 Cách Mạng Tháng 8, Q.10'),
(N'Kho Co.opmart', 'sieuthi', 1, 'sieuthi', N'789 Võ Văn Ngân, Thủ Đức'),
(N'Kho BigC', 'sieuthi', 2, 'sieuthi', N'321 Phạm Văn Đồng, Gò Vấp'),
(N'Kho Lotte Mart', 'sieuthi', 3, 'sieuthi', N'469 Nguyễn Hữu Thọ, Q.7'),
(N'Kho Aeon Mall', 'sieuthi', 4, 'sieuthi', N'30 Bờ Bao Tân Thắng, Tân Phú'),
(N'Kho Vinmart', 'sieuthi', 5, 'sieuthi', N'72 Lê Thánh Tôn, Q.1'),
(N'Kho Mega Market', 'sieuthi', 6, 'sieuthi', N'1362 Kha Vạn Cân, Thủ Đức'),
(N'Kho Trung Gian 1', 'trung_gian', 1, 'daily', N'100 Xa Lộ Hà Nội, Q.9'),
(N'Kho Trung Gian 2', 'trung_gian', 2, 'daily', N'200 Quốc Lộ 1A, Q.12'),
(N'Kho Trung Gian 3', 'trung_gian', 3, 'daily', N'300 Võ Văn Kiệt, Q.5'),
(N'Kho Lạnh 1', 'daily', 1, 'daily', N'111 Lê Văn Việt, Q.9'),
(N'Kho Lạnh 2', 'sieuthi', 1, 'sieuthi', N'222 Võ Văn Ngân, Thủ Đức'),
(N'Kho Lạnh 3', 'sieuthi', 2, 'sieuthi', N'333 Phạm Văn Đồng, Gò Vấp');

-- 10. TỒN KHO
INSERT INTO TonKho (MaKho, MaLo, SoLuong, NgayCapNhat) VALUES
(1, 1, 100, '2024-03-16'),
(1, 2, 50, '2024-04-21'),
(2, 3, 80, '2024-05-11'),
(2, 4, 30, '2024-06-06'),
(3, 5, 20, '2024-07-13'),
(4, 6, 200, '2024-08-21'),
(5, 7, 50, '2024-09-09'),
(6, 8, 50, '2024-10-15'),
(7, 9, 100, '2024-11-23'),
(8, 10, 50, '2024-12-19'),
(9, 1, 150, '2024-03-17'),
(9, 5, 20, '2024-07-14'),
(10, 6, 200, '2024-08-22'),
(10, 11, 20, '2025-01-11'),
(11, 12, 20, '2025-02-15'),
(11, 13, 30, '2025-03-21'),
(12, 14, 50, '2025-04-26'),
(12, 15, 50, '2025-05-31'),
(13, 16, 50, '2025-06-19'),
(13, 17, 20, '2025-07-23'),
(14, 18, 20, '2025-08-16'),
(14, 19, 50, '2025-09-11'),
(15, 20, 30, '2025-10-06'),
(16, 21, 20, '2025-11-13'),
(17, 22, 20, '2025-12-09'),
(18, 30, 50, '2024-08-13'),
(18, 31, 50, '2024-09-19'),
(19, 37, 100, '2025-03-19'),
(19, 38, 30, '2025-04-23'),
(20, 40, 30, '2025-06-21');


-- 11. VẬN CHUYỂN
INSERT INTO VanChuyen (MaLo, DiemDi, DiemDen, NgayBatDau, NgayKetThuc, TrangThai) VALUES
(1, N'Trang trại Xanh', N'Kho Đại lý Miền Nam', '2024-03-15 08:00', '2024-03-15 10:30', N'hoan_thanh'),
(2, N'Trang trại Xanh', N'Kho Đại lý Miền Nam', '2024-04-20 07:30', '2024-04-20 10:00', N'hoan_thanh'),
(3, N'Trang trại Hoa Sen', N'Kho Thực phẩm Sạch', '2024-05-10 08:00', '2024-05-10 11:00', N'hoan_thanh'),
(4, N'Trang trại Hoa Sen', N'Kho Thực phẩm Sạch', '2024-06-05 09:00', '2024-06-05 11:30', N'hoan_thanh'),
(5, N'Trang trại Minh Tâm', N'Kho Rau Củ Quả Tươi', '2024-07-12 07:00', '2024-07-12 09:30', N'hoan_thanh'),
(6, N'Trang trại Bình An', N'Kho Nông sản Xanh', '2024-08-20 08:30', '2024-08-20 11:00', N'hoan_thanh'),
(7, N'Trang trại Mai Xanh', N'Kho Thực phẩm An Toàn', '2024-09-08 07:30', '2024-09-08 10:00', N'hoan_thanh'),
(8, N'Trang trại Đức Phát', N'Kho Rau Sạch Organic', '2024-10-14 08:00', '2024-10-14 10:30', N'hoan_thanh'),
(9, N'Trang trại Lan Anh', N'Kho Nông sản Việt', '2024-11-22 09:00', '2024-11-22 11:30', N'hoan_thanh'),
(10, N'Trang trại Hùng Vương', N'Kho Thực phẩm Tươi Ngon', '2024-12-18 07:00', '2024-12-18 09:30', N'hoan_thanh'),
(11, N'Trang trại Nga Xanh', N'Kho Co.opmart', '2025-01-10 08:00', '2025-01-10 10:00', N'hoan_thanh'),
(12, N'Trang trại Sơn Thủy', N'Kho BigC', '2025-02-14 08:30', '2025-02-14 10:30', N'hoan_thanh'),
(13, N'Trang trại Hằng Nga', N'Kho Lotte Mart', '2025-03-20 07:30', '2025-03-20 09:30', N'hoan_thanh'),
(14, N'Trang trại Toàn Phát', N'Kho Aeon Mall', '2025-04-25 08:00', '2025-04-25 10:00', N'hoan_thanh'),
(15, N'Trang trại Xuân Mai', N'Kho Vinmart', '2025-05-30 09:00', '2025-05-30 11:00', N'hoan_thanh'),
(16, N'Trang trại Phúc Lộc', N'Kho Mega Market', '2025-06-18 07:00', '2025-06-18 09:00', N'hoan_thanh'),
(17, N'Trang trại Thảo Nguyên', N'Kho Trung Gian 1', '2025-07-22 08:00', NULL, N'dang_van_chuyen'),
(18, N'Trang trại Khoa Học', N'Kho Trung Gian 2', '2025-08-15 08:30', NULL, N'dang_van_chuyen'),
(19, N'Trang trại Diễm Phúc', N'Kho Trung Gian 3', '2025-09-10 07:30', NULL, N'dang_van_chuyen'),
(20, N'Trang trại Long Thành', N'Kho Lạnh 1', '2025-10-05 08:00', NULL, N'dang_van_chuyen'),
(30, N'Trang trại Lan Anh', N'Kho Lạnh 1', '2024-08-12 09:00', '2024-08-12 11:00', N'hoan_thanh'),
(31, N'Trang trại Hùng Vương', N'Kho Lạnh 1', '2024-09-18 08:00', '2024-09-18 10:00', N'hoan_thanh'),
(37, N'Trang trại Thảo Nguyên', N'Kho Lạnh 2', '2025-03-18 07:30', '2025-03-18 09:30', N'hoan_thanh'),
(38, N'Trang trại Khoa Học', N'Kho Lạnh 2', '2025-04-22 08:00', '2025-04-22 10:00', N'hoan_thanh'),
(40, N'Trang trại Long Thành', N'Kho Lạnh 3', '2025-06-20 09:00', '2025-06-20 11:00', N'hoan_thanh');


-- 12. ĐƠN HÀNG 
INSERT INTO DonHang (LoaiDon, MaNguoiBan, LoaiNguoiBan, MaNguoiMua, LoaiNguoiMua, NgayDat, TrangThai, TongGiaTri) VALUES
('nongdan_to_daily', 1, 'nongdan', 1, 'daily', '2024-03-15 07:00', N'hoan_thanh', 2500000),
('nongdan_to_daily', 1, 'nongdan', 1, 'daily', '2024-04-20 07:00', N'hoan_thanh', 1500000),
('nongdan_to_daily', 2, 'nongdan', 2, 'daily', '2024-05-10 07:30', N'hoan_thanh', 2400000),
('nongdan_to_daily', 2, 'nongdan', 2, 'daily', '2024-06-05 08:00', N'hoan_thanh', 900000),
('nongdan_to_daily', 3, 'nongdan', 3, 'daily', '2024-07-12 06:30', N'hoan_thanh', 960000),
('nongdan_to_daily', 4, 'nongdan', 4, 'daily', '2024-08-20 07:00', N'hoan_thanh', 16000000),
('nongdan_to_daily', 5, 'nongdan', 5, 'daily', '2024-09-08 06:30', N'hoan_thanh', 1250000),
('nongdan_to_daily', 6, 'nongdan', 6, 'daily', '2024-10-14 07:00', N'hoan_thanh', 1750000),
('nongdan_to_daily', 7, 'nongdan', 7, 'daily', '2024-11-22 07:30', N'hoan_thanh', 3000000),
('nongdan_to_daily', 8, 'nongdan', 8, 'daily', '2024-12-18 06:30', N'hoan_thanh', 1500000),
('nongdan_to_daily', 9, 'nongdan', 1, 'daily', '2025-01-10 07:00', N'hoan_thanh', 1800000),
('nongdan_to_daily', 10, 'nongdan', 2, 'daily', '2025-02-14 07:30', N'hoan_thanh', 1300000),
('nongdan_to_daily', 11, 'nongdan', 3, 'daily', '2025-03-20 06:30', N'hoan_thanh', 1620000),
('nongdan_to_daily', 12, 'nongdan', 4, 'daily', '2025-04-25 07:00', N'hoan_thanh', 2250000),
('nongdan_to_daily', 13, 'nongdan', 5, 'daily', '2025-05-30 07:30', N'hoan_thanh', 2000000),
('nongdan_to_daily', 14, 'nongdan', 6, 'daily', '2025-06-18 06:30', N'hoan_thanh', 1250000),
('nongdan_to_daily', 15, 'nongdan', 7, 'daily', '2025-07-22 07:00', N'cho_xac_nhan', 1800000),
('nongdan_to_daily', 16, 'nongdan', 8, 'daily', '2025-08-15 07:30', N'cho_xac_nhan', 1980000),
('nongdan_to_daily', 17, 'nongdan', 1, 'daily', '2025-09-10 06:30', N'cho_xac_nhan', 1750000),
('nongdan_to_daily', 18, 'nongdan', 2, 'daily', '2025-10-05 07:00', N'cho_xac_nhan', 1620000),
('daily_to_sieuthi', 1, 'daily', 1, 'sieuthi', '2024-03-16 09:00', N'hoan_thanh', 3750000),
('daily_to_sieuthi', 1, 'daily', 1, 'sieuthi', '2024-07-13 09:30', N'hoan_thanh', 960000),
('daily_to_sieuthi', 2, 'daily', 2, 'sieuthi', '2024-05-11 10:00', N'hoan_thanh', 3200000),
('daily_to_sieuthi', 2, 'daily', 2, 'sieuthi', '2024-06-06 09:30', N'hoan_thanh', 1080000),
('daily_to_sieuthi', 3, 'daily', 3, 'sieuthi', '2025-01-11 10:00', N'hoan_thanh', 2400000),
('daily_to_sieuthi', 4, 'daily', 4, 'sieuthi', '2024-08-21 09:00', N'hoan_thanh', 20000000),
('daily_to_sieuthi', 4, 'daily', 4, 'sieuthi', '2025-02-15 10:30', N'hoan_thanh', 1600000),
('daily_to_sieuthi', 5, 'daily', 5, 'sieuthi', '2025-03-21 09:00', N'hoan_thanh', 2160000),
('daily_to_sieuthi', 5, 'daily', 5, 'sieuthi', '2025-04-26 10:00', N'hoan_thanh', 2750000),
('daily_to_sieuthi', 6, 'daily', 6, 'sieuthi', '2025-05-31 09:30', N'hoan_thanh', 2500000),
('daily_to_sieuthi', 6, 'daily', 6, 'sieuthi', '2025-06-19 10:00', N'hoan_thanh', 1750000),
('daily_to_sieuthi', 7, 'daily', 1, 'sieuthi', '2025-07-23 09:00', N'cho_xac_nhan', 1800000),
('daily_to_sieuthi', 8, 'daily', 2, 'sieuthi', '2025-08-16 10:00', N'cho_xac_nhan', 1980000),
('daily_to_sieuthi', 1, 'daily', 3, 'sieuthi', '2025-09-11 09:30', N'cho_xac_nhan', 2250000),
('daily_to_sieuthi', 2, 'daily', 4, 'sieuthi', '2025-10-06 10:00', N'cho_xac_nhan', 2160000),
('daily_to_sieuthi', 1, 'daily', 2, 'sieuthi', '2024-08-13 09:00', N'hoan_thanh', 3750000),
('daily_to_sieuthi', 1, 'daily', 2, 'sieuthi', '2024-09-19 10:00', N'hoan_thanh', 4000000),
('daily_to_sieuthi', 2, 'daily', 2, 'sieuthi', '2025-03-19 09:30', N'hoan_thanh', 7500000),
('daily_to_sieuthi', 2, 'daily', 2, 'sieuthi', '2025-04-23 10:00', N'hoan_thanh', 2700000),
('daily_to_sieuthi', 3, 'daily', 3, 'sieuthi', '2025-06-21 09:00', N'hoan_thanh', 2400000);


-- 13. CHI TIẾT ĐƠN HÀNG 
INSERT INTO ChiTietDonHang (MaDonHang, MaLo, SoLuong, DonGia, ThanhTien) VALUES
(1, 1, 50, 25000, 1250000),
(1, 2, 50, 25000, 1250000),
(2, 2, 50, 30000, 1500000),
(3, 3, 80, 30000, 2400000),
(4, 4, 30, 30000, 900000),
(5, 5, 20, 48000, 960000),
(6, 6, 800, 20000, 16000000),
(7, 7, 50, 25000, 1250000),
(8, 8, 50, 35000, 1750000),
(9, 9, 100, 30000, 3000000),
(10, 10, 50, 30000, 1500000),
(11, 11, 20, 90000, 1800000),
(12, 12, 20, 65000, 1300000),
(13, 13, 30, 54000, 1620000),
(14, 14, 50, 45000, 2250000),
(15, 15, 50, 40000, 2000000),
(16, 16, 50, 25000, 1250000),
(17, 17, 20, 90000, 1800000),
(18, 18, 20, 99000, 1980000),
(19, 19, 50, 35000, 1750000),
(20, 20, 30, 54000, 1620000),
(21, 1, 100, 30000, 3000000),
(21, 2, 30, 25000, 750000),
(22, 5, 20, 48000, 960000),
(23, 3, 80, 40000, 3200000),
(24, 4, 30, 36000, 1080000),
(25, 11, 20, 120000, 2400000),
(26, 6, 800, 25000, 20000000),
(27, 12, 20, 80000, 1600000),
(28, 13, 30, 72000, 2160000),
(29, 14, 50, 55000, 2750000),
(30, 15, 50, 50000, 2500000),
(31, 16, 50, 35000, 1750000),
(32, 17, 20, 90000, 1800000),
(33, 18, 20, 99000, 1980000),
(34, 19, 50, 45000, 2250000),
(35, 20, 30, 72000, 2160000),
(36, 30, 50, 75000, 3750000),
(37, 31, 50, 80000, 4000000),
(38, 37, 100, 75000, 7500000),
(39, 38, 30, 90000, 2700000),
(40, 40, 30, 80000, 2400000),
(21, 5, 20, 48000, 960000),
(23, 4, 30, 36000, 1080000),
(26, 11, 20, 120000, 2400000),
(28, 12, 20, 80000, 1600000),
(29, 13, 30, 72000, 2160000),
(30, 14, 50, 55000, 2750000),
(31, 15, 50, 50000, 2500000),
(34, 20, 30, 72000, 2160000),
(38, 38, 30, 90000, 2700000);

-- 14. KIỂM ĐỊNH 
INSERT INTO KiemDinh (MaLo, NguoiKiemDinh, NgayKiemDinh, KetQua, BienBanKiemTra, ChuKySo) VALUES
(1, N'Trung tâm Kiểm định TP.HCM', '2024-03-15 06:00', 'dat', N'Sản phẩm đạt tiêu chuẩn VietGAP', 'SIGN001'),
(2, N'Trung tâm Kiểm định TP.HCM', '2024-04-20 06:00', 'dat', N'Sản phẩm đạt tiêu chuẩn an toàn', 'SIGN002'),
(3, N'Viện Kiểm nghiệm ATVS', '2024-05-10 06:00', 'dat', N'Sản phẩm organic đạt chuẩn', 'SIGN003'),
(4, N'Trung tâm Kiểm định TP.HCM', '2024-06-05 06:00', 'dat', N'Không phát hiện dư lượng thuốc BVTV', 'SIGN004'),
(5, N'Viện Kiểm nghiệm ATVS', '2024-07-12 06:00', 'dat', N'Đạt tiêu chuẩn VietGAP', 'SIGN005'),
(6, N'Trung tâm Kiểm định TP.HCM', '2024-08-20 06:00', 'dat', N'Gạo đạt tiêu chuẩn chất lượng', 'SIGN006'),
(7, N'Viện Kiểm nghiệm ATVS', '2024-09-08 06:00', 'dat', N'Rau sạch đạt chuẩn', 'SIGN007'),
(8, N'Trung tâm Kiểm định TP.HCM', '2024-10-14 06:00', 'dat', N'Không có kim loại nặng', 'SIGN008'),
(9, N'Viện Kiểm nghiệm ATVS', '2024-11-22 06:00', 'dat', N'Đạt tiêu chuẩn VietGAP', 'SIGN009'),
(10, N'Trung tâm Kiểm định TP.HCM', '2024-12-18 06:00', 'dat', N'Sản phẩm an toàn', 'SIGN010'),
(11, N'Viện Kiểm nghiệm ATVS', '2025-01-10 06:00', 'dat', N'Đạt chuẩn organic', 'SIGN011'),
(12, N'Trung tâm Kiểm định TP.HCM', '2025-02-14 06:00', 'dat', N'Không dư lượng thuốc BVTV', 'SIGN012'),
(13, N'Viện Kiểm nghiệm ATVS', '2025-03-20 06:00', 'dat', N'Đạt tiêu chuẩn VietGAP', 'SIGN013'),
(14, N'Trung tâm Kiểm định TP.HCM', '2025-04-25 06:00', 'dat', N'Sản phẩm đạt chuẩn', 'SIGN014'),
(15, N'Viện Kiểm nghiệm ATVS', '2025-05-30 06:00', 'dat', N'Đạt tiêu chuẩn an toàn', 'SIGN015'),
(16, N'Trung tâm Kiểm định TP.HCM', '2025-06-18 06:00', 'dat', N'Rau sạch đạt chuẩn', 'SIGN016'),
(17, N'Viện Kiểm nghiệm ATVS', '2025-07-22 06:00', 'khong_dat', N'Phát hiện dư lượng thuốc BVTV vượt ngưỡng', 'SIGN017'),
(18, N'Trung tâm Kiểm định TP.HCM', '2025-08-15 06:00', 'dat', N'Đạt tiêu chuẩn VietGAP', 'SIGN018'),
(19, N'Viện Kiểm nghiệm ATVS', '2025-09-10 06:00', 'dat', N'Sản phẩm an toàn', 'SIGN019'),
(20, N'Trung tâm Kiểm định TP.HCM', '2025-10-05 06:00', 'dat', N'Đạt chuẩn chất lượng', 'SIGN020'),
(30, N'Viện Kiểm nghiệm ATVS', '2024-08-12 06:00', 'dat', N'Đạt tiêu chuẩn VietGAP', 'SIGN030'),
(31, N'Trung tâm Kiểm định TP.HCM', '2024-09-18 06:00', 'dat', N'Sản phẩm an toàn', 'SIGN031'),
(37, N'Viện Kiểm nghiệm ATVS', '2025-03-18 06:00', 'dat', N'Đạt chuẩn organic', 'SIGN037'),
(38, N'Trung tâm Kiểm định TP.HCM', '2025-04-22 06:00', 'dat', N'Không dư lượng thuốc BVTV', 'SIGN038'),
(40, N'Viện Kiểm nghiệm ATVS', '2025-06-20 06:00', 'dat', N'Đạt tiêu chuẩn VietGAP', 'SIGN040'),
(25, N'Trung tâm Kiểm định TP.HCM', '2026-03-10 06:00', 'dat', N'Sản phẩm đạt chuẩn', 'SIGN025'),
(26, N'Viện Kiểm nghiệm ATVS', '2024-04-15 06:00', 'dat', N'Đạt tiêu chuẩn an toàn', 'SIGN026'),
(27, N'Trung tâm Kiểm định TP.HCM', '2024-05-20 06:00', 'dat', N'Rau thơm đạt chuẩn', 'SIGN027'),
(28, N'Viện Kiểm nghiệm ATVS', '2024-06-25 06:00', 'khong_dat', N'Phát hiện vi sinh vật vượt ngưỡng', 'SIGN028'),
(29, N'Trung tâm Kiểm định TP.HCM', '2024-07-30 06:00', 'dat', N'Đạt tiêu chuẩn VietGAP', 'SIGN029');

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
(1, 'nongdan', 1, 'daily', N'Vâng, tôi sẽ giao hàng vào sáng mai', SYSDATETIME()),
(1, 'daily', 1, 'sieuthi', N'Đơn hàng đã được xác nhận', SYSDATETIME()),
(2, 'nongdan', 2, 'daily', N'Cảm ơn bạn đã đặt hàng', SYSDATETIME());

INSERT INTO TinNhan (MaCuocTroChuyen, MaNguoiGui, LoaiNguoiGui, NoiDung, DaDoc, NgayGui) VALUES
-- Cuộc trò chuyện 1: Nông dân 1 - Đại lý 1
(1, 1, 'daily', N'Chào bạn, tôi muốn đặt 100kg cà chua', 1, DATEADD(HOUR, -2, SYSDATETIME())),
(1, 1, 'nongdan', N'Chào bạn, hiện tại tôi có sẵn hàng. Giá 25.000đ/kg', 1, DATEADD(HOUR, -1, SYSDATETIME())),
(1, 1, 'daily', N'Được, khi nào có thể giao hàng?', 1, DATEADD(MINUTE, -30, SYSDATETIME())),
(1, 1, 'nongdan', N'Vâng, tôi sẽ giao hàng vào sáng mai', 0, DATEADD(MINUTE, -5, SYSDATETIME())),

-- Cuộc trò chuyện 2: Đại lý 1 - Siêu thị 1
(2, 1, 'sieuthi', N'Tôi cần đặt 200kg rau muống', 1, DATEADD(HOUR, -3, SYSDATETIME())),
(2, 1, 'daily', N'Dạ, hiện tại shop có sẵn. Giá 30.000đ/kg', 1, DATEADD(HOUR, -2, SYSDATETIME())),
(2, 1, 'sieuthi', N'OK, tôi đặt luôn nhé', 1, DATEADD(HOUR, -1, SYSDATETIME())),
(2, 1, 'daily', N'Đơn hàng đã được xác nhận', 0, DATEADD(MINUTE, -10, SYSDATETIME())),

-- Cuộc trò chuyện 3: Nông dân 2 - Đại lý 2
(3, 2, 'daily', N'Bạn có bí xanh không?', 1, DATEADD(HOUR, -1, SYSDATETIME())),
(3, 2, 'nongdan', N'Có bạn, tôi có 50kg', 1, DATEADD(MINUTE, -30, SYSDATETIME())),
(3, 2, 'daily', N'Cảm ơn bạn đã đặt hàng', 0, DATEADD(MINUTE, -2, SYSDATETIME()));

PRINT 'Đã thêm dữ liệu mẫu cho chức năng chat!';
GO
