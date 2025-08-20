# Chronique des Mondes - Backend API

## 🎯 Objectif de l'application

L'objectif de l'application est de créer une plateforme JDR où un utilisateur peut être **joueur** ou **maître du jeu (MJ)** - et même les deux à la fois dans différentes campagnes !
Le socle est générique, puis des logiques métiers spécifiques à chaque jeu (ex : D&D) viennent compléter les fonctionnalités de base.

- **Création de personnage** : Par défaut, un personnage possède un nom, prénom, points de vie. Si un jeu est précisé (ex : D&D), des champs supplémentaires sont requis (caractéristiques, compétences, etc.).
- **Routage métier** : Les endpoints API sont tagués pour diriger les requêtes vers la logique métier appropriée selon le jeu.
- **Gestion de campagnes** : Création, gestion, et suivi de campagnes de jeu de rôle, incluant la gestion des combats par chapitres.

## 🏗️ Architecture Technique

### Stack Technologique
- **.NET 9** - Framework principal
- **ASP.NET Core Minimal APIs** - Endpoints REST
- **Entity Framework Core** - ORM pour base de données
- **JWT Authentication** - Sécurité et authentification
- **Aspire** - Orchestration et configuration

### Structure du Projet
```
Cdm.ApiService/
├── Endpoints/              # Définition des endpoints REST
│   ├── CharacterEndpoint.cs      # API personnages
│   ├── CampaignEndpoint.cs        # API campagnes et chapitres
│   ├── CombatEndpoint.cs          # API gestion des combats
│   ├── NpcEndpoint.cs             # API PNJ et monstres
│   ├── UserEndpoints.cs           # API utilisateurs/auth
│   └── WeatherEndpoints.cs        # API exemple météo
├── Services/               # Services métier
│   ├── JwtService.cs              # Gestion des tokens JWT
│   ├── PasswordService.cs         # Chiffrement mots de passe
│   ├── CombatService.cs           # Logique de combat
│   └── CampaignService.cs         # Logique de campagne
├── Extensions/             # Extensions et configuration
│   ├── ServiceCollectionExtensions.cs
│   └── EndpointMappingExtensions.cs
├── Tests/                  # Tests API et documentation
│   ├── README.md                  # Guide des tests
│   ├── character-config.http      # Configuration test
│   ├── Generic/                   # Tests CRUD génériques
│   ├── Dnd/                       # Tests spécifiques D&D
│   ├── Security/                  # Tests sécurité
│   └── Scenarios/                 # Tests end-to-end
└── Documentation/          # Documentation technique
    ├── README.md                  # Ce fichier
    └── UseCases.md                # Cas d'utilisation détaillés
```

## 🧑‍🤝‍🧑 Gestion des utilisateurs et des campagnes

### Rôles Multiples
- **Un utilisateur peut être MJ d'une campagne ET joueur dans une autre** simultanément
- Chaque campagne a un seul MJ (créateur de la campagne)
- Un utilisateur peut participer en tant que joueur à plusieurs campagnes

### Gestion des Campagnes
- Un utilisateur peut créer une campagne et devient alors MJ.
- Il peut inviter des joueurs à rejoindre sa campagne.
- Il peut rendre sa campagne publique pour permettre à d'autres joueurs de la rejoindre, ou la rendre accessible à d'autres MJ qui souhaitent la dupliquer et jouer avec leur propre groupe.

## 🏰 Création et Structure des Campagnes

### Tags de Système de Jeu
- **Création initiale** : Une campagne peut être taguée avec un jeu supporté (D&D, Skyrim à venir) ou rester générique
- **Tag ajouté ultérieurement** : Possibilité d'ajouter un tag plus tard pour débloquer les PNJ/monstres préconfigurés
- **Avantages du tag** : Accès à des bibliothèques de PNJ et monstres spécifiques au système de jeu

### Structure par Chapitres
Une campagne est organisée en **chapitres** successifs :

#### Création d'un Chapitre
- **Navigation** : Flèches haut/bas pour naviguer entre chapitres
- **Contenu narratif** : Blocs de texte pour décrire les événements du chapitre
- **Onglets** : PNJ et Monstres disponibles pour ce chapitre

