@echo off
echo ========================================
echo Updating NuGet Packages (Security Fix)
echo ========================================
echo.

echo Removing old packages...
rmdir /s /q packages 2>nul

echo.
echo Restoring packages with updated versions...
nuget restore SupermarketApp.sln

echo.
echo ========================================
echo Package Update Complete!
echo ========================================
echo.
echo Updated packages:
echo - Microsoft.Data.SqlClient: 5.1.1 -> 5.2.2 (Fix CVE vulnerability)
echo - Microsoft.IdentityModel.JsonWebTokens: 7.0.3 -> 8.2.1 (Fix CVE vulnerability)
echo - System.IdentityModel.Tokens.Jwt: 7.0.3 -> 8.2.1 (Fix CVE vulnerability)
echo.
echo Please rebuild the solution in Visual Studio.
echo.
pause








