# PowerShell script to update all fonts to Segoe UI 10pt
# This will replace Microsoft Sans Serif with Segoe UI across all forms

Write-Host "Starting font update process..." -ForegroundColor Cyan

# Get all .cs files in Forms directory
$files = Get-ChildItem -Path "Forms" -Filter "*.cs" -Recurse

$totalFiles = $files.Count
$processedFiles = 0
$totalReplacements = 0

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw -Encoding UTF8
    $originalContent = $content
    $fileReplacements = 0
    
    # Pattern 1: Replace Font = new Font("Microsoft Sans Serif", XF, ...
    # Match various patterns with or without GraphicsUnit
    $content = $content -replace 'new Font\("Microsoft Sans Serif", (\d+F), (FontStyle\.\w+), System\.Drawing\.GraphicsUnit\.Point, \(\(byte\)\(0\)\)\)', {
        param($match)
        $fileReplacements++
        $style = $matches[1] -replace 'FontStyle\.', ''
        if ($style -eq 'Bold') {
            return 'new Font("Segoe UI", 10, FontStyle.Bold)'
        }
        return 'new Font("Segoe UI", 10, FontStyle.Regular)'
    }
    
    # Pattern 2: Font = new Font("Microsoft Sans Serif", XF) - Simple pattern
    $content = $content -replace 'new Font\("Microsoft Sans Serif", \d+F\)', {
        param($match)
        $fileReplacements++
        return 'new Font("Segoe UI", 10, FontStyle.Regular)'
    }
    
    # Pattern 3: Font = new Font("Microsoft Sans Serif", XF, FontStyle.Y) - with style
    $content = $content -replace 'new Font\("Microsoft Sans Serif", (\d+F), (FontStyle\.\w+)\)', {
        param($match)
        $fileReplacements++
        $style = $matches[2] -replace 'FontStyle\.', ''
        if ($style -eq 'Bold') {
            return 'new Font("Segoe UI", 10, FontStyle.Bold)'
        }
        return 'new Font("Segoe UI", 10, FontStyle.Regular)'
    }
    
    # Pattern 4: Font with GraphicsUnit specified but no FontStyle
    $content = $content -replace 'new Font\("Microsoft Sans Serif", \d+F, System\.Drawing\.GraphicsUnit\.Point\)', {
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
Write-Host "=" * 60 -ForegroundColor Cyan
Write-Host "Font Update Complete!" -ForegroundColor Green
Write-Host "Processed: $processedFiles/$totalFiles files" -ForegroundColor Yellow
Write-Host "Total replacements: $totalReplacements" -ForegroundColor Yellow
Write-Host "=" * 60 -ForegroundColor Cyan



