-- =============================================
-- Script thêm dữ liệu mẫu vào SupermarketDB
-- Font chữ & cỡ chữ được chuẩn hóa (NVARCHAR)
-- =============================================

USE SupermarketDB;
GO

-- =============================================
-- 1. THÊM KHÁCH HÀNG
-- =============================================
INSERT INTO KHACHHANG (TenKH, SoDienThoai, Email, DiaChi, DiemTichLuy) VALUES
(N'Nguyễn Văn A', N'0912345678', N'nguyena@email.com', N'123 Đường Lê Lợi, Q.1, TP.HCM', 150),
(N'Trần Thị B', N'0987654321', N'tranb@email.com', N'456 Đường Nguyễn Huệ, Q.1, TP.HCM', 200),
(N'Phạm Minh C', N'0933456789', N'phamc@email.com', N'789 Đường Trần Hưng Đạo, Q.5, TP.HCM', 100),
(N'Vũ Thị Dung', N'0944567890', N'vudung@email.com', N'321 Đường Đinh Tiên Hoàng, Q.1, TP.HCM', 250),
(N'Hồ Văn Giang', N'0955678901', N'hogiang@email.com', N'654 Đường Võ Văn Kiệt, Q.4, TP.HCM', 80),
(N'Lê Thị Hoa', N'0966789012', N'lehoa@email.com', N'987 Đường Cách Mạng Tháng 8, Q.3, TP.HCM', 320),
(N'Đặng Văn Hùng', N'0977890123', N'danghung@email.com', N'159 Đường Nam Kỳ Khởi Nghĩa, Q.1, TP.HCM', 175),
(N'Bùi Thị Linh', N'0988901234', N'buitinh@email.com', N'753 Đường Tôn Đức Thắng, Q.1, TP.HCM', 210),
(N'Cao Minh Tuấn', N'0999012345', N'caotuan@email.com', N'246 Đường Mạc Đĩnh Chi, Q.1, TP.HCM', 95),
(N'Đinh Văn Kiên', N'0910123456', N'dinhkien@email.com', N'369 Đường Lê Thanh Nghị, Q.Bình Thạnh, TP.HCM', 140);
GO

-- =============================================
-- 2. THÊM NHÂN VIÊN (ngoài admin)
-- =============================================
INSERT INTO NHANVIEN (TenNV, TaiKhoan, MatKhauHash, Salt, VaiTro, TrangThai) VALUES
(N'Lê Văn B', N'le.b', 0x123456, 0x654321, N'NV', 1),
(N'Trần Thị C', N'tran.c', 0x789012, 0x210987, N'NV', 1),
(N'Phạm Minh D', N'pham.d', 0x345678, 0x876543, N'NV', 1),
(N'Vũ Thị E', N'vu.e', 0x901234, 0x432109, N'QuanLy', 1),
(N'Hồ Văn F', N'ho.f', 0x567890, 0x098765, N'NV', 1);
GO

-- =============================================
-- 3. THÊM NHÀ CUNG CẤP
-- =============================================
INSERT INTO NHACUNGCAP (TenNCC, DiaChi, SoDienThoai, Email, MST) VALUES
(N'Công ty CP Gia vị Việt', N'321 Lạc Long Quân, Q.11, TP.HCM', N'0289876543', N'contact@vietspice.com', N'5566778899'),
(N'Công ty TNHH Bánh keo Hà Nội', N'789 CMT8, Q.10, TP.HCM', N'0812345678', N'sales@hanoicandy.com', N'1122334455'),
(N'Công ty CP Đồ uống Sài Gòn', N'456 Võ Văn Kiệt, Q.5, TP.HCM', N'0287654321', N'info@saigondrink.com', N'9876543210'),
(N'Công ty TNHH Thực phẩm Việt', N'123 Xa Lộ Hà Nội, Q.9, TP.HCM', N'0283456789', N'contact@tpvn.com', N'0123456789'),
(N'Công ty CP Sữa Hà Nội', N'567 Đinh Bộ Lĩnh, Q.Bình Thạnh, TP.HCM', N'0298765432', N'sales@hanoimelk.com', N'5544332211');
GO

