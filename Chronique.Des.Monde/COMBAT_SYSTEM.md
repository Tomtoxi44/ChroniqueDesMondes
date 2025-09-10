# 🔥 Combat System - Advanced Weapons Integration

## 🎯 Vue d'ensemble

Cette branche implémente un **système de combat avancé** pour D&D 5e avec intégration complète des équipements existants.

## ✨ Fonctionnalités ajoutées

### 📊 Models de données
- **CombatSession** : Gestion des sessions de combat avec rounds et tours
- **CombatParticipant** : Participants (joueurs/PNJ) avec stats de combat
- **CombatAction** : Historique complet des actions effectuées  
- **CombatStatusEffect** : Gestion des buffs/debuffs et conditions

### ⚙️ Services métier
- **CombatEngine** : Moteur principal de combat multi-systèmes
- **DiceRoller** : Générateur de dés avec formules D&D (1d20, 2d6+3, etc.)
- **InitiativeManager** : Gestion de l'ordre d'initiative avec départage
- **DndCombatCalculator** : Calculs spécialisés D&D 5e

### 🌐 API REST
- **12 endpoints** pour gestion complète des combats
- **Support temps réel** pour sessions multijoueurs
- **Validation** des actions et état du combat

### ⚔️ Attaques d'armes avancées
- **Intégration des 33 équipements** D&D existants
- **Calculs automatiques** : bonus d'attaque, dégâts, CA
- **Propriétés d'armes** : Finesse, Polyvalente, Lourde, etc.
- **Coups critiques** avec doublement des dés
- **Types de dégâts** réels (Tranchant, Perforant, Contondant)

## 🎲 Exemples d'utilisation

### Démarrer un combat
```http
POST /api/combat/start
{
  "name": "Combat contre gobelins",
  "gameType": "DnD",
  "participants": [...]
}
```

### Attaque d'arme avancée
```http
POST /api/combat/{id}/actions
{
  "actionType": "weapon_attack",
  "actorId": 1,
  "targetId": 2,
  "equipmentId": 19  // Épée longue
}
```

### Résultat typique
```
⚔️ Thomas attaque Gobelin avec Épée longue !
🎲 Jet d'attaque: 1d20+5 = 17 (touche CA 13)
⚡ Dégâts: 1d8+3 = 6 dégâts tranchants  
🩸 Gobelin: 8/14 PV restants
```

## 🧪 Tests

Fichier de test complet disponible : `combat-advanced-weapons-test.http`

## 🚀 Prochaines étapes

1. **Résoudre la migration** de base de données
2. **Ajouter les sorts de combat** (74 sorts existants)
3. **Interface Blazor** temps réel
4. **Actions spéciales** par classe D&D

---
*Système de combat niveau AAA pour Chronique des Mondes* ⚔️