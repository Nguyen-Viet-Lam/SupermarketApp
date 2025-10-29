-- =============================================
-- Script SỬA TẤT CẢ LỖI DATABASE
-- Sửa lỗi: Invalid column name 'Barcode', 'LoaiSP', 'TrangThai'
-- =============================================

USE SupermarketDB;
GO

PRINT N'🔧 Bắt đầu sửa tất cả lỗi database...';
GO

-- =============================================
-- 1. KIỂM TRA VÀ THÊM CÁC CỘT THIẾU CHO SANPHAM
-- =============================================
PRINT N'Fixing SANPHAM table...';

-- Thêm cột Barcode nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SANPHAM') AND name = 'Barcode')
BEGIN
    ALTER TABLE SANPHAM ADD Barcode NVARCHAR(50);
    PRINT N'  ✅ Added column Barcode';
END

-- Thêm cột LoaiSP nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SANPHAM') AND name = 'LoaiSP')
BEGIN
    ALTER TABLE SANPHAM ADD LoaiSP NVARCHAR(100);
    PRINT N'  ✅ Added column LoaiSP';
END

-- Thêm cột TrangThai nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SANPHAM') AND name = 'TrangThai')
BEGIN
    ALTER TABLE SANPHAM ADD TrangThai BIT NOT NULL DEFAULT 1;
    PRINT N'  ✅ Added column TrangThai';
END

-- Thêm cột NgayTao nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SANPHAM') AND name = 'NgayTao')
BEGIN
    ALTER TABLE SANPHAM ADD NgayTao DATETIME NOT NULL DEFAULT GETDATE();
    PRINT N'  ✅ Added column NgayTao';
END

-- =============================================
-- 2. KIỂM TRA VÀ THÊM CÁC CỘT THIẾU CHO NHACUNGCAP
-- =============================================
PRINT N'Fixing NHACUNGCAP table...';

-- Thêm cột TrangThai nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NHACUNGCAP') AND name = 'TrangThai')
BEGIN
    ALTER TABLE NHACUNGCAP ADD TrangThai BIT NOT NULL DEFAULT 1;
    PRINT N'  ✅ Added column TrangThai to NHACUNGCAP';
END

-- Thêm cột NgayTao nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NHACUNGCAP') AND name = 'NgayTao')
BEGIN
    ALTER TABLE NHACUNGCAP ADD NgayTao DATETIME NOT NULL DEFAULT GETDATE();
    PRINT N'  ✅ Added column NgayTao to NHACUNGCAP';
END

-- =============================================
-- 3. KIỂM TRA VÀ THÊM CÁC CỘT THIẾU CHO KHACHHANG
-- =============================================
PRINT N'Fixing KHACHHANG table...';

-- Thêm cột Email nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('KHACHHANG') AND name = 'Email')
BEGIN
    ALTER TABLE KHACHHANG ADD Email NVARCHAR(100);
    PRINT N'  ✅ Added column Email to KHACHHANG';
END

-- Thêm cột DiaChi nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('KHACHHANG') AND name = 'DiaChi')
BEGIN
    ALTER TABLE KHACHHANG ADD DiaChi NVARCHAR(200);
    PRINT N'  ✅ Added column DiaChi to KHACHHANG';
END

-- Thêm cột NgayTao nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('KHACHHANG') AND name = 'NgayTao')
BEGIN
    ALTER TABLE KHACHHANG ADD NgayTao DATETIME NOT NULL DEFAULT GETDATE();
    PRINT N'  ✅ Added column NgayTao to KHACHHANG';
END

-- =============================================
-- 4. KIỂM TRA VÀ THÊM CÁC CỘT THIẾU CHO NHANVIEN
-- =============================================
PRINT N'Fixing NHANVIEN table...';

-- Thêm cột Salt nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NHANVIEN') AND name = 'Salt')
BEGIN
    ALTER TABLE NHANVIEN ADD Salt VARBINARY(MAX);
    PRINT N'  ✅ Added column Salt to NHANVIEN';
END

-- Thêm cột NgayTao nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NHANVIEN') AND name = 'NgayTao')
BEGIN
    ALTER TABLE NHANVIEN ADD NgayTao DATETIME NOT NULL DEFAULT GETDATE();
    PRINT N'  ✅ Added column NgayTao to NHANVIEN';
END

-- =============================================
-- 5. FIX COLUMN NAMES
-- =============================================
PRINT N'Fixing column names...';

-- Rename SoLuongTon → SoLuong nếu cần
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SANPHAM') AND name = 'SoLuongTon')
BEGIN
    EXEC sp_rename 'SANPHAM.SoLuongTon', 'SoLuong', 'COLUMN';
    PRINT N'  ✅ Renamed SoLuongTon → SoLuong';
END

