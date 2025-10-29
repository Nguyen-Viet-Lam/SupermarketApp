@echo off
echo ========================================
echo FIXING EPPLUS 6.2.10
echo ========================================

echo.
echo Step 1: Removing old packages...
if exist "packages\EPPlus.5.8.12" (
    rmdir /s /q "packages\EPPlus.5.8.12"
    echo ✅ Removed EPPlus.5.8.12
)
if exist "packages\EPPlus.4.7.3" (
    rmdir /s /q "packages\EPPlus.4.7.3"
    echo ✅ Removed EPPlus.4.7.3
)

echo.
echo ========================================
echo ✅ EPPLUS 6.2.10 CONFIGURED
echo ========================================
echo.
echo Now please perform the following in Visual Studio:
echo.
echo 1. Build → Clean Solution
echo 2. Right-click solution → Restore NuGet Packages
echo 3. Build → Rebuild Solution
echo 4. Press F5 to run
echo.
echo ========================================
pause
















