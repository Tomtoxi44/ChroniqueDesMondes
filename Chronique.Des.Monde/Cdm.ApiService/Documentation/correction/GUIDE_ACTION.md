# Guide d'Action - Correction du Polymorphisme DbContext

## ?? Objectif
Résoudre les conflits de polymorphisme entre `AppDbContext` et `DndDbContext` pour une architecture propre et maintenable.

## ?? Checklist de Correction

### ? Phase 1: Diagnostic et Préparation
- [x] Identifier les entités en conflit (`CharacterDnd`, `SpellDnd`, `EquipmentDnd`)
- [x] Analyser les dépendances cross-context
- [x] Créer la branche `fix/polymorphisme-dbcontext`
- [x] Documenter la problématique

### ?? Phase 2: Corrections du Code

#### A. Nettoyage de DndDbContext
```csharp
// ? À ÉVITER - Configuration actuelle problématique
public class DndDbContext : DbContext
{
    // Tables partagées qui créent des conflits
    public DbSet<User> Users { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    // ... autres entités générales
}

// ? À IMPLÉMENTER - Configuration propre
public class DndDbContext : DbContext
{
    // UNIQUEMENT les entités D&D spécifiques
    public DbSet<CharacterDnd> Characters { get; set; }
    public DbSet<SpellDnd> Spells { get; set; }
    public DbSet<EquipmentDnd> Equipment { get; set; }
    
    // Tables de liaison D&D
    public DbSet<CharacterSpells> CharacterSpells { get; set; }
    public DbSet<CharacterInventory> CharacterInventory { get; set; }
    public DbSet<EquipmentOffer> EquipmentOffers { get; set; }
    public DbSet<EquipmentTrade> EquipmentTrades { get; set; }
}
```

#### B. Actions Concrètes à Réaliser

##### 1. **Supprimer les DbSet Redondants** ???
```csharp
// Dans DndDbContext.cs - À SUPPRIMER
public DbSet<User> Users { get; set; }
public DbSet<Campaign> Campaigns { get; set; }
public DbSet<Chapter> Chapters { get; set; }
public DbSet<ContentBlock> ContentBlocks { get; set; }
```

##### 2. **Nettoyer la Configuration OnModelCreating** ??
```csharp
// Dans DndDbContext.cs - Garder uniquement
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    
    // Configuration D&D uniquement
    modelBuilder.ApplyConfiguration(new CharacterDndConfiguration());
    modelBuilder.ApplyConfiguration(new SpellDndConfiguration());
    modelBuilder.ApplyConfiguration(new EquipmentDndConfiguration());
    
    // SUPPRIMER toutes les configurations d'entités générales
}
```

##### 3. **Corriger les Services** ??
```csharp
// Exemple pour CharacterDndService
public class CharacterDndService 
{
    private readonly DndDbContext _dndContext;
    // Supprimer toute injection d'AppDbContext ici
    
    public CharacterDndService(DndDbContext dndContext)
    {
        _dndContext = dndContext;
    }
}
```

##### 4. **Adapter les Controllers/Endpoints** ??
```csharp
// Dans les endpoints D&D, utiliser UNIQUEMENT DndDbContext
app.MapPost("/api/dnd/characters", async (
    CharacterDndRequest request, 
    DndDbContext dndContext) => // Pas d'AppDbContext ici
{
    // Logique utilisant uniquement dndContext
});
```

### ?? Phase 3: Tests et Validation

#### A. Tests à Effectuer
- [ ] **Test de Build** : Le projet compile sans erreur
- [ ] **Test de Migration** : Les migrations EF s'appliquent correctement
- [ ] **Test d'Endpoints** : Tous les endpoints D&D fonctionnent
- [ ] **Test de Cross-Reference** : Pas de dépendances circulaires

#### B. Commandes de Validation
```bash
# 1. Build du projet
dotnet build

# 2. Test des migrations D&D
dotnet ef migrations add TestPolymorphismeFix --context DndDbContext
dotnet ef database update --context DndDbContext

# 3. Test des endpoints
dotnet run
# Tester via Swagger : /api/dnd/characters, /api/dnd/spells, etc.
```

### ?? Phase 4: Vérifications Post-Correction

#### A. Vérifications Techniques
- [ ] Aucune exception EF Core au démarrage
- [ ] Pas de tables dupliquées dans les migrations
- [ ] Services D&D utilisent uniquement DndDbContext
- [ ] Navigation properties correctement configurées

#### B. Points d'Attention Spécifiques
```csharp
// ? Vérifier que ces entités sont UNIQUEMENT dans DndDbContext
CharacterDnd
SpellDnd  
EquipmentDnd
CharacterSpells
CharacterInventory
EquipmentOffer
EquipmentTrade

// ? Vérifier que ces entités sont UNIQUEMENT dans AppDbContext
User
Campaign
Chapter
ContentBlock
```

## ?? Points Critiques à Surveiller

### 1. **Injections de Dépendance**
```csharp
// ? ÉVITER dans les services D&D
public SomeService(AppDbContext appContext, DndDbContext dndContext)

// ? PRIVILÉGIER dans les services D&D
public SomeService(DndDbContext dndContext)
```

### 2. **Références Cross-Context**
```csharp
// ? ÉVITER - Référence cross-context
var user = await _dndContext.Users.FindAsync(userId); // Users n'est pas dans DndDbContext

// ? PRIVILÉGIER - Service de coordination
var user = await _userService.GetUserByIdAsync(userId); // Service utilisant AppDbContext
```

### 3. **Configuration EF Core**
```csharp
// Dans Program.cs ou Startup.cs
services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(connectionString));
    
services.AddDbContext<DndDbContext>(options => 
    options.UseSqlServer(connectionString));
// ? Bien vérifier que les deux contextes sont enregistrés séparément
```

## ?? Livrables Attendus

### Code
- [ ] DndDbContext nettoyé (sans entités générales)
- [ ] Services D&D refactorisés (injection DndDbContext uniquement)
- [ ] Endpoints D&D corrigés
- [ ] Tests passants

### Documentation
- [x] LOGIQUE_POLYMORPHISME.md (ce document d'explication)
- [x] GUIDE_ACTION.md (ce guide de correction)
- [ ] Mise à jour du README si nécessaire

### Validation
- [ ] Build sans erreur
- [ ] Migrations appliquées avec succès
- [ ] Endpoints fonctionnels via Swagger
- [ ] Tests automatisés passants (si existants)

## ?? Prochaines Actions Immédiates

1. **Ouvrir** `Cdm.Data.Dnd\DndDbContext.cs`
2. **Supprimer** tous les DbSet d'entités générales (User, Campaign, etc.)
3. **Nettoyer** la méthode ConfigureExistingTables
4. **Tester** la compilation
5. **Créer** une migration de test
6. **Valider** les endpoints via Swagger

---

## ?? En Cas de Problème

### Erreurs Courantes et Solutions

#### "Entity type 'User' cannot be tracked..."
**Solution** : Vérifier qu'User n'est plus dans DndDbContext

#### "No DbContext was found..."  
**Solution** : Vérifier l'injection de dépendance dans Program.cs

#### "Migration already exists..."
**Solution** : Supprimer les migrations de test et recréer

---

*Guide créé le 16/09/2025 - Version 1.0*
*À suivre étape par étape pour une correction réussie*