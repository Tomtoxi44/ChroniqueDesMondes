# Chronique des Mondes - Interface Web

## 🏰 Description

Interface web moderne pour la gestion de jeux de rôle avec un thème JDR immersif. Cette application Blazor Server offre une expérience utilisateur riche avec authentification, gestion de thèmes, et outils spécialisés pour les joueurs de D&D. **Maintenant entièrement connectée à l'API backend !**

## ✨ Fonctionnalités Implémentées

### 🔐 Authentification
- **Connexion sécurisée** avec authentification par cookies et JWT
- **Inscription** avec validation de mot de passe et appels API
- **Déconnexion** automatique
- **Protection des pages** sensibles avec redirection automatique
- **Intégration complète** avec l'API backend

### 🎨 Système de Thèmes
- **5 thèmes JDR** distincts :
  - **Sombre Fantastique** (par défaut) - Ambiance mystérieuse
  - **Clair Fantastique** - Ambiance épique
  - **Feu de Dragon** - Rouge et or pour les aventures ardentes
  - **Forêt Elfique** - Vert nature pour les quêtes forestières
  - **Forge Naine** - Métallique pour les artisans
- **Changement de thème** en temps réel
- **Persistance** du thème choisi

### 🎲 Outils de Jeu
- **Lanceur de dés** complet avec :
  - Support de tous les dés standards (d4, d6, d8, d10, d12, d20, d100)
  - Modificateurs personnalisables
  - Historique des lancers
  - Animations visuelles
  - Détection des critiques (réussite/échec)

### ⚔️ Gestion des Personnages
- **Connexion API** pour récupération des personnages
- **Fallback intelligent** vers données d'exemple si API indisponible
- **Statistiques de base** (PV, CA, Niveau)
- **Classes et races** diverses
- **Interface préparée** pour création/édition complète

### 🚀 Animations et UX
- **Animations CSS** fluides et thématiques
- **Effets de particules** en arrière-plan
- **Transitions** douces entre les pages
- **Feedback visuel** sur les interactions
- **Design responsive** pour mobile et desktop
- **États de chargement** avec spinners

### 🏗️ Architecture
- **Services séparés** pour les appels API (ApiService, CharacterService)
- **Composants réutilisables** avec CSS scopé
- **Code behind** pour la logique métier (.razor.cs)
- **Validation côté client** avec DataAnnotations
- **RenderMode InteractiveServer** au niveau parent pour toute l'application
- **Injection de dépendances** complète
- **HttpClient configuré** pour Aspire

## 📁 Structure du Projet

```
Cdm.Web/
├── Components/
│   ├── Layout/           # Layouts principaux (.razor + .razor.cs)
│   ├── Pages/            # Pages de l'application (.razor + .razor.cs)
│   └── Shared/           # Composants partagés (.razor + .razor.cs)
├── Services/             # Services métier (API, Auth, Themes, Characters)
├── Models/               # DTOs et modèles de données
├── wwwroot/
│   ├── css/              # Fichiers de thèmes
│   ├── js/               # Scripts JavaScript
│   └── images/           # Assets graphiques
└── INTEGRATION_DOCUMENTATION.md  # Documentation technique détaillée
```

## 🎯 Pages Implémentées

