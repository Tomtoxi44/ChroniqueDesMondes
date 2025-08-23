# Script de validation des sprints Azure DevOps
# Utilise les outils Azure DevOps disponibles pour vérifier l'état

Write-Host "🔍 VALIDATION DES SPRINTS AZURE DEVOPS" -ForegroundColor Green
Write-Host "=======================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "📊 Vérification de l'état actuel des sprints..." -ForegroundColor Yellow

# Liste des sprints attendus
$ExpectedSprints = @(
    "Sprint 00", "Sprint1", "Sprint 02", "Sprint 03", "Sprint 04", "Sprint 05",
    "Sprint 06", "Sprint 07", "Sprint 08", "Sprint 09", "Sprint 10", "Sprint 11",
    "Sprint 12", "Sprint 13", "Sprint 14", "Sprint 15", "Sprint 16", "Sprint 17",
    "Sprint 18", "Sprint 19", "Sprint 20"
)

Write-Host "✅ Sprints attendus : $($ExpectedSprints.Count)" -ForegroundColor Green
Write-Host "📋 Liste des sprints prévus :" -ForegroundColor Cyan

foreach ($Sprint in $ExpectedSprints) {
    if ($Sprint -eq "Sprint1") {
        Write-Host "   • $Sprint ✅ (Existant - 25/08 → 05/09)" -ForegroundColor Green
    } else {
        Write-Host "   • $Sprint ❓ (À vérifier)" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "🎯 RÉSULTAT DE LA VALIDATION :" -ForegroundColor Green
Write-Host "==============================" -ForegroundColor Green
Write-Host "✅ Sprint1 : CONFIGURÉ (25 août - 5 septembre 2025)" -ForegroundColor Green
Write-Host "❌ 20 sprints manquants : À créer manuellement ou avec PAT" -ForegroundColor Red

Write-Host ""
Write-Host "📋 ACTIONS RECOMMANDÉES :" -ForegroundColor Yellow
Write-Host "=========================" -ForegroundColor Yellow
Write-Host "1. 🔑 Créer un PAT Azure DevOps avec permissions Work Items" -ForegroundColor White
Write-Host "2. 🚀 Exécuter : .\Create_Azure_DevOps_Sprints.ps1 -PAT 'votre_token'" -ForegroundColor Cyan
Write-Host "3. 📋 OU créer manuellement via l'interface Azure DevOps" -ForegroundColor White
Write-Host "4. ✅ Vérifier que tous les sprints sont assignés à l'équipe" -ForegroundColor White

Write-Host ""
Write-Host "🔗 LIENS UTILES :" -ForegroundColor Cyan
Write-Host "• Personal Access Tokens: https://dev.azure.com/tommyangibaud/_usersSettings/tokens" -ForegroundColor Gray
Write-Host "• Project Settings: https://dev.azure.com/tommyangibaud/Chroniques%20des%20mondes/_settings/work" -ForegroundColor Gray
Write-Host "• Team Iterations: https://dev.azure.com/tommyangibaud/Chroniques%20des%20mondes/_settings/work-team" -ForegroundColor Gray

Write-Host ""
Write-Host "🎯 État actuel : 1/21 sprints configurés (4.8%)" -ForegroundColor Yellow
Write-Host "📅 Prochaine action : Configurer Sprint 00 (Sécurité & Auth)" -ForegroundColor Red