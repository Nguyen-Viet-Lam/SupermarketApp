# 🏪 SupermarketApp - Ứng dụng Quản lý Bán hàng Siêu thị

<div align="center">
  <img src="https://img.shields.io/badge/.NET_Framework-4.7.2-512BD4?style=for-the-badge&logo=.net" alt=".NET Framework">
  <img src="https://img.shields.io/badge/SQL_Server-2019-CC2927?style=for-the-badge&logo=microsoft-sql-server" alt="SQL Server">
  <img src="https://img.shields.io/badge/SunnyUI-3.8.9-FF6B6B?style=for-the-badge" alt="SunnyUI">
  <img src="https://img.shields.io/badge/EF_Core-3.1-512BD4?style=for-the-badge" alt="Entity Framework">
</div>

## 📖 Giới thiệu

**SupermarketApp** là ứng dụng quản lý bán hàng siêu thị với giao diện hiện đại, được xây dựng bằng:
- **C# Windows Forms** với thư viện **SunnyUI** 
- **Entity Framework Core** để quản lý database
- **SQL Server** làm hệ quản trị cơ sở dữ liệu
- Mã hóa mật khẩu bằng **SHA256**

## ✨ Tính năng chính

### 🔐 Xác thực & Bảo mật
- ✅ Đăng nhập với mã hóa mật khẩu SHA256
- ✅ Đăng ký tài khoản mới
- ✅ Phân quyền Admin/Nhân viên
- ✅ Quản lý trạng thái tài khoản (Active/Inactive)

### 📦 Quản lý Sản phẩm
- ✅ Thêm, sửa, xóa sản phẩm
- ✅ Quản lý tồn kho
- ✅ Tìm kiếm sản phẩm
- ✅ Hiển thị giá, đơn vị tính

### 👥 Quản lý Khách hàng
- ✅ Thông tin khách hàng (tên, SĐT, email, địa chỉ)
- ✅ Hệ thống điểm tích lũy
- ✅ Lịch sử mua hàng

### 🛒 Bán hàng
- ✅ Tạo hóa đơn mới
- ✅ Thêm nhiều sản phẩm vào đơn hàng
- ✅ Tính tổng tiền tự động
- ✅ Liên kết với khách hàng

### 📊 Thống kê & Báo cáo
- ✅ Doanh thu theo thời gian
- ✅ Sản phẩm bán chạy
- ✅ Báo cáo tồn kho

## 🎨 Giao diện

