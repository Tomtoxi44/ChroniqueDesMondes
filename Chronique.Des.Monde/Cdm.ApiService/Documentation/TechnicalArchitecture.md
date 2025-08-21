# Architecture Technique - Chronique des Mondes

Ce document détaille l'architecture technique complète du projet, les configurations et les aspects de développement.

## 🏗️ Structure du Projet

### Projets de la Solution
```
Cdm.ApiService/          # API REST principale
├── Endpoints/           # Définition des endpoints
├── Services/           # Services métier
├── Extensions/         # Extensions et configuration
├── Tests/             # Tests API
├── Scripts/           # Scripts d'injection SQL
└── Documentation/     # Documentation

Cdm.Data.Common/        # Modèles de données génériques
Cdm.Data.Dnd/          # Modèles de données D&D
Cdm.Business.Common/   # Logique métier générique
Cdm.Business.Dnd/      # Logique métier D&D
Cdm.Web/              # Interface Blazor
Cdm.Migrations/       # Migrations Entity Framework
```

### Architecture en Couches

#### **Couche API (Cdm.ApiService)**
- **Minimal APIs** pour les endpoints REST
- **Middleware d'authentification** JWT
- **Validation des modèles** automatique
- **Gestion d'erreurs** centralisée

#### **Couche Business (Cdm.Business.*)**
- **Services métier** par domaine
- **Validation des règles** métier
- **Calculs spécialisés** (D&D, etc.)
- **Orchestration** des opérations

#### **Couche Data (Cdm.Data.*)**
- **Entity Framework Core** comme ORM
- **Modèles** par système de jeu
- **Configurations** des entités
- **Migrations** automatisées

#### **Couche Web (Cdm.Web)**
- **Blazor Server** pour l'interface
- **Composants réutilisables**
- **Services HTTP** typés
- **Gestion d'état** centralisée

## 🗄️ Modèles de Données

### Architecture Multi-Système

#### **Modèles Génériques (Cdm.Data.Common)**
```csharp
public abstract class ACharacter
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Picture { get; set; }
    public string? Background { get; set; }
    public int Life { get; set; }
    public int Leveling { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string UserEmail { get; set; }
    public string Password { get; set; }
}
```

#### **Modèles Spécialisés D&D (Cdm.Data.Dnd)**
```csharp
public class CharacterDnd : ACharacter
{
    public string Class { get; set; }
    public int ClassArmor { get; set; }
    public int Strong { get; set; }
    public int AdditionalStrong { get; set; }
    public int Dexterity { get; set; }
    public int AdditionalDexterity { get; set; }
    public int Constitution { get; set; }
    public int AdditionalConstitution { get; set; }
    public int Intelligence { get; set; }
    public int AdditionalIntelligence { get; set; }
    public int Wisdoms { get; set; }
    public int AdditionalWisdoms { get; set; }
    public int Charism { get; set; }
    public int AdditionalCharism { get; set; }
}
```

### Nouveaux Modèles (À Implémenter)

#### **Sorts**
```csharp
public class Spell
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? ImageUrl { get; set; }
    public string GameType { get; set; }
    public int CreatedByUserId { get; set; } // 0 = Admin
    public bool IsPublic { get; set; }
    public List<string> Tags { get; set; }
    public SpellDndProperties? DndProperties { get; set; }
}

public class SpellDndProperties
{
    public int Level { get; set; }
    public string School { get; set; }
    public string CastingTime { get; set; }
    public string Range { get; set; }
    public string Duration { get; set; }
    public List<string> Components { get; set; }
    public string? DamageFormula { get; set; }
    public bool RequiresAttackRoll { get; set; }
    public bool RequiresSavingThrow { get; set; }
}
```

