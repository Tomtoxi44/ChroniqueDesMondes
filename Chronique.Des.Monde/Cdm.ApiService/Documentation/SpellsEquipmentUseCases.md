# Cas d'Utilisation - Sorts et Équipements

Ce document présente des scénarios détaillés pour les systèmes de sorts et d'équipements, incluant la distinction entre contenus officiels (injection admin) et privés (utilisateurs), ainsi que les mécaniques d'échange **d'équipements uniquement**.

## 🪄 Cas d'Utilisation Sorts

**Important** : Les sorts ne peuvent **PAS** être échangés entre joueurs. Chaque joueur doit apprendre ses sorts individuellement (officiels ou créer ses propres sorts privés).

### Cas 1 : Consultation des Sorts Disponibles (Officiels + Privés)

#### Contexte
Lisa (Mage D&D) consulte tous les sorts disponibles pour son personnage.

#### Workflow de Consultation Bi-Source

##### 1. Vue globale : Sorts officiels + privés utilisateur
```http
GET /spells?gameType=dnd&userId=3 HTTP/1.1
Authorization: Bearer {jwt_token_lisa}

Response: 200 OK
{
  "officialSpells": [
    {
      "id": 1,
      "name": "Boule de Feu",
      "level": 3,
      "school": "Évocation",
      "source": "official",
      "isPublic": true,
      "createdByUserId": 0,
      "description": "Une explosion de flammes dévastatrice",
      "dndProperties": {
        "damageFormula": "8d6",
        "savingThrow": "Dextérité",
        "castingTime": "1 action"
      }
    },
    {
      "id": 2,
      "name": "Projectile Magique",
      "level": 1,
      "school": "Évocation", 
      "source": "official",
      "isPublic": true,
      "createdByUserId": 0
    }
  ],
  "userPrivateSpells": [
    {
      "id": 157,
      "name": "Flèche Spectrale Personnalisée",
      "level": 2,
      "school": "Évocation",
      "source": "private",
      "isPublic": false,
      "createdByUserId": 3,
      "description": "Version modifiée par Lisa pour sa campagne"
    }
  ],
  "totalAvailable": 25,
  "userCanCreate": true,
  "exchangeNotice": "❌ Les sorts ne peuvent pas être échangés entre joueurs"
}
```

##### 2. Vue filtrée : Sorts officiels uniquement
```http
GET /spells/official?gameType=dnd HTTP/1.1
Authorization: Bearer {jwt_token_lisa}

Response: 200 OK
{
  "officialSpells": [
    // Seulement les sorts injectés par l'admin (CreatedByUserId = 0)
  ],
  "source": "administrative_injection",
  "lastUpdated": "2024-01-15T00:00:00Z",
  "totalCount": 23
}
```

### Cas 2 : Création d'un Sort Privé par un Utilisateur

#### Contexte
Thomas (MJ) veut créer un sort D&D personnalisé **privé** pour sa campagne.

#### Workflow de Création Privée

##### 1. Création du sort privé avec tag D&D
```http
POST /spell?userId=2 HTTP/1.1
X-GameType: dnd
Authorization: Bearer {jwt_token_thomas}
Content-Type: application/json

{
  "name": "Lame Spectrale de Thomas",
  "description": "Sort unique créé pour la campagne 'Les Terres Oubliées'",
  "imageUrl": "custom_spectral_blade.jpg",
  "gameType": "dnd",
  "isPublic": false,  // OBLIGATOIRE : Les sorts utilisateurs sont toujours privés
  "tags": ["évocation", "force", "attaque"],
  "dndProperties": {
    "level": 2,
    "school": "Évocation",
    "castingTime": "1 action",
    "range": "Contact",
    "duration": "1 minute",
    "components": ["V", "S"],
    "damageFormula": "2d8 + modificateur",
    "requiresAttackRoll": true,
    "requiresSavingThrow": false
  }
}

Response: 201 Created
{
  "id": 158,
  "name": "Lame Spectrale de Thomas",
  "source": "private",
  "createdByUserId": 2,
  "isPublic": false,
  "visibility": "PRIVÉ - Visible uniquement par Thomas",
  "calculatedProperties": {
    "spellAttackFormula": "1d20 + modificateur Intelligence + bonus maîtrise",
    "averageDamage": "13 + modificateur Intelligence"
  },
  "restrictions": [
    "❌ Ne peut pas être partagé avec d'autres utilisateurs",
    "❌ Ne peut pas être échangé entre joueurs",
    "✅ Peut être appris par tous les personnages de Thomas",
    "✅ Modifiable uniquement par Thomas"
  ]
}
```

