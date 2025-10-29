-- ========================================
-- SỬA LỖI BẢNG CAIDAT
-- ========================================

USE SupermarketDB;
GO

-- Kiểm tra và sửa bảng CAIDAT
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CAIDAT')
BEGIN
    -- Tạo bảng CAIDAT nếu chưa tồn tại
    CREATE TABLE CAIDAT (
        MaCaiDat INT PRIMARY KEY IDENTITY(1,1),
        TenCaiDat NVARCHAR(100) NOT NULL UNIQUE,
        GiaTri NVARCHAR(MAX),
        MoTa NVARCHAR(500),
        NgayCapNhat DATETIME NOT NULL DEFAULT GETDATE(),
        NguoiCapNhat NVARCHAR(100)
    );
    PRINT '✅ Đã tạo bảng CAIDAT';
END
ELSE
BEGIN
    PRINT '⚠️ Bảng CAIDAT đã tồn tại, đang kiểm tra cột...';
    
    -- Thêm cột TenCaiDat nếu chưa có
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CAIDAT') AND name = 'TenCaiDat')
    BEGIN
        ALTER TABLE CAIDAT ADD TenCaiDat NVARCHAR(100) NOT NULL;
        PRINT '✅ Đã thêm cột TenCaiDat';
    END
    
    -- Thêm cột GiaTri nếu chưa có
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CAIDAT') AND name = 'GiaTri')
    BEGIN
        ALTER TABLE CAIDAT ADD GiaTri NVARCHAR(MAX);
        PRINT '✅ Đã thêm cột GiaTri';
    END
    
    -- Thêm cột MoTa nếu chưa có
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CAIDAT') AND name = 'MoTa')
    BEGIN
        ALTER TABLE CAIDAT ADD MoTa NVARCHAR(500);
        PRINT '✅ Đã thêm cột MoTa';
    END
    
    -- Thêm cột NgayCapNhat nếu chưa có
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CAIDAT') AND name = 'NgayCapNhat')
    BEGIN
        ALTER TABLE CAIDAT ADD NgayCapNhat DATETIME NOT NULL DEFAULT GETDATE();
        PRINT '✅ Đã thêm cột NgayCapNhat';
    END
    
    -- Thêm cột NguoiCapNhat nếu chưa có
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CAIDAT') AND name = 'NguoiCapNhat')
    BEGIN
        ALTER TABLE CAIDAT ADD NguoiCapNhat NVARCHAR(100);
        PRINT '✅ Đã thêm cột NguoiCapNhat';
    END
    
    -- Thêm unique constraint cho TenCaiDat nếu chưa có
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CAIDAT_TenCaiDat')
    BEGIN
        CREATE UNIQUE INDEX IX_CAIDAT_TenCaiDat ON CAIDAT(TenCaiDat);
        PRINT '✅ Đã thêm unique index cho TenCaiDat';
    END
END

-- Chèn dữ liệu mẫu nếu chưa có
IF NOT EXISTS (SELECT * FROM CAIDAT WHERE TenCaiDat = 'TenCuaHang')
BEGIN
    INSERT INTO CAIDAT (TenCaiDat, GiaTri, MoTa, NgayCapNhat, NguoiCapNhat)
    VALUES ('TenCuaHang', N'Siêu thị ABC', N'Tên cửa hàng', GETDATE(), 'System');
    PRINT '✅ Đã thêm dữ liệu mẫu: TenCuaHang';
END

IF NOT EXISTS (SELECT * FROM CAIDAT WHERE TenCaiDat = 'DiaChi')
BEGIN
    INSERT INTO CAIDAT (TenCaiDat, GiaTri, MoTa, NgayCapNhat, NguoiCapNhat)
    VALUES ('DiaChi', N'123 Đường ABC, Quận XYZ', N'Địa chỉ cửa hàng', GETDATE(), 'System');
    PRINT '✅ Đã thêm dữ liệu mẫu: DiaChi';
END

IF NOT EXISTS (SELECT * FROM CAIDAT WHERE TenCaiDat = 'SoDienThoai')
BEGIN
    INSERT INTO CAIDAT (TenCaiDat, GiaTri, MoTa, NgayCapNhat, NguoiCapNhat)
    VALUES ('SoDienThoai', N'0123456789', N'Số điện thoại cửa hàng', GETDATE(), 'System');
    PRINT '✅ Đã thêm dữ liệu mẫu: SoDienThoai';
END

-- Hiển thị dữ liệu
SELECT * FROM CAIDAT;

PRINT '✅ Hoàn thành sửa lỗi bảng CAIDAT!';
GO
















