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

## 📅 Phase 3 : Sessions et Notifications (Prévu) ✨ NOUVEAU

### Système de Sessions
- 📅 **Lancement de sessions** - Transformation créateur en MJ actif
- 📅 **Multi-sources** - Sessions depuis campagnes créées ou rejointes
- 📅 **Invitations pré-session** - Invitation joueurs avant lancement
- 📅 **Notifications multi-canal** - WebSocket + email pour absents

### Progression et Sauvegarde
- 📅 **Progression par chapitres** - Avancement avec sauvegarde automatique
- 📅 **Barre de progression** - Visualisation chapitre actuel vs total
- 📅 **Historique de sessions** - Restauration d'états précédents
- 📅 **Points de sauvegarde** - Sauvegarde aux moments critiques

### Combat Temps Réel
- 📅 **Invitations dynamiques** - Ajout joueurs en cours de combat
- 📅 **Notifications de tour** - Alertes visuelles "À votre tour !"
- 📅 **Interface synchronisée** - État temps réel pour tous participants
- 📅 **Gestion des déconnexions** - Reconnexion avec rattrapage d'état

### Système de Notifications
- 📅 **WebSocket temps réel** - Notifications instantanées pour connectés
- 📅 **Emails automatiques** - Notifications pour joueurs hors ligne
- 📅 **Types d'alertes** - Sessions, tours, invitations, échanges
- 📅 **Préférences utilisateur** - Configuration méthodes de notification

### Authentification Avancée
- 📅 **Reset mot de passe** - Système complet avec emails sécurisés
- 📅 **Tokens temporaires** - Gestion expiration et sécurité
- 📅 **Notifications sécurité** - Alertes connexions et modifications

## 📊 Phase 4 : Statistiques et Succès (Prévu) ✨ NOUVEAU

### Collecte et Analyse de Données
- 📅 **Métriques de sessions** - Fréquence, durée, participation temporelle
- 📅 **Analyse des dés** - Moyennes, chance, distribution, patterns
- 📅 **Performance combat** - Dégâts, précision, efficacité par personnage
- 📅 **Progression personnages** - Évolution niveaux, équipements, expérience

### Système de Succès Gamifié
- 📅 **Framework achievements** - 5 niveaux de rareté, 7 catégories
- 📅 **Déblocage contextuel** - Succès liés aux actions spécifiques
- 📅 **Célébrations visuelles** - Animations, confettis, partage social
- 📅 **Progression visible** - Suivi temps réel vers prochains objectifs

### Analyses Comportementales
- 📅 **Patterns de jeu** - Heures préférées, style, habitudes
- 📅 **Comparaisons sociales** - Classements amis, communauté
- 📅 **Tendances temporelles** - Évolution performance dans le temps
- 📅 **Prédictions IA** - Suggestions personnalisées d'amélioration

### Rapports et Visualisations
- 📅 **Dashboard personnel** - Widget configurables, métriques clés
- 📅 **Rapports automatiques** - Analyses mensuelles/annuelles
- 📅 **Graphiques interactifs** - Évolution, comparaisons, tendances
- 📅 **Export données** - Partage, backup, analyses externes

## 🌟 Phase 5 : Campagnes Avancées (Futur)

### Structure de Campagnes
- 📅 **Système de chapitres** - Organisation narrative par chapitres
- 📅 **PNJ par chapitre** - Création et gestion des personnages non-joueurs
- 📅 **Contextes comportementaux** - Réactions selon l'attitude des joueurs
- 📅 **Liaison narrative** - Référencement PNJ dans les événements

### Gestion Multi-Joueurs
- 📅 **Campagnes publiques** - Découverte et rejointe de campagnes ouvertes
- 📅 **Duplication campagnes** - Clonage pour autres groupes de joueurs
- 📅 **Permissions avancées** - Gestion fine des droits par rôle

### Intelligence Artificielle
- 📅 **Génération de contenu** - IA pour PNJ, lieux, événements
- 📅 **Assistance narration** - Suggestions contextuelles pour MJ
- 📅 **Création automatique** - Monstres et défis équilibrés

## 🔮 Phase 6 : Extensions et Optimisations (Vision)

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