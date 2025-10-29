-- =============================================
-- Script xóa trường GhiChu khỏi hóa đơn
-- Cập nhật database SupermarketDB
-- =============================================

USE SupermarketDB;
GO

-- Xóa cột GhiChu khỏi bảng HOADON nếu tồn tại
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'HOADON') AND name = 'GhiChu')
BEGIN
    ALTER TABLE HOADON
    DROP COLUMN GhiChu;
    
    PRINT N'✓ Đã xóa cột GhiChu khỏi bảng HOADON';
END
ELSE
BEGIN
    PRINT N'⚠ Cột GhiChu không tồn tại trong bảng HOADON';
END
GO

PRINT N'';
PRINT N'✅ Cập nhật database thành công!';
GO


