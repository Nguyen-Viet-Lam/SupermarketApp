-- =============================================
-- Script cập nhật database SupermarketDB
-- Thêm trường LoaiKH vào bảng KHACHHANG
-- =============================================

USE SupermarketDB;
GO

-- Thêm cột LoaiKH nếu chưa tồn tại
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'KHACHHANG') AND name = 'LoaiKH')
BEGIN
    ALTER TABLE KHACHHANG
    ADD LoaiKH NVARCHAR(50) DEFAULT 'Vãng lai';
    
    PRINT N'✓ Đã thêm cột LoaiKH vào bảng KHACHHANG';
END
ELSE
BEGIN
    PRINT N'⚠ Cột LoaiKH đã tồn tại';
END
GO

-- Cập nhật dữ liệu mẫu nếu có khách hàng mới
IF EXISTS (SELECT * FROM KHACHHANG WHERE LoaiKH IS NULL)
BEGIN
    UPDATE KHACHHANG
    SET LoaiKH = 'Vãng lai'
    WHERE LoaiKH IS NULL;
    
    PRINT N'✓ Đã cập nhật LoaiKH cho các khách hàng hiện có';
END
GO

PRINT N'';
PRINT N'✅ Cập nhật database thành công!';
GO


