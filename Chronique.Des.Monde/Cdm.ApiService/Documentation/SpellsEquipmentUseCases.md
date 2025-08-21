# Cas d'Utilisation - Sorts et Équipements

Ce document présente des scénarios détaillés pour les systèmes de sorts et d'équipements, incluant la gestion des modificateurs D&D et les interfaces utilisateur.

## 🪄 Cas d'Utilisation Sorts

### Cas 1 : Création d'un Sort D&D par un Utilisateur

#### Contexte
Thomas (MJ) veut créer un sort personnalisé "Flèche Spectrale" pour sa campagne D&D.

#### Workflow Complet

##### 1. Accès à la page de création de sorts
```http
GET /spells?gameType=dnd&userId=2 HTTP/1.1
Authorization: Bearer {jwt_token_thomas}

Response: 200 OK
{
  "spells": [
    // Liste des sorts D&D existants
  ],
  "canCreateCustom": true,
  "userGameTypes": ["dnd", "generic"]
}
```

##### 2. Création du sort personnalisé avec tag D&D
```http
POST /spell?userId=2 HTTP/1.1
X-GameType: dnd
Authorization: Bearer {jwt_token_thomas}
Content-Type: application/json

{
  "name": "Flèche Spectrale",
  "description": "Une flèche d'énergie pure qui traverse les armures physiques",
  "imageUrl": "custom_spectral_arrow.jpg",
  "gameType": "dnd",
  "isPublic": false,
  "tags": ["évocation", "force", "attaque", "magique"],
  "dndProperties": {
    "level": 2,
    "school": "Évocation",
    "castingTime": "1 action",
    "range": "36 mètres",
    "duration": "Instantané",
    "components": ["V", "S"],
    "damageFormula": "2d6 + modificateur",
    "requiresAttackRoll": true,
    "requiresSavingThrow": false,
    "damageType": "Force"
  }
}

Response: 201 Created
{
  "id": 157,
  "name": "Flèche Spectrale",
  "createdByUserId": 2,
  "spellAttackFormula": "1d20 + modificateur Intelligence + bonus maîtrise",
  "calculatedForMage": {
    "level5Mage": "+7 pour toucher, 2d6+4 dégâts"
  }
}
```

### Cas 2 : Personnage Apprend un Sort avec Calculs Automatiques

#### Contexte
Lisa (Mage niveau 5, Intelligence 18) veut apprendre le sort "Flèche Spectrale" de Thomas.

#### Workflow avec Calculs Automatiques

##### 1. Consultation des sorts disponibles depuis l'interface personnage
```http
GET /character/16/available-spells HTTP/1.1
Authorization: Bearer {jwt_token_lisa}

Response: 200 OK
{
  "characterInfo": {
    "id": 16,
    "name": "Lyralei l'Archimage",
    "class": "Magicien",
    "level": 5,
    "spellcastingAbility": "intelligence",
    "intelligenceScore": 18,
    "proficiencyBonus": 3,
    "calculatedBonuses": {
      "spellAttackBonus": 7,  // (18-10)/2 + 3 = 4+3 = 7
      "spellSaveDC": 15       // 8 + 4 + 3 = 15
    }
  },
  "availableSpells": [
    {
      "id": 157,
      "name": "Flèche Spectrale",
      "level": 2,
      "school": "Évocation",
      "createdBy": "Thomas",
      "previewCalculation": {
        "attackBonus": "+7 pour toucher",
        "damage": "2d6+4 dégâts de force",
        "slotRequired": "Emplacement niveau 2"
      },
      "canLearn": true,
      "alreadyKnown": false
    }
  ]
}
```

