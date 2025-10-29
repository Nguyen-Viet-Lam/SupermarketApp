@echo off
echo ==========================================
echo Xoa khach hang tu ID 10 tro di
echo ==========================================

sqlcmd -S localhost -d SupermarketDB -E -Q "DELETE FROM CTHOADON WHERE MaHD IN (SELECT MaHD FROM HOADON WHERE MaKH IN (SELECT MaKH FROM KHACHHANG WHERE MaKH > 9));"
sqlcmd -S localhost -d SupermarketDB -E -Q "DELETE FROM HOADON WHERE MaKH IN (SELECT MaKH FROM KHACHHANG WHERE MaKH > 9);"
sqlcmd -S localhost -d SupermarketDB -E -Q "DELETE FROM KHACHHANG WHERE MaKH > 9;"
sqlcmd -S localhost -d SupermarketDB -E -Q "SELECT COUNT(*) as SoKhachHangConLai FROM KHACHHANG;"

echo.
echo ==========================================
echo Hoan thanh!
echo ==========================================
pause


