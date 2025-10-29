-- ========================================
-- CẬP NHẬT DATABASE CHO PHÂN LOẠI KHÁCH HÀNG
-- ========================================

-- Thêm cột LoaiKH vào bảng KHACHHANG
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('KHACHHANG') AND name = 'LoaiKH')
BEGIN
    ALTER TABLE KHACHHANG ADD LoaiKH NVARCHAR(50) DEFAULT 'Vãng lai';
    PRINT '✅ Đã thêm cột LoaiKH vào bảng KHACHHANG';
END
ELSE
BEGIN
    PRINT '⚠️ Cột LoaiKH đã tồn tại trong bảng KHACHHANG';
END

-- Cập nhật phân loại khách hàng dựa trên điểm tích lũy
UPDATE KHACHHANG 
SET LoaiKH = CASE 
    WHEN DiemTichLuy >= 1000 THEN 'VIP'
    WHEN DiemTichLuy >= 500 THEN 'Thân quen'
    ELSE 'Vãng lai'
END
WHERE LoaiKH IS NULL OR LoaiKH = 'Vãng lai';

PRINT '✅ Đã cập nhật phân loại khách hàng dựa trên điểm tích lũy';

-- Hiển thị thống kê phân loại khách hàng
SELECT 
    LoaiKH as 'Loại khách hàng',
    COUNT(*) as 'Số lượng',
    AVG(DiemTichLuy) as 'Điểm tích lũy TB',
    SUM(DiemTichLuy) as 'Tổng điểm tích lũy'
FROM KHACHHANG 
GROUP BY LoaiKH
ORDER BY 
    CASE LoaiKH 
        WHEN 'VIP' THEN 1 
        WHEN 'Thân quen' THEN 2 
        WHEN 'Vãng lai' THEN 3 
    END;

PRINT '✅ Hoàn thành cập nhật phân loại khách hàng!';
















