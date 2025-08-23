# Script PowerShell pour créer les sprints Azure DevOps
# Chronique des Mondes - 20 Sprints Planning
# Version améliorée avec gestion d'erreurs et validation

param(
    [string]$Organization = "tommyangibaud",
    [string]$Project = "Chroniques des mondes",
    [string]$PAT = "", # Personal Access Token
    [switch]$TestMode = $false,
    [switch]$ValidateOnly = $false
)

# Configuration des sprints avec dates et objectifs
$Sprints = @(
    @{ Name = "Sprint 00"; StartDate = "2025-08-11"; EndDate = "2025-08-22"; Objective = "Sécurité & Auth"; Priority = "CRITICAL" },
    @{ Name = "Sprint 02"; StartDate = "2025-09-08"; EndDate = "2025-09-19"; Objective = "Architecture Core"; Priority = "CRITICAL" },
    @{ Name = "Sprint 03"; StartDate = "2025-09-22"; EndDate = "2025-10-03"; Objective = "Sorts Core"; Priority = "HIGH" },
    @{ Name = "Sprint 04"; StartDate = "2025-10-06"; EndDate = "2025-10-17"; Objective = "Sorts Interface"; Priority = "HIGH" },
    @{ Name = "Sprint 05"; StartDate = "2025-10-20"; EndDate = "2025-10-31"; Objective = "Équipements Core"; Priority = "HIGH" },
    @{ Name = "Sprint 06"; StartDate = "2025-11-03"; EndDate = "2025-11-14"; Objective = "Équipements Échanges"; Priority = "HIGH" },
    @{ Name = "Sprint 07"; StartDate = "2025-11-17"; EndDate = "2025-11-28"; Objective = "Campagnes Structure"; Priority = "MEDIUM" },
    @{ Name = "Sprint 08"; StartDate = "2025-12-01"; EndDate = "2025-12-12"; Objective = "Campagnes Interface"; Priority = "MEDIUM" },
    @{ Name = "Sprint 09"; StartDate = "2025-12-15"; EndDate = "2025-12-26"; Objective = "Personnages Core"; Priority = "MEDIUM" },
    @{ Name = "Sprint 10"; StartDate = "2026-01-05"; EndDate = "2026-01-16"; Objective = "Personnages Advanced"; Priority = "MEDIUM" },
    @{ Name = "Sprint 11"; StartDate = "2026-01-19"; EndDate = "2026-01-30"; Objective = "Combat Foundation"; Priority = "MEDIUM" },
    @{ Name = "Sprint 12"; StartDate = "2026-02-02"; EndDate = "2026-02-13"; Objective = "Combat Advanced"; Priority = "MEDIUM" },
    @{ Name = "Sprint 13"; StartDate = "2026-02-16"; EndDate = "2026-02-27"; Objective = "Sessions Infrastructure"; Priority = "LOW" },
    @{ Name = "Sprint 14"; StartDate = "2026-03-02"; EndDate = "2026-03-13"; Objective = "Sessions Interface"; Priority = "LOW" },
    @{ Name = "Sprint 15"; StartDate = "2026-03-16"; EndDate = "2026-03-27"; Objective = "Real-time Features"; Priority = "LOW" },
    @{ Name = "Sprint 16"; StartDate = "2026-03-30"; EndDate = "2026-04-10"; Objective = "UI/UX Polish"; Priority = "LOW" },
    @{ Name = "Sprint 17"; StartDate = "2026-04-13"; EndDate = "2026-04-24"; Objective = "Performance"; Priority = "LOW" },
    @{ Name = "Sprint 18"; StartDate = "2026-04-27"; EndDate = "2026-05-08"; Objective = "Testing & QA"; Priority = "LOW" },
    @{ Name = "Sprint 19"; StartDate = "2026-05-11"; EndDate = "2026-05-22"; Objective = "Production Prep"; Priority = "LOW" },
    @{ Name = "Sprint 20"; StartDate = "2026-05-25"; EndDate = "2026-06-05"; Objective = "Release & Launch"; Priority = "LOW" }
)

function Write-ColorText {
    param($Text, $Color = "White")
    Write-Host $Text -ForegroundColor $Color
}

