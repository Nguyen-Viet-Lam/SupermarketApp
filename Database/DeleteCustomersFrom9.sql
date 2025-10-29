-- =============================================
-- Script xóa khách hàng từ ID 10 trở đi
-- Giữ lại dữ liệu khách hàng ID 1-9
-- =============================================

USE SupermarketDB;
GO

-- Xóa chi tiết hóa đơn liên quan đến khách hàng từ ID 10 trở đi
DELETE FROM CTHOADON
WHERE MaHD IN (SELECT MaHD FROM HOADON WHERE MaKH IN (SELECT MaKH FROM KHACHHANG WHERE MaKH > 9));
GO

-- Xóa hóa đơn liên quan đến khách hàng từ ID 10 trở đi
DELETE FROM HOADON
WHERE MaKH IN (SELECT MaKH FROM KHACHHANG WHERE MaKH > 9);
GO

-- Xóa khách hàng từ ID 10 trở đi
DELETE FROM KHACHHANG
WHERE MaKH > 9;
GO

PRINT N'✓ Đã xóa khách hàng từ ID 10 trở đi';
PRINT N'✓ Giữ lại dữ liệu khách hàng ID 1-9';
GO

