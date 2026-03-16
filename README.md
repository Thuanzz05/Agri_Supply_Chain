# Hệ thống Quản lý Nông sản & Chuỗi cung ứng (Agri Supply Chain)

## Mô tả bài toán

Xây dựng hệ thống quản lý toàn diện chuỗi cung ứng nông sản từ trang trại đến siêu thị, đảm bảo truy xuất nguồn gốc và kiểm soát chất lượng trong toàn bộ quy trình. Hệ thống giải quyết bài toán minh bạch thông tin, theo dõi chất lượng sản phẩm nông nghiệp và tạo niềm tin cho người tiêu dùng thông qua việc cung cấp thông tin chi tiết về nguồn gốc, quy trình sản xuất, vận chuyển và bảo quản của từng lô nông sản.

Với sự phát triển của công nghệ và nhu cầu ngày càng cao về an toàn thực phẩm, hệ thống này sẽ kết nối tất cả các bên tham gia trong chuỗi cung ứng - từ nông dân sản xuất, đại lý phân phối, đến siêu thị bán lẻ - tạo thành một mạng lưới thông tin minh bạch và đáng tin cậy. Hệ thống cung cấp giao diện web thân thiện cho phép các bên dễ dàng quản lý và theo dõi toàn bộ quy trình từ sản xuất đến tiêu thụ.


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
