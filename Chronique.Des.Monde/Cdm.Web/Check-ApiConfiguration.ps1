# Script de Validation de Configuration HttpClient
# Usage: .\Check-ApiConfiguration.ps1

Write-Host "🔍 Vérification de la configuration API..." -ForegroundColor Cyan

# Vérifier que l'API Service est accessible
$apiUrl = "https://localhost:7428"
$webUrl = "https://localhost:7153"

Write-Host "📡 Test de connectivité vers $apiUrl..." -ForegroundColor Yellow

try {
    # Test simple de connectivité (ignore les erreurs SSL pour le dev)
    $response = Invoke-WebRequest -Uri "$apiUrl/health" -Method GET -SkipCertificateCheck -TimeoutSec 5 -ErrorAction SilentlyContinue
    if ($response.StatusCode -eq 200) {
        Write-Host "✅ API Service accessible sur $apiUrl" -ForegroundColor Green
    } else {
        Write-Host "⚠️  API Service répond mais status: $($response.StatusCode)" -ForegroundColor Yellow
    }
} catch {
    Write-Host "❌ API Service non accessible sur $apiUrl" -ForegroundColor Red
    Write-Host "   Assurez-vous que Cdm.ApiService est démarré" -ForegroundColor Red
}

# Vérifier les fichiers de configuration
Write-Host "📁 Vérification des fichiers de configuration..." -ForegroundColor Yellow

$files = @(
    "Cdm.Web\appsettings.json",
    "Cdm.Web\appsettings.Development.json",
    "Cdm.Web\Program.cs"
)

foreach ($file in $files) {
    if (Test-Path $file) {
        Write-Host "✅ $file existe" -ForegroundColor Green
    } else {
        Write-Host "❌ $file manquant" -ForegroundColor Red
    }
}

# Vérifier la configuration dans appsettings
if (Test-Path "Cdm.Web\appsettings.Development.json") {
    $config = Get-Content "Cdm.Web\appsettings.Development.json" | ConvertFrom-Json
    if ($config.Services.ApiService.Url) {
        Write-Host "✅ URL API configurée: $($config.Services.ApiService.Url)" -ForegroundColor Green
    } else {
        Write-Host "❌ URL API non configurée dans appsettings.Development.json" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "🚀 Instructions de démarrage:" -ForegroundColor Cyan
Write-Host "1. Démarrer l'API Service: cd Cdm.ApiService && dotnet run" -ForegroundColor White
Write-Host "2. Démarrer le Web Frontend: cd Cdm.Web && dotnet run" -ForegroundColor White
Write-Host "3. Tester l'inscription sur: $webUrl/register" -ForegroundColor White

Write-Host ""
Write-Host "🐛 En cas d'erreur BaseAddress:" -ForegroundColor Yellow
Write-Host "- Vérifier que l'API Service tourne sur le bon port" -ForegroundColor White
Write-Host "- Regarder les logs au démarrage du Web Frontend" -ForegroundColor White
Write-Host "- L'ApiService a maintenant une correction automatique" -ForegroundColor White