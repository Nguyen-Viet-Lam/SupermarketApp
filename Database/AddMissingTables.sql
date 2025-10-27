-- =============================================
-- Script bổ sung 3 bảng còn thiếu cho SupermarketDB
-- Thêm: NHACUNGCAP, PHIEUNHAP, CTPHIEUNHAP
-- =============================================

USE SupermarketDB;
GO

PRINT N'Bắt đầu tạo các bảng bổ sung...';
GO

-- =============================================
-- Bảng NHACUNGCAP - Quản lý nhà cung cấp
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NHACUNGCAP]') AND type in (N'U'))
BEGIN
    CREATE TABLE NHACUNGCAP (
        MaNCC INT PRIMARY KEY IDENTITY(1,1),
        TenNCC NVARCHAR(200) NOT NULL,
        DiaChi NVARCHAR(500),
        SDT NVARCHAR(20),
        Email NVARCHAR(100),
        MaSoThue NVARCHAR(50),
        NguoiLienHe NVARCHAR(200),
        TrangThai BIT NOT NULL DEFAULT 1,
        NgayTao DATETIME2 NOT NULL DEFAULT GETDATE(),
        GhiChu NVARCHAR(500)
    );
    PRINT N'✓ Đã tạo bảng NHACUNGCAP';
END
ELSE
BEGIN
    PRINT N'⚠ Bảng NHACUNGCAP đã tồn tại, bỏ qua';
END
GO

-- =============================================
-- Bảng PHIEUNHAP - Quản lý phiếu nhập hàng
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PHIEUNHAP]') AND type in (N'U'))
BEGIN
    CREATE TABLE PHIEUNHAP (
        MaPN INT PRIMARY KEY IDENTITY(1,1),
        NgayNhap DATETIME2 NOT NULL DEFAULT GETDATE(),
        MaNCC INT NOT NULL,
        MaNV INT NOT NULL,
        TongTien DECIMAL(18,2) NOT NULL DEFAULT 0,
        GhiChu NVARCHAR(500),
        TrangThai NVARCHAR(50) NOT NULL DEFAULT N'Chờ duyệt',
        CONSTRAINT FK_PhieuNhap_NhaCungCap FOREIGN KEY (MaNCC) REFERENCES NHACUNGCAP(MaNCC),
        CONSTRAINT FK_PhieuNhap_NhanVien FOREIGN KEY (MaNV) REFERENCES NHANVIEN(MaNV)
    );
    PRINT N'✓ Đã tạo bảng PHIEUNHAP';
END
ELSE
BEGIN
    PRINT N'⚠ Bảng PHIEUNHAP đã tồn tại, bỏ qua';
END
GO

-- =============================================
-- Bảng CTPHIEUNHAP - Chi tiết phiếu nhập
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CTPHIEUNHAP]') AND type in (N'U'))
BEGIN
    CREATE TABLE CTPHIEUNHAP (
        MaPN INT NOT NULL,
        MaSP INT NOT NULL,
        SoLuong INT NOT NULL,
        DonGiaNhap DECIMAL(18,2) NOT NULL,
        ThanhTien AS (SoLuong * DonGiaNhap) PERSISTED,
        PRIMARY KEY (MaPN, MaSP),
        CONSTRAINT FK_CTPhieuNhap_PhieuNhap FOREIGN KEY (MaPN) REFERENCES PHIEUNHAP(MaPN) ON DELETE CASCADE,
        CONSTRAINT FK_CTPhieuNhap_SanPham FOREIGN KEY (MaSP) REFERENCES SANPHAM(MaSP)
    );
    PRINT N'✓ Đã tạo bảng CTPHIEUNHAP';
END
ELSE
BEGIN
    PRINT N'⚠ Bảng CTPHIEUNHAP đã tồn tại, bỏ qua';
END
GO

-- =============================================
-- Thêm dữ liệu mẫu
-- =============================================

