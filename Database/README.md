# Hướng dẫn cài đặt Database - SupermarketApp

## 📋 Yêu cầu
- SQL Server 2019 trở lên (hoặc SQL Server Express)
- Visual Studio 2022
- .NET Framework 4.7.2+

## 🚀 Cách cài đặt Database

### Bước 1: Mở SQL Server Management Studio (SSMS)

### Bước 2: Chạy script tạo database
1. Mở file `CreateDatabase.sql` trong thư mục `Database/`
2. Kết nối tới SQL Server của bạn
3. Nhấn **Execute** (hoặc F5) để chạy script

Script sẽ tự động:
- ✅ Tạo database `SupermarketDB`
- ✅ Tạo tất cả các bảng: NHANVIEN, KHACHHANG, SANPHAM, HOADON, CTHOADON
- ✅ Thêm dữ liệu mẫu

### Bước 3: Kiểm tra database
```sql
USE SupermarketDB;
GO

-- Kiểm tra các bảng đã tạo
SELECT * FROM NHANVIEN;
SELECT * FROM KHACHHANG;
SELECT * FROM SANPHAM;
```

## 🔧 Cấu hình Connection String

### Tìm tên SQL Server của bạn
Chạy câu lệnh sau trong SSMS:
```sql
SELECT @@SERVERNAME;
```

### Cập nhật file `App.config`
Mở file `App.config` và sửa connection string:

```xml
<connectionStrings>
  <add name="SupermarketDB" 
       connectionString="Server=TÊN_SERVER_CỦA_BẠN;Database=SupermarketDB;Trusted_Connection=True;TrustServerCertificate=True" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

**Ví dụ:**
- Nếu server của bạn là `DESKTOP-ABC123\SQLEXPRESS`:
  ```xml
  connectionString="Server=DESKTOP-ABC123\SQLEXPRESS;Database=SupermarketDB;Trusted_Connection=True;TrustServerCertificate=True"
  ```

- Nếu bạn dùng SQL Server authentication:
  ```xml
  connectionString="Server=TÊN_SERVER;Database=SupermarketDB;User Id=sa;Password=MẬT_KHẨU;TrustServerCertificate=True"
  ```

## 👤 Tài khoản mặc định

Sau khi chạy script, bạn có thể đăng nhập với:

### 🔑 Tài khoản Admin
- **Username:** `admin`
- **Password:** `admin123`
- **Vai trò:** Quản trị viên

### 🔑 Tài khoản Nhân viên
- **Username:** `nhanvien1`
- **Password:** `nv123`
- **Vai trò:** Nhân viên

## 📊 Cấu trúc Database

### Bảng NHANVIEN
```
MaNV          INT (PK, IDENTITY)
TenNV         NVARCHAR(100)
TaiKhoan      NVARCHAR(50) UNIQUE
MatKhauHash   VARBINARY(MAX)
Salt          VARBINARY(MAX)
VaiTro        NVARCHAR(20) - 'Admin' hoặc 'NV'
TrangThai     BIT - 1: Active, 0: Inactive
NgayTao       DATETIME
```

### Bảng KHACHHANG
```
MaKH          INT (PK, IDENTITY)
TenKH         NVARCHAR(100)
SoDienThoai   NVARCHAR(15)
Email         NVARCHAR(100)
DiaChi        NVARCHAR(200)
DiemTichLuy   INT DEFAULT 0
NgayTao       DATETIME
```

### Bảng SANPHAM
```
MaSP          INT (PK, IDENTITY)
TenSP         NVARCHAR(200)
DonGia        DECIMAL(18,2)
SoLuongTon    INT
DonVi         NVARCHAR(20)
MoTa          NVARCHAR(500)
NgayTao       DATETIME
```

### Bảng HOADON
```
MaHD          INT (PK, IDENTITY)
MaKH          INT (FK -> KHACHHANG)
MaNV          INT (FK -> NHANVIEN)
NgayLap       DATETIME
TongTien      DECIMAL(18,2)
GhiChu        NVARCHAR(200)
```

### Bảng CTHOADON (Chi tiết hóa đơn)
```
MaHD          INT (PK, FK -> HOADON)
MaSP          INT (PK, FK -> SANPHAM)
SoLuong       INT
DonGiaBan     DECIMAL(18,2)
ThanhTien     COMPUTED (SoLuong * DonGiaBan)
```

## 🛠️ Troubleshooting

### Lỗi: Cannot open database "SupermarketDB"
➡️ **Giải pháp:** Chạy lại script `CreateDatabase.sql`

### Lỗi: Login failed for user
➡️ **Giải pháp:** 
1. Kiểm tra Connection String trong `App.config`
2. Đảm bảo SQL Server đang chạy
3. Kiểm tra Windows Authentication hoặc SQL Authentication

### Lỗi: A network-related or instance-specific error
➡️ **Giải pháp:**
1. Bật SQL Server Browser service
2. Enable TCP/IP trong SQL Server Configuration Manager
3. Kiểm tra firewall

## 📝 Ghi chú

- Mật khẩu được mã hóa bằng **SHA256**
- Database sử dụng **Entity Framework Core 3.1**
- Hỗ trợ cả **Windows Authentication** và **SQL Server Authentication**

## 💡 Tips

1. **Backup database thường xuyên:**
   ```sql
   BACKUP DATABASE SupermarketDB 
   TO DISK = 'C:\Backup\SupermarketDB.bak'
   ```

2. **Reset dữ liệu về ban đầu:**
   - Chạy lại file `CreateDatabase.sql` (script sẽ tự động xóa và tạo lại database)

3. **Thêm tài khoản mới:**
   - Sử dụng chức năng Đăng ký trong ứng dụng
   - Hoặc insert trực tiếp vào database (nhớ hash password)

## 🎯 Chức năng chính của ứng dụng

✅ Đăng nhập / Đăng ký với mã hóa mật khẩu  
✅ Quản lý sản phẩm (thêm, sửa, xóa, tìm kiếm)  
✅ Quản lý khách hàng  
✅ Bán hàng và tạo hóa đơn  
✅ Thống kê doanh thu  
✅ Giao diện đẹp với SunnyUI  

---

**Phát triển bởi:** SupermarketApp Team  
**Ngày cập nhật:** 2025

