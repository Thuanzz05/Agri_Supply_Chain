# Web Agri Supply Chain

Hệ thống quản lý chuỗi cung ứng nông sản với kiến trúc Microservices

## Kiến trúc hệ thống

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   React App     │    │   API Gateway   │    │  Microservices  │
│   (Frontend)    │◄──►│    (Ocelot)     │◄──►│                 │
└─────────────────┘    └─────────────────┘    │ • AuthService   │
                                              │ • NongDanService│
                                              │ • DaiLyService  │
                                              │ • SieuThiService│
                                              │ • AdminService  │
                                              └─────────────────┘


## Công nghệ sử dụng

### Backend
- **.NET 8.0** - Framework chính
- **Ocelot** - API Gateway
- **SQL Server** - Database

### Frontend
- **React** - UI Framework 

## Cấu trúc dự án
CNWeb_Agri_Supply_Chain/
├── AuthService/          # Xác thực và JWT
├── NongDanService/       # Quản lý nông dân
├── DaiLyService/         # Quản lý đại lý
├── SieuThiService/       # Quản lý siêu thị
├── AdminService/         # Quản lý admin
├── Gateway/              # API Gateway (Ocelot)
└── GiaoDien/            # Frontend (React)