#### **Équipements**
```csharp
public class Equipment
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? ImageUrl { get; set; }
    public string GameType { get; set; }
    public int CreatedByUserId { get; set; }
    public bool IsPublic { get; set; }
    public List<string> Tags { get; set; }
    public EquipmentGenericProperties? GenericProperties { get; set; }
    public EquipmentDndProperties? DndProperties { get; set; }
}

public class EquipmentGenericProperties
{
    public decimal Weight { get; set; }
    public int Value { get; set; }
    public string? AttackBonusAbility { get; set; }
    public string? DamageFormula { get; set; }
}

public class EquipmentDndProperties
{
    public string EquipmentType { get; set; }
    public string? WeaponCategory { get; set; }
    public string? ArmorCategory { get; set; }
    public int? ArmorClassBase { get; set; }
    public string? DamageType { get; set; }
    public List<string> Properties { get; set; }
    public string Rarity { get; set; }
    public bool RequiresAttunement { get; set; }
}
```

#### **Échanges d'Équipements**
```csharp
public class EquipmentOffer
{
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public int GameMasterId { get; set; }
    public int TargetPlayerId { get; set; }
    public int EquipmentId { get; set; }
    public int Quantity { get; set; }
    public string? Message { get; set; }
    public OfferStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RespondedAt { get; set; }
}

public class EquipmentTrade
{
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public int FromPlayerId { get; set; }
    public int ToPlayerId { get; set; }
    public int EquipmentId { get; set; }
    public int Quantity { get; set; }
    public string? Message { get; set; }
    public TradeStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public enum OfferStatus
{
    Pending, Accepted, Declined, Cancelled
}

public enum TradeStatus
{
    Proposed, Accepted, Declined, Completed, Cancelled
}
```

#### **Relations Personnage-Sort/Équipement**
```csharp
public class CharacterSpell
{
    public int CharacterId { get; set; }
    public int SpellId { get; set; }
    public DateTime LearnedDate { get; set; }
    public bool IsPrepared { get; set; }
}

public class CharacterEquipment
{
    public int CharacterId { get; set; }
    public int EquipmentId { get; set; }
    public int Quantity { get; set; }
    public bool IsEquipped { get; set; }
    public Dictionary<string, object>? CustomProperties { get; set; }
}
```

## 🔧 Configuration

### JWT Configuration
```json
{
  "Jwt": {
    "SecretKey": "votre-clé-secrète-256-bits",
    "Issuer": "ChroniqueDesMondes",
    "Audience": "ChroniqueDesMondes-Users",
    "ExpiryMinutes": 1440
  }
}
```

### Entity Framework Configuration
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ChroniqueDesMondes;Trusted_Connection=true;"
  }
}
```

### Aspire Configuration
```csharp
var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.Cdm_ApiService>("apiservice");

builder.AddProject<Projects.Cdm_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
```

## 🧪 Tests

### Structure des Tests
```
Tests/
├── Generic/                    # Tests CRUD de base
│   ├── character-crud.http     # Opérations personnages
│   └── user-auth.http         # Authentification
├── Dnd/                       # Tests spécifiques D&D
│   ├── character-dnd.http     # Personnages D&D
│   └── calculations.http      # Calculs modificateurs
├── Spells/                    # Tests sorts
│   ├── official-spells.http   # Sorts officiels
│   ├── private-spells.http    # Sorts privés
│   └── character-learning.http # Apprentissage sorts
├── Equipment/                 # Tests équipements
│   ├── inventory.http         # Gestion inventaire
│   ├── gm-offers.http        # Propositions MJ
│   └── player-trades.http    # Échanges joueurs
├── Security/                 # Tests sécurité
│   ├── jwt-validation.http   # Validation tokens
│   └── permissions.http      # Contrôles accès
└── Scenarios/                # Tests end-to-end
    ├── campaign-creation.http # Création campagne complète
    └── combat-session.http   # Session de combat
```

### Configuration Tests
```http
### Configuration globale
@baseUrl = https://localhost:7428
@authToken = {{login_response.response.body.$.token}}

### Variables communes
@userId = 1
@characterId = 5
@campaignId = 2
```

## 🔄 Services Métier

### Interface de Services
```csharp
public interface ISpellcastingService
{
    SpellcastingAbility GetSpellcastingAbility(string characterClass);
    int CalculateSpellAttackBonus(Character character);
    int CalculateSpellSaveDC(Character character);
    int GetProficiencyBonus(int characterLevel);
    bool CanLearnSpell(Character character, Spell spell);
}

