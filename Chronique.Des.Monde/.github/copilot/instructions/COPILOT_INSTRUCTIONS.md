# 🧠 Instructions de Contexte pour GitHub Copilot
## Chronique des Mondes - Projet VTT D&D Moderne

---

## 🎯 **VISION DU PROJET**

**Chronique des Mondes** est une **plateforme VTT (Virtual Tabletop) moderne** extensible pour ### **🔧 Principe Générique d'Abord (OBLIGATOIRE)**
- 🌐 **TOUJOURS créer la version générique** avant la spécialisée
- 🎯 **GameType enum** pour distinguer les systèmes (Generic, DnD, Pathfinder)  
- 📁 **Séparer Common/ et Dnd/** rigoureusement dans tous les projets
- ⚡ **Héritage de classes abstraites** : ACharacter, ASpell, AEquipment
- 🔄 **Extensibilité** : penser aux futurs systèmes de jeu dès la conception
- 📊 **APIs génériques** avec paramètres GameType pour filtrage

### **⚙️ Conventions de Code C#**
- ✅ **Pas d'underscore** (`_xxx`) pour les membres privés
- ✅ **Toujours utiliser `this.`** pour les membres de classe  
- ✅ **PascalCase** pour classes, méthodes, propriétés, enums
- ✅ **camelCase** pour variables locales et paramètres
- ✅ **Pas de code mort** ou TODO non justifié
- ✅ **Validation des permissions** sur tous endpoints sensibles
- ✅ **Exceptions métier** (`BusinessException`) pour la logique
- ✅ **Documentation XML** pour méthodes publiques importantes*multiple systèmes de jeux de rôle** en ligne, avec une interface sombre professionnelle et des fonctionnalités avancées de gestion de campagne.

### **🔧 PRINCIPE ARCHITECTURAL FONDAMENTAL : GÉNÉRIQUE D'ABORD**

**RÈGLE D'OR** : Toute fonctionnalité est développée de manière **générique par défaut**, puis **spécialisée par système de jeu** si nécessaire.

```
🌐 GÉNÉRIQUE (Common) → 🎲 SPÉCIALISÉ (Dnd, Pathfinder, etc.)
   ├── Modèles de base          ├── Extensions spécifiques
   ├── Logique commune          ├── Règles particulières  
   ├── API générales            ├── Calculs spécialisés
   └── Interface standard       └── UI adaptée
```

### **Objectifs Principaux**
- ✅ **Interface moderne** Blazor avec thème sombre
- ⚔️ **Système de combat D&D 5e** complet avec calculs automatiques  
- 🎲 **Gestion des sorts** (74 sorts D&D intégrés)
- 🛡️ **Système d'équipements** (33 équipements avec propriétés D&D)
- 👥 **Gestion des campagnes** multi-joueurs avec invitations
- 🔐 **Authentification JWT** sécurisée
- 📊 **API REST** complète pour toutes les interactions

---

## 🏗️ **ARCHITECTURE TECHNIQUE**

### **Stack Technologique (.NET 9)**
```
Frontend:  Blazor Server + MudBlazor (thème sombre)
Backend:   .NET 9 Minimal APIs + Entity Framework Core
Database:  SQL Server (LocalDB + Azure SQL)
Hosting:   .NET Aspire (orchestration des services)
Auth:      JWT + ASP.NET Core Identity
Testing:   xUnit + HTTP Client tests
```

### **Structure Multi-Projets**
```
📁 Chronique.Des.Monde/                    # Solution racine
├── 🌐 Cdm.Web/                           # Interface Blazor utilisateur
├── 🔌 Cdm.ApiService/                     # API REST principale
├── 🏠 Cdm.AppHost/                        # Orchestration Aspire
├── 🗄️ Cdm.Data.Common/                   # Modèles de données génériques
├── 🎲 Cdm.Data.Dnd/                       # Modèles D&D spécifiques
├── ⚡ Cdm.Business.Common/                # Logique métier générique
├── 🎯 Cdm.Business.Dnd/                   # Logique métier D&D
├── 🔄 Cdm.Migrations/                     # Migrations Entity Framework
├── 🛠️ Cdm.MigrationsManager/             # Gestionnaire de migrations
└── 🧪 Cdm.Tests/                          # Tests unitaires et intégration
```

### **Architecture en Couches**

#### **🌐 Couche Présentation (Cdm.Web)**
- **Blazor Server** avec **MudBlazor** pour l'UI moderne
- **Thème sombre** professionnel avec design responsive
- **Composants réutilisables** pour personnages, sorts, équipements
- **Services HTTP typés** pour communication API
- **Gestion d'état** centralisée pour les sessions utilisateur

#### **🔌 Couche API (Cdm.ApiService)**  
- **Minimal APIs** pour endpoints REST optimisés
- **Middleware JWT** pour authentification sécurisée
- **Validation automatique** des modèles avec FluentValidation
- **Gestion d'erreurs** centralisée avec middleware personnalisé
- **Documentation Swagger** automatique avec exemples

#### **⚡ Couche Business (Cdm.Business.*)**
- **Services métier** par domaine (Combat, Sorts, Équipements)
- **Moteur de combat D&D 5e** avec calculs automatiques
- **Validation des règles** métier et permissions
- **Orchestration** des opérations complexes
- **Cache** pour optimiser les performances

#### **🗄️ Couche Data (Cdm.Data.*)**
- **Entity Framework Core 9** comme ORM principal
- **Modèles par système** (Generic vs D&D spécifique)  
- **Configurations Fluent API** pour relations complexes
- **Migrations automatisées** avec scripts de données
- **Héritage TPH** (Table Per Hierarchy) pour personnages

---

## 🎲 **SYSTÈMES FONCTIONNELS**

### **⚔️ Système de Combat D&D 5e**
```csharp
// Moteur de combat principal
CombatEngine -> DndCombatCalculator
├── Initiative automatique (1d20 + mod)
├── Actions de combat (Attaque, Sort, Mouvement)  
├── Calculs de dégâts avec résistances
├── Gestion des tours et conditions
└── Historique complet des actions
```

**Endpoints Combat (/api/combat):**
- `POST /start` - Démarrer un combat
- `POST /join` - Rejoindre un combat
- `POST /action` - Exécuter une action
- `GET /{id}` - État du combat
- `POST /end` - Terminer le combat

### **✨ Système de Sorts D&D**
```csharp  
// 74 sorts D&D 5e intégrés
SpellDnd : ASpell
├── Niveau (0-9), École de magie
├── Temps d'incantation, Portée, Durée
├── Composants (V, S, M), Matériaux
├── Calculs de dégâts automatiques
└── Emplacements de sorts par niveau
```

**Endpoints Sorts (/api/spells):**
- `GET /official` - Sorts officiels D&D
- `GET /{id}` - Détails d'un sort
- `POST /character/{id}/assign` - Assigner à un personnage
- `GET /character/{id}` - Sorts du personnage

### **🛡️ Système d'Équipements**  
```csharp
// 33 équipements D&D avec propriétés
EquipmentDnd : AEquipment  
├── Catégorie (Arme, Armure, Objet)
├── Propriétés (Finesse, Polyvalent, etc.)
├── Dégâts, CA, Modificateurs
├── Rareté et coût en po
└── Conditions d'utilisation
```

**Endpoints Équipements (/api/equipment):**
- `GET /official` - Équipements officiels D&D
- `GET /{id}` - Détails d'un équipement  
- `POST /character/{id}/give` - Donner à un personnage
- `GET /character/{id}/inventory` - Inventaire du personnage

### **👥 Système de Campagnes**
```csharp
Campaign -> Chapters -> ContentBlocks
├── Gestion multi-joueurs avec rôles
├── Invitations par email avec tokens
├── Chapitres et contenu organisé  
├── NPCs et dialogues intégrés
└── Sessions de jeu avec historique
```

### **🔐 Système d'Authentification**
- **JWT Tokens** avec refresh automatique
- **Rôles utilisateur** (Player, GameMaster, Admin)
- **Permissions granulaires** par campagne
- **Sécurisation** de tous les endpoints sensibles

---

## � **ARCHITECTURE GÉNÉRIQUE D'ABORD - EXEMPLES CONCRETS**

### **Pattern d'Extension Systématique**
```csharp
// 1️⃣ ÉTAPE 1 : Créer la classe générique (Common)
public abstract class ACharacter 
{
    public GameType GameType { get; set; } = GameType.Generic;
    public string Name { get; set; }
    public int Level { get; set; }
    // ... propriétés communes à TOUS les systèmes
}

// 2️⃣ ÉTAPE 2 : Spécialiser par système (Dnd)  
public class CharacterDnd : ACharacter
{
    public string Race { get; set; }      // Spécifique D&D
    public string Class { get; set; }     // Spécifique D&D  
    public DndStats Stats { get; set; }   // Spécifique D&D
}

// 3️⃣ ÉTAPE 3 : Futurs systèmes (extensibilité)
public class CharacterPathfinder : ACharacter { /* ... */ }
public class CharacterCoC : ACharacter { /* ... */ }
```

### **Services Génériques avec Spécialisation**
```csharp
// Service générique de base
public interface ICharacterService<T> where T : ACharacter
{
    Task<T> CreateAsync(T character);
    Task<IEnumerable<T>> GetByGameTypeAsync(GameType gameType);
}

