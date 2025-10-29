-- =============================================
-- Script FIX DATABASE SCHEMA
-- Fix column mismatches between DB and Models
-- =============================================

USE SupermarketDB;
GO

PRINT N'🔧 Bắt đầu fix database schema...';
GO

-- =============================================
-- 1. FIX BẢNG SANPHAM
-- =============================================
PRINT N'Fixing SANPHAM table...';

-- Rename SoLuongTon → SoLuong (để khớp với Model)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SANPHAM') AND name = 'SoLuongTon')
BEGIN
    EXEC sp_rename 'SANPHAM.SoLuongTon', 'SoLuong', 'COLUMN';
    PRINT N'  ✓ Renamed SoLuongTon → SoLuong';
END

-- Thêm cột LoaiSP nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SANPHAM') AND name = 'LoaiSP')
BEGIN
    ALTER TABLE SANPHAM ADD LoaiSP NVARCHAR(100);
    PRINT N'  ✓ Added column LoaiSP';
END

-- Thêm cột Barcode nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SANPHAM') AND name = 'Barcode')
BEGIN
    ALTER TABLE SANPHAM ADD Barcode NVARCHAR(50);
    PRINT N'  ✓ Added column Barcode';
END

-- Thêm cột TrangThai nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SANPHAM') AND name = 'TrangThai')
BEGIN
    ALTER TABLE SANPHAM ADD TrangThai BIT NOT NULL DEFAULT 1;
    PRINT N'  ✓ Added column TrangThai';
END

-- =============================================
-- 2. FIX BẢNG KHACHHANG
-- =============================================
PRINT N'Fixing KHACHHANG table...';

-- Rename SoDienThoai → SDT (để khớp với Model)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('KHACHHANG') AND name = 'SoDienThoai')
BEGIN
    EXEC sp_rename 'KHACHHANG.SoDienThoai', 'SDT', 'COLUMN';
    PRINT N'  ✓ Renamed SoDienThoai → SDT';
END

-- Email và DiaChi đã có sẵn, không cần thêm

-- =============================================
-- 3. UPDATE DỮ LIỆU MẪU
-- =============================================
PRINT N'Updating sample data...';

-- Update LoaiSP cho sản phẩm mẫu
UPDATE SANPHAM SET LoaiSP = N'Thực phẩm' WHERE MaSP IN (1, 2, 3, 6, 7, 8);
UPDATE SANPHAM SET LoaiSP = N'Đồ uống' WHERE MaSP IN (5);
UPDATE SANPHAM SET LoaiSP = N'Gia vị' WHERE MaSP IN (9);
UPDATE SANPHAM SET LoaiSP = N'Đồ ăn vặt' WHERE MaSP IN (4, 10);
PRINT N'  ✓ Updated LoaiSP for products';

-- Set TrangThai = 1 cho tất cả sản phẩm
UPDATE SANPHAM SET TrangThai = 1 WHERE TrangThai IS NULL;
PRINT N'  ✓ Updated TrangThai for products';

GO

PRINT N'';
PRINT N'✅ ĐÃ FIX XONG DATABASE SCHEMA!';
PRINT N'';
PRINT N'📋 Thay đổi:';
PRINT N'  1. SANPHAM.SoLuongTon → SoLuong';
PRINT N'  2. SANPHAM + LoaiSP (NVARCHAR(100))';
PRINT N'  3. SANPHAM + Barcode (NVARCHAR(50))';
PRINT N'  4. SANPHAM + TrangThai (BIT, DEFAULT 1)';
PRINT N'  5. KHACHHANG.SoDienThoai → SDT';
PRINT N'';
PRINT N'✅ Bây giờ có thể chạy lại app!';
GO







