# PowerShell script to update all fonts to Segoe UI 10pt
# This will replace Microsoft Sans Serif with Segoe UI across all forms

Write-Host "Starting font update process..." -ForegroundColor Cyan

# Get all .cs files in Forms directory
$files = Get-ChildItem -Path "Forms" -Filter "*.cs"

$totalFiles = $files.Count
$processedFiles = 0
$totalReplacements = 0

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw -Encoding UTF8
    $originalContent = $content
    $fileReplacements = 0
    
    # Replace all instances of "Microsoft Sans Serif" with "Segoe UI"
    # Match pattern: Font = new Font("Microsoft Sans Serif", XF or just X, ...)
    
    # Pattern 1: Full constructor with GraphicsUnit
    # Example: new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
    $content = $content -replace 'new Font\("Microsoft Sans Serif", \d+F?, FontStyle\.\w+, System\.Drawing\.GraphicsUnit\.Point, \(\(byte\)\(0\)\)\)', {
        param($match)
        $line = $match.ToString()
        $fileReplacements++
        if ($line -match 'FontStyle\.(Bold|Italic|Underline|Strikeout)') {
            $style = $matches[1]
            return "new Font(`"Segoe UI`", 10, FontStyle.$style)"
        }
        return 'new Font("Segoe UI", 10, FontStyle.Regular)'
    }
    
    # Pattern 2: Simple constructor without GraphicsUnit
    # Example: new Font("Microsoft Sans Serif", 12F)
    $content = $content -replace 'new Font\("Microsoft Sans Serif", \d+F?\)', {
        param($match)
        $fileReplacements++
        return 'new Font("Segoe UI", 10, FontStyle.Regular)'
    }
    
    # Pattern 3: With FontStyle only (most common in DataGridView)
    # Example: new Font("Microsoft Sans Serif", 12F, FontStyle.Bold)
    $content = $content -replace 'new Font\("Microsoft Sans Serif", \d+F?, FontStyle\.(\w+)\)', {
        param($match)
        $style = $matches[1]
        $fileReplacements++
        return "new Font(`"Segoe UI`", 10, FontStyle.$style)"
    }
    
    # Pattern 4: With GraphicsUnit but no FontStyle
    # Example: new Font("Microsoft Sans Serif", 12F, System.Drawing.GraphicsUnit.Point)
    $content = $content -replace 'new Font\("Microsoft Sans Serif", \d+F?, System\.Drawing\.GraphicsUnit\.Point\)', {
        param($match)
        $fileReplacements++
        return 'new Font("Segoe UI", 10, FontStyle.Regular)'
    }
    
    # If content changed, write the file
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -Encoding UTF8 -NoNewline
        $processedFiles++
        $totalReplacements += $fileReplacements
        Write-Host "✓ Updated: $($file.Name) ($fileReplacements replacements)" -ForegroundColor Green
    } else {
        Write-Host "⊘ No changes: $($file.Name)" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "==================================================================" -ForegroundColor Cyan
Write-Host "Font Update Complete!" -ForegroundColor Green
Write-Host "Processed files: $processedFiles/$totalFiles" -ForegroundColor Yellow
Write-Host "Total replacements: $totalReplacements" -ForegroundColor Yellow
Write-Host "==================================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "All fonts have been updated to:" -ForegroundColor White
Write-Host "  - Font Family: Segoe UI" -ForegroundColor Cyan
Write-Host "  - Font Size: 10" -ForegroundColor Cyan
Write-Host "  - Font Style: Regular (or Bold when specified)" -ForegroundColor Cyan