// Spécialisation D&D avec logique métier spécifique  
public class CharacterDndService : ICharacterService<CharacterDnd>
{
    // Logique spécifique D&D (calculs de stats, etc.)
}
```

### **Endpoints API Génériques**
```csharp
// Endpoint générique avec filtrage par GameType
app.MapGet("/api/characters", (GameType? gameType) => 
{
    return gameType switch
    {
        GameType.DnD => dndService.GetAllAsync(),
        GameType.Pathfinder => pathfinderService.GetAllAsync(),
        _ => genericService.GetAllAsync()
    };
});
```

---

## �📊 **MODÈLES DE DONNÉES PRINCIPAUX**

### **Hiérarchie des Personnages**
```csharp
public abstract class ACharacter  
{
    public int Id { get; set; }
    public string Name { get; set; }
    public GameType GameType { get; set; }  // Generic, DnD, Pathfinder
    public bool IsNpc { get; set; }
    public bool IsHostile { get; set; }
    // ... autres propriétés communes
}

public class CharacterDnd : ACharacter
{
    public int Level { get; set; }
    public string Class { get; set; }
    public string Race { get; set; }  
    public DndStats Stats { get; set; }
    // ... propriétés spécifiques D&D
}
```

### **Hiérarchie des Sorts**
```csharp  
public abstract class ASpell
{
    public int Id { get; set; }
    public string Name { get; set; }
    public GameType GameType { get; set; }
    public bool IsPublic { get; set; }
    // ... propriétés communes
}

