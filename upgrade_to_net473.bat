@echo off
echo ========================================
echo UPGRADING TO .NET FRAMEWORK 4.7.3
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
        echo 🎉 UPGRADE TO .NET 4.7.3 SUCCESSFUL!
        echo - Target framework: .NET Framework 4.7.3
        echo - Packages updated for compatibility
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
