-- =============================================
-- 4. THÊM SẢN PHẨM (đa dạng, font chuẩn)
-- =============================================
-- Bánh keo
INSERT INTO SANPHAM (TenSP, DonGia, SoLuong, DonVi, LoaiSP, Barcode, MoTa, TrangThai) VALUES
(N'Bánh Oreo gói 137g', 22000, 250, N'Gói', N'Bánh keo', N'8888001000', N'Bánh quy ngọt, nhân kem', 1),
(N'Bánh Alpenliebe vị sữa', 18000, 180, N'Gói', N'Bánh keo', N'8888001001', N'Bánh giòn, thơm mát', 1),
(N'Bánh Snack Lays vị tự nhiên', 12000, 300, N'Gói', N'Bánh keo', N'8888001002', N'Bánh snack, giòn', 1),

-- Đồ uống gia dụng
(N'Bột giặt OMO matic 3.5kg', 135000, 85, N'Gói', N'Đồ dùng gia đình', N'7777001000', N'Bột giặt máy, hiệu quả', 1),
(N'Nước lau sàn Sunlight 750g', 32000, 120, N'Chai', N'Đồ dùng gia đình', N'7777001001', N'Tẩy rửa mạnh', 1),
(N'Nước rửa chén Sunlight 750g', 32000, 140, N'Chai', N'Đồ dùng gia đình', N'7777001002', N'Tẩy rửa hiệu quả', 1),

-- Thực phẩm tươi
(N'Cá hồi Na Uy', 350000, 45, N'Kg', N'Thực phẩm tươi', N'6666001000', N'Cá hồi tươi, chất lượng', 1),
(N'Gạo ST25 cao cấp', 25000, 600, N'Kg', N'Thực phẩm tươi', N'6666001001', N'Gạo thơm, ngon', 1),
(N'Rau cải ngot hữu cơ', 15000, 150, N'Kg', N'Thực phẩm tươi', N'6666001002', N'Rau sạch, an toàn', 1),

-- Đồ uống
(N'Coca Cola lon 330ml', 10000, 700, N'Lon', N'Đồ uống', N'5555001000', N'Nước ngọt, lạnh', 1),
(N'Nước suối Aquafina 500ml', 5000, 800, N'Chai', N'Đồ uống', N'5555001001', N'Nước tinh khiết', 1),
(N'Nước mâm Nam Ngự 500ml', 28000, 120, N'Chai', N'Đồ uống', N'5555001002', N'Nước mâm chất lượng', 1),
(N'Nước lau sản Vim 1L', 38000, 95, N'Chai', N'Đồ uống', N'5555001003', N'Tẩy rửa mạnh', 1),
(N'Sting đầu 330ml', 9000, 550, N'Chai', N'Đồ uống', N'5555001004', N'Nước tăng lực', 1),
(N'Dâu ăn Neptune 1L', 45000, 110, N'Chai', N'Gia vị', N'4444001000', N'Dâu ăn cao cấp', 1),
(N'Dâu gói Clear Men 630g', 118000, 80, N'Gói', N'Sản phẩm chăm sóc', N'4444001001', N'Dâu gội chuyên biệt', 1),

-- Sản phẩm chăm sóc
(N'Kem đánh răng P/S 230g', 28000, 160, N'Tuýp', N'Sản phẩm chăm sóc', N'3333001000', N'Kem đánh răng hiệu quả', 1),
(N'Gel Clear Men', 118000, 100, N'Chai', N'Sản phẩm chăm sóc', N'3333001001', N'Gel tắm nam', 1),
(N'Nước hoa Nước hoa Dior', 850000, 25, N'Chai', N'Sản phẩm chăm sóc', N'3333001002', N'Nước hoa sang trọng', 1);
GO

-- =============================================
-- 5. THÊM PHIẾU NHẬP (từ nhà cung cấp)
-- =============================================
INSERT INTO PHIEUNHAP (MaNCC, MaNV, NgayNhap, GhiChu, TongTien) VALUES
(1, 2, DATEADD(DAY, -15, GETDATE()), N'Nhập hàng lần 1', 5000000),
(2, 2, DATEADD(DAY, -10, GETDATE()), N'Nhập hàng lần 2', 3500000),
(3, 3, DATEADD(DAY, -5, GETDATE()), N'Nhập hàng lần 3', 4200000),
(4, 4, DATEADD(DAY, -3, GETDATE()), N'Nhập hàng tươi', 6800000);
GO

