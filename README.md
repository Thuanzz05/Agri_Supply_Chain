Frontend: https://github.com/Thuanzz05/Agri_Supply_Chain_UI

# Hệ thống Quản lý Nông sản & Chuỗi cung ứng (Agri Supply Chain)

## Mô tả bài toán
Hệ thống quản lý toàn diện chuỗi cung ứng nông sản từ trang trại đến siêu thị:

- **Nông dân**: Quản lý trang trại, đăng ký lô nông sản (loại sản phẩm, nguồn gốc, ngày thu hoạch, chứng nhận), tạo mã định danh để truy xuất nguồn gốc.
- **Đại lý**: Quản lý kho trung gian và số lượng tồn kho, theo dõi quá trình vận chuyển và lưu trữ của từng lô hàng, thực hiện kiểm định chất lượng nông sản và lưu biên bản kiểm tra.
- **Siêu thị**: Xem thông tin lô hàng từ đại lý trước khi nhập hàng, bán hàng cho khách hàng, cung cấp thông tin truy xuất nguồn gốc.
- **Admin**: Quản trị hệ thống, tạo báo cáo sản lượng và tồn kho, cảnh báo lô nông sản sắp hết hạn hoặc quá hạn.

**Quy trình chuỗi cung ứng**: Trang trại Nông dân → Kho trung gian → Đại lý → Siêu thị → Khách hàng


## Các vai trò

- **Admin**: Quản trị người dùng (Nông dân, Đại lý, Siêu thị), báo cáo tổng hợp
- **Nông dân**: Quản lý trang trại, đăng ký lô nông sản, sản phẩm
- **Đại lý**: Vận chuyển, kho trung gian, kiểm định chất lượng
- **Siêu thị**: Bán hàng, cung cấp thông tin truy xuất

## Cấu trúc dự án

```
Agri_Supply_Chain/
├── NongDanService/       # API quản lý nông dân, trang trại, lô nông sản
├── DaiLyService/         # API quản lý đại lý, kho, vận chuyển, tồn kho
├── SieuThiService/       # API quản lý siêu thị, bán hàng
├── AdminService/         # API admin, quản lý người dùng, thống kê
├── Gateway/              # API Gateway, authentication
└── database.sql          # Database schema
```

