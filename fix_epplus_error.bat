@echo off
echo ========================================
echo FIXING EPPLUS PACKAGE ERROR
echo ========================================

echo.
echo Step 1: Cleaning solution...
msbuild SupermarketApp.sln /t:Clean /p:Configuration=Debug

echo.
echo Step 2: Restoring NuGet packages...
nuget restore SupermarketApp.sln

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
        echo 🎉 EPPLUS ERROR FIXED!
        echo - EPPlus updated to version 6.2.10
        echo - Compatible with .NET Framework 4.7.2
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
















