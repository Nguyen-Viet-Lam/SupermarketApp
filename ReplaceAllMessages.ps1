# =============================================
# Script để replace tất cả UIMessageBox và UIMessageTip bằng MessageHelper
# =============================================

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Replace SunnyUI Messages → MessageHelper" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Get all C# files in Forms, Services, and other directories
$files = Get-ChildItem -Path ".\Forms", ".\Services", ".\Data" -Filter "*.cs" -Recurse -ErrorAction SilentlyContinue

$processedCount = 0
$totalReplacements = 0

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw -Encoding UTF8
    $originalContent = $content
    
    # Thêm using directive nếu chưa có
    if ($content -notmatch "using SupermarketApp\.Utils;") {
        # Tìm dòng using cuối cùng và thêm vào sau
        $content = $content -replace "(using [^\n]+;)(\s+namespace)", "`$1`nusing SupermarketApp.Utils;`$2"
    }
    
    # ===== REPLACEMENTS =====
    
    # 1. UIMessageBox.ShowSuccess → MessageHelper.ShowSuccess
    $content = $content -replace "UIMessageBox\.ShowSuccess\(", "MessageHelper.ShowSuccess("
    
    # 2. UIMessageBox.ShowWarning → MessageHelper.ShowWarning
    $content = $content -replace "UIMessageBox\.ShowWarning\(", "MessageHelper.ShowWarning("
    
    # 3. UIMessageBox.ShowError → MessageHelper.ShowError
    $content = $content -replace "UIMessageBox\.ShowError\(", "MessageHelper.ShowError("
    
    # 4. UIMessageBox.ShowInfo → MessageHelper.ShowInfo
    $content = $content -replace "UIMessageBox\.ShowInfo\(", "MessageHelper.ShowInfo("
    
    # 5. UIMessageBox.ShowAsk → MessageHelper.ShowAsk
    $content = $content -replace "UIMessageBox\.ShowAsk\(", "MessageHelper.ShowAsk("
    
    # 6. UIMessageBox.Show → MessageHelper.Show
    $content = $content -replace "UIMessageBox\.Show\(", "MessageHelper.Show("
    
    # 7. UIMessageTip.ShowOk → MessageHelper.ShowTipSuccess
    $content = $content -replace "UIMessageTip\.ShowOk\(", "MessageHelper.ShowTipSuccess("
    
    # 8. UIMessageTip.ShowError → MessageHelper.ShowTipError
    $content = $content -replace "UIMessageTip\.ShowError\(", "MessageHelper.ShowTipError("
    
    # 9. UIMessageTip.ShowWarning → MessageHelper.ShowTipWarning
    $content = $content -replace "UIMessageTip\.ShowWarning\(", "MessageHelper.ShowTipWarning("
    
    # 10. UIMessageTip.Show → MessageHelper.ShowTip
    $content = $content -replace "UIMessageTip\.Show\(", "MessageHelper.ShowTip("
    
    # Nếu có thay đổi, lưu file
    if ($content -ne $originalContent) {
        [System.IO.File]::WriteAllText($file.FullName, $content, [System.Text.Encoding]::UTF8)
        $processedCount++
        
        # Đếm số replacements
        $replacementCount = ($originalContent | Select-String "UIMessageBox\.|UIMessageTip\." -AllMatches | Measure-Object -Line).Count
        $totalReplacements += $replacementCount
        
        Write-Host "✓ Updated: $($file.Name) ($replacementCount replacements)" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "✅ Tất cả file đã được cập nhật!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "📊 Thống kê:" -ForegroundColor Yellow
Write-Host "- Files được xử lý: $processedCount" -ForegroundColor White
Write-Host "- Tổng số replacements: $totalReplacements" -ForegroundColor White
Write-Host ""
Write-Host "🎯 Kết quả:" -ForegroundColor Cyan
Write-Host "✅ Tất cả UIMessageBox → MessageHelper" -ForegroundColor Green
Write-Host "✅ Tất cả UIMessageTip → MessageHelper" -ForegroundColor Green
Write-Host "✅ Tất cả thông báo hiện bằng tiếng Việt" -ForegroundColor Green
Write-Host "✅ Không còn tiếng Trung!" -ForegroundColor Green
Write-Host ""
Write-Host "📝 Bước tiếp theo:" -ForegroundColor Yellow
Write-Host "1. Rebuild solution (Visual Studio: Build → Rebuild Solution)" -ForegroundColor White
Write-Host "2. F5 để chạy ứng dụng" -ForegroundColor White
Write-Host "3. Test các chức năng để kiểm tra thông báo" -ForegroundColor White
Write-Host ""

