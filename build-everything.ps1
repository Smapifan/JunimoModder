# JunimoModder Cross-Platform Buildscript lokal
# Dieses Skript lädt alle Tools (soweit möglich) und baut für Windows, Android, Mac.
# Funktioniert aus PowerShell heraus, Windows/macOS bevorzugt.

# 1. .NET 8, 9, 10 installieren (falls nötig)
$dotnetVersions = @('8.0', '9.0', '10.0')
foreach ($v in $dotnetVersions) {
    $sdkFound = &dotnet --list-sdks | Select-String "^$v"
    if (-not $sdkFound) {
        Write-Host "Bitte .NET SDK $v manuell installieren. Download unter https://dotnet.microsoft.com/download/dotnet/$v"
        Start-Sleep -Seconds 2
    } else {
        Write-Host ".NET SDK $v ist vorhanden."
    }
}

# 2. MAUI-Workloads installieren
Write-Host "Installiere MAUI-Workloads (dies kann dauern)..."
& dotnet workload install maui maui-android maui-maccatalyst maui-windows wasm-tools --skip-sign-check --source https://api.nuget.org/v3/index.json

# 3. Für Android: Java & Android-SDK (nur Windows Beispiel)
if ($env:OS -eq "Windows_NT") {
    if (-not (Get-Command "javac" -ErrorAction SilentlyContinue)) {
        Write-Host "Bitte Java 17 installieren: https://adoptium.net/temurin/releases/?version=17"
    }
    # Android SDK kann mit Android Studio oder Befehl extra geladen werden
    Write-Host "Bitte Android SDK (command line tools) installieren, falls nicht vorhanden: https://developer.android.com/studio"
}

# 4. NuGet Restore
dotnet restore

# 5. Builds
# Windows (wenn Windows)
if ($env:OS -eq "Windows_NT") {
    Write-Host "Baue Windows EXE..."
    dotnet publish -f:net8.0-windows10.0.19041.0 -c Release -o output-win
    Write-Host "EXE in output-win"
}

# Android
Write-Host "Baue Android APK/AAB..."
dotnet publish -f:net8.0-android -c Release -o output-android
Write-Host "APK/AAB in output-android"

# Mac Catalyst (wenn macOS)
if ($IsMacOS -or $env:OSTYPE -like "*darwin*") {
    Write-Host "Baue Mac Catalyst App..."
    dotnet publish -f:net8.0-maccatalyst -c Release -o output-mac
    Write-Host "App-Bundle in output-mac"
}

Write-Host "Alle Builds abgeschlossen."