### Cas 3 : Tentative d'Accès à un Sort Privé d'Autrui

#### Contexte
Lisa essaie d'apprendre le sort privé de Thomas.

#### Workflow de Restriction d'Accès

```http
GET /spell/158 HTTP/1.1
Authorization: Bearer {jwt_token_lisa}

Response: 403 Forbidden
{
  "error": "PRIVATE_SPELL_ACCESS_DENIED",
  "message": "Ce sort est privé et appartient à un autre utilisateur",
  "spellInfo": {
    "name": "Lame Spectrale de Thomas",
    "owner": "Thomas",
    "source": "private"
  },
  "suggestions": [
    {
      "action": "create_similar",
      "description": "Créer votre propre version de ce type de sort",
      "endpoint": "POST /spell"
    },
    {
      "action": "use_official_alternative",
      "description": "Utiliser un sort officiel similaire",
      "alternatives": [
        {
          "id": 45,
          "name": "Lame Enflammée",
          "level": 2,
          "school": "Évocation"
        }
      ]
    }
  ],
  "exchangeNotice": "❌ Les sorts ne peuvent jamais être échangés entre utilisateurs"
}
```

### Cas 4 : Apprentissage Mixte (Sorts Officiels + Privés)

#### Contexte
Lisa apprend des sorts de différentes sources pour son personnage.

#### Workflow d'Apprentissage Multi-Source

##### 1. Apprentissage d'un sort officiel
```http
POST /character/16/spells/1 HTTP/1.1
Authorization: Bearer {jwt_token_lisa}

Response: 201 Created
{
  "success": true,
  "spellLearned": {
    "spellId": 1,
    "name": "Boule de Feu",
    "source": "official",
    "learnedDate": "2024-01-15T14:30:00Z",
    "calculatedForCharacter": {
      "attackBonus": "N/A - Sort de sauvegarde",
      "saveDC": 15,
      "damage": "8d6 feu"
    }
  }
}
```

##### 2. Apprentissage d'un sort privé personnel
```http
POST /character/16/spells/157 HTTP/1.1
Authorization: Bearer {jwt_token_lisa}

Response: 201 Created
{
  "success": true,
  "spellLearned": {
    "spellId": 157,
    "name": "Flèche Spectrale Personnalisée",
    "source": "private",
    "createdBy": "Lisa",
    "learnedDate": "2024-01-15T14:35:00Z",
    "calculatedForCharacter": {
      "attackBonus": "+7 pour toucher",
      "damage": "2d6+4 force"
    }
  },
  "characterSpellSummary": {
    "officialSpells": 4,
    "privateSpells": 2,
    "totalKnown": 6
  }
}
```

## ⚔️ Cas d'Utilisation Équipements

**Important** : Contrairement aux sorts, les équipements peuvent être échangés entre joueurs selon les règles définies.

### Cas 5 : Consultation d'Équipements Officiels vs Privés

#### Contexte
Gorthak (Guerrier D&D) cherche une nouvelle épée.

#### Workflow de Consultation

