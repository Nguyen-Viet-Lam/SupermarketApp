@echo off
echo ========================================
echo FIXING ALL PACKAGE ERRORS
echo ========================================

echo.
echo Step 1: Removing packages folder...
if exist "packages" (
    rmdir /s /q packages
    echo ✅ Packages folder removed
) else (
    echo ⚠️ Packages folder not found
)

echo.
echo Step 2: Removing packages.lock.json...
if exist "packages.lock.json" (
    del packages.lock.json
    echo ✅ packages.lock.json removed
) else (
    echo ⚠️ packages.lock.json not found
)

echo.
echo Step 3: Cleaning solution...
msbuild SupermarketApp.sln /t:Clean /p:Configuration=Debug

echo.
echo Step 4: Restoring NuGet packages...
nuget restore SupermarketApp.sln

echo.
echo Step 5: Rebuilding solution...
msbuild SupermarketApp.sln /t:Rebuild /p:Configuration=Debug /p:Platform="Any CPU" /verbosity:minimal

echo.
echo Step 6: Checking build result...
if %ERRORLEVEL% EQU 0 (
    echo ✅ BUILD SUCCESSFUL!
    echo.
    echo Checking for exe file...
    if exist "bin\Debug\SupermarketApp.exe" (
        echo ✅ SupermarketApp.exe found!
        echo File size:
        dir "bin\Debug\SupermarketApp.exe"
        echo.
        echo 🎉 ALL ERRORS FIXED!
        echo - EPPlus updated to 6.2.10
        echo - Packages restored successfully
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
