#### Liaison des PNJ aux Événements
- **Référencement** : Possibilité de lier un PNJ à un bloc de texte
- **Contextualisation** : Backgrounds comportementaux selon l'attitude des joueurs
  - 🟢 **Comportement amical** : Encadré vert avec dialogue/attitude cordiale
  - 🟡 **Comportement neutre** : Encadré jaune avec attitude standard
  - 🔴 **Comportement hostile** : Encadré rouge avec attitude agressive

### Gestion des PNJ et Monstres par Campagne

#### Types de Création
1. **PNJ Générique** : Nom, prénom, description (obligatoire) - pour usage ponctuel
2. **PNJ/Monstre Spécialisé** : Avec stats du système de jeu (ex: D&D) pour les combats

#### Exemple de Workflow
```
Campagne D&D "Les Terres Oubliées" (tag: dnd)
├── Chapitre 1: "L'Arrivée au Village"
│   ├── PNJ: Aubergiste Brom (générique - nom, description)
│   └── Monstre: Gobelins (D&D - stats complètes pour combat)
├── Chapitre 2: "La Forêt Hantée"
│   ├── PNJ: Ermite Sage (générique)
│   └── Monstre: Loup-garou (D&D - CA, PV, attaques)
```

## 🧙‍♂️ Création de personnages

### Logique Métier Générique vs Spécialisée
- Un utilisateur peut créer un personnage générique ou spécifique à un jeu (ex : D&D).
- Un personnage D&D ne peut rejoindre qu'une campagne D&D (idem pour d'autres jeux).
- Une option de duplication permet de changer le système de jeu d'un personnage ou de le rendre générique.
- Le mode générique permet d'ajouter manuellement des champs personnalisés pour s'adapter à des systèmes non pris en charge.

### Routage par Header X-GameType
```http
POST /character/dnd HTTP/1.1
X-GameType: dnd
Content-Type: application/json

{
  "name": "Thorek",
  "class": "Guerrier",
  "race": "Nain",
  "strength": 16,
  "dexterity": 10,
  // ... autres stats D&D
}
```

## ⚔️ Gestion des combats

### Interface MJ de Combat
- **Vue par chapitre** : Le MJ visualise son chapitre avec onglets PNJ/Monstres
- **Sélection d'adversaires** : Choix des PNJ/monstres à inclure dans le combat
- **Déclenchement** : Lance le système de combat avec les participants sélectionnés

### Système de Combat Automatisé
- Le MJ peut déclencher un combat dans une campagne.
- L'application permet de lancer des dés, d'ajouter des modificateurs selon le système de jeu, et de suivre l'état du combat.
- Pour D&D, le calcul des attaques, dégâts, et comparaisons avec la CA ennemie sont automatisés.
- Pour les systèmes non pris en charge, le suivi est manuel, mais le MJ garde la main sur les points de vie et les caractéristiques.

## 📡 Endpoints API

### Authentification
| Méthode | Endpoint | Description | Corps |
|---------|----------|-------------|--------|
| `POST` | `/login` | Connexion utilisateur | `{ email, password }` |
| `POST` | `/register` | Inscription nouveau compte | `{ userName, userEmail, password }` |

### Personnages
| Méthode | Endpoint | Description | Headers Requis |
|---------|----------|-------------|----------------|
| `GET` | `/character?userId={id}` | Liste des personnages | `Authorization: Bearer {token}` |
| `GET` | `/character/{id}` | Détails d'un personnage | `Authorization: Bearer {token}` |
| `POST` | `/character?userId={id}` | Création générique | `Authorization`, `X-GameType: generic` |
| `POST` | `/character/dnd?userId={id}` | Création D&D | `Authorization`, `X-GameType: dnd` |
| `PUT` | `/character/{id}` | Modification générique | `Authorization`, `X-GameType: generic` |
| `PUT` | `/character/dnd/{id}` | Modification D&D | `Authorization`, `X-GameType: dnd` |
| `DELETE` | `/character/{id}` | Suppression | `Authorization: Bearer {token}` |