public class SpellDnd : ASpell  
{
    public int Level { get; set; }        // 0-9
    public string School { get; set; }    // Évocation, etc.
    public string CastingTime { get; set; }
    public string Range { get; set; }
    public string Components { get; set; } // V, S, M
    // ... propriétés D&D spécifiques
}
```

### **Système de Combat**
```csharp
public class CombatSession
{
    public int Id { get; set; }
    public string SessionId { get; set; }
    public CombatStatus Status { get; set; }
    public int CurrentRound { get; set; }
    public List<CombatParticipant> Participants { get; set; }
    public List<CombatAction> Actions { get; set; }
}

public class CombatAction  
{
    public int Id { get; set; }
    public ActionType Type { get; set; }  // Attack, Spell, Move
    public string Description { get; set; }
    public string Result { get; set; }    // JSON des résultats
    public DateTime ExecutedAt { get; set; }
}
```

---

## ⚙️ **CONVENTIONS DE DÉVELOPPEMENT**

### **🎯 Conventions de Code C#**
- ✅ **Pas d'underscore** (`_xxx`) pour les membres privés
- ✅ **Toujours utiliser `this.`** pour les membres de classe  
- ✅ **PascalCase** pour classes, méthodes, propriétés, enums
- ✅ **camelCase** pour variables locales et paramètres
- ✅ **Pas de code mort** ou TODO non justifié
- ✅ **Validation des permissions** sur tous endpoints sensibles
- ✅ **Exceptions métier** (`BusinessException`) pour la logique
- ✅ **Documentation XML** pour méthodes publiques importantes

### **🏗️ Architecture & Separation**  
- ✅ **Séparer Business/Data/API** rigoureusement
- ✅ **Injection de dépendances** partout (services, repositories)
- ✅ **Services typés HTTP** pour communication API
- ✅ **Configuration par appsettings** (pas de hardcoding)
- ✅ **Middleware** pour cross-cutting concerns
- ✅ **Logs structurés** pour actions importantes

### **🗄️ Entity Framework & Base de Données**
- ✅ **Fluent API** pour configurations complexes  
- ✅ **Migrations** pour tous changements de schéma
- ✅ **TPH (Table Per Hierarchy)** pour héritage
- ✅ **Indexes** sur colonnes fréquemment queryées  
- ✅ **Relations** avec DeleteBehavior.Restrict pour éviter cycles
- ✅ **Seeding** pour données de référence (sorts, équipements)

### **🌐 Frontend Blazor & UI**
- ✅ **MudBlazor** pour tous les composants UI
- ✅ **Thème sombre** cohérent sur toute l'application
- ✅ **Composants réutilisables** pour entités métier
- ✅ **Services HTTP typés** pour appels API
- ✅ **Gestion d'état** centralisée avec services Scoped
- ✅ **Responsive design** mobile/desktop

### **🧪 Tests & Qualité**
- ✅ **Tests unitaires** pour logique métier critique
- ✅ **Tests d'intégration** pour endpoints API  
- ✅ **Fichiers .http** pour tests manuels
- ✅ **Build sans warnings** obligatoire
- ✅ **Validation** avant chaque commit

---

## 🔄 **FLUX DE DONNÉES TYPIQUES**

### **Création d'un Personnage**
```
UI (Blazor) -> HTTP Client -> API Endpoint -> Business Service -> EF Repository -> Database
                    ↓
            Validation + Rules -> Character Creation -> Return DTO
