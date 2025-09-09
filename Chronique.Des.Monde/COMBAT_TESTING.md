# 🔥 Combat System - Guide de test rapide

## 🚀 Démarrage rapide

### 1. Prérequis
```bash
# S'assurer d'être sur la bonne branche
git checkout feature/combat-system-advanced

# Compiler le projet
dotnet build
```

### 2. Lancement de l'API
```bash
# Windows
.\deploy-combat.bat

# Linux/Mac
./deploy-combat.sh

# Ou manuellement
dotnet run --project Cdm.ApiService
```

### 3. Tests avec VS Code
1. Ouvrir `combat-advanced-weapons-test.http`
2. Cliquer sur "Send Request" pour chaque test
3. Observer les résultats en JSON

## 🎯 Tests essentiels

### Test 1 : Démarrage de combat
```http
POST https://localhost:7240/api/combat/start
```
**Résultat attendu** : Combat créé avec 4 participants

### Test 2 : Attaque à l'épée longue
```http
POST https://localhost:7240/api/combat/{id}/actions
{
  "actionType": "weapon_attack",
  "equipmentId": 19  // Épée longue
}
```
**Résultat attendu** :
```json
{
  "success": true,
  "actionDescription": "⚔️ Thomas attaque Gobelin avec Épée longue !\n🎲 Jet d'attaque: 1d20+5 = 17 (touche CA 13)\n⚡ Dégâts: 1d8+3 = 6 dégâts tranchants\n🩸 Gobelin: 8/14 PV restants",
  "damageDealt": 6,
  "damageType": "Tranchant"
}
```

## 🛠️ Dépannage

### Erreur de migration
```bash
# Créer une nouvelle base de données
dotnet ef database drop --project Cdm.Migrations --startup-project Cdm.ApiService
dotnet ef database update --project Cdm.Migrations --startup-project Cdm.ApiService
```

### Port déjà utilisé
- Changer le port dans `Cdm.ApiService/appsettings.json`
- Ou arrêter les autres instances

### Données de test manquantes
- Vérifier que les équipements D&D sont injectés
- Utiliser les endpoints d'administration pour vérifier

## 📊 Endpoints clés

| Endpoint | Description |
|----------|-------------|
| `POST /api/combat/start` | Démarrer un combat |
| `POST /api/combat/{id}/actions` | Exécuter une action |
| `GET /api/combat/{id}` | État du combat |
| `POST /api/combat/{id}/advance-turn` | Tour suivant |

## 🎲 Équipements testables

| ID | Nom | Dégâts | Propriétés |
|----|-----|--------|-----------|
| 9 | Dague | 1d4 | Finesse, Légère, Lancer |
| 19 | Épée longue | 1d8 | Polyvalente (1d10) |
| 22 | Cimeterre | 1d6 | Finesse, Légère |
| 23 | Espadon | 2d6 | Lourde, Deux mains |
| 28 | Arc long | 1d8 | Lourde, Deux mains, Munitions |

## 🏆 Succès attendus

✅ **Descriptions immersives** avec noms d'armes réels  
✅ **Calculs automatiques** selon D&D 5e  
✅ **Coups critiques** avec doublement des dés  
✅ **Gestion des PV** et états des participants  
✅ **Types de dégâts** corrects (Tranchant, Perforant, etc.)

---

**🎯 Objectif : Prouver que le système de combat fonctionne avec tes 33 équipements !** ⚔️