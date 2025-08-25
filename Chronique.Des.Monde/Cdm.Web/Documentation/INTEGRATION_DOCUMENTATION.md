# Intégration Front-End et Back-End - Documentation

## 🔗 Connexions API Implémentées

### Services Créés

#### 1. ApiService
- **Fichier**: `Cdm.Web\Services\ApiService.cs`
- **Interface**: `IApiService`
- **Responsabilité**: Gestion des appels API pour l'authentification
- **Endpoints**: 
  - `POST /login` - Connexion utilisateur
  - `POST /register` - Inscription utilisateur

#### 2. CharacterService
- **Fichier**: `Cdm.Web\Services\CharacterService.cs`
- **Interface**: `ICharacterService`
- **Responsabilité**: Gestion des personnages via l'API
- **Endpoints**:
  - `GET /character?userId={id}` - Récupération des personnages
  - `GET /character/{id}` - Récupération d'un personnage
  - `POST /character?userId={id}` - Création d'un personnage
  - `POST /character/dnd?userId={id}` - Création d'un personnage D&D

#### 3. JwtService
- **Fichier**: `Cdm.Web\Services\JwtService.cs`
- **Interface**: `IJwtService`
- **Responsabilité**: Validation et gestion des tokens JWT
- **Méthodes**:
  - `ValidateToken()` - Validation d'un token
  - `IsTokenValid()` - Vérification de validité
  - `GetUserIdFromToken()` - Extraction de l'ID utilisateur
  - `GetEmailFromToken()` - Extraction de l'email

### Modèles de Données

#### 1. ApiModels.cs
```csharp
- LoginRequest/LoginResponse
- RegisterRequest/RegisterResponse
- ApiResponse<T> (classe générique pour les réponses)
```

#### 2. CharacterModels.cs
```csharp
- Character (modèle principal)
- CharacterRequest (pour création)
- CharacterDndRequest (pour D&D spécifique)
```

## 📂 Code Behind Implémenté

### Pages Migrées

#### 1. Login.razor
- **Code Behind**: `Login.razor.cs`
- **Fonctionnalités**:
  - Validation de formulaire
  - Appel API pour connexion
  - Gestion des erreurs
  - Animations JavaScript

#### 2. Register.razor
- **Code Behind**: `Register.razor.cs`
- **Fonctionnalités**:
  - Validation complexe (mot de passe, email)
  - Appel API pour inscription
  - Vérification de confirmation de mot de passe
  - Indicateur de force du mot de passe

#### 3. Characters.razor
- **Code Behind**: `Characters.razor.cs`
- **Fonctionnalités**:
  - Chargement des personnages depuis l'API
  - Fallback vers données d'exemple
  - Gestion des états de chargement
  - Actions de création/édition/consultation

#### 4. Dice.razor
- **Code Behind**: `Dice.razor.cs`
- **Fonctionnalités**:
  - Logique de lancer de dés
  - Historique des lancers
  - Animations de résultats
  - Détection des critiques

#### 5. MainLayout.razor
- **Code Behind**: `MainLayout.razor.cs`
- **Fonctionnalités**:
  - Gestion de la déconnexion
  - Navigation conditionnelle

#### 6. ThemeSelector.razor
- **Code Behind**: `ThemeSelector.razor.cs`
- **Fonctionnalités**:
  - Sélection de thèmes
  - Persistance des préférences
  - Gestion des événements

## 🔧 Configuration et Injection de Dépendances

### Program.cs - Services Ajoutés

```csharp
// HttpClients configurés avec fallback intelligent
builder.Services.AddHttpClient<IApiService, ApiService>(client =>
{
    var apiServiceUrl = builder.Configuration.GetConnectionString("apiservice") 
                       ?? builder.Configuration["Services:ApiService:Url"]
                       ?? (builder.Environment.IsDevelopment() 
                           ? "https://localhost:7428"  // Dev URL
                           : "https+http://apiservice"); // Aspire URL
    
    client.BaseAddress = new Uri(apiServiceUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Services métier
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IJwtService, JwtService>();
```

### Configuration HTTP

- **Base URL Développement**: `https://localhost:7428`
- **Base URL Production**: Résolu par Aspire (`https+http://apiservice`)
- **Headers**: `Accept: application/json`
- **Authentification**: Bearer token via JWT
- **Headers personnalisés**: `X-GameType: Dnd`
- **Timeout**: 30 secondes

