# Roadmap Priorisée - Chronique des Mondes

Cette roadmap redéfinie présente l'évolution du projet avec une priorité logique basée sur l'analyse de l'état actuel et des dépendances techniques.

## 📊 **Analyse de l'État Actuel**

### ✅ **Fondations Terminées (Phase 1)**
- Authentification JWT avec modèle `User`
- Personnages D&D avec `CharacterDnd` héritant de `ACharacter`
- Architecture modulaire avec projets séparés (Common, Dnd, Business, Data)
- Entity Framework avec contextes séparés (`AppDbContext`, `DndDbContext`) 
- API REST avec Minimal APIs .NET 9
- Tests automatisés et DataSeeder opérationnel
- Interface Blazor avec page de statistiques

### 🔍 **État Technique Détecté**
- **Base de données** : Uniquement `Users` et `CharacterDnd` implémentés
- **Contextes EF** : Structure prête mais tables Spells/Equipment non créées
- **Migrations** : Infrastructure en place via `Cdm.Migrations`
- **Business Logic** : Séparation Common/Dnd fonctionnelle
- **Documentation** : Très complète avec spécifications détaillées

## 🎯 **Priorité Logique Recommandée**

### **PRIORITÉ 1 : Base de Données et Modèles (IMMÉDIAT)**
*Fondations techniques obligatoires*

**Justification** : Impossible d'implémenter les fonctionnalités sans les modèles de données correspondants.

**Actions :**
1. **Création des modèles Entity Framework**
   - `Spell`, `Equipment`, `Campaign`, `Chapter`, `NPC`
   - Tables de liaison : `CharacterSpells`, `CharacterEquipment`
   - Relations et contraintes de validation

2. **Migrations de base de données**
   - Migration 2 : `CreateSpellsAndEquipmentTables`
   - Migration 3 : `CreateCampaignSystem`
   - Migration 4 : `CreateCharacterRelations`

3. **Configuration des DbContext**
   - Mise à jour `AppDbContext` et `DndDbContext`
   - Configuration des relations et index

**Durée estimée** : 1-2 semaines

---

### **PRIORITÉ 2 : Système Sorts et Équipements (COURT TERME)**
*Valeur métier immédiate*

**Justification** : Fonctionnalités core du JDR, permet de tester l'architecture bi-niveau, fondement pour les échanges.

**Phase 2A : Sorts (2-3 semaines)**
- Implémentation des services `SpellService` et `SpellBusiness`
- Endpoints CRUD pour sorts avec validation GameType
- Injection des sorts D&D officiels via scripts SQL
- Système d'apprentissage personnage-sort
- Interface Blazor pour consultation/création de sorts

**Phase 2B : Équipements (2-3 semaines)**
- Services `EquipmentService` et `EquipmentBusiness`
- Système d'inventaire avec quantités multiples
- Injection équipements D&D officiels
- Interface de gestion d'inventaire

**Phase 2C : Échanges d'Équipements (1-2 semaines)**
- Tables `EquipmentOffers` et `EquipmentTrades`
- Services d'échange MJ→Joueur et Joueur→Joueur
- Validation sécurisée des transactions
- Interface d'échange intuitive

**Durée totale** : 5-8 semaines

---

### **PRIORITÉ 3 : Système de Campagnes (MOYEN TERME)**
*Structure de jeu essentielle*

**Justification** : Nécessaire pour tester les sorts/équipements en contexte réel, prépare les sessions.

**Phase 3A : Structure de Campagnes (3-4 semaines)**
- Modèles `Campaign`, `Chapter`, `NPC`, `CampaignPlayers`
- Services de création et gestion de campagnes
- Système de chapitres avec contenu narratif
- Gestion des PNJ par chapitre avec comportements

**Phase 3B : Système de Combat (2-3 semaines)**
- Tables `Combats` et `CombatParticipants`
- Logique d'initiative et tours de jeu
- Calculs automatiques D&D (CA, dégâts, modificateurs)
- Interface MJ pour lancement de combats

**Durée totale** : 5-7 semaines

---

### **PRIORITÉ 4 : Sessions et Notifications (LONG TERME)**
*Fonctionnalités avancées*

**Justification** : Nécessite toutes les fondations précédentes, apporte l'expérience temps réel.

**Phase 4A : Infrastructure Sessions (3-4 semaines)**
- Tables `Sessions`, `SessionParticipants`, `SessionSaves`
- Services de lancement et gestion de sessions
- Système de sauvegarde automatique
- Progression par chapitres avec persistence

**Phase 4B : Notifications Temps Réel (2-3 semaines)**
- Tables `Notifications`, `CampaignInvitations`
- Intégration WebSocket/SignalR
- Système d'emails automatiques
- Invitations pré-session et dynamiques

**Phase 4C : Combat Temps Réel (2-3 semaines)**
- Invitations dynamiques en cours de combat
- Notifications "À votre tour" avec interface visuelle
- Synchronisation état de combat entre participants
- Gestion des déconnexions/reconnexions

**Durée totale** : 7-10 semaines

---

### **PRIORITÉ 5 : Statistiques et Succès (TRÈS LONG TERME)**
*Gamification et engagement*

**Justification** : Fonctionnalités d'engagement, nécessite une base de données riche d'événements.