##### 2. Apprentissage du sort
```http
POST /character/16/spells/157 HTTP/1.1
Authorization: Bearer {jwt_token_lisa}
Content-Type: application/json

{
  "learnMethod": "study",
  "notes": "Sort appris pendant le repos long au chapitre 3"
}

Response: 201 Created
{
  "success": true,
  "spellLearned": {
    "spellId": 157,
    "characterId": 16,
    "learnedDate": "2024-01-15T14:30:00Z",
    "isPrepared": true
  },
  "updatedCharacterSpells": {
    "knownSpellsCount": 8,
    "preparedSpellsCount": 9,
    "maxPreparedSpells": 9 // niveau 5 + modificateur Int = 5+4 = 9
  }
}
```

### Cas 3 : Restriction de Compatibilité Système

#### Contexte
Paul (Personnage Skyrim) essaie d'apprendre un sort D&D.

#### Workflow de Validation

```http
POST /character/25/spells/157 HTTP/1.1
Authorization: Bearer {jwt_token_paul}

Response: 400 Bad Request
{
  "error": "SYSTEM_INCOMPATIBILITY",
  "message": "Un personnage Skyrim ne peut pas apprendre des sorts D&D",
  "characterGameType": "skyrim",
  "spellGameType": "dnd",
  "suggestions": [
    {
      "action": "duplicate_character",
      "description": "Dupliquer le personnage en version D&D",
      "endpoint": "POST /character/duplicate"
    },
    {
      "action": "find_equivalent",
      "description": "Rechercher un sort équivalent pour Skyrim",
      "endpoint": "GET /spells?gameType=skyrim&similar=157"
    }
  ]
}
```

## ⚔️ Cas d'Utilisation Équipements

### Cas 4 : Création d'Équipement avec Bonus D&D

#### Contexte
Thomas crée une épée magique personnalisée pour sa campagne.

#### Workflow de Création

```http
POST /equipment?userId=2 HTTP/1.1
X-GameType: dnd
Authorization: Bearer {jwt_token_thomas}
Content-Type: application/json

{
  "name": "Lame de Vérité",
  "description": "Une épée longue qui brille d'une lumière dorée et révèle les mensonges",
  "imageUrl": "truth_blade.jpg",
  "gameType": "dnd",
  "isPublic": false,
  "tags": ["épée", "magique", "détection", "radiant"],
  "genericProperties": {
    "weight": 1.5,
    "value": 5000
  },
  "dndProperties": {
    "equipmentType": "Weapon",
    "weaponCategory": "Martial",
    "damageFormula": "1d8 + modificateur Force + 1",
    "damageType": "Tranchant",
    "properties": ["Versatile", "Finesse"],
    "rarity": "Rare",
    "requiresAttunement": true,
    "magicalProperties": [
      {
        "name": "Détection des Mensonges",
        "description": "La lame rougeoie quand une créature à 3m dit un mensonge"
      },
      {
        "name": "Dégâts Radiants",
        "description": "+1d4 dégâts radiants contre les créatures maléfiques"
      }
    ]
  }
}

Response: 201 Created
{
  "id": 248,
  "name": "Lame de Vérité",
  "equipmentSlot": "MainHand",
  "calculatedStats": {
    "averageDamage": "6.5 + Mod.Force",
    "criticalRange": "19-20",
    "versatileDamage": "1d10 + Mod.Force + 1"
  }
}
```

### Cas 5 : Ajout d'Équipement à l'Inventaire avec Calculs

#### Contexte
Lisa ajoute la "Lame de Vérité" à son inventaire et l'équipe.

#### Workflow avec Calculs Automatiques

