# Chronique des Mondes - Backend API

## 🎯 Objectif de l'application

L'objectif de l'application est de créer une plateforme JDR où un utilisateur peut être joueur ou maître du jeu (MJ).  
Le socle est générique, puis des logiques métiers spécifiques à chaque jeu (ex : D&D) viennent compléter les fonctionnalités de base.

- **Création de personnage** : Par défaut, un personnage possède un nom, prénom, points de vie. Si un jeu est précisé (ex : D&D), des champs supplémentaires sont requis (caractéristiques, compétences, etc.).
- **Routage métier** : Les endpoints API sont tagués pour diriger les requêtes vers la logique métier appropriée selon le jeu.
- **Gestion de campagnes** : Création, gestion, et suivi de campagnes de jeu de rôle, incluant la gestion des combats.

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
│   ├── UserEndpoints.cs           # API utilisateurs/auth
│   └── WeatherEndpoints.cs        # API exemple météo
├── Services/               # Services métier
│   ├── JwtService.cs              # Gestion des tokens JWT
│   └── PasswordService.cs         # Chiffrement mots de passe
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
    └── README.md                  # Ce fichier
```

## 🧑‍🤝‍🧑 Gestion des utilisateurs et des campagnes

- Un utilisateur peut créer une campagne et devient alors MJ.
- Il peut inviter des joueurs à rejoindre sa campagne.
- Il peut rendre sa campagne publique pour permettre à d'autres joueurs de la rejoindre, ou la rendre accessible à d'autres MJ qui souhaitent la dupliquer et jouer avec leur propre groupe.

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

### Météo (Exemple)
| Méthode | Endpoint | Description |
|---------|----------|-------------|
| `GET` | `/weatherforecast` | Prévisions météo test |

## 📝 Cas d'utilisation

### Cas d'utilisation 1 : Combat dans une campagne D&D

- **Contexte** : Chapitre 3 d'une campagne D&D, un combat est prévu.
- **Préparation** : Le MJ a préconfiguré le combat avec les PNJ/monstres à affronter, ou les ajoute à la volée.
- **Déroulement** :
    - Au lancement du combat, un jet d'initiative est effectué pour tous les participants.
    - Les joueurs reçoivent une notification quand c'est leur tour.
    - Un joueur peut lancer une attaque, choisir une cible, et lancer les dés nécessaires.
    - L'application calcule automatiquement le résultat (prise en compte de l'arme, modificateurs, CA ennemie, etc.).
    - Si l'attaque réussit, le joueur lance les dés de dégâts, et les points de vie de la cible sont mis à jour.
- **Pour les systèmes non pris en charge** : Les calculs sont manuels, mais l'interface permet au MJ de modifier les valeurs à la main.

### Cas d'utilisation 2 : Création de personnage multi-système

```http
# 1. Création d'un personnage D&D
POST /character/dnd?userId=1 HTTP/1.1
X-GameType: dnd
Authorization: Bearer {jwt_token}
Content-Type: application/json

{
  "name": "Gorthak",
  "class": "Guerrier",
  "race": "Nain",
  "level": 1,
  "strength": 16,
  "dexterity": 10,
  "constitution": 15,
  "intelligence": 11,
  "wisdom": 13,
  "charisma": 8,
  "hitPoints": 12,
  "armorClass": 16
}

# 2. Duplication pour un autre système
POST /character?userId=1 HTTP/1.1
X-GameType: generic
Authorization: Bearer {jwt_token}
Content-Type: application/json

{
  "name": "Gorthak (Skyrim)",
  "customFields": {
    "stamina": 100,
    "magicka": 50,
    "skill_onehanded": 75
  }
}
```

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
- Contrôle d'accès par utilisateur (un utilisateur ne peut voir que ses personnages)

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
```

## 📋 Prochaines Étapes

### Fonctionnalités à Implémenter
- **Campagnes** - CRUD et gestion des participants
- **Combats** - Système d'initiative et tours de jeu
- **Sorts et Équipements** - Bibliothèques d'objets magiques
- **Invitations** - Système de notifications pour rejoindre campagnes
- **Chat en temps réel** - Communication entre joueurs via SignalR

### Améliorations Techniques
- **Cache** - Redis pour les données fréquemment consultées
- **Rate Limiting** - Protection contre les abus
- **Monitoring** - Métriques et logs centralisés
- **Documentation OpenAPI** - Spécifications API complètes
- **Tests d'intégration** - Validation des workflows complets

---

*Développé avec ❤️ pour la communauté JDR - Une API robuste et extensible pour tous vos besoins de jeu de rôle !*