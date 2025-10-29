-- Sửa lỗi bảng CAIDAT - Cập nhật dữ liệu mẫu
USE SupermarketDB;
GO

-- Cập nhật dữ liệu hiện có
UPDATE CAIDAT 
SET 
    TenCaiDat = 'TenCuaHang',
    GiaTri = ISNULL(TenCuaHang, 'Siêu thị ABC'),
    MoTa = 'Tên cửa hàng',
    NguoiCapNhat = 'System'
WHERE MaCaiDat = 1;

-- Thêm dữ liệu cho DiaChi nếu chưa có
IF NOT EXISTS (SELECT * FROM CAIDAT WHERE TenCaiDat = 'DiaChi')
BEGIN
    DECLARE @DiaChi NVARCHAR(300);
    SELECT @DiaChi = ISNULL(DiaChi, '123 Đường ABC, Quận XYZ') FROM CAIDAT WHERE MaCaiDat = 1;
    
    INSERT INTO CAIDAT (TenCuaHang, DiaChi, SoDienThoai, TenCaiDat, GiaTri, MoTa, NgayCapNhat, NguoiCapNhat)
    VALUES ('Siêu thị ABC', @DiaChi, '0123456789', 'DiaChi', @DiaChi, 'Địa chỉ cửa hàng', GETDATE(), 'System');
END

-- Thêm dữ liệu cho SoDienThoai nếu chưa có
IF NOT EXISTS (SELECT * FROM CAIDAT WHERE TenCaiDat = 'SoDienThoai')
BEGIN
    DECLARE @SoDienThoai NVARCHAR(20);
    SELECT @SoDienThoai = ISNULL(SoDienThoai, '0123456789') FROM CAIDAT WHERE MaCaiDat = 1;
    
    INSERT INTO CAIDAT (TenCuaHang, DiaChi, SoDienThoai, TenCaiDat, GiaTri, MoTa, NgayCapNhat, NguoiCapNhat)
    VALUES ('Siêu thị ABC', '123 Đường ABC', @SoDienThoai, 'SoDienThoai', @SoDienThoai, 'Số điện thoại', GETDATE(), 'System');
END

-- Hiển thị kết quả
SELECT MaCaiDat, TenCaiDat, GiaTri, MoTa, NgayCapNhat 
FROM CAIDAT 
WHERE TenCaiDat IS NOT NULL 
ORDER BY MaCaiDat;

PRINT '✅ Hoàn thành cập nhật dữ liệu CAIDAT!';
GO
