##### 1. Ajout à l'inventaire
```http
POST /character/16/inventory/248 HTTP/1.1
Authorization: Bearer {jwt_token_lisa}
Content-Type: application/json

{
  "quantity": 1,
  "autoEquip": true,
  "notes": "Trouvée dans le trésor du chapitre 4"
}

Response: 201 Created
{
  "success": true,
  "inventoryItem": {
    "equipmentId": 248,
    "quantity": 1,
    "isEquipped": true,
    "equipmentSlot": "MainHand"
  },
  "characterUpdates": {
    "previousMainHand": {
      "name": "Dague Simple",
      "action": "moved_to_inventory"
    },
    "recalculatedStats": {
      "armorClass": 12, // Inchangé
      "attackBonuses": {
        "lameDeTruth": {
          "attackBonus": "+7", // Dex 14 (+2) + Prof 3 + Finesse = +5... Wait!
          "damageBonus": "1d8+3", // Dex +2 + bonus magique +1
          "specialDamage": "1d4 radiant vs maléfiques"
        }
      },
      "carriedWeight": "15.2 kg",
      "encumbrance": "Light"
    }
  },
  "attunementRequired": {
    "message": "Cette arme nécessite un lien. Procéder au rituel d'attunement ?",
    "duration": "1 heure de repos court",
    "currentAttunements": "1/3"
  }
}
```

##### 2. Processus d'attunement
```http
POST /character/16/attunement/248 HTTP/1.1
Authorization: Bearer {jwt_token_lisa}
Content-Type: application/json

{
  "method": "short_rest",
  "duration": "1_hour"
}

Response: 200 OK
{
  "attunementSuccessful": true,
  "attunedItem": {
    "name": "Lame de Vérité",
    "attunedAt": "2024-01-15T16:00:00Z",
    "fullBenefits": true
  },
  "updatedAbilities": {
    "passiveDetection": {
      "name": "Détection des Mensonges",
      "range": "3 mètres",
      "trigger": "Automatically when lies are spoken"
    }
  },
  "attunementSlots": "2/3 utilisés"
}
```

### Cas 6 : Gestion Multi-Quantités d'Équipements

#### Contexte
Gorthak (Guerrier) collecte plusieurs potions de soins.

#### Workflow de Gestion d'Inventaire

##### 1. Ajout de potions multiples
```http
POST /character/15/inventory/89 HTTP/1.1
Authorization: Bearer {jwt_token_thomas}
Content-Type: application/json

{
  "equipmentId": 89, // Potion de Soins
  "quantity": 5,
  "source": "purchased_at_shop",
  "unitPrice": 50
}

Response: 201 Created
{
  "inventoryUpdated": {
    "equipmentName": "Potion de Soins",
    "previousQuantity": 2,
    "newQuantity": 7,
    "totalValue": 350, // 7 × 50
    "stackable": true
  },
  "characterInventory": {
    "totalItems": 23,
    "totalWeight": "67.5 kg",
    "totalValue": "2,847 po",
    "encumbranceStatus": "Medium Load"
  }
}
```

##### 2. Utilisation d'une potion en combat
```http
PUT /character/15/inventory/89 HTTP/1.1
Authorization: Bearer {jwt_token_thomas}
Content-Type: application/json

{
  "action": "use_item",
  "quantity": 1,
  "context": "combat",
  "targetSelf": true
}

Response: 200 OK
{
  "itemUsed": {
    "name": "Potion de Soins",
    "effect": "Récupère 2d4+2 points de vie",
    "diceRoll": [3, 2], // Résultat 3+2+2 = 7 PV
    "healingAmount": 7
  },
  "characterUpdates": {
    "hitPoints": {
      "before": 45,
      "healed": 7,
      "after": 52,
      "maximum": 68
    },
    "inventory": {
      "potionsRemaining": 6
    }
  }
}
```

## 🎯 Cas d'Utilisation Interface Personnage

### Cas 7 : Vue Unifiée Personnage avec Sorts et Équipements

#### Contexte
Interface complète d'un personnage D&D avec calculs en temps réel.

#### État de l'Interface

