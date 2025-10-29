@echo off
echo ========================================
echo FIXING SYNTAX ERROR IN DATASEEDERUPDATE
echo ========================================

echo.
echo Step 1: Cleaning solution...
msbuild SupermarketApp.sln /t:Clean /p:Configuration=Debug

echo.
echo Step 2: Rebuilding solution...
msbuild SupermarketApp.sln /t:Rebuild /p:Configuration=Debug /p:Platform="Any CPU" /verbosity:minimal

echo.
echo Step 3: Checking build result...
if %ERRORLEVEL% EQU 0 (
    echo ✅ BUILD SUCCESSFUL!
    echo.
    echo Checking for exe file...
    if exist "bin\Debug\SupermarketApp.exe" (
        echo ✅ SupermarketApp.exe found!
        echo File size:
        dir "bin\Debug\SupermarketApp.exe"
        echo.
        echo 🎉 SYNTAX ERROR FIXED!
        echo - DataSeederUpdate.cs syntax corrected
        echo - All braces properly closed
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
