##### 1. Vue des équipements disponibles
```http
GET /equipment?gameType=dnd&userId=2&type=weapon HTTP/1.1
Authorization: Bearer {jwt_token_thomas}

Response: 200 OK
{
  "officialEquipment": [
    {
      "id": 10,
      "name": "Épée Longue",
      "source": "official",
      "rarity": "Commun",
      "createdByUserId": 0,
      "dndProperties": {
        "weaponCategory": "Martial",
        "damageFormula": "1d8 + modificateur Force",
        "damageType": "Tranchant",
        "properties": ["Versatile (1d10)"]
      }
    },
    {
      "id": 11,
      "name": "Épée Longue +1",
      "source": "official",
      "rarity": "Peu Commun",
      "createdByUserId": 0
    }
  ],
  "userPrivateEquipment": [
    {
      "id": 248,
      "name": "Lame de Vérité de Thomas",
      "source": "private",
      "rarity": "Rare",
      "createdByUserId": 2,
      "description": "Épée personnalisée pour ma campagne",
      "visibility": "Privée - Créée par Thomas"
    }
  ],
  "exchangeNotice": "✅ Les équipements peuvent être échangés entre joueurs"
}
```

### Cas 6 : Création d'Équipement Générique (Campagne Non-D&D)

#### Contexte
Sophie (MJ campagne générique) crée un objet pour sa campagne steampunk.

#### Workflow Création Générique

```http
POST /equipment?userId=5 HTTP/1.1
X-GameType: generic
Authorization: Bearer {jwt_token_sophie}
Content-Type: application/json

{
  "name": "Pistolet à Vapeur Artisanal",
  "description": "Arme steampunk fonctionnant à la vapeur compressée",
  "imageUrl": "steampunk_gun.jpg",
  "gameType": "generic",
  "isPublic": false,
  "tags": ["steampunk", "arme", "vapeur", "distance"],
  "genericProperties": {
    "weight": 2.5,
    "value": 150,
    "attackBonusAbility": null,  // Pas de bonus automatique
    "damageFormula": null        // Pas de formule automatique
  }
}

Response: 201 Created
{
  "id": 325,
  "name": "Pistolet à Vapeur Artisanal",
  "source": "private",
  "gameType": "generic",
  "automaticCalculations": false,
  "usage": {
    "combatAssistance": "❌ Aucune - Gestion manuelle par le MJ",
    "rules": "Sophie définit elle-même les règles d'utilisation selon son système JDR"
  },
  "visibility": "Privé - Visible uniquement par Sophie",
  "exchangeCapability": "✅ Peut être proposé aux joueurs de ses campagnes"
}
```

## 🔄 Cas d'Utilisation Échanges d'Équipements

### Cas 7 : MJ Propose un Équipement à un Joueur

#### Contexte
Thomas (MJ) veut donner l'épée "Lame de Vérité" qu'il a créée à Lisa (joueur de sa campagne).

#### Workflow Proposition MJ → Joueur

##### 1. Thomas propose l'équipement à Lisa
```http
POST /campaign/42/equipment/offer HTTP/1.1
Authorization: Bearer {jwt_token_thomas}
Content-Type: application/json

{
  "targetPlayerId": 3,
  "equipmentId": 248,  // Lame de Vérité
  "quantity": 1,
  "message": "Cette épée magique sera parfaite pour ton personnage mage/guerrier !"
}

Response: 201 Created
{
  "offerId": 1001,
  "campaignId": 42,
  "gameMasterId": 2,
  "targetPlayerId": 3,
  "equipmentInfo": {
    "name": "Lame de Vérité",
    "rarity": "Rare",
    "description": "Épée longue magique avec détection des mensonges"
  },
  "quantity": 1,
  "status": "Pending",
  "createdAt": "2024-01-15T16:00:00Z",
  "notification": {
    "message": "Proposition envoyée à Lisa",
    "playerNotified": true
  }
}
```

