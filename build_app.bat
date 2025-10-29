@echo off
echo Building SupermarketApp...
cd /d "C:\Users\ASUS\OneDrive\Máy tính\QuanLyBanHnagSieuThi\QuanLyBanHnagSieuThi\SupermarketApp"
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" SupermarketApp.sln /p:Configuration=Debug /p:Platform="Any CPU" /verbosity:minimal
if %ERRORLEVEL% EQU 0 (
    echo Build successful!
    echo Checking for exe file...
    if exist "bin\Debug\SupermarketApp.exe" (
        echo SupermarketApp.exe found!
        dir "bin\Debug\SupermarketApp.exe"
    ) else (
        echo SupermarketApp.exe not found!
    )
) else (
    echo Build failed!
)
pause