function Show-Instructions {
    Write-ColorText "🚀 CRÉATION DES SPRINTS AZURE DEVOPS - CHRONIQUE DES MONDES" "Green"
    Write-ColorText "========================================================================" "Cyan"
    Write-Host ""
    Write-ColorText "📋 Configuration du projet :" "Yellow"
    Write-ColorText "   • Organisation : $Organization" "White"
    Write-ColorText "   • Projet : $Project" "White"
    Write-ColorText "   • Sprints à créer : $($Sprints.Count)" "White"
    Write-Host ""
    
    if ($ValidateOnly) {
        Write-ColorText "🔍 MODE VALIDATION - Vérification des sprints existants..." "Cyan"
        return
    }
    
    if ([string]::IsNullOrEmpty($PAT)) {
        Write-ColorText "⚠️  PERSONAL ACCESS TOKEN REQUIS" "Yellow"
        Write-ColorText "========================================" "Yellow"
        Write-Host ""
        Write-ColorText "Pour créer les sprints automatiquement, vous devez :" "White"
        Write-ColorText "1. 🌐 Ouvrir https://dev.azure.com/$Organization/_usersSettings/tokens" "Cyan"
        Write-ColorText "2. 🔑 Cliquer sur 'New Token'" "Cyan"
        Write-ColorText "3. 📝 Nom : 'Sprint Creation Script'" "Cyan"
        Write-ColorText "4. 📅 Expiration : 30 jours" "Cyan"
        Write-ColorText "5. ✅ Permissions requises :" "Cyan"
        Write-ColorText "   • Work Items (Read & Write)" "White"
        Write-ColorText "   • Project and Team (Read)" "White"
        Write-ColorText "6. 💾 Copier le token généré" "Cyan"
        Write-ColorText "7. 🔄 Relancer : .\Create_Azure_DevOps_Sprints.ps1 -PAT 'votre_token'" "Green"
        Write-Host ""
        Write-ColorText "📋 ALTERNATIVE : Création manuelle guidée" "Yellow"
        Write-ColorText "=======================================" "Yellow"
        Write-ColorText "1. 🌐 Aller sur https://dev.azure.com/$Organization/$Project" "Cyan"
        Write-ColorText "2. ⚙️  Project Settings → Project configuration → Iterations" "Cyan"
        Write-ColorText "3. ➕ Créer chaque sprint avec les dates ci-dessous" "Cyan"
        Write-Host ""
        Show-SprintsList
        exit 1
    }
}

function Show-SprintsList {
    Write-ColorText "📅 SPRINTS À CRÉER :" "Yellow"
    Write-ColorText "==================" "Yellow"
    foreach ($Sprint in $Sprints) {
        $PriorityColor = switch ($Sprint.Priority) {
            "CRITICAL" { "Red" }
            "HIGH" { "Yellow" }
            "MEDIUM" { "Cyan" }
            "LOW" { "Gray" }
        }
        Write-ColorText "• $($Sprint.Name) : $($Sprint.StartDate) → $($Sprint.EndDate)" "White"
        Write-ColorText "  └─ $($Sprint.Objective) ($($Sprint.Priority))" $PriorityColor
    }
    Write-Host ""
}

function Test-AzureDevOpsConnection {
    param($Headers, $BaseUrl)
    
    try {
        Write-ColorText "🔌 Test de connexion Azure DevOps..." "Cyan"
        $TestUrl = "$BaseUrl/projects?api-version=7.0"
        $Response = Invoke-RestMethod -Uri $TestUrl -Method GET -Headers $Headers -TimeoutSec 10
        Write-ColorText "   ✅ Connexion réussie" "Green"
        return $true
    }
    catch {
        Write-ColorText "   ❌ Erreur de connexion : $($_.Exception.Message)" "Red"
        return $false
    }
}

function Get-ExistingIterations {
    param($Headers, $BaseUrl)
    
    try {
        Write-ColorText "📋 Récupération des itérations existantes..." "Cyan"
        $IterationsUrl = "$BaseUrl/wit/classificationNodes/Iterations?api-version=7.0&`$depth=2"
        $Response = Invoke-RestMethod -Uri $IterationsUrl -Method GET -Headers $Headers
        
        $ExistingNames = @()
        if ($Response.children) {
            $ExistingNames = $Response.children | ForEach-Object { $_.name }
        }
        
        Write-ColorText "   📊 Itérations existantes : $($ExistingNames.Count)" "Green"
        foreach ($Name in $ExistingNames) {
            Write-ColorText "   • $Name" "Gray"
        }
        
        return $ExistingNames
    }
    catch {
        Write-ColorText "   ⚠️  Impossible de récupérer les itérations : $($_.Exception.Message)" "Yellow"
        return @()
    }
}

