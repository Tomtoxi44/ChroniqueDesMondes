# Roadmap - Chronique des Mondes

Cette roadmap présente l'évolution prévue du projet par phases de développement.

## 🎯 Phase 1 : Fondations (Actuel - Terminé ✅)

### Authentification et Base
- ✅ **Authentification JWT** - Système de connexion/inscription sécurisé
- ✅ **Personnages génériques** - Création, modification, suppression
- ✅ **Personnages D&D** - Extension avec stats complètes D&D
- ✅ **Architecture modulaire** - Séparation des logiques métier par jeu
- ✅ **API REST** - Endpoints avec validation et gestion d'erreurs
- ✅ **Tests automatisés** - Suite de tests HTTP pour validation

### Infrastructure
- ✅ **Entity Framework Core** - ORM avec migrations automatisées
- ✅ **Minimal APIs .NET 9** - Endpoints performants et modernes
- ✅ **Aspire** - Orchestration et configuration des services
- ✅ **Blazor Server** - Interface utilisateur moderne

## 🚀 Phase 2 : Sorts et Équipements (En cours 🔄)

### Système de Sorts
- 🔄 **Architecture bi-niveau** - Sorts officiels vs privés utilisateurs
- 🔄 **Injection administrative** - Scripts SQL pour sorts officiels D&D
- 🔄 **Calculs automatiques D&D** - Modificateurs selon les classes
- 🔄 **Apprentissage de sorts** - Système de liaison personnage-sort
- 🔄 **Validation compatibilité** - Sorts D&D uniquement pour personnages D&D

### Système d'Équipements
- 🔄 **Inventaires multi-instances** - Quantités d'objets par personnage
- 🔄 **Équipements officiels** - Base de données d'objets D&D standard
- 🔄 **Création personnalisée** - Équipements privés utilisateurs
- 🔄 **Calculs automatiques** - CA, bonus d'attaque, modificateurs

### Système d'Échanges
- 🔄 **Propositions MJ → Joueur** - Distribution d'équipements en campagne
- 🔄 **Échanges Joueur → Joueur** - Trade entre personnages
- 🔄 **Validation sécurisée** - Vérification quantités et permissions
- 🔄 **Interface d'échange** - UX intuitive pour proposer/accepter

### Interfaces Utilisateur
- 🔄 **Pages Sorts** - Consultation, création, apprentissage
- 🔄 **Pages Équipements** - Inventaire, échanges, gestion
- 🔄 **Interface MJ** - Distribution d'équipements aux joueurs

## 📅 Phase 3 : Campagnes Avancées (Prévu)

### Structure de Campagnes
- 📅 **Système de chapitres** - Organisation narrative par chapitres
- 📅 **PNJ par chapitre** - Création et gestion des personnages non-joueurs
- 📅 **Contextes comportementaux** - Réactions selon l'attitude des joueurs
- 📅 **Liaison narrative** - Référencement PNJ dans les événements

### Gestion Multi-Joueurs
- 📅 **Invitations campagne** - Système d'invitation par email/notification
- 📅 **Campagnes publiques** - Découverte et rejointe de campagnes ouvertes
- 📅 **Duplication campagnes** - Clonage pour autres groupes de joueurs
- 📅 **Permissions avancées** - Gestion fine des droits par rôle

### Système de Combat
- 📅 **Initiative automatique** - Gestion des tours de combat
- 📅 **Calculs D&D** - Attaques, dégâts, CA automatiques
- 📅 **Interface temps réel** - État du combat partagé en direct
- 📅 **Historique combat** - Logs détaillés des actions

## 🌟 Phase 4 : Extensions et Optimisations (Futur)

### Nouveaux Systèmes de Jeu
- 📅 **Skyrim** - Sorts, objets et règles spécifiques
- 📅 **Pathfinder** - Extension du système D&D
- 📅 **Warhammer** - Nouveau système complet
- 📅 **Système générique étendu** - Framework pour ajouts communautaires

### Fonctionnalités Avancées
- 📅 **Intelligence Artificielle** - Assistance création PNJ, événements, lieux
- 📅 **Chat temps réel** - Communication entre joueurs avec SignalR
- 📅 **Notifications push** - Alertes échanges, invitations, tours de combat
- 📅 **Système de sauvegarde** - Snapshots d'état de campagne

### Performance et Scalabilité
- 📅 **Cache Redis** - Optimisation des requêtes fréquentes
- 📅 **Rate Limiting** - Protection contre les abus
- 📅 **Monitoring avancé** - Métriques et logs centralisés
- 📅 **API GraphQL** - Alternative pour requêtes complexes

## 🔮 Phase 5 : Écosystème et Communauté (Vision)

### Marketplace et Partage
- 📅 **Bibliothèque communautaire** - Partage de campagnes entre MJ
- 📅 **Système de notation** - Reviews des campagnes et contenus
- 📅 **Outils d'import/export** - Compatibilité avec d'autres outils JDR
- 📅 **API publique** - Intégration avec outils tiers

### Outils de Création
- 📅 **Éditeur visuel de cartes** - Création de plans et donjons
- 📅 **Générateur de PNJ/Monstres** - Création assistée par IA
- 📅 **Templates de campagnes** - Modèles prêts à l'emploi
- 📅 **Système de mods** - Extensions développées par la communauté

### Monétisation et Business
- 📅 **Contenu premium** - Campagnes officielles payantes
- 📅 **Abonnements MJ** - Fonctionnalités avancées pour les maîtres du jeu
- 📅 **Partenariats éditeurs** - Intégration contenu officiel D&D, Pathfinder
- 📅 **API commerciale** - Licensing pour développeurs tiers

## 📈 Métriques de Succès

### Phase 2 (Sorts et Équipements)
- **Objectif** : 100% des fonctionnalités de base implémentées
- **Métriques** :
  - Sorts officiels D&D injectés : 50+ sorts
  - Équipements officiels : 100+ objets
  - Taux d'utilisation des échanges : 70% des campagnes
  - Performance API : <200ms pour 95% des requêtes

### Phase 3 (Campagnes Avancées)
- **Objectif** : Expérience de jeu complète et fluide
- **Métriques** :
  - Campagnes créées par mois : 1000+
  - Joueurs actifs : 5000+
  - Sessions de combat par semaine : 500+
  - Satisfaction utilisateur : 4.5/5

### Phases Futures
- **Utilisateurs actifs mensuels** : 50,000+ (Phase 4)
- **Systèmes de jeu supportés** : 10+ (Phase 4)
- **Revenus mensuels récurrents** : Confidentiel (Phase 5)
- **Partenariats éditeurs** : 3+ (Phase 5)

## 🛠️ Ressources et Équipe

### Compétences Requises par Phase

#### Phase 2 (Actuelle)
- **Backend .NET** - Développement API et logique métier
- **Frontend Blazor** - Interfaces utilisateur modernes
- **Base de données** - Conception schémas et optimisations
- **Tests** - Validation automatisée et qualité

#### Phase 3
- **SignalR** - Communication temps réel
- **DevOps** - Déploiement et monitoring
- **UX/UI Design** - Expérience utilisateur avancée

#### Phases Futures
- **Intelligence Artificielle** - Intégration IA pour génération de contenu
- **Mobile** - Applications natives iOS/Android
- **Business Development** - Partenariats et monétisation

---

*Cette roadmap est évolutive et s'adapte selon les retours utilisateurs et les priorités du marché.*

*Retour au [README principal](./README.md)*