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

## ⚔️ Gestion des Combats

### Interface MJ
- **Vue par chapitre** avec sélection PNJ/Monstres
- **Déclenchement combat** avec participants choisis
- **Calculs automatiques** pour D&D (CA, dégâts, modificateurs)
- **Gestion manuelle** pour systèmes non supportés

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
- **[Cas d'usage Sorts et Équipements](./SpellsEquipmentUseCases.md)** - Exemples concrets officiels vs privés

### Documents Fonctionnels
- **[Cas d'utilisation généraux](./UseCases.md)** - Scénarios complets campagnes et combats

### Tests et Validation
- **[Guide des Tests](../Tests/README.md)** - Documentation complète des tests API
- Structure de tests par domaine (Generic, Dnd, Spells, Equipment, Security, Scenarios)

---

**Une API robuste et extensible pour la communauté JDR !** 🎲✨

*Pour plus de détails techniques, consultez la documentation spécialisée ci-dessus*