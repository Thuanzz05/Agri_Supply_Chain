# Hệ thống Quản lý Nông sản & Chuỗi cung ứng (Agri Supply Chain)

## Mô tả bài toán

Xây dựng hệ thống quản lý toàn diện chuỗi cung ứng nông sản từ trang trại đến siêu thị, đảm bảo truy xuất nguồn gốc và kiểm soát chất lượng.

## Các vai trò

- **Admin**: Quản trị người dùng (Nông dân, Đại lý, Siêu thị), báo cáo tổng hợp
- **Nông dân**: Quản lý trang trại, đăng ký lô nông sản, sản phẩm
- **Đại lý**: Vận chuyển, kho trung gian, kiểm định chất lượng
- **Siêu thị**: Bán hàng, cung cấp thông tin truy xuất

## Chức năng chính

1. **Đăng ký lô nông sản** - Nguồn gốc, chứng nhận, mã QR
2. **Vận chuyển & Kho** - Theo dõi di chuyển, quản lý tồn kho
3. **Truy xuất nguồn gốc** - Quét QR code xem lịch sử sản phẩm
4. **Kiểm định chất lượng** - Biên bản kiểm tra có chữ ký số
5. **Báo cáo thống kê** - Sản lượng, tồn kho, doanh thu
6. **Cảnh báo hết hạn** - Thông báo tự động lô sắp/quá hạn

## Kiến trúc hệ thống

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   React App     │    │   API Gateway   │    │  Microservices  │
│   (Frontend)    │◄──►│  Authentication │◄──►│                 │
└─────────────────┘    └─────────────────┘    │ • NongDanService│
                                              │ • DaiLyService  │
                                              │ • SieuThiService│
                                              │ • AdminService  │
                                              └─────────────────┘
```

## Công nghệ sử dụng

### Backend
- **.NET 8.0** - Framework chính
- **SQL Server** - Database
- **JWT** - Authentication
- **Swagger** - API Documentation

### Frontend
- **React** - UI Framework 


## Cấu trúc dự án

```
Agri_Supply_Chain/
├── NongDanService/       # API quản lý nông dân, trang trại, lô nông sản
├── DaiLyService/         # API quản lý đại lý, kho, vận chuyển, tồn kho
├── SieuThiService/       # API quản lý siêu thị, bán hàng
├── AdminService/         # API quản lý admin, người dùng, thống kê
├── Gateway/              # API Gateway, authentication
└── database.sql          # Database schema
```
