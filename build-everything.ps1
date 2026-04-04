# JunimoModder: Lokales Cross-Platform-Build-Skript für Windows/Android/macOS (.NET MAUI 9/8 auf Actions)
# Ausführen im Repository-Hauptverzeichnis!

function Is-MacOS {
    return ($IsMacOS -or $env:OSTYPE -like "*darwin*")
}

# .NET SDK Erkennung
$netSdk = & dotnet --list-sdks
if ($netSdk -notmatch "9.0") {
    Write-Host ".NET 9 SDK NICHT gefunden! Bitte von https://dotnet.microsoft.com/download/dotnet/9.0 installieren."
    exit 1
}
Write-Host ".NET 9 SDK gefunden."

# MAUI-Workloads für die jeweilige Plattform installieren
Write-Host "Installiere benötigte MAUI-Workloads..."

dotnet workload install maui-android maui-windows wasm-tools --skip-sign-check --source https://api.nuget.org/v3/index.json

if (Is-MacOS) {
    # MacCatalyst mit .NET 8, da .NET 9 nicht unter Actions funktioniert
    $macCatalystSdk = & dotnet --list-sdks | Select-String "^8\.0"
    if (-not $macCatalystSdk) {
        Write-Host ".NET 8 SDK NICHT gefunden! Für MacCatalyst mit .NET 8 brauchst du https://dotnet.microsoft.com/download/dotnet/8.0"
        exit 1
    }
    dotnet workload install maui-maccatalyst wasm-tools --skip-sign-check --source https://api.nuget.org/v3/index.json
}

# ---------- Android ----------
Write-Host "`n=== Android Build ==="
dotnet restore JunimoModder.Android/JunimoModder.Android.csproj
dotnet publish JunimoModder.Android/JunimoModder.Android.csproj -c Release -o output-android
Write-Host "Android Build: output-android (APK/AAB)"

# ---------- Windows ----------
if ($env:OS -eq "Windows_NT") {
    Write-Host "`n=== Windows Build ==="
    dotnet restore JunimoModder.Windows/JunimoModder.Windows.csproj
    dotnet publish JunimoModder.Windows/JunimoModder.Windows.csproj -c Release -o output-win
    Write-Host "Windows Build: output-win (EXE)"
}

# ---------- macOS ----------
if (Is-MacOS) {
    Write-Host "`n=== macOS Build (.NET 8) ==="
    dotnet restore JunimoModder.MacOS/JunimoModder.MacOS.csproj
    dotnet publish JunimoModder.MacOS/JunimoModder.MacOS.csproj -c Release -o output-mac
    Write-Host "macOS Build: output-mac (App)"
}

Write-Host "`nAlle Builds abgeschlossen. Die fertigen Pakete liegen in output-*."
