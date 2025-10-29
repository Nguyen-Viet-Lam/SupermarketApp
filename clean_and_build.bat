@echo off
REM ========================================
REM Clean SupermarketApp Project
REM ========================================

echo Cleaning SupermarketApp...
echo.

REM Get current directory
cd /d "%~dp0"

REM Delete bin directory
if exist "bin" (
    echo Deleting bin directory...
    rmdir /s /q bin
    echo bin directory deleted successfully.
) else (
    echo bin directory not found.
)

echo.

REM Delete obj directory
if exist "obj" (
    echo Deleting obj directory...
    rmdir /s /q obj
    echo obj directory deleted successfully.
) else (
    echo obj directory not found.
)

echo.
echo ========================================
echo Clean completed successfully!
echo ========================================
echo.
echo Next steps:
echo 1. Open Visual Studio
echo 2. Build menu: Clean Solution
echo 3. Build menu: Rebuild Solution
echo 4. Press F5 to run
echo.
pause