##### 2. Lisa consulte ses propositions en attente
```http
GET /campaign/42/equipment/offers?playerId=3 HTTP/1.1
Authorization: Bearer {jwt_token_lisa}

Response: 200 OK
{
  "pendingOffers": [
    {
      "offerId": 1001,
      "fromGameMaster": "Thomas",
      "equipment": {
        "name": "Lame de Vérité",
        "rarity": "Rare",
        "image": "truth_blade.jpg",
        "description": "Une épée longue qui brille d'une lumière dorée",
        "dndProperties": {
          "damageFormula": "1d8 + modificateur Force + 1",
          "properties": ["Versatile", "Finesse", "Détection Mensonges"]
        }
      },
      "quantity": 1,
      "message": "Cette épée magique sera parfaite pour ton personnage mage/guerrier !",
      "receivedAt": "2024-01-15T16:00:00Z",
      "canAccept": true,
      "compatibilityCheck": {
        "characterGameType": "dnd",
        "equipmentGameType": "dnd",
        "compatible": true
      }
    }
  ]
}
```

##### 3. Lisa accepte la proposition
```http
PUT /campaign/42/equipment/offer/1001 HTTP/1.1
Authorization: Bearer {jwt_token_lisa}
Content-Type: application/json

{
  "response": "accepted",
  "characterId": 16,  // Lyralei l'Archimage
  "message": "Merci Thomas ! Cette épée sera parfaite pour Lyralei."
}

Response: 200 OK
{
  "success": true,
  "offerStatus": "Accepted",
  "equipmentAdded": {
    "characterId": 16,
    "equipmentId": 248,
    "name": "Lame de Vérité",
    "quantity": 1,
    "addedToInventory": true
  },
  "gmInventoryStatus": {
    "equipmentStillAvailable": true,
    "message": "L'équipement reste disponible chez le MJ pour d'autres propositions"
  },
  "characterUpdates": {
    "inventoryCount": 12,
    "canEquip": true,
    "slot": "MainHand"
  }
}
```

### Cas 8 : Échange Direct entre Joueurs

#### Contexte
Lisa veut donner une potion de soins à Paul (autre joueur de la campagne) en échange de rien.

#### Workflow Échange Joueur → Joueur

##### 1. Lisa propose l'échange à Paul
```http
POST /campaign/42/equipment/trade HTTP/1.1
Authorization: Bearer {jwt_token_lisa}
Content-Type: application/json

{
  "toPlayerId": 4,  // Paul
  "fromCharacterId": 16,  // Lyralei
  "toCharacterId": 22,    // Personnage de Paul
  "equipmentId": 89,      // Potion de Soins
  "quantity": 2,
  "message": "Tu en auras plus besoin que moi pour le prochain combat !"
}

Response: 201 Created
{
  "tradeId": 2001,
  "campaignId": 42,
  "fromPlayer": "Lisa",
  "toPlayer": "Paul",
  "equipment": {
    "name": "Potion de Soins",
    "quantity": 2
  },
  "status": "Proposed",
  "validationChecks": {
    "senderHasQuantity": true,
    "gameTypeCompatible": true,
    "playersInSameCampaign": true,
    "canProceed": true
  },
  "createdAt": "2024-01-15T17:30:00Z"
}
```

##### 2. Paul consulte ses demandes d'échange
```http
GET /campaign/42/equipment/trade-requests?playerId=4 HTTP/1.1
Authorization: Bearer {jwt_token_paul}

Response: 200 OK
{
  "pendingTrades": [
    {
      "tradeId": 2001,
      "fromPlayer": "Lisa",
      "fromCharacter": "Lyralei l'Archimage",
      "toCharacter": "Gareth l'Épéiste",
      "equipment": {
        "name": "Potion de Soins",
        "quantity": 2,
        "description": "Récupère 2d4+2 points de vie",
        "value": 100  // 2 × 50po
      },
      "message": "Tu en auras plus besoin que moi pour le prochain combat !",
      "proposedAt": "2024-01-15T17:30:00Z",
      "canAccept": true
    }
  ]
}
```