### Campagnes
| Méthode | Endpoint | Description | Headers Requis |
|---------|----------|-------------|----------------|
| `GET` | `/campaign?userId={id}` | Campagnes de l'utilisateur | `Authorization: Bearer {token}` |
| `GET` | `/campaign/{id}` | Détails d'une campagne | `Authorization: Bearer {token}` |
| `POST` | `/campaign?userId={id}` | Création de campagne | `Authorization`, `X-GameType: {type}` |
| `PUT` | `/campaign/{id}` | Modification de campagne | `Authorization: Bearer {token}` |
| `DELETE` | `/campaign/{id}` | Suppression de campagne | `Authorization: Bearer {token}` |

### Chapitres
| Méthode | Endpoint | Description | Headers Requis |
|---------|----------|-------------|----------------|
| `GET` | `/campaign/{id}/chapters` | Chapitres d'une campagne | `Authorization: Bearer {token}` |
| `GET` | `/chapter/{id}` | Détails d'un chapitre | `Authorization: Bearer {token}` |
| `POST` | `/campaign/{id}/chapter` | Création de chapitre | `Authorization: Bearer {token}` |
| `PUT` | `/chapter/{id}` | Modification de chapitre | `Authorization: Bearer {token}` |
| `DELETE` | `/chapter/{id}` | Suppression de chapitre | `Authorization: Bearer {token}` |

### PNJ/Monstres
| Méthode | Endpoint | Description | Headers Requis |
|---------|----------|-------------|----------------|
| `GET` | `/chapter/{id}/npcs` | PNJ d'un chapitre | `Authorization: Bearer {token}` |
| `GET` | `/npc/{id}` | Détails d'un PNJ | `Authorization: Bearer {token}` |
| `POST` | `/chapter/{id}/npc` | Création PNJ/Monstre | `Authorization`, `X-GameType: {type}` |
| `PUT` | `/npc/{id}` | Modification PNJ/Monstre | `Authorization`, `X-GameType: {type}` |
| `DELETE` | `/npc/{id}` | Suppression PNJ/Monstre | `Authorization: Bearer {token}` |

### Combats
| Méthode | Endpoint | Description | Headers Requis |
|---------|----------|-------------|----------------|
| `POST` | `/chapter/{id}/combat/start` | Démarrer un combat | `Authorization: Bearer {token}` |
| `GET` | `/combat/{id}` | État du combat | `Authorization: Bearer {token}` |
| `POST` | `/combat/{id}/action` | Action de combat | `Authorization: Bearer {token}` |
| `PUT` | `/combat/{id}/end` | Terminer le combat | `Authorization: Bearer {token}` |

### Météo (Exemple)
| Méthode | Endpoint | Description |
|---------|----------|-------------|
| `GET` | `/weatherforecast` | Prévisions météo test |

## 🛡️ Sécurité

### Authentification JWT
- Tous les endpoints personnages nécessitent un token JWT valide
- Le token contient l'ID utilisateur pour l'autorisation
- Expiration configurable des tokens

### Headers Obligatoires
- `Authorization: Bearer {token}` - Authentification
- `X-GameType: {gametype}` - Routage métier (dnd, generic, skyrim, etc.)

### Validation des Données
- Validation automatique des modèles
- Sanitisation des entrées utilisateur
- Contrôle d'accès par utilisateur (un utilisateur ne peut voir que ses personnages/campagnes)
- Contrôle d'accès MJ (seul le MJ peut modifier sa campagne)

## 🔧 Configuration

### Variables d'Environnement
```json
{
  "Jwt": {
    "SecretKey": "votre-clé-secrète-256-bits",
    "Issuer": "ChroniqueDesMondes",
    "Audience": "ChroniqueDesMondes-Users",
    "ExpiryMinutes": 1440
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ChroniqueDesMondes;Trusted_Connection=true;"
  }
}
```

