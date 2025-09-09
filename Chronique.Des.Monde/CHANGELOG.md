# 📋 CHANGELOG - Combat System

## [1.0.0-combat-alpha] - 2024-12-26

### 🆕 Ajouts majeurs

#### 🗄️ Base de données
- **CombatSession** : Gestion complète des sessions de combat
  - Support multi-systèmes (D&D 5e, Generic)
  - Gestion des rounds et tours
  - Environnement et paramètres configurables
  - Historique des actions en temps réel

- **CombatParticipant** : Participants dynamiques
  - Support joueurs et PNJ
  - Stats de combat en temps réel (PV, CA, vitesse)
  - Position sur le champ de bataille
  - Ressources (sorts, capacités spéciales)

- **CombatAction** : Historique détaillé
  - Enregistrement de toutes les actions
  - Jets de dés avec détails complets
  - Calculs de dégâts et soins
  - Métadonnées pour analytics

- **CombatStatusEffect** : Gestion des effets
  - Buffs, debuffs, conditions D&D
  - Durée et intensité configurables
  - Effets périodiques et sauvegardes
  - Interface visuelle (icônes, couleurs)

#### ⚙️ Services métier

- **CombatEngine** : Moteur principal
  - Architecture multi-systèmes
  - Validation des actions
  - Gestion automatique des tours
  - API RESTful complète

- **DiceRoller** : Système de dés avancé
  - Formules D&D : `1d20`, `2d6+3`, `4d4+4`
  - Avantage/Désavantage D&D 5e
  - Jets critiques avec doublement
  - Logs détaillés pour debugging

- **InitiativeManager** : Gestion des tours
  - Ordre d'initiative avec départage
  - Insertion dynamique de participants
  - Gestion des participants éliminés
  - Rapport d'initiative formaté

- **DndCombatCalculator** : Calculs D&D 5e
  - Modificateurs de caractéristiques automatiques
  - Bonus de maîtrise par niveau
  - Calcul de CA avec armures/boucliers
  - Bonus d'attaque avec armes
  - DD de sauvegarde des sorts

#### 🌐 API REST

**12 endpoints principaux :**
- `POST /api/combat/start` - Démarrer un combat
- `GET /api/combat/{id}` - État du combat
- `POST /api/combat/{id}/participants` - Ajouter participant
- `POST /api/combat/{id}/initiative` - Lancer initiative
- `POST /api/combat/{id}/start-phase` - Démarrer combat
- `POST /api/combat/{id}/actions` - Exécuter action
- `POST /api/combat/{id}/advance-turn` - Tour suivant
- `POST /api/combat/{id}/pause` - Pause/Reprise
- `POST /api/combat/{id}/end` - Terminer combat
- `POST /api/combat/participants/{id}/status-effects` - Effets
- `DELETE /api/combat/status-effects/{id}` - Supprimer effet
- `GET /api/combat/participants/{id}/actions` - Actions disponibles

### ⚔️ Intégration équipements

#### Armes D&D intégrées (33 équipements)
- **Armes simples** : Dague, Gourdin, Javeline, Masse, Bâton, etc.
- **Armes de guerre** : Épée longue, Rapière, Espadon, Marteau, etc.
- **Armes à distance** : Arc court/long, Arbalète légère/lourde, Fronde

#### Propriétés d'armes fonctionnelles
- **Finesse** : Choix Force/Dextérité optimal
- **Polyvalente** : Dégâts 1 main vs 2 mains
- **Lourde** : Restrictions pour petites créatures
- **Légère** : Combat à deux armes
- **Lancer** : Attaques à distance
- **Allonge** : Portée étendue

#### Calculs automatiques
- **Bonus d'attaque** : Caractéristique + Maîtrise + Magique
- **Dégâts** : Formule de base + Modificateur + Critique
- **Types de dégâts** : Tranchant, Perforant, Contondant
- **Coups critiques** : Doublement des dés (D&D 5e)

### 🎲 Exemples de sortie

```
⚔️ Thomas attaque Gobelin avec Épée longue !
🎲 Jet d'attaque: 1d20+5 = 17 (touche CA 13)
⚡ Dégâts: 1d8+3 = 6 dégâts tranchants
🩸 Gobelin: 8/14 PV restants
```

```
💥 COUP CRITIQUE ! Lisa attaque avec Dague !
🎯 Jet naturel 20 + 3 = 23
💥 Dégâts critiques: 2d4+1 = 7 dégâts perforants
💀 Gobelin Archer est éliminé !
```

### 🧪 Tests et documentation

- **combat-advanced-weapons-test.http** : Tests complets
- **COMBAT_SYSTEM.md** : Documentation technique
- **deploy-combat.bat/sh** : Scripts de déploiement
- **Migration** : `20250909204429_AddCombatSystem`

### 🔧 Configuration technique

- **.NET 9** : Framework moderne
- **Entity Framework Core** : ORM avec relations optimisées
- **Dependency Injection** : Architecture modulaire
- **Swagger OpenAPI** : Documentation automatique
- **Logging** : Traces détaillées pour debugging

### 🚀 Prochaines étapes

1. **Sorts de combat** : Intégration des 74 sorts D&D
2. **Interface Blazor** : UI temps réel pour le combat
3. **Actions spéciales** : Capacités par classe D&D
4. **IA pour PNJ** : Comportement automatique
5. **Cartes tactiques** : Positionnement visuel

---

### 📊 Métriques

- **19 fichiers** créés/modifiés
- **4939 lignes** ajoutées
- **12 endpoints** API
- **33 équipements** intégrés
- **4 modèles** de données
- **5 services** métier

---

**🏆 Résultat : Système de combat niveau AAA pour D&D 5e !** ⚔️