Ứng dụng sử dụng **SunnyUI 3.8.9** với thiết kế:
- 🎨 Màu sắc hiện đại, chuyên nghiệp
- 🖱️ Thân thiện với người dùng
- 📱 Layout responsive
- 🌈 Màu chủ đạo: Đỏ (#DC2626) & Xanh đen (#1F2937)

## 🚀 Cài đặt & Sử dụng

### Yêu cầu hệ thống

- **Windows 10/11**
- **Visual Studio 2022** (Community trở lên)
- **SQL Server 2019** hoặc **SQL Server Express**
- **.NET Framework 4.7.2+**

### Bước 1: Clone project

```bash
git clone https://github.com/your-username/SupermarketApp.git
cd SupermarketApp
```

### Bước 2: Cài đặt Database

1. Mở **SQL Server Management Studio (SSMS)**
2. Chạy script `Database/CreateDatabase.sql`
3. Database `SupermarketDB` sẽ được tạo tự động với dữ liệu mẫu

📖 **Chi tiết:** Xem file [`Database/README.md`](Database/README.md)

### Bước 3: Cấu hình Connection String

Mở file `App.config` và cập nhật connection string:

```xml
<connectionStrings>
  <add name="SupermarketDB" 
       connectionString="Server=YOUR_SERVER_NAME;Database=SupermarketDB;Trusted_Connection=True;TrustServerCertificate=True" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

**Ví dụ:**
```xml
connectionString="Server=DESKTOP-ABC123\SQLEXPRESS;Database=SupermarketDB;Trusted_Connection=True;TrustServerCertificate=True"
```

### Bước 4: Restore NuGet Packages

Trong Visual Studio:
1. Right-click vào Solution → **Restore NuGet Packages**
2. Hoặc chạy: `dotnet restore` trong terminal

### Bước 5: Build & Run

1. Nhấn **F5** hoặc click **Start** trong Visual Studio
2. Ứng dụng sẽ mở màn hình đăng nhập

### Bước 6: Đăng nhập

**Tài khoản Admin mặc định:**
- Username: `admin`
- Password: `admin123`

**Tài khoản Nhân viên:**
- Username: `nhanvien1`
- Password: `nv123`

## 📁 Cấu trúc Project

```
SupermarketApp/
├── Data/
│   ├── Models/              # Entity models
│   │   ├── NhanVien.cs
│   │   ├── KhachHang.cs
│   │   ├── SanPham.cs
│   │   ├── HoaDon.cs
│   │   └── CTHoaDon.cs
│   └── SupermarketContext.cs # DbContext
├── Forms/
│   ├── LoginForm.cs         # Form đăng nhập
│   ├── RegisterForm.cs      # Form đăng ký
│   ├── MainForm.cs          # Form chính
│   ├── ProductForm.cs       # Quản lý sản phẩm
│   ├── CustomerForm.cs      # Quản lý khách hàng
│   ├── InvoiceForm.cs       # Bán hàng
│   └── ReportForm.cs        # Thống kê
├── Services/
│   ├── AuthService.cs       # Xác thực
│   ├── ProductService.cs    # Nghiệp vụ sản phẩm
│   └── InvoiceService.cs    # Nghiệp vụ hóa đơn
├── Database/
│   ├── CreateDatabase.sql   # Script tạo DB
│   └── README.md            # Hướng dẫn DB
├── App.config               # Cấu hình
├── Program.cs               # Entry point
└── README.md                # File này
```

## 🗄️ Database Schema

### Bảng NHANVIEN (Nhân viên)
| Trường | Kiểu | Mô tả |
|--------|------|-------|
| MaNV | INT (PK) | Mã nhân viên |
| TenNV | NVARCHAR(100) | Họ tên |
| TaiKhoan | NVARCHAR(50) | Tên đăng nhập (UNIQUE) |
| MatKhauHash | VARBINARY(MAX) | Mật khẩu đã mã hóa |
| VaiTro | NVARCHAR(20) | 'Admin' hoặc 'NV' |
| TrangThai | BIT | 1: Active, 0: Inactive |

### Bảng SANPHAM (Sản phẩm)
| Trường | Kiểu | Mô tả |
|--------|------|-------|
| MaSP | INT (PK) | Mã sản phẩm |
| TenSP | NVARCHAR(200) | Tên sản phẩm |
| DonGia | DECIMAL(18,2) | Đơn giá |
| SoLuongTon | INT | Số lượng tồn kho |
| DonVi | NVARCHAR(20) | Đơn vị tính |

### Bảng KHACHHANG (Khách hàng)
| Trường | Kiểu | Mô tả |
|--------|------|-------|
| MaKH | INT (PK) | Mã khách hàng |
| TenKH | NVARCHAR(100) | Họ tên |
| SoDienThoai | NVARCHAR(15) | Số điện thoại |
| DiemTichLuy | INT | Điểm tích lũy |

### Bảng HOADON & CTHOADON
Chi tiết xem tại [`Database/README.md`](Database/README.md)

## 🔧 Công nghệ sử dụng

| Công nghệ | Phiên bản | Mục đích |
|-----------|-----------|----------|
| .NET Framework | 4.7.2 | Framework chính |
| C# | 8.0 | Ngôn ngữ lập trình |
| SunnyUI | 3.8.9 | Thư viện UI |
| Entity Framework Core | 3.1.32 | ORM |
| SQL Server | 2019+ | Database |
| Microsoft.Data.SqlClient | 1.1.3 | SQL connector |

## 📸 Screenshots

### Màn hình Đăng nhập
![Login](https://via.placeholder.com/800x500?text=Login+Form)

### Màn hình Chính
![Main](https://via.placeholder.com/800x500?text=Main+Dashboard)

### Quản lý Sản phẩm
![Products](https://via.placeholder.com/800x500?text=Product+Management)

## 🐛 Troubleshooting

### Lỗi kết nối database
**Vấn đề:** `Cannot open database "SupermarketDB"`

**Giải pháp:**
1. Kiểm tra SQL Server đang chạy
2. Chạy lại script `CreateDatabase.sql`
3. Kiểm tra Connection String trong `App.config`

### Lỗi NuGet packages
**Vấn đề:** Missing packages

**Giải pháp:**
```bash
# Trong Package Manager Console
Update-Package -reinstall
```

### Lỗi Entity Framework
**Vấn đề:** Migration issues

**Giải pháp:**
```bash
# Xóa và tạo lại database
# Chạy lại CreateDatabase.sql
```

## 📝 Lưu ý

- ⚠️ **Không** commit file `App.config` với connection string thật lên Git
- 🔒 Mật khẩu được mã hóa bằng SHA256 (one-way hash)
- 💾 Nên backup database thường xuyên
- 🔑 Đổi mật khẩu admin ngay khi triển khai thực tế

## 🤝 Đóng góp

Mọi đóng góp đều được chào đón! Hãy:
1. Fork project
2. Tạo branch mới (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Mở Pull Request

## 📄 License

Dự án này được phát hành dưới giấy phép **MIT License**.

## 👨‍💻 Tác giả

**SupermarketApp Development Team**

- GitHub: [@your-username](https://github.com/your-username)
- Email: your.email@example.com

## 🌟 Tính năng sắp tới

- [ ] Xuất hóa đơn PDF
- [ ] Gửi email cho khách hàng
- [ ] Dashboard với biểu đồ
- [ ] Quản lý nhập hàng
- [ ] Quản lý nhà cung cấp
- [ ] Báo cáo Excel
- [ ] Multi-language support

---

<div align="center">
  <p>⭐ Nếu thấy project hữu ích, hãy cho một star nhé! ⭐</p>
  <p>Made with ❤️ by SupermarketApp Team</p>
</div>

