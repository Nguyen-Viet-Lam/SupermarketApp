# Script để replace UIMessageBox thành MessageHelper trong tất cả các Forms
$files = Get-ChildItem -Path ".\Forms" -Filter "*.cs" -Exclude "LoginForm.cs","RegisterForm.cs"

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw -Encoding UTF8
    
    # Thêm using directive nếu chưa có
    if ($content -notmatch "using SupermarketApp\.Utils;") {
        $content = $content -replace "(using SupermarketApp\.Services;)", "`$1`r`nusing SupermarketApp.Utils;"
    }
    
    # Replace các phương thức
    $content = $content -replace "UIMessageBox\.ShowSuccess\(", "MessageHelper.ShowSuccess("
    $content = $content -replace "UIMessageBox\.ShowWarning\(", "MessageHelper.ShowWarning("
    $content = $content -replace "UIMessageBox\.ShowError\(", "MessageHelper.ShowError("
    $content = $content -replace "UIMessageBox\.ShowInfo\(", "MessageHelper.ShowInfo("
    $content = $content -replace "UIMessageBox\.ShowAsk\(", "MessageHelper.ShowAsk("
    $content = $content -replace "UIMessageBox\.Show\(([^,]+),\s*([^,]+),", "MessageHelper.Show(`$1, `$2"
    
    # Save file với UTF-8 encoding
    [System.IO.File]::WriteAllText($file.FullName, $content, [System.Text.Encoding]::UTF8)
    
    Write-Host "✓ Updated: $($file.Name)" -ForegroundColor Green
}

Write-Host "`n✅ Đã replace xong tất cả UIMessageBox thành MessageHelper!" -ForegroundColor Green