### Pages Publiques
- **/** - Page d'accueil avec présentation
- **/login** - Connexion utilisateur (connectée à l'API)
- **/register** - Inscription nouveau compte (connectée à l'API)

### Pages Authentifiées
- **/characters** - Gestion des personnages (connectée à l'API)
- **/dice** - Lanceur de dés
- **/campaigns** - Campagnes (lien préparé)
- **/spells** - Sorts (lien préparé)
- **/equipment** - Équipements (lien préparé)
- **/bestiary** - Bestiaire (lien préparé)

## 🔗 Intégration API

### Services Connectés
- **IApiService** - Authentification (login/register)
- **ICharacterService** - Gestion des personnages
- **IJwtService** - Validation des tokens JWT
- **IAuthenticationService** - Gestion de session

### Endpoints Utilisés
```
POST /login          - Connexion utilisateur
POST /register       - Inscription utilisateur
GET  /character      - Liste des personnages
GET  /character/{id} - Détails d'un personnage
POST /character      - Création de personnage
POST /character/dnd  - Création personnage D&D
```

### Configuration HTTP
- **Base URL**: `https+http://apiservice` (Aspire-compatible)
- **Authentification**: Bearer JWT tokens
- **Headers**: JSON + GameType personnalisé
- **Gestion d'erreurs**: Try-catch + logging + fallbacks

## 🛡️ Sécurité

- **Authentification par cookies** sécurisée + JWT
- **Autorisation** sur les pages sensibles
- **Validation** côté client et serveur
- **Protection CSRF** intégrée
- **Headers sécurisés** pour les appels API

## 🎨 Design et UX

### Couleurs et Thèmes
- **Palette cohérente** pour chaque thème
- **Contraste optimisé** pour la lisibilité
- **Dégradés** et effets visuels immersifs

### Typographie
- **Cinzel** pour les titres (police fantasy)
- **Roboto** pour le contenu (lisibilité)
- **Hiérarchie claire** des textes

### Animations
- **Fadeins** pour les apparitions
- **Slides** pour les transitions
- **Glow effects** pour les éléments importants
- **Hover effects** sur les cartes et boutons
- **Loading spinners** pour les états de chargement

## 🔧 Configuration

### Services Configurés
- `IAuthenticationService` - Gestion de l'authentification
- `IApiService` - Appels API d'authentification
- `ICharacterService` - Gestion des personnages via API
- `IJwtService` - Validation des tokens
- `IThemeService` - Gestion des thèmes
- `HttpContextAccessor` - Accès au contexte HTTP

### Middlewares
- Authentification/Authorization
- Antiforgery protection
- Static files serving
- Exception handling

### HttpClient Configuration
- **Aspire-compatible** base URLs
- **Automatic retry** policies
- **Bearer token** injection
- **JSON serialization** optimisée

### RenderMode Configuration
- **InteractiveServer** défini au niveau `Routes.razor`
- **Héritage automatique** pour toutes les pages
- **Optimisation des performances** avec rendermode unifié

## 📱 Responsive Design

- **Mobile First** approach
- **Breakpoints** optimisés pour tablettes/desktop
- **Navigation adaptative** selon la taille d'écran
- **Grids flexibles** pour les listes de contenu

## 🎮 Fonctionnalités JDR

### Lanceur de Dés
- **Formules standard** D&D (xdY+Z)
- **Animation des résultats** avec feedback visuel
- **Détection automatique** des critiques sur d20
- **Historique persistant** de session

### Gestion des Personnages (Connectée API)
- **Récupération dynamique** depuis le backend
- **Fallback intelligent** si API indisponible
- **Support multi-systèmes** (D&D, etc.)
- **CRUD complet** préparé

### Thématique Fantasy
- **Iconographie** appropriée (⚔️🏹🔮🎲)
- **Vocabulaire** immersif (Aventurier, Héros, Quête)
- **Couleurs** évoquant les univers fantastiques

## 🔄 Code Behind et Architecture

### Avantages de la Séparation
- **Logique métier** séparée de la présentation
- **Injection de dépendances** propre
- **Tests unitaires** facilités
- **Maintenance** simplifiée
- **IntelliSense** amélioré

### Pattern Utilisé
```csharp
// Page.razor - Interface utilisateur pure
@page "/example"
<div>@Property</div>

// Page.razor.cs - Logique métier
public partial class Example : ComponentBase
{
    [Inject] private IService Service { get; set; }
    public string Property { get; set; }
}
```

## 🚀 Prochaines Étapes

### Pages à Connecter
- **Campagnes** - Gestion des parties (API prête)
- **Sorts** - Bibliothèque de magie
- **Équipements** - Arsenal et objets
- **Bestiaire** - Créatures et monstres

### Améliorations Techniques
- **Pagination** des listes
- **Cache local** des données
- **Real-time updates** avec SignalR
- **Progressive Web App** features
- **Tests automatisés** complets

---

*Développé avec ❤️ pour la communauté JDR - Maintenant avec une API backend complètement intégrée !*