**Phase 5A : Collecte de Données (2-3 semaines)**
- Tables `PlayerStatistics`, `DiceRolls`, `CombatActions`
- Services de collecte automatique d'événements
- Intégration dans tous les systèmes existants

**Phase 5B : Système de Succès (3-4 semaines)**
- Tables `Achievements`, `PlayerAchievements`
- Moteur de déblocage de succès
- 55+ succès prédéfinis avec célébrations visuelles
- Interface de progression et classements

**Phase 5C : Analyses Avancées (3-4 semaines)**
- Tables `SessionActivities`, `PlayerReports`
- Rapports automatiques mensuels/annuels
- Analyses prédictives et recommandations IA
- Dashboard personnalisé avec widgets configurables

**Durée totale** : 8-11 semaines

---

## 📋 **Plan d'Exécution Détaillé**

### **Sprint 1-2 : Fondations DB (2 semaines)**
**Objectif** : Préparer toute l'infrastructure de données

**Livrables :**
- [ ] Modèles EF pour Spells, Equipment, Campaign, Chapter, NPC
- [ ] Migrations 2-4 créées et testées
- [ ] DbContext configurés avec relations
- [ ] Seed data pour sorts/équipements D&D officiels
- [ ] Tests d'intégration base de données

### **Sprint 3-6 : Sorts (4 semaines)**
**Objectif** : Système de sorts complet et opérationnel

**Livrables :**
- [ ] Services et business logic pour sorts
- [ ] Endpoints CRUD avec validation GameType
- [ ] Système d'apprentissage personnage-sort
- [ ] Interface Blazor sorts (consultation, création, apprentissage)
- [ ] Tests HTTP complets pour sorts

### **Sprint 7-10 : Équipements (4 semaines)**
**Objectif** : Inventaires et échanges fonctionnels

**Livrables :**
- [ ] Services équipements avec quantités multiples
- [ ] Système d'échanges MJ→Joueur et Joueur→Joueur
- [ ] Interface inventaire et échanges
- [ ] Validation sécurisée des transactions
- [ ] Tests complets échanges d'équipements

### **Sprint 11-16 : Campagnes et Combat (6 semaines)**
**Objectif** : Structure de jeu et combat utilisables

**Livrables :**
- [ ] Système complet de campagnes et chapitres
- [ ] Gestion des PNJ avec comportements
- [ ] Combat avec initiative et calculs D&D
- [ ] Interface MJ pour gestion de campagnes
- [ ] Tests scénarios complets de jeu

### **Sprint 17-26 : Sessions Temps Réel (10 semaines)**
**Objectif** : Expérience de jeu synchronisée

**Livrables :**
- [ ] Lancement et gestion de sessions
- [ ] Notifications WebSocket + email
- [ ] Combat temps réel avec invitations dynamiques
- [ ] Sauvegarde automatique et progression
- [ ] Interface complète sessions temps réel

### **Sprint 27-37 : Statistiques et Succès (11 semaines)**
**Objectif** : Gamification et engagement à long terme

**Livrables :**
- [ ] Collecte automatique de toutes les métriques
- [ ] 55+ succès avec déblocage contextuel
- [ ] Rapports personnalisés et analyses
- [ ] Dashboard statistiques interactif
- [ ] Système de prédictions et recommandations

## ⚡ **Critères de Priorisation Utilisés**

### **1. Dépendances Techniques**
- Les modèles de données sont prérequis à tout développement
- Les sorts/équipements sont nécessaires pour tester les campagnes
- Les campagnes doivent exister avant les sessions
- Les statistiques nécessitent des données d'événements

### **2. Valeur Métier**
- Sorts et équipements = cœur du JDR, valeur immédiate
- Campagnes = structure de jeu, fonctionnalité majeure
- Sessions = différenciation concurrentielle importante
- Statistiques = engagement long terme, nice-to-have

### **3. Complexité d'Implémentation**
- Base de données = complexe mais court
- CRUD sorts/équipements = moyennement complexe
- Sessions temps réel = très complexe techniquement
- Statistiques = complexe analytiquement

### **4. Risques et Testing**
- Commencer par les fondations permet de tester l'architecture
- Builds incrémentaux réduisent les risques d'intégration
- Chaque phase peut être testée indépendamment

## 🎯 **Jalons de Validation**

### **Milestone 1 (Semaine 2)** : Infrastructure DB
- Toutes les tables créées et relationnées
- Seed data injecté avec succès
- Tests d'intégration passent

### **Milestone 2 (Semaine 10)** : Système Sorts/Équipements
- CRUD complet avec échanges fonctionnels
- Interface utilisateur opérationnelle
- Architecture bi-niveau validée

### **Milestone 3 (Semaine 16)** : Campagnes et Combat
- Campagne complète créable et jouable
- Combat D&D avec calculs automatiques
- Interface MJ fonctionnelle

### **Milestone 4 (Semaine 26)** : Sessions Temps Réel
- Sessions multi-joueurs synchronisées
- Notifications en temps réel opérationnelles
- Combat collaboratif fonctionnel

### **Milestone 5 (Semaine 37)** : Produit Complet
- Système de statistiques et succès intégré
- Expérience utilisateur complète et polished
- Prêt pour déploiement production

---

**Cette roadmap priorisée permet un développement incrémental avec validation continue, minimisant les risques tout en maximisant la valeur livrée à chaque étape.** 🚀