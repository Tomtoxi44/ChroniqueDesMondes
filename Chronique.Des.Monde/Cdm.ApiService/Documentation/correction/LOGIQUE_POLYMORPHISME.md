# Logique du Polymorphisme dans ChroniqueDesMondes

## Problématique Identifiée

### Issue Principale
Le système actuel présente une confusion architecturale entre le **polymorphisme d'entités** et la **séparation de contextes de données**. Cette confusion génère des erreurs EF Core et une complexité non nécessaire.

### Détails du Problème

#### 1. **Double Héritage Conflictuel**
```csharp
// Dans AppDbContext (contexte général)
public abstract class Character { }
public class CharacterDnd : Character { }

// Dans DndDbContext (contexte spécifique D&D)
public class CharacterDnd : Character { } // MÊME CLASSE !
```

**Problème** : EF Core ne peut pas mapper la même entité dans deux contextes différents avec des configurations potentiellement différentes.

#### 2. **Navigation Properties Cross-Context**
```csharp
// CharacterDnd pointe vers des entités D&D
public virtual ICollection<CharacterSpells> CharacterSpells { get; set; }
public virtual ICollection<CharacterInventory> CharacterInventory { get; set; }

// Mais CharacterSpells et CharacterInventory ne sont que dans DndDbContext
// Alors que CharacterDnd existe dans AppDbContext
```

**Problème** : Référence à des entités qui n'existent pas dans le même contexte.

#### 3. **Configuration DbSet Redondante**
```csharp
// Dans AppDbContext
public DbSet<CharacterDnd> CharacterDnd { get; set; }

// Dans DndDbContext  
public DbSet<CharacterDnd> CharacterDnd { get; set; }
```

**Problème** : Même entité configurée dans deux contextes = conflit EF Core.

## Solutions Architecturales

### Solution 1: **Séparation Complète des Contextes** ? RECOMMANDÉE
- `AppDbContext` : Entités générales (User, Campaign, Chapter, etc.)
- `DndDbContext` : Entités D&D complètes (CharacterDnd, SpellDnd, etc.)
- **Pas de partage d'entités entre contextes**

### Solution 2: **Polymorphisme Unifié**
- Tout dans `AppDbContext` avec héritage TPH (Table Per Hierarchy)
- Un seul contexte pour tous les systèmes de jeu
- Plus complexe à maintenir

### Solution 3: **Pattern Repository Cross-Context**
- Interfaces communes pour les opérations cross-système
- Implémentations spécifiques par contexte
- Coordination via services métier

## Architecture Cible (Solution 1)

### AppDbContext
```csharp
public class AppDbContext : DbContext
{
    // Entités générales uniquement
    public DbSet<User> Users { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<ContentBlock> ContentBlocks { get; set; }
    
    // PAS de CharacterDnd ici !
}
```

### DndDbContext
```csharp
public class DndDbContext : DbContext
{
    // Entités D&D complètes
    public DbSet<CharacterDnd> Characters { get; set; }
    public DbSet<SpellDnd> Spells { get; set; }
    public DbSet<EquipmentDnd> Equipment { get; set; }
    public DbSet<CharacterSpells> CharacterSpells { get; set; }
    public DbSet<CharacterInventory> CharacterInventory { get; set; }
    
    // Tables de liaison et échanges
    public DbSet<EquipmentOffer> EquipmentOffers { get; set; }
    public DbSet<EquipmentTrade> EquipmentTrades { get; set; }
}
```

## Plan d'Implémentation

### Phase 1: Nettoyage des Contextes
1. ? Supprimer `CharacterDnd` de `AppDbContext` 
2. ? Garder uniquement les entités générales dans `AppDbContext`
3. ? Migrer toutes les entités D&D vers `DndDbContext`

### Phase 2: Correction des Services
1. ? Ajuster les services pour utiliser le bon contexte
2. ? Implémenter les patterns de coordination cross-context
3. ? Tests unitaires pour chaque service

### Phase 3: Migration des Données
1. ? Créer les migrations de nettoyage
2. ? Tester la migration sur environnement de dev
3. ? Valider l'intégrité des données

### Phase 4: Tests et Validation
1. ? Tests d'intégration end-to-end
2. ? Validation des performances
3. ? Documentation mise à jour

## Avantages de la Solution Recommandée

### ?? **Clarté Architecturale**
- Séparation nette des responsabilités
- Chaque contexte gère son domaine
- Pas de confusion entre systèmes de jeu

### ? **Performance**
- Contextes plus petits = requêtes plus rapides
- Pas de tracking d'entités non utilisées
- Optimisation possible par système

### ?? **Maintenabilité**
- Ajout de nouveaux systèmes de jeu facilité
- Évolution indépendante des contextes
- Débogage plus simple

### ??? **Robustesse**
- Pas de conflits EF Core
- Isolation des erreurs par contexte
- Transactions plus ciblées

## Risques et Mitigations

### ?? **Risque : Requêtes Cross-Context**
**Mitigation** : Services de coordination avec interfaces bien définies

### ?? **Risque : Duplication de Code**
**Mitigation** : Classes de base communes et patterns partagés

### ?? **Risque : Complexité des Transactions**
**Mitigation** : Pattern Unit of Work ou Saga Pattern si nécessaire

## Prochaines Étapes

1. ?? **Appliquer les corrections dans DndDbContext**
2. ?? **Tests complets des endpoints**
3. ?? **Validation des migrations**
4. ?? **Déploiement en environnement de test**

---

*Document créé le 16/09/2025 - Version 1.0*
*Auteur: Assistant IA - Révision: Équipe de développement*