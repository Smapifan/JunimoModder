# Powershell-Skript, lokal ausführen
# Benötigt: Windows 10/11, macOS, oder Ubuntu je nachdem!
# Lädt .NET 8, 9, 10, Java/AndroidSDK (nur für Android), baut alle Bundles

# .NET SDK Install (ändert nichts wenn schon installiert)
$dotnetVersions = @('8.0', '9.0', '10.0')
foreach ($v in $dotnetVersions) {
    &"dotnet" --list-sdks | Select-String "^$v" | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Installiere .NET SDK $v..."
        Invoke-WebRequest -Uri "https://dotnet.microsoft.com/download/dotnet/$v" -OutFile "dotnet-installer-$v.exe"
        Start-Process -Wait -FilePath "dotnet-installer-$v.exe" -ArgumentList "/install", "/quiet"
    }
}

# Android: Java & Android Command Line Tools laden
if ($env:OS -eq "Windows_NT") {
    # Beispiel für Windows – auf macOS/Ubuntu bitte mit brew/sdkmanager/jdk portable anpassen
    choco install openjdk android-sdk -y
}

# Restore
dotnet restore

# Build Windows
if ($env:OS -eq "Windows_NT") {
    dotnet publish -f:net8.0-windows10.0.19041.0 -c Release -o output-win
    Write-Host "Windows .exe in output-win"
}
# Build Android – vorausgesetzt: Java/AndroidSDK installiert
dotnet publish -f:net8.0-android -c Release -o output-android
Write-Host "Android .apk in output-android"

# Build MacCatalyst
if ($IsMacOS) {
    dotnet publish -f:net8.0-maccatalyst -c Release -o output-mac
    Write-Host "Mac .app in output-mac"
}
