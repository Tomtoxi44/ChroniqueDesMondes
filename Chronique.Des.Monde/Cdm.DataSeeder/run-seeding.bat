@echo off
echo ?? Lancement du Data Seeder pour Chronique des Mondes
echo ====================================================
echo.

echo ?? V�rification des pr�requis...
dotnet --version > nul 2>&1
if %errorlevel% neq 0 (
    echo ? .NET n'est pas install� ou n'est pas dans le PATH
    pause
    exit /b 1
)

echo ? .NET d�tect�
echo.

echo ??? Compilation du projet...
dotnet build --configuration Release > nul 2>&1
if %errorlevel% neq 0 (
    echo ? Erreur lors de la compilation
    echo ?? Essayez de compiler manuellement : dotnet build
    pause
    exit /b 1
)

echo ? Compilation r�ussie
echo.

echo ?? Ex�cution du seeding...
echo.
dotnet run --configuration Release

echo.
echo ?? Le seeding est termin� !
echo ?? Vous pouvez maintenant d�marrer votre API et utiliser :
echo    Email : root@test.com
echo    Mot de passe : root
echo.
pause