### Services Configurés
- **JWT Authentication** - Sécurité des endpoints
- **Entity Framework** - Accès base de données
- **CORS** - Autorisation cross-origin pour le frontend
- **Swagger** - Documentation API automatique

## 🧪 Tests et Documentation

### Tests API Disponibles
Le dossier `Tests/` contient une suite complète de tests HTTP :
- **Tests CRUD génériques** - Validation des opérations de base
- **Tests spécifiques D&D** - Validation de la logique métier D&D
- **Tests de sécurité** - Validation de l'authentification et autorisation
- **Scénarios end-to-end** - Tests de workflows complets

### Utilisation des Tests
1. Démarrer l'API : `dotnet run`
2. Configurer les variables dans `Tests/character-config.http`
3. Exécuter les tests avec l'extension REST Client de VS Code

## 🚀 Développement

### Démarrage Rapide
```bash
# 1. Cloner et naviguer vers le projet
cd Cdm.ApiService

# 2. Restaurer les dépendances
dotnet restore

# 3. Mettre à jour la base de données
dotnet ef database update

# 4. Lancer l'API
dotnet run

# 5. Accéder à Swagger
# https://localhost:7428/swagger
```

### Ajout d'un Nouveau Système de Jeu
1. **Créer les modèles** dans `Cdm.Data.{GameType}`
2. **Implémenter la logique** dans `Cdm.Business.{GameType}`
3. **Ajouter les endpoints** dans `Endpoints/{GameType}Endpoint.cs`
4. **Configurer le routage** par header `X-GameType`
5. **Ajouter les tests** dans `Tests/{GameType}/`
6. **Ajouter les PNJ/monstres** prédéfinis en base

### Structure des Données
```csharp
// Modèle générique
public class Character
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int UserId { get; set; }
    public string GameType { get; set; }
    public Dictionary<string, object> CustomFields { get; set; }
}

// Modèle D&D spécialisé
public class CharacterDnd : Character
{
    public string Class { get; set; }
    public string Race { get; set; }
    public int Level { get; set; }
    public int Strength { get; set; }
    public int Dexterity { get; set; }
    // ... autres stats D&D
}

// Modèles Campagne
public class Campaign
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int GameMasterId { get; set; }
    public string GameType { get; set; }
    public List<Chapter> Chapters { get; set; }
    public List<Player> Players { get; set; }
}

public class Chapter
{
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public int ChapterNumber { get; set; }
    public string Title { get; set; }
    public List<NarrativeBlock> NarrativeBlocks { get; set; }
    public List<Npc> Npcs { get; set; }
}

public class Npc
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; } // "npc" ou "monster"
    public string GameType { get; set; }
    public Dictionary<string, object> Stats { get; set; }
    public List<BehaviorContext> Behaviors { get; set; }
}

public class BehaviorContext
{
    public string PlayerAttitude { get; set; } // "friendly", "neutral", "hostile"
    public string NpcResponse { get; set; }
    public string BackgroundColor { get; set; } // "green", "yellow", "red"
}
```

## 📋 Prochaines Étapes

### Fonctionnalités à Implémenter
- **Interface IA** - Assistance pour création de chapitres, lieux, PNJ et monstres
- **Bibliothèques pré-configurées** - PNJ et monstres D&D en base de données
- **Système d'invitations** - Notifications pour rejoindre campagnes
- **Chat en temps réel** - Communication entre joueurs via SignalR
- **Gestion des tours** - Interface temps réel pour les combats

### Améliorations Techniques
- **Cache** - Redis pour les données fréquemment consultées
- **Rate Limiting** - Protection contre les abus
- **Monitoring** - Métriques et logs centralisés
- **Documentation OpenAPI** - Spécifications API complètes
- **Tests d'intégration** - Validation des workflows complets

## 📖 Documentation Détaillée

Pour des cas d'utilisation complets et des exemples détaillés, consultez :
- **[Cas d'utilisation détaillés](./UseCases.md)** - Scénarios complets avec exemples API

---

*Développé avec ❤️ pour la communauté JDR - Une API robuste et extensible pour tous vos besoins de jeu de rôle !*