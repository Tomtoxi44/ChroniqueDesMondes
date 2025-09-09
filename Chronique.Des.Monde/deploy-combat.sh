#!/bin/bash

# 🔥 Script de déploiement du système de combat
# Chronique des Mondes - Combat System Alpha

echo "🚀 Déploiement du système de combat D&D..."

# 1. Vérifier que nous sommes sur la bonne branche
CURRENT_BRANCH=$(git branch --show-current)
if [ "$CURRENT_BRANCH" != "feature/combat-system-advanced" ]; then
    echo "❌ Erreur: Vous devez être sur la branche feature/combat-system-advanced"
    exit 1
fi

echo "✅ Branche correcte: $CURRENT_BRANCH"

# 2. Build du projet
echo "🔨 Compilation du projet..."
dotnet build
if [ $? -ne 0 ]; then
    echo "❌ Erreur de compilation"
    exit 1
fi

echo "✅ Compilation réussie"

# 3. Appliquer les migrations (optionnel)
read -p "🗄️ Voulez-vous appliquer les migrations de base de données? (y/N): " apply_migrations
if [[ $apply_migrations =~ ^[Yy]$ ]]; then
    echo "📊 Application des migrations..."
    dotnet ef database update --project Cdm.Migrations --startup-project Cdm.ApiService
    if [ $? -ne 0 ]; then
        echo "⚠️ Erreur lors de l'application des migrations (base de données peut-être non configurée)"
    else
        echo "✅ Migrations appliquées"
    fi
fi

# 4. Démarrage de l'API
read -p "🌐 Voulez-vous démarrer l'API maintenant? (y/N): " start_api
if [[ $start_api =~ ^[Yy]$ ]]; then
    echo "🚀 Démarrage de l'API..."
    echo "📋 Tests disponibles dans: combat-advanced-weapons-test.http"
    echo "🔗 Swagger UI: https://localhost:7240/swagger"
    dotnet run --project Cdm.ApiService
fi

echo ""
echo "🎯 Système de combat prêt !"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "📁 Fichiers de test : combat-advanced-weapons-test.http"
echo "📖 Documentation  : COMBAT_SYSTEM.md"
echo "🏷️  Version        : v1.0.0-combat-alpha"
echo "⚔️  Fonctionnalités: 33 équipements D&D intégrés"
echo "🎲 Dés            : Formules D&D (1d20, 2d6+3, etc.)"
echo "🧮 Calculs        : Automatiques D&D 5e (CA, dégâts)"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"