```http
GET /character/16/complete-view HTTP/1.1
Authorization: Bearer {jwt_token_lisa}

Response: 200 OK
{
  "character": {
    "id": 16,
    "name": "Lyralei l'Archimage",
    "class": "Magicien",
    "race": "Elfe",
    "level": 5,
    "baseStats": {
      "strength": 8,
      "dexterity": 14,
      "constitution": 13,
      "intelligence": 18,
      "wisdom": 15,
      "charisma": 12
    },
    "calculatedStats": {
      "armorClass": 13, // 10 + Dex(+2) + Armure de Mage(+1)
      "hitPoints": { "current": 35, "maximum": 35 },
      "proficiencyBonus": 3,
      "spellcasting": {
        "ability": "intelligence",
        "attackBonus": 7,
        "saveDC": 15,
        "modifierBonus": 4
      }
    }
  },
  "spells": {
    "knownSpells": [
      {
        "id": 157,
        "name": "Flèche Spectrale",
        "level": 2,
        "isPrepared": true,
        "calculatedDamage": "2d6+4 force",
        "attackRoll": "1d20+7"
      },
      // ... autres sorts
    ],
    "spellSlots": {
      "level1": { "max": 4, "used": 1 },
      "level2": { "max": 3, "used": 0 },
      "level3": { "max": 2, "used": 1 }
    }
  },
  "equipment": {
    "equipped": [
      {
        "slot": "MainHand",
        "item": {
          "id": 248,
          "name": "Lame de Vérité",
          "isAttuned": true,
          "attackBonus": "+5", // Dex +2 + Prof +3
          "damageRoll": "1d8+3",
          "specialEffects": ["Détection Mensonges", "+1d4 vs Maléfiques"]
        }
      },
      {
        "slot": "Armor",
        "item": {
          "id": 112,
          "name": "Robe de Mage",
          "armorClass": "+1",
          "magicalProperties": ["Résistance sorts niveau 1"]
        }
      }
    ],
    "inventory": [
      {
        "id": 89,
        "name": "Potion de Soins",
        "quantity": 6,
        "quickUse": true
      }
      // ... autres objets
    ],
    "attunement": {
      "slotsUsed": 2,
      "slotsMax": 3,
      "attunedItems": ["Lame de Vérité", "Amulette de Protection"]
    }
  },
  "availableActions": {
    "canLearnNewSpells": true,
    "canEquipItems": ["Bouclier", "Arme à une main", "Armure légère"],
    "canAttuneItems": 1,
    "canCreateCustomSpells": true,
    "canCreateCustomEquipment": true
  }
}
```

### Cas 8 : Restriction et Suggestions Intelligentes

#### Contexte
Un personnage non-magique tente d'utiliser des sorts.

#### Workflow de Validation et Suggestions

```http
GET /character/22/available-spells HTTP/1.1
Authorization: Bearer {jwt_token_fighter}

Response: 200 OK
{
  "characterInfo": {
    "id": 22,
    "name": "Gareth l'Épéiste",
    "class": "Guerrier",
    "isSpellcaster": false
  },
  "availableSpells": [],
  "restrictions": {
    "noSpellcasting": {
      "message": "Les Guerriers ne peuvent pas lancer de sorts",
      "suggestions": [
        {
          "type": "multiclass",
          "description": "Prendre un niveau dans une classe de lanceur de sorts",
          "requirements": ["Intelligence ou Sagesse ou Charisme ≥ 13"]
        },
        {
          "type": "magic_items",
          "description": "Utiliser des objets magiques à usage limité",
          "examples": ["Parchemins", "Baguettes avec charges", "Objets à mot de commande"]
        },
        {
          "type": "feat_magic_initiate",
          "description": "Prendre le don 'Initié à la Magie' au niveau 4",
          "benefits": ["2 sorts mineurs + 1 sort niveau 1 par jour"]
        }
      ]
    }
  }
}
```

Ces cas d'utilisation montrent la richesse et la complexité du système, tout en maintenant une expérience utilisateur fluide grâce aux calculs automatiques et aux suggestions intelligentes.

---

*Retour aux [Spécifications Sorts et Équipements](./SpellsAndEquipment.md)*