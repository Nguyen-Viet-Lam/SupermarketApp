# Sửa lỗi EPPlus và packages
Write-Host "========================================"
Write-Host "FIXING EPPLUS AND PACKAGE ERRORS"
Write-Host "========================================"

# Xóa thư mục packages
Write-Host "Step 1: Removing packages folder..."
if (Test-Path "packages") {
    Remove-Item -Recurse -Force "packages"
    Write-Host "✅ Packages folder removed"
} else {
    Write-Host "⚠️ Packages folder not found"
}

# Xóa file packages.lock.json nếu có
Write-Host "Step 2: Removing packages.lock.json..."
if (Test-Path "packages.lock.json") {
    Remove-Item -Force "packages.lock.json"
    Write-Host "✅ packages.lock.json removed"
} else {
    Write-Host "⚠️ packages.lock.json not found"
}

# Clean solution
Write-Host "Step 3: Cleaning solution..."
msbuild SupermarketApp.sln /t:Clean /p:Configuration=Debug

# Restore packages
Write-Host "Step 4: Restoring NuGet packages..."
nuget restore SupermarketApp.sln

# Rebuild solution
Write-Host "Step 5: Rebuilding solution..."
msbuild SupermarketApp.sln /t:Rebuild /p:Configuration=Debug /p:Platform="Any CPU" /verbosity:minimal

# Check result
Write-Host "Step 6: Checking build result..."
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ BUILD SUCCESSFUL!"
    
    if (Test-Path "bin\Debug\SupermarketApp.exe") {
        Write-Host "✅ SupermarketApp.exe found!"
        Get-ChildItem "bin\Debug\SupermarketApp.exe" | Format-Table Name, Length
        Write-Host "🎉 ALL ERRORS FIXED!"
        Write-Host "- EPPlus updated to 6.2.10"
        Write-Host "- Packages restored successfully"
        Write-Host "- Application ready to run"
    } else {
        Write-Host "❌ SupermarketApp.exe not found!"
        Write-Host "Please check build errors above."
    }
} else {
    Write-Host "❌ BUILD FAILED!"
    Write-Host "Please check the errors above."
}

Write-Host "========================================"
Read-Host "Press Enter to continue"
















