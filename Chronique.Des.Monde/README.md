# 🌃 **CHRONIQUE DES MONDES**
## VTT D&D Moderne avec Interface Sombre

[![.NET 9](https://img.shields.io/badge/.NET-9.0-purple)](https://dotnet.microsoft.com/)
[![Blazor](https://img.shields.io/badge/Blazor-Server-blue)](https://blazor.net/)
[![MudBlazor](https://img.shields.io/badge/MudBlazor-6.11.2-orange)](https://mudblazor.com/)

---

## 📋 **ÉTAT ACTUEL DU PROJET**

### ✅ **COMPLÉTÉ - Interface Moderne**
- **🌃 Thème sombre professionnel** avec MudBlazor
- **📱 Design responsive** mobile/desktop
- **3 pages essentielles** : Accueil, Personnages, Login
- **6 personnages fictifs** avec données D&D complètes
- **Navigation moderne** avec drawer et hamburger menu

### ✅ **COMPLÉTÉ - Backend API**
- **🛡️ Système de combat D&D** complet avec 33 équipements
- **✨ 74 sorts D&D** intégrés avec calculs automatiques
- **📊 API REST** : 12 endpoints combat fonctionnels
- **🗄️ Base de données** SQL Server avec migrations
- **🔐 JWT Authentication** pour sécurité

---

## 🚀 **DÉMARRAGE RAPIDE**

### **Prerequisites**
- .NET 9 SDK
- SQL Server (LocalDB ou instance)
- Visual Studio 2022 ou VS Code

### **Installation**
```bash
# Cloner le repository
git clone https://github.com/Tomtoxi44/ChroniqueDesMondes.git
cd ChroniqueDesMondes/Chronique.Des.Monde

# Restaurer les packages
dotnet restore

# Démarrer l'interface moderne
dotnet run --project Cdm.Web
```

### **Accès Application**
- **Interface Web** : http://localhost:5222
- **Pages disponibles** :
  - `/` - Accueil avec dashboard
  - `/characters` - Gestion des personnages  
  - `/login` - Connexion (accepte tout email/mot de passe)

---

## 🏗️ **ARCHITECTURE TECHNIQUE**

### **📁 Structure des Projets**
```
Chronique.Des.Monde/
├── Cdm.Web/              # 🌃 Interface Blazor moderne
├── Cdm.ApiService/       # 🔌 API REST endpoints
├── Cdm.Business.Common/  # 📋 Logique métier partagée
├── Cdm.Business.Dnd/     # ⚔️ Logique combat D&D
├── Cdm.Data.Common/      # 🗄️ Accès données commun
├── Cdm.Data.Dnd/         # 🎲 Entités D&D
├── Cdm.Migrations/       # 📊 Migrations base de données
└── Cdm.Tests/           # 🧪 Tests unitaires
```

### **🌐 Stack Technique**
- **Frontend** : Blazor Server + MudBlazor 6.11.2
- **Backend** : ASP.NET Core 9 Web API
- **Base de données** : SQL Server + Entity Framework Core
- **Architecture** : Clean Architecture + CQRS
- **Injection** : .NET DI Container
- **Tests** : xUnit + FluentAssertions

---

## 🎨 **INTERFACE MODERNE**

### **🌃 Design System**
- **Thème** : Sombre uniquement (pas de mode clair)
- **Couleurs** : Bleu moderne (#4A90E2), Violet élégant (#7B68EE)  
- **Typography** : Inter (Google Fonts) pour modernité
- **Layout** : MudLayout avec AppBar + Drawer
- **Responsive** : Mobile-first avec breakpoints

### **📱 Pages Implémentées**
1. **Accueil** (`/`) : Dashboard avec stats et activité récente
2. **Personnages** (`/characters`) : Grille de personnages avec stats D&D
3. **Login** (`/login`) : Formulaire de connexion moderne

### **🎯 Données Fictives**
- **6 personnages** complets avec toutes les caractéristiques D&D
- **Stats réalistes** : PV, CA, vitesse, caractéristiques (FOR, DEX, etc.)
- **Races variées** : Nain, Elfe, Humain, Halfelin, Demi-Orc, Tieffelin
- **Classes diverses** : Guerrier, Rôdeuse, Magicien, Clerc, Barbare, Roublard

---

## ⚔️ **SYSTÈME DE COMBAT D&D**

### **✅ Backend Complet**
- **33 équipements D&D** avec stats automatiques
- **74 sorts** avec calculs de dégâts
- **Système d'initiative** et gestion des tours
- **Calculs automatiques** : CA, PV, jets de sauvegarde
- **API REST** prête pour intégration

### **🔧 Endpoints API Disponibles**
```
GET    /api/combat/{id}           # État du combat
POST   /api/combat                # Créer combat
POST   /api/combat/{id}/action    # Exécuter action
GET    /api/equipment             # Liste équipements
GET    /api/spells                # Liste sorts
POST   /api/characters            # Créer personnage
```

---

## 📋 **ROADMAP & TODO**

### **🔥 PRIORITÉ HAUTE**
- [ ] **Connecter frontend aux API** backend existantes
- [ ] **Authentification réelle** avec JWT
- [ ] **CRUD personnages** complet (créer/modifier/supprimer)
- [ ] **Page de création** de personnage avec formulaire

### **⚡ PRIORITÉ MOYENNE**
- [ ] **Interface de combat** temps réel avec SignalR
- [ ] **Gestion des campagnes** avec liste et détails
- [ ] **Système de sorts** avec interface de lancement
- [ ] **Upload d'images** pour avatars des personnages

### **⭐ PRIORITÉ BASSE**
- [ ] **Mode hors ligne** avec synchronisation
- [ ] **Cartes tactiques** pour combats
- [ ] **Système de macros** pour actions répétées
- [ ] **Export PDF** des fiches de personnages

### **🧪 TECHNIQUE**
- [ ] **Tests d'intégration** API + Frontend
- [ ] **Documentation OpenAPI** complète
- [ ] **CI/CD pipeline** avec GitHub Actions
- [ ] **Containerisation Docker** pour déploiement

---

## 🔧 **DÉVELOPPEMENT**

### **🛠️ Commands Utiles**
```bash
# Lancer l'interface web seulement
dotnet run --project Cdm.Web

# Lancer l'API backend (si nécessaire)
dotnet run --project Cdm.ApiService

# Exécuter les tests
dotnet test

# Créer une migration
dotnet ef migrations add <NomMigration> --project Cdm.Migrations

# Appliquer les migrations
dotnet ef database update --project Cdm.Migrations
```

### **🎯 Branches Git**
- **`main`** : Version stable de production
- **`feature/frontend-modernization-phase1`** : Interface moderne (ACTUELLE)
- **`develop`** : Intégration continue des nouvelles fonctionnalités

---

## 📊 **MÉTRIQUES**

### **📈 Progression Actuelle**
- **Interface Frontend** : ✅ 100% (Design moderne terminé)
- **Backend API** : ✅ 85% (Combat system complet)
- **Authentification** : ⚡ 60% (JWT backend prêt, frontend à connecter)
- **Base de données** : ✅ 90% (Migrations + Seeding automatique)
- **Tests** : ⚠️ 30% (Tests backend, frontend à tester)

### **📝 Statistiques Techniques**
- **27 fichiers** modifiés dans la refonte frontend
- **1122 lignes** ajoutées, 2266 lignes supprimées
- **2 thèmes** supprimés (fantasy), 1 thème moderne ajouté
- **3 pages** essentielles créées
- **6 personnages** fictifs intégrés

---

## 🤝 **CONTRIBUTION**

### **📋 Guidelines**
1. **Fork** le repository
2. **Créer une branche** feature/nom-fonctionnalite
3. **Développer** avec tests si applicable
4. **Tester** l'interface et l'API
5. **Pull Request** vers develop

### **🎨 Standards UI**
- **Respecter** le thème sombre moderne
- **Utiliser** MudBlazor pour cohérence
- **Tests** responsive mobile/desktop
- **Pas d'éléments** enfantins ou fantastiques

---

## 📞 **SUPPORT**

### **🐛 Bug Reports**
Utiliser les [GitHub Issues](https://github.com/Tomtoxi44/ChroniqueDesMondes/issues) avec labels appropriés

### **💡 Suggestions**
Les demandes de fonctionnalités sont bienvenues via Issues avec label `enhancement`

---

## 📄 **LICENCE**

Ce projet est sous licence MIT. Voir [LICENSE](LICENSE) pour détails.

---

## 🎊 **REMERCIEMENTS**

- **MudBlazor** pour le framework UI moderne
- **D&D 5e SRD** pour les règles et données
- **Microsoft** pour .NET 9 et Blazor
- **Communauté open source** pour l'inspiration

---

*Interface moderne terminée ✅ | Backend combat prêt ✅ | Prochaine étape : Connexion API-Frontend* 🚀