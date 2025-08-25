# Chronique des Mondes - Backend API

## 🎯 Vue d'ensemble

**Chronique des Mondes** est une plateforme JDR où un utilisateur peut être **joueur** ou **maître du jeu (MJ)** - et même les deux à la fois dans différentes campagnes !

### Principe Architectural
Le socle est **générique**, puis des logiques métiers spécifiques à chaque jeu (ex : D&D) viennent compléter les fonctionnalités de base.

- **Création de personnage** : Générique par défaut, spécialisé selon le jeu choisi
- **Routage métier** : Headers `X-GameType` pour diriger vers la logique appropriée
- **Gestion de campagnes** : Structure par chapitres avec PNJ et combats
- **Système de sorts et équipements** : Architecture bi-niveau (officiels + privés)

## 🏗️ Stack Technique

- **.NET 9** - Framework principal
- **ASP.NET Core Minimal APIs** - Endpoints REST
- **Entity Framework Core** - ORM pour base de données
- **JWT Authentication** - Sécurité et authentification
- **Aspire** - Orchestration et configuration

## 🧑‍🤝‍🧑 Rôles et Permissions

### Utilisateurs Multi-Rôles
- **Un utilisateur peut être MJ d'une campagne ET joueur dans une autre**
- Chaque campagne a un seul MJ (créateur)
- Un utilisateur peut participer à plusieurs campagnes

### Gestion des Campagnes
- Création et invitation de joueurs
- Campagnes publiques ou privées
- Duplication possible entre MJ

## 🏰 Structure des Campagnes

### Tags de Système de Jeu
- **Générique** : Pas de tag, gestion manuelle complète
- **Spécialisé** : Tag D&D, Skyrim, etc. pour débloquer les fonctionnalités automatiques
- **Évolutif** : Ajout de tag possible ultérieurement

### Organisation par Chapitres
- **Navigation** séquentielle entre chapitres
- **Contenu narratif** avec blocs de texte
- **PNJ et Monstres** par chapitre
- **Gestion comportementale** : 🟢 Amical | 🟡 Neutre | 🔴 Hostile

## 🧙‍♂️ Système de Personnages

### Logique Générique vs Spécialisée
- **Générique** : Nom, points de vie, champs personnalisés
- **D&D** : Stats complètes, classes, races, compétences
- **Compatibilité** : Un personnage D&D ne peut rejoindre qu'une campagne D&D
- **Duplication** : Changement de système possible

### Routage API
```http
POST /character?userId={id} HTTP/1.1
X-GameType: dnd  # ou generic, skyrim, etc.
Authorization: Bearer {token}
```

## 🪄 Système de Sorts

### Architecture Bi-Niveau

#### **Sorts Officiels 🌟**
- **Injection administrative** par scripts SQL
- **Publics pour tous** les utilisateurs
- **Exemples** : D&D (disponible), Skyrim (à venir)

#### **Sorts Privés 👤**  
- **Créés par les utilisateurs** individuellement
- **Privés uniquement** à leur créateur
- **Pas d'échange possible** entre utilisateurs

### Types de Sorts
- **Génériques** : Titre, description (gestion manuelle)
- **Spécialisés D&D** : Calculs automatiques des modificateurs
- **Compatibilité** : Validation stricte par gameType

### Calculs D&D Automatiques
```csharp
Magicien    → Intelligence + Bonus Maîtrise
Clerc       → Sagesse + Bonus Maîtrise  
Paladin     → Charisme + Bonus Maîtrise
```

## ⚔️ Système d'Équipements

### Architecture Bi-Niveau avec Échanges

#### **Équipements Officiels 🌟**
- **Injection administrative** par scripts SQL
- **Publics pour tous**

#### **Équipements Privés 👤**
- **Créés par les utilisateurs**
- **Multi-instances** possibles

### **Système d'Échange 🔄**

#### **MJ → Joueur**
- **Proposition** d'équipements aux joueurs de sa campagne
- **Copie** : L'équipement reste chez le MJ après acceptation
- **Re-proposable** à d'autres joueurs

