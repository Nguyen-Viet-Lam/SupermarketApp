# Script to install EPPlus package
$packageName = "EPPlus"
$version = "4.5.3.3"

# Download EPPlus package
$url = "https://www.nuget.org/api/v2/package/$packageName/$version"
$tempPath = "$env:TEMP\$packageName.$version.nupkg"
$extractPath = "$env:TEMP\$packageName.$version"

# Download package
Write-Host "Downloading $packageName $version..."
Invoke-WebRequest -Uri $url -OutFile $tempPath

# Extract package
Write-Host "Extracting package..."
Expand-Archive -Path $tempPath -DestinationPath $extractPath -Force

# Copy DLL to packages folder
$packagesPath = "packages\$packageName.$version"
$libPath = "$extractPath\lib\net40\EPPlus.dll"

if (Test-Path $libPath) {
    if (!(Test-Path $packagesPath)) {
        New-Item -ItemType Directory -Path $packagesPath -Force
    }
    Copy-Item $libPath "$packagesPath\EPPlus.dll" -Force
    Write-Host "EPPlus.dll copied to $packagesPath"
} else {
    Write-Host "EPPlus.dll not found in extracted package"
}

# Cleanup
Remove-Item $tempPath -Force
Remove-Item $extractPath -Recurse -Force

Write-Host "EPPlus installation completed!"