##### 3. Paul accepte l'échange
```http
PUT /campaign/42/equipment/trade/2001 HTTP/1.1
Authorization: Bearer {jwt_token_paul}
Content-Type: application/json

{
  "response": "accepted",
  "message": "Merci beaucoup Lisa ! Ça va m'aider énormément."
}

Response: 200 OK
{
  "success": true,
  "tradeCompleted": {
    "tradeId": 2001,
    "status": "Completed",
    "completedAt": "2024-01-15T17:35:00Z"
  },
  "inventoryChanges": {
    "fromPlayer": {
      "playerId": 3,
      "characterId": 16,
      "removed": {
        "equipmentName": "Potion de Soins",
        "quantity": 2
      },
      "newQuantity": 4  // Il lui en reste 4
    },
    "toPlayer": {
      "playerId": 4,
      "characterId": 22,
      "added": {
        "equipmentName": "Potion de Soins",
        "quantity": 2
      },
      "newQuantity": 2  // Il en a maintenant 2
    }
  },
  "notifications": {
    "fromPlayerNotified": true,
    "toPlayerNotified": true,
    "gmNotified": true  // Le MJ est informé de l'échange
  }
}
```

### Cas 9 : Refus d'Échange et Gestion d'Erreurs

#### Contexte
Paul refuse un autre échange et cas d'erreur avec quantité insuffisante.

#### Workflow de Refus et Validation

##### 1. Paul refuse une proposition d'échange
```http
PUT /campaign/42/equipment/trade/2002 HTTP/1.1
Authorization: Bearer {jwt_token_paul}
Content-Type: application/json

{
  "response": "declined",
  "message": "Désolé, j'ai besoin de garder cet équipement pour l'instant."
}

Response: 200 OK
{
  "success": true,
  "tradeStatus": "Declined",
  "fromPlayerNotified": true,
  "message": "Échange refusé. Le proposeur a été notifié."
}
```

##### 2. Tentative d'échange avec quantité insuffisante
```http
POST /campaign/42/equipment/trade HTTP/1.1
Authorization: Bearer {jwt_token_lisa}
Content-Type: application/json

{
  "toPlayerId": 4,
  "fromCharacterId": 16,
  "toCharacterId": 22,
  "equipmentId": 89,
  "quantity": 10,  // Lisa n'en a que 4
  "message": "Je te donne toutes mes potions !"
}

Response: 400 Bad Request
{
  "error": "INSUFFICIENT_QUANTITY",
  "message": "Quantité insuffisante pour effectuer cet échange",
  "details": {
    "requested": 10,
    "available": 4,
    "equipmentName": "Potion de Soins"
  },
  "suggestions": [
    {
      "action": "adjust_quantity",
      "description": "Réduire la quantité à 4 ou moins",
      "maxAvailable": 4
    }
  ]
}
```

## 🎯 Cas d'Utilisation Interface Administrative

### Cas 10 : Injection de Sorts Officiels (Script Admin)

#### Contexte
L'équipe de développement ajoute les sorts officiels Skyrim.

#### Workflow d'Injection Administrative

##### 1. Script d'injection SQL (exécuté par l'admin)
```sql
-- Injection administrative des sorts Skyrim (dans 6 semaines)
INSERT INTO Spells (Name, Description, GameType, IsPublic, CreatedByUserId, SkyrimProperties)
VALUES 
(
  'Flammes',
  'Jet de flammes continu qui brûle les ennemis proches',
  'skyrim',
  true,
  0,  -- 0 = Administrateur
  '{"school":"Destruction","magickaCost":5,"baseDamage":8,"skillLevel":"Novice"}'
),
(
  'Soin',
  'Restaure instantanément la santé du lanceur',
  'skyrim', 
  true,
  0,
  '{"school":"Restoration","magickaCost":25,"healAmount":"25","skillLevel":"Novice"}'
),
(
  'Boule de Glace',
  'Projectile de glace qui ralentit et endommage',
  'skyrim',
  true, 
  0,
  '{"school":"Destruction","magickaCost":30,"baseDamage":25,"effect":"slow","skillLevel":"Apprentice"}'
);

-- Vérification post-injection
SELECT COUNT(*) as 'Sorts Skyrim Officiels' 
FROM Spells 
WHERE GameType = 'skyrim' AND CreatedByUserId = 0;
```

