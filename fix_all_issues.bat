@echo off
echo ========================================
echo FIXING SUPERMARKETAPP PACKAGES ISSUES
echo ========================================

echo.
echo Step 1: Restoring NuGet packages...
nuget restore SupermarketApp.sln

echo.
echo Step 2: Cleaning solution...
msbuild SupermarketApp.sln /t:Clean /p:Configuration=Debug

echo.
echo Step 3: Rebuilding solution...
msbuild SupermarketApp.sln /t:Rebuild /p:Configuration=Debug /p:Platform="Any CPU" /verbosity:minimal

echo.
echo Step 4: Checking build result...
if %ERRORLEVEL% EQU 0 (
    echo ✅ BUILD SUCCESSFUL!
    echo.
    echo Checking for exe file...
    if exist "bin\Debug\SupermarketApp.exe" (
        echo ✅ SupermarketApp.exe found!
        echo File size:
        dir "bin\Debug\SupermarketApp.exe"
        echo.
        echo 🎉 ALL PACKAGES ISSUES FIXED!
        echo - Package versions corrected
        echo - No more "Package not found" errors
        echo - Application ready to run
    ) else (
        echo ❌ SupermarketApp.exe not found!
        echo Please check build errors above.
    )
) else (
    echo ❌ BUILD FAILED!
    echo Please check the errors above.
)

echo.
echo ========================================
pause