public interface IEquipmentService
{
    int CalculateArmorClass(Character character);
    Dictionary<string, int> CalculateAbilityModifiers(Character character);
    bool CanEquipItem(Character character, Equipment equipment);
    EquipmentSlot GetEquipmentSlot(Equipment equipment);
}

public interface IEquipmentExchangeService
{
    Task<EquipmentOffer> CreateOfferAsync(int campaignId, int gmId, int playerId, int equipmentId, int quantity, string? message);
    Task<EquipmentTrade> ProposeTradeAsync(int campaignId, int fromPlayerId, int toPlayerId, int equipmentId, int quantity, string? message);
    Task<bool> ValidateTradeAsync(int tradeId);
}
```

### Injection de Dépendances
```csharp
// Program.cs
builder.Services.AddScoped<ISpellcastingService, SpellcastingService>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<IEquipmentExchangeService, EquipmentExchangeService>();

// Authentification
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        // Configuration JWT
    });

// Entity Framework
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
```

## 📊 Scripts d'Injection

### Sorts D&D Officiels
```sql
-- Scripts/DndSpellsInjection.sql
INSERT INTO Spells (Name, Description, GameType, IsPublic, CreatedByUserId, DndProperties)
VALUES 
('Boule de Feu', 'Explosion dévastatrice de flammes', 'dnd', true, 0, 
 '{"level":3,"school":"Évocation","castingTime":"1 action","range":"45 mètres","duration":"Instantané","components":["V","S","M"],"damageFormula":"8d6","requiresAttackRoll":false,"requiresSavingThrow":true,"savingThrowAbility":"Dextérité"}'),

('Projectile Magique', 'Projectiles d''énergie pure', 'dnd', true, 0,
 '{"level":1,"school":"Évocation","castingTime":"1 action","range":"36 mètres","duration":"Instantané","components":["V","S"],"damageFormula":"1d4+1","requiresAttackRoll":true,"requiresSavingThrow":false}'),

('Soin', 'Restaure les points de vie', 'dnd', true, 0,
 '{"level":1,"school":"Évocation","castingTime":"1 action","range":"Contact","duration":"Instantané","components":["V","S"],"damageFormula":"1d8+mod","requiresAttackRoll":false,"requiresSavingThrow":false}');
```

### Équipements D&D Officiels
```sql
-- Scripts/DndEquipmentInjection.sql
INSERT INTO Equipment (Name, Description, GameType, IsPublic, CreatedByUserId, DndProperties)
VALUES 
('Épée Longue', 'Arme martiale polyvalente', 'dnd', true, 0,
 '{"equipmentType":"Weapon","weaponCategory":"Martial","damageFormula":"1d8","damageType":"Tranchant","properties":["Versatile (1d10)"],"rarity":"Commun","requiresAttunement":false}'),

('Armure de Cuir', 'Armure légère flexible', 'dnd', true, 0,
 '{"equipmentType":"Armor","armorCategory":"Light","armorClassBase":11,"armorClassDexBonus":2,"rarity":"Commun","requiresAttunement":false}'),

('Potion de Soins', 'Récupère des points de vie', 'dnd', true, 0,
 '{"equipmentType":"Consumable","properties":["Healing 2d4+2"],"rarity":"Commun","requiresAttunement":false}');
```

## 🚀 Déploiement

### Commandes de Migration
```bash
# Créer une nouvelle migration
dotnet ef migrations add NomDeLaMigration --project Cdm.Migrations

# Appliquer les migrations
dotnet ef database update --project Cdm.Migrations

# Rollback vers une migration spécifique
dotnet ef database update NomDeLaMigration --project Cdm.Migrations

# Générer script SQL
dotnet ef migrations script --project Cdm.Migrations
```

### Pipeline CI/CD
```yaml
# .github/workflows/deploy.yml
name: Deploy to Production

on:
  push:
    branches: [ main ]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0'
        
    - name: Build
      run: dotnet build --configuration Release
      
    - name: Test
      run: dotnet test --no-build --configuration Release
      
    - name: Deploy
      run: dotnet publish --configuration Release
```

---

*Retour au [README principal](./README.md)*