-- =============================================
-- 6. THÊM CHI TIẾT PHIẾU NHẬP
-- =============================================
INSERT INTO CTPHIEUNHAP (MaPhieu, MaSP, SoLuong, DonGiaNhap) VALUES
(1, 1, 200, 15000),
(1, 2, 150, 12000),
(1, 3, 100, 8000),
(2, 4, 80, 95000),
(2, 5, 120, 22000),
(3, 6, 200, 18000),
(3, 7, 150, 25000),
(4, 8, 500, 18000),
(4, 9, 300, 20000);
GO

-- =============================================
-- 7. THÊM HÓA ĐƠN BÁN HÀNG (lịch sử)
-- =============================================
INSERT INTO HOADON (MaKH, MaNV, NgayLap, TongTien) VALUES
(1, 2, DATEADD(DAY, -7, GETDATE()), 250000),
(2, 3, DATEADD(DAY, -6, GETDATE()), 450000),
(3, 4, DATEADD(DAY, -5, GETDATE()), 125000),
(4, 2, DATEADD(DAY, -4, GETDATE()), 380000),
(5, 3, DATEADD(DAY, -3, GETDATE()), 620000),
(6, 4, DATEADD(DAY, -2, GETDATE()), 290000),
(7, 2, DATEADD(DAY, -1, GETDATE()), 180000),
(8, 3, GETDATE(), 520000);
GO

-- =============================================
-- 8. THÊM CHI TIẾT HÓA ĐƠN
-- =============================================
INSERT INTO CTHOADON (MaHD, MaSP, SoLuong, DonGiaBan) VALUES
-- Hóa đơn 1
(1, 1, 5, 22000),
(1, 19, 3, 28000),

-- Hóa đơn 2
(2, 4, 2, 135000),
(2, 5, 1, 32000),
(2, 10, 2, 5000),

-- Hóa đơn 3
(3, 1, 3, 22000),
(3, 2, 2, 18000),
(3, 3, 2, 12000),

-- Hóa đơn 4
(4, 4, 1, 135000),
(4, 6, 2, 32000),

-- Hóa đơn 5
(5, 8, 1, 350000),
(5, 9, 2, 25000),

-- Hóa đơn 6
(6, 11, 2, 15000),
(6, 12, 1, 32000),

-- Hóa đơn 7
(7, 11, 1, 10000),
(7, 12, 2, 5000),

-- Hóa đơn 8
(8, 13, 1, 118000),
(8, 14, 2, 28000),
(8, 15, 1, 850000);
GO

-- =============================================
-- 9. CẬP NHẬT ĐIỂM TÍCH LŨY KHÁCH HÀNG
-- =============================================
UPDATE KHACHHANG SET DiemTichLuy = DiemTichLuy + 25 WHERE MaKH IN (1, 2, 3, 4, 5, 6, 7, 8);
GO

-- =============================================
-- 10. KIỂM TRA DỮ LIỆU
-- =============================================
PRINT '===== KIỂM TRA DỮ LIỆU =====';
PRINT '';
PRINT 'Tổng số khách hàng:';
SELECT COUNT(*) AS 'Số KH' FROM KHACHHANG;

PRINT 'Tổng số nhân viên:';
SELECT COUNT(*) AS 'Số NV' FROM NHANVIEN;

PRINT 'Tổng số sản phẩm:';
SELECT COUNT(*) AS 'Số SP' FROM SANPHAM;

PRINT 'Tổng số hóa đơn:';
SELECT COUNT(*) AS 'Số HĐ' FROM HOADON;

PRINT 'Tổng số nhà cung cấp:';
SELECT COUNT(*) AS 'Số NCC' FROM NHACUNGCAP;

PRINT 'Tổng doanh thu:';
SELECT CONVERT(VARCHAR(20), SUM(TongTien), 1) AS 'Tổng Doanh Thu' FROM HOADON;

PRINT '';
PRINT '✅ Dữ liệu mẫu đã được thêm thành công!';
GO