#### **Joueur → Joueur**
- **Échange direct** entre joueurs de la même campagne
- **Transfert** : L'équipement change de propriétaire
- **Validation** : Quantités, compatibilité, permissions

## ⚔️ Gestion des Combats ✨ AMÉLIORÉ

### Interface MJ de Combat
- **Vue par chapitre** avec sélection PNJ/Monstres
- **Déclenchement combat** avec participants choisis
- **Calculs automatiques** pour D&D (CA, dégâts, modificateurs)
- **Gestion manuelle** pour systèmes non supportés

### **Combat en Temps Réel** 🆕
- **Invitations dynamiques** : Ajout de joueurs en cours de combat
- **Jet d'initiative automatique** : Intégration immédiate dans l'ordre des tours
- **Notifications de tour** : Alertes visuelles et sonores pour le joueur actif
- **Interface visuelle** : Cadres colorés et animations pour indiquer les tours
- **Pop-ups de notification** : Alertes discrètes "À votre tour !"

### Gestion des Tours
- **Ordre d'initiative** : Calcul et affichage automatique
- **Timer optionnel** : Limite de temps par tour
- **Actions contextuelles** : Suggestions selon la situation
- **État en temps réel** : Synchronisation pour tous les participants

## 📡 Endpoints Principaux

### Authentification
- `POST /login` - Connexion
- `POST /register` - Inscription

### Personnages
- `GET /character?userId={id}` - Liste personnages
- `POST /character?userId={id}` - Création (+ header X-GameType)
- `PUT /character/{id}` - Modification
- `DELETE /character/{id}` - Suppression

### Sorts
- `GET /spells?gameType={type}&userId={id}` - Sorts disponibles
- `GET /spells/official?gameType={type}` - Sorts officiels uniquement
- `POST /spell?userId={id}` - Création sort privé
- `POST /character/{id}/spells/{spellId}` - Apprendre sort

### Équipements
- `GET /equipment?gameType={type}&userId={id}` - Équipements disponibles
- `POST /equipment?userId={id}` - Création équipement privé
- `GET /character/{id}/inventory` - Inventaire personnage

### Échanges d'Équipements
- `POST /campaign/{id}/equipment/offer` - MJ propose équipement
- `POST /campaign/{id}/equipment/trade` - Échange joueur→joueur
- `GET /campaign/{id}/equipment/offers?playerId={id}` - Propositions en attente

### Sessions et Combats ✨ NOUVEAU
- `POST /campaign/{id}/session/start` - Lancement de session
- `POST /session/{id}/combat/{combatId}/invite-player` - Inviter joueur en combat
- `PUT /session/{id}/combat/{combatId}/join` - Rejoindre combat en cours
- `GET /session/{id}/save-status` - État des sauvegardes
- `PUT /session/{id}/chapter/complete` - Compléter un chapitre

### Invitations et Notifications
- `POST /campaign/{id}/invite` - Inviter joueurs à campagne
- `PUT /invitation/{id}/respond` - Répondre à invitation
- `GET /user/{id}/campaigns/available` - Campagnes disponibles pour session
- `POST /auth/password/reset-request` - Demande reset mot de passe
- `POST /auth/password/reset-confirm` - Confirmation nouveau mot de passe

### Statistiques et Succès ✨ NOUVEAU
- `GET /user/{id}/stats/sessions/frequency` - Métriques de participation
- `GET /user/{id}/stats/dice/performance` - Analyse jets de dés et chance
- `GET /user/{id}/stats/combat/overview` - Performance de combat globale
- `GET /character/{id}/stats/evolution` - Évolution temporelle personnage
- `GET /user/{id}/achievements/{category}` - Succès par catégorie
- `POST /user/{id}/achievements/check` - Vérification déblocage succès
- `GET /user/{id}/stats/reports/monthly` - Rapport mensuel personnalisé

## 🎮 Système de Sessions ✨ NOUVEAU

### Lancement et Gestion des Sessions
- **Sessions multi-sources** : Lancement depuis campagnes créées ou publiques rejointes
- **Transformation en MJ** : Le lanceur devient automatiquement MJ de la session
- **Invitations pré-session** : Invitation de joueurs avant le lancement
- **Notifications multi-canal** : WebSocket temps réel + email pour absents