##### 2. Résultat pour les utilisateurs après injection
```http
GET /spells/official?gameType=skyrim HTTP/1.1
Authorization: Bearer {jwt_token_any_user}

Response: 200 OK
{
  "officialSpells": [
    {
      "id": 501,
      "name": "Flammes",
      "source": "official",
      "gameType": "skyrim",
      "createdByUserId": 0,
      "skyrimProperties": {
        "school": "Destruction",
        "magickaCost": 5,
        "baseDamage": 8
      }
    }
    // ... autres sorts Skyrim
  ],
  "injectionInfo": {
    "addedBy": "Administrator",
    "injectionDate": "2024-03-01T00:00:00Z",
    "totalSkyrimSpells": 45
  },
  "availableForAllUsers": true
}
```

### Cas 11 : Interface Personnage Multi-Source avec Échanges

#### Contexte
Interface complète montrant la distinction entre sources et les échanges en cours.

#### État de l'Interface

```http
GET /character/16/complete-view HTTP/1.1
Authorization: Bearer {jwt_token_lisa}

Response: 200 OK
{
  "character": {
    "id": 16,
    "name": "Lyralei l'Archimage",
    "gameType": "dnd"
  },
  "spells": {
    "knownSpells": [
      {
        "id": 1,
        "name": "Boule de Feu",
        "source": "official",
        "icon": "🌟",
        "modifiable": false,
        "shareStatus": "Disponible pour tous les joueurs D&D",
        "canExchange": false,
        "exchangeNotice": "❌ Les sorts ne peuvent jamais être échangés"
      },
      {
        "id": 157,
        "name": "Flèche Spectrale Personnalisée",
        "source": "private",
        "icon": "👤",
        "modifiable": true,
        "shareStatus": "Privé - Visible uniquement par vous",
        "canExchange": false,
        "exchangeNotice": "❌ Les sorts ne peuvent jamais être échangés"
      }
    ],
    "availableToLearn": {
      "officialCount": 23,
      "privateCount": 3,
      "restrictedCount": 5  // Sorts privés d'autres utilisateurs
    }
  },
  "equipment": {
    "inventory": [
      {
        "id": 10,
        "name": "Épée Longue",
        "source": "official",
        "icon": "🌟",
        "modifiable": false,
        "quantity": 1,
        "canTrade": true,
        "exchangeNotice": "✅ Peut être échangé avec d'autres joueurs"
      },
      {
        "id": 248,
        "name": "Lame de Vérité",
        "source": "private", 
        "icon": "👤",
        "modifiable": false,  // Créée par le MJ
        "quantity": 1,
        "canTrade": true,
        "receivedFrom": "Thomas (MJ)",
        "exchangeNotice": "✅ Peut être échangé avec d'autres joueurs"
      },
      {
        "id": 89,
        "name": "Potion de Soins",
        "source": "official",
        "icon": "🌟",
        "quantity": 4,
        "canTrade": true,
        "exchangeNotice": "✅ Peut être échangé avec d'autres joueurs"
      }
    ]
  },
  "exchanges": {
    "pendingOffers": [
      {
        "type": "gm_offer",
        "fromGM": "Thomas",
        "equipment": "Anneau de Protection",
        "quantity": 1,
        "receivedAt": "2024-01-15T18:00:00Z"
      }
    ],
    "pendingTrades": [
      {
        "type": "player_trade",
        "fromPlayer": "Sophie",
        "equipment": "Épée Courte +1",
        "quantity": 1,
        "proposedAt": "2024-01-15T17:45:00Z"
      }
    ],
    "recentActivity": [
      {
        "type": "trade_completed",
        "with": "Paul",
        "equipment": "Potion de Soins",
        "quantity": 2,
        "direction": "given",
        "completedAt": "2024-01-15T17:35:00Z"
      }
    ]
  },
  "creationRights": {
    "canCreateSpells": true,
    "canCreateEquipment": true,
    "canTradeEquipment": true,
    "restrictions": [
      "❌ Vos créations de sorts seront privées et non échangeables",
      "❌ Pas de partage de sorts avec autres utilisateurs",
      "✅ Vos équipements peuvent être proposés aux joueurs de vos campagnes",
      "✅ Échanges d'équipements possibles entre joueurs de même campagne"
    ]
  }
}
```

