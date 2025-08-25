@echo off
echo ?? Lancement du Data Seeder pour Chronique des Mondes
echo ====================================================
echo.

echo ?? Vérification des prérequis...
dotnet --version > nul 2>&1
if %errorlevel% neq 0 (
    echo ? .NET n'est pas installé ou n'est pas dans le PATH
    pause
    exit /b 1
)

echo ? .NET détecté
echo.

echo ??? Compilation du projet...
dotnet build --configuration Release > nul 2>&1
if %errorlevel% neq 0 (
    echo ? Erreur lors de la compilation
    echo ?? Essayez de compiler manuellement : dotnet build
    pause
    exit /b 1
)

echo ? Compilation réussie
echo.

echo ?? Exécution du seeding...
echo.
dotnet run --configuration Release

echo.
echo ?? Le seeding est terminé !
echo ?? Vous pouvez maintenant démarrer votre API et utiliser :
echo    Email : root@test.com
echo    Mot de passe : root
echo.
pause