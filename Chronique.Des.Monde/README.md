# 🏰 Chronique des Mondes

**Application de gestion de campagnes de jeux de rôle multi-système**

## 📖 Vue d'ensemble

Chronique des Mondes est une plateforme complète dédiée à la création, gestion et animation de campagnes de jeux de rôle. L'application permet aux Maîtres du Jeu (MJ) et aux joueurs de collaborer dans un environnement numérique riche et interactif.

## 🎯 Objectif Final

### Pour les Maîtres du Jeu
- **Création de campagnes** avec système d'invitations pour les joueurs
- **Gestion des chapitres** et contenus narratifs structurés
- **Bibliothèque de sorts** (officiels et personnalisés) avec calculs automatiques
- **Gestion des PNJ** avec outils de combat et statistiques
- **Système d'échange d'équipements** entre MJ et joueurs
- **Outils de narration** avec blocs de contenu modulaires

### Pour les Joueurs
- **Création de personnages** avec support multi-système (D&D 5e, générique, extensible)
- **Gestion de sorts** personnalisés et accès aux sorts officiels
- **Inventaire d'équipements** avec possibilité d'échange
- **Participation aux campagnes** sur invitation
- **Suivi de progression** et historique des aventures

## 🎮 Systèmes de Jeu Supportés

### Actuellement
- **D&D 5e** : Support complet avec calculs automatiques des bonus, DD, dégâts
- **Générique** : Système flexible pour tous types de JDR

### Prévus
- **Skyrim** : Système Elder Scrolls adapté au jeu de rôle
- **Extensibilité** : Architecture modulaire pour ajouter d'autres systèmes

## 🛠️ Fonctionnalités Principales

### 🏛️ Gestion des Campagnes
- Création et modification de campagnes
- Système d'invitations par email avec notifications automatiques
- Permissions granulaires (MJ, joueurs, spectateurs)
- Campagnes publiques et privées

### 📚 Système de Chapitres
- Organisation narrative en chapitres ordonnés
- Blocs de contenu modulaires (narration, dialogue, action)
- Gestion des PNJ par chapitre
- Notes privées pour le MJ

### ⚔️ Gestion des Personnages
- Création multi-système avec factory pattern
- Calculs automatiques des statistiques (D&D)
- Support PNJ/Joueurs avec gestion des hostiles
- Tags et organisation flexible

### 🪄 Système de Sorts
- **Sorts officiels** : Base de données pré-remplie par système
- **Sorts personnalisés** : Création par les utilisateurs (privés)
- **Calculs D&D automatiques** : Bonus d'attaque, DD de sauvegarde, dégâts
- **Recherche avancée** : Par école, niveau, nom, tags
- **Pas d'échange** : Les sorts restent privés à leur créateur

### 🎒 Système d'Équipements
- **Équipements officiels** et **équipements personnalisés**
- **Échanges MJ → Joueur** : Le MJ peut proposer ses équipements
- **Échanges Joueur ↔ Joueur** : Entre membres de la même campagne
- **Calculs automatiques** : Bonus, CA, dégâts (D&D)

## 🏗️ Architecture Technique

### Backend (.NET 9)
- **API REST** avec authentification JWT
- **Architecture modulaire** : Business, Data, Services séparés
- **Multi-bases** : AppDbContext (générique) + DndDbContext (spécialisé)
- **Services métier** avec validation des permissions
- **Injection par clé** pour la spécialisation par système de jeu

### Frontend (Blazor)
- **Interface moderne** et responsive
- **Composants réutilisables** par système de jeu
- **Temps réel** pour les interactions en campagne

### Services Externes
- **Azure Communication Services** : Envoi d'emails transactionnels
- **Authentication** : JWT avec gestion des rôles
- **Base de données** : SQL Server avec Entity Framework Core

## 🚀 Roadmap

### Phase 1 ✅ (Terminée)
- Système d'authentification et utilisateurs
- Gestion des campagnes et invitations
- Système de personnages D&D
- Emails transactionnels

### Phase 2 ✅ (Terminée)
- Système de sorts complet (officiels + personnalisés)
- Chapitres et blocs de contenu
- Gestion des PNJ

### Phase 3 🔄 (En cours)
- Système d'équipements et échanges
- Interface Blazor complète
- Tests et déploiement

### Phase 4 📋 (Prévue)
- Système de combat automatisé
- Support Skyrim
- Outils de campagne avancés (cartes, calendrier)
- Mode temps réel pour les sessions

## 🎲 Vision

Devenir **la référence** pour la gestion de campagnes JDR numériques, alliant :
- **Simplicité d'usage** pour les débutants
- **Puissance et flexibilité** pour les MJ expérimentés  
- **Collaboration fluide** entre tous les participants
- **Support multi-système** pour s'adapter à tous les univers
- **Automatisation intelligente** des calculs et règles

---

**Chronique des Mondes - Là où naissent les légendes** ⚔️✨