### ⚠️ Correction d'Erreur Importante

**Problème résolu**: `System.InvalidOperationException: An invalid request URI was provided`

**Solution**: Configuration hybride avec fallback URLs pour développement et Aspire pour production.

Voir `HttpClient_Configuration_Fix.md` pour les détails complets.

## 🛡️ Sécurité et Authentification

### Flow d'Authentification

1. **Connexion** (`Login.razor`)
   - Saisie utilisateur/mot de passe
   - Appel `POST /login` via `ApiService`
   - Réception du token JWT
   - Stockage dans les claims de cookie

2. **Utilisation du Token**
   - Token récupéré via `GetCurrentUserToken()`
   - Ajouté dans les headers `Authorization: Bearer`
   - Utilisé pour tous les appels API protégés

3. **Déconnexion** (`MainLayout.razor`)
   - Suppression du cookie d'authentification
   - Redirection vers la page d'accueil

### Protection des Endpoints

- **RequireAuth** component pour les pages protégées
- **Authorization headers** automatiques via les services
- **Validation côté client** avec DataAnnotations

## 🎯 Fonctionnalités Avancées

### Gestion d'Erreurs

- **Try-catch** dans tous les services
- **Logging** détaillé des erreurs et des requêtes
- **Messages utilisateur** informatifs
- **Fallback** vers données d'exemple si API indisponible

### Logging Amélioré

- **URL de base** loggée au démarrage
- **Requêtes sortantes** avec URL cible
- **Réponses** avec codes de statut et taille
- **Erreurs** avec contexte complet

### Optimisations Performance

- **Chargement asynchrone** des données
- **États de loading** avec spinners
- **Mise en cache** côté client (StateHasChanged)
- **HttpClient réutilisable** via DI
- **Timeout configuré** (30s)

### Expérience Utilisateur

- **Animations CSS** fluides
- **Feedback visuel** des actions
- **Messages d'erreur** contextuels
- **États de chargement** informatifs

## 🚀 Endpoints API Utilisés

### Backend Configuration

L'API backend expose les endpoints suivants via `UserEndpoints.cs` et `CharacterEndpoint.cs`:

#### Authentification
- `POST /login` - Connexion avec email/password
- `POST /register` - Inscription nouvel utilisateur

#### Personnages
- `GET /character?userId={id}` - Liste des personnages
- `GET /character/{id}` - Détails d'un personnage
- `POST /character?userId={id}` - Création personnage générique
- `POST /character/dnd?userId={id}` - Création personnage D&D

### Headers Requis

- `Authorization: Bearer {token}` - Pour les endpoints protégés
- `X-GameType: Dnd` - Spécification du type de jeu
- `Content-Type: application/json` - Pour les POST

### URLs par Environnement

#### Développement Local
- **API Service**: `https://localhost:7428`
- **Web Frontend**: `https://localhost:7153`

#### Production/Aspire
- **API Service**: Résolu par Aspire
- **Web Frontend**: Résolu par Aspire

## 📋 Configuration des Fichiers

### appsettings.json
```json
{
  "Services": {
    "ApiService": {
      "Url": "https://localhost:7428"
    }
  },
  "Logging": {
    "LogLevel": {
      "Cdm.Web.Services": "Debug"
    }
  }
}
```

### Hiérarchie de Configuration
1. **ConnectionString "apiservice"** (Aspire)
2. **Services:ApiService:Url** (appsettings.json)  
3. **Fallback développement** (`https://localhost:7428`)
4. **Fallback production** (`https+http://apiservice`)

## 📋 Prochaines Étapes

### À Implémenter

1. **Récupération ID Utilisateur** depuis le token JWT
2. **Pages d'édition** des personnages
3. **Gestion d'erreurs** plus fine avec codes HTTP
4. **Cache local** des données utilisateur
5. **Synchronisation offline** des données

### Améliorations Possibles

1. **Intercepteurs HTTP** pour les erreurs globales
2. **Retry policy** pour les appels API
3. **Pagination** pour les listes de personnages
4. **Real-time updates** avec SignalR
5. **Progressive Web App** (PWA) features
6. **Health checks** pour les services API

---

*L'intégration front-end/back-end est maintenant fonctionnelle avec une architecture clean et extensible !* 🎯