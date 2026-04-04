# JunimoModder: Lokaler Cross-Platform-Build für Windows/Android/macOS (.NET 9)
# Ausführen im Root des Repositories!

# .NET 9-Erkennung und Install-Hinweis
$dotnetVersion = & dotnet --list-sdks | Select-String "^9"
if (-not $dotnetVersion) {
    Write-Host ".NET 9 SDK nicht gefunden! Bitte von https://dotnet.microsoft.com/download/dotnet/9.0 installieren."
    exit 1
}
else {
    Write-Host ".NET 9 SDK gefunden."
}

# MAUI-Workloads installieren
Write-Host "Installiere MAUI-Workloads..."
dotnet workload install maui maui-android maui-windows maui-maccatalyst wasm-tools --skip-sign-check --source https://api.nuget.org/v3/index.json

# Plattform-Erkennung für Builds
function Is-MacOS {
    return ($IsMacOS -or $env:OSTYPE -like "*darwin*")
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
    Write-Host "`n=== macOS Build ==="
    dotnet restore JunimoModder.MacOS/JunimoModder.MacOS.csproj
    dotnet publish JunimoModder.MacOS/JunimoModder.MacOS.csproj -c Release -o output-mac
    Write-Host "macOS Build: output-mac (App)"
}

Write-Host "`nAlle Builds abgeschlossen. Die fertigen Pakete liegen in den jeweiligen output-* Ordnern."