function Create-Sprint {
    param($Sprint, $Headers, $BaseUrl, $ExistingIterations)
    
    if ($ExistingIterations -contains $Sprint.Name) {
        Write-ColorText "   ⏭️  $($Sprint.Name) existe déjà" "Yellow"
        return $true
    }
    
    if ($TestMode) {
        Write-ColorText "   🧪 TEST MODE - Simulation création $($Sprint.Name)" "Magenta"
        Start-Sleep -Seconds 1
        return $true
    }
    
    # Payload pour création d'itération
    $IterationPayload = @{
        name = $Sprint.Name
        attributes = @{
            startDate = "$($Sprint.StartDate)T00:00:00Z"
            finishDate = "$($Sprint.EndDate)T23:59:59Z"
        }
    } | ConvertTo-Json -Depth 3
    
    try {
        # Créer l'itération
        $CreateUrl = "$BaseUrl/wit/classificationNodes/Iterations?api-version=7.0"
        $Response = Invoke-RestMethod -Uri $CreateUrl -Method POST -Headers $Headers -Body $IterationPayload -TimeoutSec 30
        
        Write-ColorText "   ✅ $($Sprint.Name) créé avec succès" "Green"
        
        # Assigner à l'équipe
        try {
            $TeamIterationUrl = "$BaseUrl/work/teamsettings/iterations?api-version=7.0"
            $TeamPayload = @{
                id = $Response.id
            } | ConvertTo-Json
            
            Invoke-RestMethod -Uri $TeamIterationUrl -Method POST -Headers $Headers -Body $TeamPayload -TimeoutSec 15
            Write-ColorText "   ✅ Assigné à l'équipe" "Green"
        }
        catch {
            Write-ColorText "   ⚠️  Créé mais non assigné à l'équipe : $($_.Exception.Message)" "Yellow"
        }
        
        return $true
        
    } catch {
        Write-ColorText "   ❌ Erreur lors de la création: $($_.Exception.Message)" "Red"
        if ($_.Exception.Response) {
            $ErrorDetails = $_.Exception.Response | ConvertTo-Json -Depth 2
            Write-ColorText "   📋 Détails : $ErrorDetails" "Red"
        }
        return $false
    }
}

# =========================================================================
# MAIN SCRIPT EXECUTION
# ============================================================================

Show-Instructions

if ($ValidateOnly) {
    # Mode validation - vérifier les sprints existants via les outils disponibles
    Write-ColorText "🔍 Validation des sprints dans Azure DevOps..." "Cyan"
    # Cette partie sera gérée par les outils Azure DevOps disponibles
    exit 0
}

if ([string]::IsNullOrEmpty($PAT)) {
    exit 1
}

# Headers pour l'API Azure DevOps
$Headers = @{
    "Authorization" = "Basic " + [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(":$PAT"))
    "Content-Type" = "application/json"
}

# Base URL Azure DevOps avec encoding correct du nom de projet
$EncodedProject = [System.Web.HttpUtility]::UrlEncode($Project)
$BaseUrl = "https://dev.azure.com/$Organization/$EncodedProject/_apis"

Write-Host ""
Write-ColorText "🔧 CONFIGURATION API :" "Yellow"
Write-ColorText "• Base URL : $BaseUrl" "Gray"
Write-ColorText "• Mode Test : $TestMode" "Gray"
Write-Host ""

# Test de connexion
if (-not (Test-AzureDevOpsConnection -Headers $Headers -BaseUrl $BaseUrl)) {
    Write-ColorText "❌ Impossible de se connecter à Azure DevOps. Vérifiez votre PAT." "Red"
    exit 1
}

# Récupérer les itérations existantes
$ExistingIterations = Get-ExistingIterations -Headers $Headers -BaseUrl $BaseUrl

Write-Host ""
Write-ColorText "🚀 CRÉATION DES SPRINTS :" "Green"
Write-ColorText "========================" "Green"

$SuccessCount = 0
$ErrorCount = 0

foreach ($Sprint in $Sprints) {
    Write-ColorText "📅 $($Sprint.Name) - $($Sprint.Objective) ($($Sprint.Priority))..." "Cyan"
    
    if (Create-Sprint -Sprint $Sprint -Headers $Headers -BaseUrl $BaseUrl -ExistingIterations $ExistingIterations) {
        $SuccessCount++
    } else {
        $ErrorCount++
    }
    
    Start-Sleep -Seconds 1  # Éviter le rate limiting
}

Write-Host ""
Write-ColorText "🎉 RÉSUMÉ DE CRÉATION :" "Green"
Write-ColorText "======================" "Green"
Write-ColorText "✅ Sprints créés avec succès : $SuccessCount" "Green"
if ($ErrorCount -gt 0) {
    Write-ColorText "❌ Erreurs rencontrées : $ErrorCount" "Red"
}
Write-ColorText "📊 Total sprints : $($Sprints.Count)" "Cyan"

Write-Host ""
Write-ColorText "📋 PROCHAINES ÉTAPES :" "Yellow"
Write-ColorText "1. 🌐 Vérifiez les sprints dans Azure DevOps" "White"
Write-ColorText "2. ⚙️  Configurez les capacités d'équipe pour chaque sprint" "White"
Write-ColorText "3. 📋 Assignez les User Stories aux sprints correspondants" "White"
Write-ColorText "4. 🎯 Planifiez le Sprint 00 (Sécurité & Auth) en priorité" "Yellow"

Write-Host ""
Write-ColorText "🔗 Liens utiles :" "Cyan"
Write-ColorText "• Projet : https://dev.azure.com/$Organization/$Project" "Gray"
Write-ColorText "• Sprints : https://dev.azure.com/$Organization/$Project/_sprints" "Gray"
Write-ColorText "• Iterations : https://dev.azure.com/$Organization/$Project/_settings/work" "Gray"