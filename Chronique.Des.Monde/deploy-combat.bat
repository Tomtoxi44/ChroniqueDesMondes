@echo off
REM 🔥 Script de déploiement du système de combat
REM Chronique des Mondes - Combat System Alpha

echo 🚀 Déploiement du système de combat D&D...

REM 1. Vérifier la branche
git branch --show-current > current_branch.tmp
set /p CURRENT_BRANCH=<current_branch.tmp
del current_branch.tmp

if not "%CURRENT_BRANCH%"=="feature/combat-system-advanced" (
    echo ❌ Erreur: Vous devez être sur la branche feature/combat-system-advanced
    pause
    exit /b 1
)

echo ✅ Branche correcte: %CURRENT_BRANCH%

REM 2. Build du projet
echo 🔨 Compilation du projet...
dotnet build
if errorlevel 1 (
    echo ❌ Erreur de compilation
    pause
    exit /b 1
)

echo ✅ Compilation réussie

REM 3. Appliquer les migrations (optionnel)
set /p apply_migrations="🗄️ Voulez-vous appliquer les migrations de base de données? (y/N): "
if /i "%apply_migrations%"=="y" (
    echo 📊 Application des migrations...
    dotnet ef database update --project Cdm.Migrations --startup-project Cdm.ApiService
    if errorlevel 1 (
        echo ⚠️ Erreur lors de l'application des migrations ^(base de données peut-être non configurée^)
    ) else (
        echo ✅ Migrations appliquées
    )
)

REM 4. Démarrage de l'API
set /p start_api="🌐 Voulez-vous démarrer l'API maintenant? (y/N): "
if /i "%start_api%"=="y" (
    echo 🚀 Démarrage de l'API...
    echo 📋 Tests disponibles dans: combat-advanced-weapons-test.http
    echo 🔗 Swagger UI: https://localhost:7240/swagger
    dotnet run --project Cdm.ApiService
)

echo.
echo 🎯 Système de combat prêt !
echo ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
echo 📁 Fichiers de test : combat-advanced-weapons-test.http
echo 📖 Documentation  : COMBAT_SYSTEM.md
echo 🏷️  Version        : v1.0.0-combat-alpha
echo ⚔️  Fonctionnalités: 33 équipements D^&D intégrés
echo 🎲 Dés            : Formules D^&D ^(1d20, 2d6+3, etc.^)
echo 🧮 Calculs        : Automatiques D^&D 5e ^(CA, dégâts^)
echo ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
pause