### Progression et Sauvegarde
- **Progression par chapitres** : Avancement automatique avec sauvegarde
- **Barre de progression** : Visualisation chapitre actuel vs total
- **Sauvegarde automatique** : Intervalles configurables et points critiques
- **Historique de session** : Restauration d'états précédents

### Sessions en Temps Réel
- **État synchronisé** : Tous les participants voient le même état
- **Notifications push** : Alertes pour événements importants
- **Gestion des déconnexions** : Reconnexion automatique avec rattrapage

## 📊 Système de Statistiques et Succès ✨ NOUVEAU

### Analyse de Performance
- **Métriques de sessions** : Fréquence, durée, participation mensuelle/annuelle
- **Statistiques de dés** : Moyennes, chance, distribution des résultats D20
- **Performance combat** : Dégâts, précision, survivabilité par personnage
- **Progression personnages** : Évolution niveaux, équipements, expérience

### Analyse Comportementale
- **Patterns de jeu** : Heures préférées, style de jeu, préférences
- **Comparaisons sociales** : Classements entre amis, communauté
- **Tendances temporelles** : Évolution performance sur le temps
- **Rapports personnalisés** : Analyses mensuelles/annuelles détaillées

### Système de Succès/Achievements
- **5 niveaux de rareté** : Commun → Légendaire avec célébrations
- **7 catégories** : Combat, Exploration, Social, Maîtrise, Collection, Chance, Progression
- **Déblocage contextuel** : Succès liés aux actions spécifiques
- **Progression visible** : Suivi temps réel vers prochains succès

### Collecte Automatique
- **Jets de dés** : Tous types, contextes, résultats avec historique
- **Actions combat** : Attaques, sorts, dégâts, cibles détaillées  
- **Activités session** : Quêtes, trésors, niveaux, interactions sociales
- **Métadonnées** : Sessions, durées, participants, campagnes
 
## 🔒 Sécurité

### Authentification & Autorisation
- **JWT Tokens** pour toutes les requêtes
- **Contrôle d'accès** par utilisateur/MJ
- **Validation gameType** pour compatibilité

### Restrictions
- **Sorts officiels** : Non modifiables par utilisateurs
- **Sorts privés** : Aucun partage/échange possible
- **Équipements** : Échanges uniquement dans même campagne
- **Permissions MJ** : Seul le MJ peut modifier sa campagne

## 📖 Documentation Détaillée

### Documents Techniques
- **[Architecture Technique](./TechnicalArchitecture.md)** - Structure du projet, modèles de données, configuration
- **[Schéma de Base de Données](./DatabaseSchema.md)** - Schéma complet avec état actuel et évolutions prévues
- **[Spécifications Sorts et Équipements](./SpellsAndEquipment.md)** - Architecture bi-niveau détaillée
- **[Sessions et Notifications](./SessionsAndNotifications.md)** - Système complet de sessions temps réel ✨ NOUVEAU
- **[Statistiques et Succès](./StatisticsAndAchievements.md)** - Métriques, analyses et achievements ✨ NOUVEAU

### Documents Fonctionnels
- **[Cas d'usage Sorts et Équipements](./SpellsEquipmentUseCases.md)** - Exemples concrets officiels vs privés
- **[Cas d'usage Sessions et Combat](./SessionsUseCases.md)** - Scénarios sessions temps réel ✨ NOUVEAU
- **[Cas d'usage Statistiques](./StatisticsUseCases.md)** - Analyses et prédictions IA ✨ NOUVEAU
- **[Cas d'utilisation généraux](./UseCases.md)** - Scénarios complets campagnes et combats
- **[Roadmap](./Roadmap.md)** - Planification par phases avec métriques

### Tests et Validation
- **[Guide des Tests](../Tests/README.md)** - Documentation complète des tests API
- Structure de tests par domaine (Generic, Dnd, Spells, Equipment, Security, Scenarios)

---

**Une API robuste et extensible pour la communauté JDR !** 🎲✨

*Pour plus de détails techniques, consultez la documentation spécialisée ci-dessus*