-- Rename SoDienThoai → SDT trong KHACHHANG nếu cần
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('KHACHHANG') AND name = 'SoDienThoai')
BEGIN
    EXEC sp_rename 'KHACHHANG.SoDienThoai', 'SDT', 'COLUMN';
    PRINT N'  ✅ Renamed SoDienThoai → SDT';
END

-- =============================================
-- 6. CẬP NHẬT DỮ LIỆU MẪU
-- =============================================
PRINT N'Updating sample data...';

-- Cập nhật Barcode cho các sản phẩm hiện có
UPDATE SANPHAM SET Barcode = '8934561000001' WHERE MaSP = 1 AND (Barcode IS NULL OR Barcode = '');
UPDATE SANPHAM SET Barcode = '8934561000002' WHERE MaSP = 2 AND (Barcode IS NULL OR Barcode = '');
UPDATE SANPHAM SET Barcode = '8934561000003' WHERE MaSP = 3 AND (Barcode IS NULL OR Barcode = '');
UPDATE SANPHAM SET Barcode = '8934561000004' WHERE MaSP = 4 AND (Barcode IS NULL OR Barcode = '');
UPDATE SANPHAM SET Barcode = '8934561000005' WHERE MaSP = 5 AND (Barcode IS NULL OR Barcode = '');

-- Cập nhật LoaiSP cho các sản phẩm hiện có
UPDATE SANPHAM SET LoaiSP = N'Thực phẩm' WHERE MaSP IN (1, 2, 3) AND (LoaiSP IS NULL OR LoaiSP = '');
UPDATE SANPHAM SET LoaiSP = N'Đồ uống' WHERE MaSP = 5 AND (LoaiSP IS NULL OR LoaiSP = '');
UPDATE SANPHAM SET LoaiSP = N'Gia vị' WHERE MaSP = 9 AND (LoaiSP IS NULL OR LoaiSP = '');
UPDATE SANPHAM SET LoaiSP = N'Đồ ăn vặt' WHERE MaSP IN (4, 10) AND (LoaiSP IS NULL OR LoaiSP = '');

-- Đảm bảo TrangThai = 1 cho tất cả sản phẩm và nhà cung cấp
UPDATE SANPHAM SET TrangThai = 1 WHERE TrangThai IS NULL;
UPDATE NHACUNGCAP SET TrangThai = 1 WHERE TrangThai IS NULL;

-- Cập nhật NgayTao cho các record hiện có
UPDATE SANPHAM SET NgayTao = GETDATE() WHERE NgayTao IS NULL;
UPDATE KHACHHANG SET NgayTao = GETDATE() WHERE NgayTao IS NULL;
UPDATE NHANVIEN SET NgayTao = GETDATE() WHERE NgayTao IS NULL;
UPDATE NHACUNGCAP SET NgayTao = GETDATE() WHERE NgayTao IS NULL;

PRINT N'  ✅ Updated sample data';

-- =============================================
-- 7. KIỂM TRA KẾT QUẢ
-- =============================================
PRINT N'Verifying results...';

-- Kiểm tra cấu trúc bảng SANPHAM
PRINT N'📋 SANPHAM table structure:';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'SANPHAM'
ORDER BY ORDINAL_POSITION;

-- Kiểm tra cấu trúc bảng NHACUNGCAP
PRINT N'📋 NHACUNGCAP table structure:';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'NHACUNGCAP'
ORDER BY ORDINAL_POSITION;

-- Kiểm tra cấu trúc bảng KHACHHANG
PRINT N'📋 KHACHHANG table structure:';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'KHACHHANG'
ORDER BY ORDINAL_POSITION;

-- Kiểm tra cấu trúc bảng NHANVIEN
PRINT N'📋 NHANVIEN table structure:';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'NHANVIEN'
ORDER BY ORDINAL_POSITION;

GO

PRINT N'';
PRINT N'🎉 ĐÃ SỬA XONG TẤT CẢ LỖI DATABASE!';
PRINT N'';
PRINT N'✅ Các vấn đề đã được sửa:';
PRINT N'  - Thêm cột Barcode vào SANPHAM';
PRINT N'  - Thêm cột LoaiSP vào SANPHAM';
PRINT N'  - Thêm cột TrangThai vào SANPHAM và NHACUNGCAP';
PRINT N'  - Thêm cột NgayTao vào tất cả bảng';
PRINT N'  - Thêm cột Email, DiaChi vào KHACHHANG';
PRINT N'  - Thêm cột Salt vào NHANVIEN';
PRINT N'  - Cập nhật dữ liệu mẫu';
PRINT N'';
PRINT N'🚀 Bây giờ có thể chạy lại ứng dụng mà không bị lỗi!';
PRINT N'';
PRINT N'📝 Hướng dẫn:';
PRINT N'  1. Chạy script này trong SQL Server Management Studio';
PRINT N'  2. Đảm bảo database SupermarketDB đã được chọn';
PRINT N'  3. Nhấn F5 để thực thi';
PRINT N'  4. Chạy lại ứng dụng SupermarketApp';
GO

