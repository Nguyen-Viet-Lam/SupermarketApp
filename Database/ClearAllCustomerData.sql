-- =============================================
-- Script xóa TẤT CẢ dữ liệu khách hàng
-- CẨN THẬN: Script này sẽ xóa TẤT CẢ khách hàng
-- =============================================

USE SupermarketDB;
GO

-- Xóa dữ liệu trong bảng CTHOADON có liên quan đến khách hàng
DELETE FROM CTHOADON
WHERE MaHD IN (SELECT MaHD FROM HOADON WHERE MaKH IS NOT NULL);
GO

-- Xóa dữ liệu hóa đơn liên quan đến khách hàng
-- Giữ lại các hóa đơn khách lẻ (MaKH IS NULL)
DELETE FROM HOADON
WHERE MaKH IS NOT NULL;
GO

-- Xóa TẤT CẢ khách hàng
DELETE FROM KHACHHANG;
GO

PRINT N'✓ Đã xóa tất cả dữ liệu khách hàng và hóa đơn liên quan';
PRINT N'✓ Database hiện tại sạch, sẵn sàng để thêm dữ liệu mới';
GO