## 🔒 Cas d'Utilisation Sécurité et Permissions

### Cas 12 : Validation des Permissions de Modification et d'Échange

#### Contexte
Thomas essaie de modifier différents types de contenus et d'effectuer des échanges non autorisés.

#### Workflow de Validation

##### 1. Tentative de modification d'un sort officiel (INTERDIT)
```http
PUT /spell/1 HTTP/1.1
Authorization: Bearer {jwt_token_thomas}
Content-Type: application/json

{
  "name": "Boule de Feu Modifiée"
}

Response: 403 Forbidden
{
  "error": "OFFICIAL_CONTENT_MODIFICATION_DENIED",
  "message": "Les sorts officiels ne peuvent pas être modifiés par les utilisateurs",
  "spellInfo": {
    "name": "Boule de Feu",
    "source": "official",
    "createdByUserId": 0
  },
  "alternatives": [
    {
      "action": "duplicate_as_private",
      "description": "Créer une version privée basée sur ce sort",
      "endpoint": "POST /spell/duplicate/1"
    }
  ]
}
```

##### 2. Tentative d'échange avec joueur d'une autre campagne (INTERDIT)
```http
POST /campaign/42/equipment/trade HTTP/1.1
Authorization: Bearer {jwt_token_thomas}
Content-Type: application/json

{
  "toPlayerId": 8,  // Joueur d'une autre campagne
  "fromCharacterId": 15,
  "toCharacterId": 25,
  "equipmentId": 248,
  "quantity": 1
}

Response: 403 Forbidden
{
  "error": "CROSS_CAMPAIGN_TRADE_DENIED",
  "message": "Les échanges ne sont possibles qu'entre joueurs de la même campagne",
  "details": {
    "yourCampaign": 42,
    "targetPlayerCampaigns": [35, 47],  // Autres campagnes du joueur cible
    "sharedCampaigns": []
  },
  "suggestions": [
    {
      "action": "invite_to_campaign",
      "description": "Inviter le joueur à rejoindre votre campagne",
      "endpoint": "POST /campaign/42/invite"
    }
  ]
}
```

##### 3. Modification d'un sort privé personnel (AUTORISÉ)
```http
PUT /spell/158 HTTP/1.1
Authorization: Bearer {jwt_token_thomas}
Content-Type: application/json

{
  "description": "Description mise à jour pour ma campagne"
}

Response: 200 OK
{
  "success": true,
  "message": "Sort privé modifié avec succès",
  "updatedSpell": {
    "id": 158,
    "name": "Lame Spectrale de Thomas",
    "source": "private",
    "lastModified": "2024-01-15T16:00:00Z"
  }
}
```

Ces cas d'utilisation montrent clairement :
- **Sorts** : Pas d'échange possible, apprentissage individuel uniquement
- **Équipements** : Échanges riches et sécurisés entre MJ et joueurs
- **Sécurité** : Validation stricte des permissions et des règles métier

---

*Retour aux [Spécifications Sorts et Équipements](./SpellsAndEquipment.md)*