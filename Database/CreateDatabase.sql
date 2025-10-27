-- =============================================
-- Script tạo database SupermarketDB
-- Quản lý bán hàng siêu thị
-- =============================================

USE master;
GO

-- Xóa database nếu đã tồn tại (cẩn thận khi chạy!)
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'SupermarketDB')
BEGIN
    ALTER DATABASE SupermarketDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE SupermarketDB;
END
GO

-- Tạo database mới
CREATE DATABASE SupermarketDB;
GO

USE SupermarketDB;
GO

-- =============================================
-- Bảng NHANVIEN - Quản lý nhân viên và tài khoản
-- =============================================
CREATE TABLE NHANVIEN (
    MaNV INT PRIMARY KEY IDENTITY(1,1),
    TenNV NVARCHAR(100) NOT NULL,
    TaiKhoan NVARCHAR(50) NOT NULL UNIQUE,
    MatKhauHash VARBINARY(MAX) NOT NULL,
    Salt VARBINARY(MAX) NOT NULL,
    VaiTro NVARCHAR(20) NOT NULL DEFAULT 'NV', -- 'Admin' hoặc 'NV'
    TrangThai BIT NOT NULL DEFAULT 1, -- 1: Active, 0: Inactive
    NgayTao DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- =============================================
-- Bảng KHACHHANG - Quản lý khách hàng
-- =============================================
CREATE TABLE KHACHHANG (
    MaKH INT PRIMARY KEY IDENTITY(1,1),
    TenKH NVARCHAR(100) NOT NULL,
    SoDienThoai NVARCHAR(15),
    Email NVARCHAR(100),
    DiaChi NVARCHAR(200),
    DiemTichLuy INT DEFAULT 0,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- =============================================
-- Bảng SANPHAM - Quản lý sản phẩm
-- =============================================
CREATE TABLE SANPHAM (
    MaSP INT PRIMARY KEY IDENTITY(1,1),
    TenSP NVARCHAR(200) NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL,
    SoLuong INT NOT NULL DEFAULT 0,
    DonVi NVARCHAR(20),
    LoaiSP NVARCHAR(100),
    Barcode NVARCHAR(50),
    MoTa NVARCHAR(500),
    TrangThai BIT NOT NULL DEFAULT 1,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- =============================================
-- Bảng HOADON - Quản lý hóa đơn
-- =============================================
CREATE TABLE HOADON (
    MaHD INT PRIMARY KEY IDENTITY(1,1),
    MaKH INT,
    MaNV INT NOT NULL,
    NgayLap DATETIME NOT NULL DEFAULT GETDATE(),
    TongTien DECIMAL(18,2) NOT NULL,
    GhiChu NVARCHAR(200),
    FOREIGN KEY (MaKH) REFERENCES KHACHHANG(MaKH),
    FOREIGN KEY (MaNV) REFERENCES NHANVIEN(MaNV)
);
GO

-- =============================================
-- Bảng CTHOADON - Chi tiết hóa đơn
-- =============================================
CREATE TABLE CTHOADON (
    MaHD INT NOT NULL,
    MaSP INT NOT NULL,
    SoLuong INT NOT NULL,
    DonGiaBan DECIMAL(18,2) NOT NULL,
    ThanhTien AS (SoLuong * DonGiaBan) PERSISTED,
    PRIMARY KEY (MaHD, MaSP),
    FOREIGN KEY (MaHD) REFERENCES HOADON(MaHD),
    FOREIGN KEY (MaSP) REFERENCES SANPHAM(MaSP)
);
GO

-- =============================================
-- Bảng NHACUNGCAP - Quản lý nhà cung cấp
-- =============================================
CREATE TABLE NHACUNGCAP (
    MaNCC INT PRIMARY KEY IDENTITY(1,1),
    TenNCC NVARCHAR(200) NOT NULL,
    DiaChi NVARCHAR(500),
    SDT NVARCHAR(20),
    Email NVARCHAR(100),
    MaSoThue NVARCHAR(50),
    NguoiLienHe NVARCHAR(200),
    TrangThai BIT NOT NULL DEFAULT 1,
    GhiChu NVARCHAR(500),
    NgayTao DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- =============================================
-- Bảng PHIEUNHAP - Phiếu nhập hàng
-- =============================================
CREATE TABLE PHIEUNHAP (
    MaPN INT PRIMARY KEY IDENTITY(1,1),
    NgayNhap DATETIME NOT NULL DEFAULT GETDATE(),
    MaNCC INT NOT NULL,
    MaNV INT NOT NULL,
    TongTien DECIMAL(18,2) NOT NULL,
    GhiChu NVARCHAR(500),
    TrangThai NVARCHAR(50) NOT NULL DEFAULT N'Chờ duyệt',
    FOREIGN KEY (MaNCC) REFERENCES NHACUNGCAP(MaNCC),
    FOREIGN KEY (MaNV) REFERENCES NHANVIEN(MaNV)
);
GO

-- =============================================
-- Bảng CTPHIEUNHAP - Chi tiết phiếu nhập
-- =============================================
CREATE TABLE CTPHIEUNHAP (
    MaPN INT NOT NULL,
    MaSP INT NOT NULL,
    SoLuong INT NOT NULL,
    DonGiaNhap DECIMAL(18,2) NOT NULL,
    ThanhTien AS (SoLuong * DonGiaNhap) PERSISTED,
    PRIMARY KEY (MaPN, MaSP),
    FOREIGN KEY (MaPN) REFERENCES PHIEUNHAP(MaPN),
    FOREIGN KEY (MaSP) REFERENCES SANPHAM(MaSP)
);
GO

-- =============================================
-- Bảng CAIDAT - Cấu hình ứng dụng
-- =============================================
CREATE TABLE CAIDAT (
    MaCaiDat INT PRIMARY KEY IDENTITY(1,1),
    TenCaiDat NVARCHAR(100) NOT NULL UNIQUE,
    GiaTri NVARCHAR(MAX),
    MoTa NVARCHAR(500),
    NgayCapNhat DATETIME NOT NULL DEFAULT GETDATE(),
    NguoiCapNhat NVARCHAR(100)
);
GO

-- =============================================
-- Dữ liệu mẫu
-- =============================================

-- NHANVIEN (Nhân viên) sẽ được tự động tạo bởi DataSeederUpdate.cs khi ứng dụng khởi động
-- Admin account sẽ được tạo với PBKDF2 hash + Salt
-- Để tránh lỗi: NHANVIEN table vẫn yêu cầu MatKhauHash và Salt NOT NULL

-- Thêm khách hàng mẫu
INSERT INTO KHACHHANG (TenKH, SoDienThoai, Email, DiaChi, DiemTichLuy)
VALUES 
    (N'Trần Thị B', '0901234567', 'tranb@email.com', N'123 Nguyễn Huệ, Q1, TP.HCM', 0),
    (N'Lê Văn C', '0912345678', 'lec@email.com', N'456 Lê Lợi, Q1, TP.HCM', 0),
    (N'Phạm Thị D', '0923456789', 'phamd@email.com', N'789 Trần Hưng Đạo, Q5, TP.HCM', 0);

-- Thêm sản phẩm mẫu
INSERT INTO SANPHAM (TenSP, DonGia, SoLuong, DonVi, MoTa, TrangThai)
VALUES 
    (N'Gạo ST25 5kg', 120000, 100, N'túi', N'Gạo đặc sản chất lượng cao', 1),
    (N'Dầu ăn Simply 1L', 45000, 200, N'chai', N'Dầu ăn tinh luyện', 1),
    (N'Sữa tươi Vinamilk 1L', 32000, 150, N'hộp', N'Sữa tươi tiệt trùng', 1),
    (N'Mì gói Hảo Hảo (Thùng 30 gói)', 90000, 80, N'thùng', N'Mì ăn liền vị chua cay', 1),
    (N'Nước ngọt Coca Cola 1.5L', 18000, 300, N'chai', N'Nước giải khát có gas', 1),
    (N'Bánh mì sandwich', 25000, 50, N'ổ', N'Bánh mì tươi mỗi ngày', 1),
    (N'Trứng gà (Vỉ 10 quả)', 35000, 120, N'vỉ', N'Trứng gà tươi sạch', 1),
    (N'Thịt heo ba chỉ', 150000, 40, N'kg', N'Thịt heo tươi sạch', 1),
    (N'Cà phê G7 3in1 (Hộp 20 gói)', 65000, 90, N'hộp', N'Cà phê hòa tan tiện lợi', 1),
    (N'Kem Walls Magnum', 28000, 60, N'que', N'Kem que cao cấp', 1);

GO

PRINT N'✓ Tạo database SupermarketDB thành công!';
PRINT N'✓ Đã tạo các bảng: NHANVIEN, KHACHHANG, SANPHAM, HOADON, CTHOADON';
PRINT N'✓ Đã thêm dữ liệu mẫu (Khách hàng, Sản phẩm)';
PRINT N'';
PRINT N'⚠️ QUAN TRỌNG:';
PRINT N'Tài khoản Admin sẽ được tự động tạo khi chạy ứng dụng:';
PRINT N'  - Username: admin';
PRINT N'  - Password: admin123';
PRINT N'  - Vai trò: Quản trị viên';
PRINT N'';
PRINT N'Tạo các tài khoản khác bằng chức năng ĐĂNG KÝ trong ứng dụng hoặc admin panel';
GO

