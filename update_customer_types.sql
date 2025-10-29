-- Cập nhật phân loại khách hàng
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('KHACHHANG') AND name = 'LoaiKH')
BEGIN
    ALTER TABLE KHACHHANG ADD LoaiKH NVARCHAR(50) DEFAULT 'Vãng lai';
    PRINT 'Đã thêm cột LoaiKH';
END
ELSE
BEGIN
    PRINT 'Cột LoaiKH đã tồn tại';
END

-- Cập nhật phân loại dựa trên điểm tích lũy
UPDATE KHACHHANG 
SET LoaiKH = CASE 
    WHEN DiemTichLuy >= 1000 THEN 'VIP'
    WHEN DiemTichLuy >= 500 THEN 'Thân quen'
    ELSE 'Vãng lai'
END
WHERE LoaiKH IS NULL OR LoaiKH = 'Vãng lai';

-- Hiển thị thống kê
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

PRINT 'Hoàn thành cập nhật phân loại khách hàng!';
