-- Thêm nhà cung cấp mẫu
IF NOT EXISTS (SELECT * FROM NHACUNGCAP)
BEGIN
    INSERT INTO NHACUNGCAP (TenNCC, DiaChi, SDT, Email, MaSoThue, NguoiLienHe, TrangThai, GhiChu)
    VALUES 
        (N'Công ty TNHH Thực phẩm Sạch', N'123 Nguyễn Văn Linh, Q7, TP.HCM', '0283456789', 'contact@thucphamsach.vn', '0123456789', N'Nguyễn Văn B', 1, N'Cung cấp thực phẩm tươi sống'),
        (N'Công ty CP Đồ uống Việt Nam', N'456 Lê Văn Việt, Q9, TP.HCM', '0287654321', 'sales@douongvn.com', '9876543210', N'Trần Thị C', 1, N'Cung cấp nước giải khát, đồ uống'),
        (N'Công ty TNHH Bánh kẹo Hà Nội', N'789 Trường Chinh, Hà Nội', '0241234567', 'info@banhkeohanoi.vn', '5555666777', N'Phạm Văn D', 1, N'Cung cấp bánh kẹo, snack'),
        (N'Công ty CP Gia vị Việt', N'321 CMT8, Q10, TP.HCM', '0289876543', 'contact@giaviviet.vn', '1112223334', N'Lê Thị E', 1, N'Gia vị, hương liệu'),
        (N'Công ty TNHH Hàng tiêu dùng ABC', N'555 Điện Biên Phủ, Q3, TP.HCM', '0285551234', 'info@abccorp.vn', '4445556667', N'Hoàng Văn F', 1, N'Hàng tiêu dùng tổng hợp');
    
    PRINT N'✓ Đã thêm 5 nhà cung cấp mẫu';
END
GO

-- Thêm phiếu nhập mẫu (nếu có sản phẩm)
IF EXISTS (SELECT * FROM SANPHAM) AND EXISTS (SELECT * FROM NHACUNGCAP) AND EXISTS (SELECT * FROM NHANVIEN)
BEGIN
    IF NOT EXISTS (SELECT * FROM PHIEUNHAP)
    BEGIN
        DECLARE @MaNV INT = (SELECT TOP 1 MaNV FROM NHANVIEN WHERE VaiTro IN ('Admin', 'QuanLy'));
        DECLARE @MaNCC INT = (SELECT TOP 1 MaNCC FROM NHACUNGCAP);
        
        INSERT INTO PHIEUNHAP (NgayNhap, MaNCC, MaNV, TongTien, TrangThai, GhiChu)
        VALUES 
            (GETDATE(), @MaNCC, @MaNV, 5000000, N'Đã duyệt', N'Nhập hàng định kỳ tháng 10'),
            (DATEADD(DAY, -7, GETDATE()), @MaNCC, @MaNV, 3500000, N'Đã duyệt', N'Nhập hàng bổ sung');
        
        PRINT N'✓ Đã thêm 2 phiếu nhập mẫu';
        
        -- Thêm chi tiết phiếu nhập
        DECLARE @MaPN1 INT = (SELECT TOP 1 MaPN FROM PHIEUNHAP ORDER BY MaPN);
        DECLARE @MaSP1 INT = (SELECT TOP 1 MaSP FROM SANPHAM ORDER BY MaSP);
        DECLARE @MaSP2 INT = (SELECT MaSP FROM SANPHAM ORDER BY MaSP OFFSET 1 ROWS FETCH NEXT 1 ROWS ONLY);
        
        IF @MaSP1 IS NOT NULL AND @MaSP2 IS NOT NULL
        BEGIN
            INSERT INTO CTPHIEUNHAP (MaPN, MaSP, SoLuong, DonGiaNhap)
            VALUES 
                (@MaPN1, @MaSP1, 50, 100000),
                (@MaPN1, @MaSP2, 100, 40000);
            
            PRINT N'✓ Đã thêm chi tiết phiếu nhập mẫu';
        END
    END
END
GO

-- =============================================
-- Kiểm tra kết quả
-- =============================================
PRINT N'';
PRINT N'=================================================';
PRINT N'KẾT QUẢ TẠO BẢNG:';
PRINT N'=================================================';

SELECT 
    TABLE_NAME AS N'Tên bảng',
    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = t.TABLE_NAME) AS N'Số cột'
FROM INFORMATION_SCHEMA.TABLES t
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

PRINT N'';
PRINT N'✅ Hoàn thành! Database đã có đủ 8 bảng.';
PRINT N'';
GO