```

### **Combat D&D**  
```
Combat UI -> Combat API -> Combat Engine -> DndCalculator
     ↓            ↓              ↓              ↓
Save State -> Update DB -> Apply Rules -> Calculate Results
```

### **Attribution de Sort**
```
Character Page -> Spell Assignment API -> Business Validation -> Database Update
       ↓                    ↓                      ↓                   ↓  
Check Permissions -> Verify Spell Level -> Update CharacterSpells -> Return Status
```

---

## 🚀 **POINTS D'ENTRÉE PRINCIPAUX**

### **🌐 Application Web (Port 5222)**
- **Pages Blazor** : `/`, `/characters`, `/login`, `/campaign/{id}`
- **Composants** : `CharacterCard`, `SpellList`, `CombatInterface`
- **Services** : `CharacterService`, `SpellService`, `CombatService`

### **🔌 API REST (Port 7428)**  
- **Authentication** : `/api/auth/*` (login, register, refresh)
- **Characters** : `/api/characters/*` (CRUD + D&D specific)
- **Spells** : `/api/spells/*` (official, custom, assignments)  
- **Equipment** : `/api/equipment/*` (official, inventory, trades)
- **Combat** : `/api/combat/*` (sessions, actions, calculations)
- **Campaigns** : `/api/campaigns/*` (management, invitations)

### **🏠 Aspire Dashboard (Port 17241)**
- **Monitoring** des services et santé applicative
- **Logs centralisés** de tous les composants
- **Métriques** de performance et utilisation

---

## 🎯 **DOMAINES MÉTIER CLÉS**

### **⚔️ Combat & Actions**
- **Initiative** : Calcul automatique 1d20 + modificateur Dextérité  
- **Actions** : Attaque, Sort, Mouvement, Esquive, etc.
- **Dégâts** : Calculs avec résistances/vulnérabilités
- **Conditions** : Empoisonné, Charmé, Paralysé, etc.
- **Tours** : Gestion séquentielle avec timer optionnel

### **🎲 Sorts & Magie**  
- **Emplacements** : Gestion par niveau (1er à 9ème) 
- **Écoles** : Abjuration, Évocation, Illusion, etc.
- **Composants** : Verbal, Somatique, Matériel
- **Concentration** : Suivi des sorts actifs
- **Rituels** : Sorts lancés sans emplacement

### **🛡️ Équipements & Inventaire**
- **Catégories** : Armes, Armures, Outils, Objets magiques
- **Propriétés** : Finesse, Polyvalent, Lancer, etc.  
- **Attunement** : Objets magiques nécessitant harmonisation
- **Encombrement** : Calcul du poids transporté
- **Économie** : Prix en pièces d'or, commerce entre joueurs

### **👥 Social & Campagnes**
- **Rôles** : Joueur, Maître de Jeu, Observateur
- **Invitations** : System par email avec validation
- **Sessions** : Historique et planification  
- **NPCs** : Personnages non-joueurs avec dialogues
- **Contenu** : Chapitres, lieux, événements narratifs

---

## 📝 **INSTRUCTIONS SPÉCIFIQUES POUR L'AGENT**

### **🎯 Lors de Modifications de Code**
1. **PRIORITÉ 1** : Créer/modifier la version générique (Common) AVANT la spécialisée
2. **PRIORITÉ 2** : Utiliser GameType enum pour distinguer les systèmes de jeu  
3. **Respecter l'architecture** multi-couches existante (Common -> Dnd)
4. **Valider les permissions** pour toute action sensible  
5. **Utiliser les services existants** avant d'en créer de nouveaux
6. **Suivre les conventions** de nommage établies (A* pour abstraites)
7. **Ajouter des tests** pour nouvelle fonctionnalité majeure
8. **Documenter** les endpoints et services complexes

### **🗄️ Lors de Changements de Base de Données**  
1. **Créer une migration** EF pour tout changement de schéma
2. **Préserver les données** existantes (pas de suppression destructive)
3. **Ajouter des indexes** pour nouvelles requêtes fréquentes
4. **Utiliser TPH** pour héritage de classes (ACharacter, ASpell, etc.)
5. **Éviter les cycles** de suppression en cascade

### **🌐 Lors d'Ajout d'UI Blazor**
1. **Utiliser MudBlazor** exclusivement pour les composants
2. **Maintenir le thème sombre** sur tous nouveaux composants  
3. **Créer des composants réutilisables** pour entités communes
4. **Implémenter responsive design** mobile/desktop
5. **Utiliser services HTTP typés** pour appels API

### **🔧 Lors de Debugging/Troubleshooting**
1. **Vérifier les logs** Aspire dashboard en premier  
2. **Tester les endpoints** avec fichiers .http
3. **Valider les migrations** EF avant exécution
4. **Confirmer les permissions** utilisateur
5. **Vérifier la cohérence** des données D&D

---

## 🏁 **ÉTAT ACTUEL & PROCHAINES ÉTAPES**

### ✅ **Fonctionnalités Complètes**
- Interface Blazor moderne avec thème sombre
- Système de combat D&D 5e avec 33 équipements  
- 74 sorts D&D intégrés avec calculs automatiques
- API REST complète avec 40+ endpoints
- Authentification JWT sécurisée
- Base de données avec migrations complètes
- Orchestration Aspire fonctionnelle

### 🚧 **En Cours de Développement**  
- Interface de création de campagne avancée
- Système de notifications en temps réel
- Import/Export de personnages D&D Beyond
- Intégration cartes interactives  
- Mode hors-ligne avec synchronisation

### 🎯 **Priorités Techniques**
- Optimisation des performances (cache, requêtes)
- Tests automatisés complets (unit + integration)  
- Documentation API Swagger complète
- CI/CD pipeline pour déploiement Azure
- Monitoring et métriques avancées

---

*Ce document de contexte est lu automatiquement par GitHub Copilot pour garantir la cohérence architecturale et fonctionnelle du projet Chronique des Mondes.*