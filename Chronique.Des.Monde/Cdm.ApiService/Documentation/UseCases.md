# Cas d'Utilisation Détaillés - Chronique des Mondes

Ce document présente des scénarios complets d'utilisation de l'API Chronique des Mondes avec des exemples concrets de requêtes et de workflows.

## 🎭 Cas d'Utilisation 1 : Création complète d'une campagne D&D

### Contexte
Marie veut créer une campagne D&D appelée "Les Terres Oubliées" avec plusieurs chapitres, PNJ et monstres.

### Workflow Complet

#### 1. Création de la campagne
```http
POST /campaign?userId=1 HTTP/1.1
X-GameType: dnd
Authorization: Bearer {jwt_token}
Content-Type: application/json

{
  "name": "Les Terres Oubliées",
  "description": "Une aventure épique dans un monde post-apocalyptique où la magie a refaçonné la réalité.",
  "gameType": "dnd",
  "isPublic": false
}

Response: 201 Created
{
  "id": 42,
  "name": "Les Terres Oubliées",
  "gameMasterId": 1,
  "gameType": "dnd",
  "createdAt": "2024-01-15T10:00:00Z"
}
```

#### 2. Création du Chapitre 1
```http
POST /campaign/42/chapter HTTP/1.1
Authorization: Bearer {jwt_token}
Content-Type: application/json

{
  "chapterNumber": 1,
  "title": "L'Arrivée au Village de Braelynn",
  "narrativeBlocks": [
    {
      "order": 1,
      "content": "Les aventuriers arrivent au village de Braelynn au coucher du soleil. Le village semble étrangement calme.",
      "linkedNpcId": null
    },
    {
      "order": 2,
      "content": "L'aubergiste Brom les accueille avec méfiance.",
      "linkedNpcId": "will_be_created_next"
    }
  ]
}

Response: 201 Created
{
  "id": 101,
  "campaignId": 42,
  "chapterNumber": 1,
  "title": "L'Arrivée au Village de Braelynn"
}
```

#### 3. Création d'un PNJ générique (Aubergiste)
```http
POST /chapter/101/npc HTTP/1.1
X-GameType: generic
Authorization: Bearer {jwt_token}
Content-Type: application/json

{
  "name": "Brom",
  "surname": "Brewwright",
  "description": "Un aubergiste robuste aux cheveux grisonnants, ancien aventurier reconverti. Il a l'œil perçant et sait juger les gens.",
  "type": "npc",
  "gameType": "generic",
  "behaviors": [
    {
      "playerAttitude": "friendly",
      "npcResponse": "Brom sourit chaleureusement et offre sa meilleure ale. Il partage des rumeurs locales et des conseils utiles.",
      "backgroundContext": "Le groupe a été poli et respectueux"
    },
    {
      "playerAttitude": "neutral",
      "npcResponse": "Brom reste professionnel mais distant. Il répond aux questions basiques mais ne s'étend pas.",
      "backgroundContext": "Le groupe ne montre ni hostilité ni particular charisme"
    },
    {
      "playerAttitude": "hostile",
      "npcResponse": "Brom devient méfiant et appelle discrètement la garde du village. Les prix augmentent mystérieusement.",
      "backgroundContext": "Le groupe a été menaçant ou irrespectueux"
    }
  ]
}
```

#### 4. Création d'un monstre D&D (Gobelins)
```http
POST /chapter/101/npc HTTP/1.1
X-GameType: dnd
Authorization: Bearer {jwt_token}
Content-Type: application/json

{
  "name": "Gobelins des Caves",
  "description": "Trois gobelins malicieux qui hantent les caves sous l'auberge",
  "type": "monster",
  "gameType": "dnd",
  "stats": {
    "armorClass": 15,
    "hitPoints": 7,
    "speed": "9m",
    "strength": 8,
    "dexterity": 14,
    "constitution": 10,
    "intelligence": 10,
    "wisdom": 8,
    "charisma": 8,
    "challengeRating": "1/4",
    "attacks": [
      {
        "name": "Cimeterre",
        "bonus": 4,
        "damage": "1d6+2 tranchant"
      },
      {
        "name": "Arc court",
        "bonus": 4,
        "damage": "1d6+2 perforant",
        "range": "24/96m"
      }
    ],
    "skills": ["Discrétion +6"],
    "senses": ["Vision dans le noir 18m"],
    "languages": ["Commun", "Gobelin"]
  },
  "quantity": 3
}
```

#### 5. Création du Chapitre 2
```http
POST /campaign/42/chapter HTTP/1.1
Authorization: Bearer {jwt_token}
Content-Type: application/json

{
  "chapterNumber": 2,
  "title": "Les Caves Hantées",
  "narrativeBlocks": [
    {
      "order": 1,
      "content": "Des bruits étranges proviennent des caves de l'auberge pendant la nuit.",
      "linkedNpcId": null
    },
    {
      "order": 2,
      "content": "Brom révèle que des créatures ont élu domicile dans les caves et perturbent son commerce.",
      "linkedNpcId": 201  // ID du PNJ Brom créé précédemment
    }
  ]
}
```

---

## 🎲 Cas d'Utilisation 2 : Lancement d'un combat D&D

### Contexte
Le MJ (Marie) lance le combat contre les gobelins du Chapitre 2. Les joueurs Thomas (Guerrier) et Lisa (Mage) participent.

### Workflow de Combat

#### 1. Le MJ visualise son chapitre et lance le combat
```http
GET /chapter/102 HTTP/1.1
Authorization: Bearer {jwt_token}

Response: 200 OK
{
  "id": 102,
  "title": "Les Caves Hantées",
  "npcs": [
    {
      "id": 301,
      "name": "Gobelins des Caves",
      "type": "monster",
      "quantity": 3,
      "stats": { /* stats D&D complètes */ }
    }
  ]
}
```

#### 2. Démarrage du combat
```http
POST /chapter/102/combat/start HTTP/1.1
Authorization: Bearer {jwt_token}
Content-Type: application/json

{
  "participants": [
    {
      "type": "player",
      "characterId": 15,  // Thomas - Guerrier
      "userId": 2
    },
    {
      "type": "player", 
      "characterId": 16,  // Lisa - Mage
      "userId": 3
    },
    {
      "type": "npc",
      "npcId": 301,       // Gobelins
      "quantity": 3
    }
  ]
}

Response: 201 Created
{
  "combatId": 501,
  "status": "initiative_phase",
  "turnOrder": [
    { "participantId": 16, "name": "Lisa", "initiative": 18, "type": "player" },
    { "participantId": 301, "name": "Gobelin 1", "initiative": 15, "type": "npc" },
    { "participantId": 15, "name": "Thomas", "initiative": 12, "type": "player" },
    { "participantId": 302, "name": "Gobelin 2", "initiative": 10, "type": "npc" },
    { "participantId": 303, "name": "Gobelin 3", "initiative": 8, "type": "npc" }
  ],
  "currentTurn": {
    "participantId": 16,
    "name": "Lisa",
    "type": "player",
    "userId": 3
  }
}
```

#### 3. Tour de Lisa (Mage) - Lance un sort
```http
POST /combat/501/action HTTP/1.1
Authorization: Bearer {jwt_token_lisa}
Content-Type: application/json

{
  "actionType": "spell_attack",
  "spellName": "Projectile Magique",
  "targetId": 301,
  "diceRolls": {
    "attackRoll": null,  // Projectile magique touche automatiquement
    "damageRoll": [3, 1, 4]  // 3 projectiles : 1d4+1 chacun
  }
}

Response: 200 OK
{
  "success": true,
  "result": {
    "hit": true,
    "totalDamage": 11,  // (3+1) + (1+1) + (4+1)
    "target": {
      "name": "Gobelin 1",
      "remainingHp": -4,  // 7 HP - 11 dégâts = mort
      "status": "dead"
    }
  },
  "nextTurn": {
    "participantId": 301,
    "name": "Gobelin 1",
    "type": "npc",
    "status": "skipped_dead"
  }
}
```

#### 4. Tour automatique du Gobelin 2 (géré par l'IA)
```http
# Requête automatique du système
POST /combat/501/action HTTP/1.1
Authorization: Bearer {jwt_token_gm}
Content-Type: application/json

{
  "actionType": "attack",
  "attackName": "Cimeterre",
  "targetId": 15,  // Thomas le Guerrier
  "diceRolls": {
    "attackRoll": 16,  // 1d20 + 4 = 16
    "damageRoll": [4]  // 1d6 + 2 = 6 dégâts
  },
  "automated": true
}

Response: 200 OK
{
  "success": true,
  "result": {
    "hit": true,  // 16 vs CA 18 de Thomas = échec
    "actualHit": false,
    "message": "Le gobelin rate son attaque contre l'armure de Thomas"
  }
}
```

---

## 👥 Cas d'Utilisation 3 : Utilisateur Multi-Rôles

### Contexte
Thomas est MJ de sa propre campagne "Pirates des Mers du Sud" ET joueur dans la campagne "Les Terres Oubliées" de Marie.

### Workflow Multi-Rôles

#### 1. Thomas consulte ses campagnes (toutes confondues)
```http
GET /campaign?userId=2 HTTP/1.1
Authorization: Bearer {jwt_token_thomas}

Response: 200 OK
{
  "campaigns": [
    {
      "id": 42,
      "name": "Les Terres Oubliées",
      "role": "player",
      "gameMaster": "Marie",
      "gameType": "dnd",
      "myCharacter": {
        "id": 15,
        "name": "Thomas Ironshield",
        "class": "Guerrier"
      }
    },
    {
      "id": 58,
      "name": "Pirates des Mers du Sud",
      "role": "gamemaster",
      "gameType": "generic",
      "playerCount": 3,
      "currentChapter": 4
    }
  ]
}
```

#### 2. Thomas gère SA campagne comme MJ
```http
GET /campaign/58 HTTP/1.1
Authorization: Bearer {jwt_token_thomas}

Response: 200 OK
{
  "id": 58,
  "name": "Pirates des Mers du Sud",
  "description": "Aventures en haute mer dans un monde de pirates fantastiques",
  "role": "gamemaster",
  "gameType": "generic",
  "chapters": [
    {
      "id": 201,
      "chapterNumber": 1,
      "title": "Le Port de Tortuga"
    },
    {
      "id": 202,
      "chapterNumber": 2,
      "title": "L'Abordage du Galion Maudit"
    }
    // ... autres chapitres
  ],
  "players": [
    {
      "userId": 4,
      "userName": "Paul",
      "characterName": "Captain Redbeard"
    },
    {
      "userId": 5,
      "userName": "Sophie",
      "characterName": "Luna Nightblade"
    }
  ]
}
```

#### 3. Thomas ajoute un PNJ à SA campagne
```http
POST /chapter/202/npc HTTP/1.1
X-GameType: generic
Authorization: Bearer {jwt_token_thomas}
Content-Type: application/json

{
  "name": "Capitaine Skullbeard",
  "description": "Un capitaine pirate mort-vivant qui commande le galion maudit",
  "type": "npc",
  "gameType": "generic",
  "customStats": {
    "hitPoints": 150,
    "armorValue": "Haute",
    "weaponDamage": "Sabre maudit - 2d8",
    "specialAbilities": [
      "Immunité aux dégâts non-magiques",
      "Régénération 5 HP par tour",
      "Aura de peur"
    ]
  },
  "behaviors": [
    {
      "playerAttitude": "negotiation",
      "npcResponse": "Skullbeard ricane et propose un défi : une course de navires contre la liberté de l'équipage",
      "backgroundContext": "Les joueurs tentent de négocier plutôt que de se battre"
    }
  ]
}
```

---

## 🔄 Cas d'Utilisation 4 : Duplication de Personnage Multi-Système

### Contexte
Lisa a un personnage D&D "Lyralei l'Archimage" et veut le dupliquer pour rejoindre la campagne générique de Thomas.

### Workflow de Duplication

#### 1. Lisa consulte son personnage D&D existant
```http
GET /character/16 HTTP/1.1
Authorization: Bearer {jwt_token_lisa}

Response: 200 OK
{
  "id": 16,
  "name": "Lyralei l'Archimage",
  "gameType": "dnd",
  "class": "Magicien",
  "race": "Elfe",
  "level": 3,
  "stats": {
    "strength": 8,
    "dexterity": 14,
    "constitution": 13,
    "intelligence": 18,
    "wisdom": 15,
    "charisma": 12,
    "hitPoints": 18,
    "armorClass": 12
  },
  "spells": ["Projectile Magique", "Bouclier", "Boule de Feu"],
  "equipment": ["Bâton de Mage", "Robe d'Apprenti"]
}
```

#### 2. Duplication vers un personnage générique
```http
POST /character?userId=3 HTTP/1.1
X-GameType: generic
Authorization: Bearer {jwt_token_lisa}
Content-Type: application/json

{
  "name": "Lyralei la Navigatrice",
  "description": "Une ancienne mage qui a troqué ses sorts contre la navigation et l'aventure en mer",
  "gameType": "generic",
  "baseCharacterId": 16,  // Référence au personnage original
  "customFields": {
    "hitPoints": 18,
    "armorValue": "Légère",
    "skills": [
      "Navigation",
      "Connaissance des étoiles", 
      "Cartographie",
      "Premiers secours"
    ],
    "equipment": [
      "Sextant magique",
      "Cartes marines enchantées",
      "Dague de marin",
      "Vêtements de voyage"
    ],
    "background": "Ancienne mage reconvertie en navigatrice après avoir découvert sa passion pour l'exploration maritime"
  }
}

Response: 201 Created
{
  "id": 45,
  "name": "Lyralei la Navigatrice",
  "gameType": "generic",
  "userId": 3,
  "originalCharacterId": 16
}
```

#### 3. Lisa rejoint la campagne de Thomas avec son nouveau personnage
```http
POST /campaign/58/join HTTP/1.1
Authorization: Bearer {jwt_token_lisa}
Content-Type: application/json

{
  "characterId": 45,
  "message": "Je voudrais rejoindre votre équipage en tant que navigatrice !"
}

Response: 200 OK
{
  "status": "invitation_sent",
  "message": "Demande envoyée au MJ Thomas. En attente d'approbation."
}
```

---

## 🤖 Cas d'Utilisation 5 : Intégration IA Future

### Contexte
Marie veut utiliser l'assistance IA pour enrichir sa campagne D&D avec de nouveaux PNJ et lieux.

### Workflow avec IA (Fonctionnalité Future)

#### 1. Demande d'assistance pour créer un PNJ
```http
POST /ai/generate/npc HTTP/1.1
X-GameType: dnd
Authorization: Bearer {jwt_token}
Content-Type: application/json

{
  "campaignId": 42,
  "chapterId": 102,
  "context": {
    "location": "Village de Braelynn",
    "mood": "mystérieux",
    "role": "marchand",
    "playerLevel": 3
  },
  "requirements": {
    "type": "npc",
    "gameType": "dnd",
    "shouldHaveQuest": true,
    "difficultyLevel": "medium"
  }
}

Response: 200 OK
{
  "generatedNpc": {
    "name": "Maître Aldric",
    "surname": "Moonwhisper", 
    "description": "Un marchand elfe aux yeux argentés qui vend des objets magiques rares. Il semble connaître plus de secrets qu'il n'en révèle.",
    "type": "npc",
    "gameType": "dnd",
    "stats": {
      "armorClass": 13,
      "hitPoints": 27,
      "skills": ["Persuasion +6", "Arcanes +5", "Investigation +4"],
      "equipment": ["Amulette de Protection", "Bourse Magique", "Parchemins Mystérieux"]
    },
    "behaviors": [
      {
        "playerAttitude": "curious",
        "npcResponse": "Aldric révèle l'existence d'un artefact ancien caché dans les ruines proches, mais demande un service en échange.",
        "questHook": "Récupérer un cristal volé par des bandits"
      }
    ],
    "aiGenerationMetadata": {
      "model": "chronicleAI-v2.1",
      "confidence": 0.87,
      "alternatives": 2
    }
  }
}
```

#### 2. Génération d'un lieu avec contexte
```http
POST /ai/generate/location HTTP/1.1
X-GameType: dnd
Authorization: Bearer {jwt_token}
Content-Type: application/json

{
  "campaignId": 42,
  "context": {
    "nearLocation": "Village de Braelynn",
    "theme": "ruines anciennes",
    "dangerLevel": "medium",
    "containsArtifact": true
  }
}

Response: 200 OK
{
  "generatedLocation": {
    "name": "Ruines de l'Ancien Temple de Séléné",
    "description": "Des ruines de marbre blanc partiellement enfouies dans la forêt. Des symboles lunaires brillent faiblement sur les murs.",
    "rooms": [
      {
        "name": "Sanctuaire Principal",
        "description": "Une grande salle avec un autel de marbre fissuré. Une lueur bleue émane des fissures.",
        "encounters": ["Squelettes gardiens x3"],
        "treasures": ["Cristal de Lune (artefact)"],
        "traps": ["Piège à rayons lunaires"]
      }
    ],
    "suggestedNpcs": [
      {
        "name": "Esprit de la Prêtresse",
        "role": "guide spectral",
        "interaction": "Donne des indices sur l'utilisation du cristal"
      }
    ]
  }
}
```

---

## 📊 Cas d'Utilisation 6 : Gestion des Permissions et Sécurité

### Contexte
Vérification des permissions et contrôles d'accès dans différents scénarios.

### Scénarios de Sécurité

#### 1. Tentative d'accès non autorisé à une campagne
```http
GET /campaign/42 HTTP/1.1
Authorization: Bearer {jwt_token_unauthorized_user}

Response: 403 Forbidden
{
  "error": "ACCESS_DENIED",
  "message": "Vous n'êtes ni le MJ ni un joueur de cette campagne",
  "allowedActions": [
    "Demander à rejoindre la campagne si elle est publique",
    "Contacter le MJ pour une invitation"
  ]
}
```

#### 2. Joueur essayant de modifier un PNJ (seul le MJ peut)
```http
PUT /npc/201 HTTP/1.1
Authorization: Bearer {jwt_token_player}
Content-Type: application/json

{
  "name": "Brom Modifié"
}

Response: 403 Forbidden
{
  "error": "INSUFFICIENT_PRIVILEGES", 
  "message": "Seul le MJ peut modifier les PNJ de la campagne",
  "yourRole": "player",
  "requiredRole": "gamemaster"
}
```

#### 3. Validation du GameType
```http
POST /character/dnd?userId=1 HTTP/1.1
X-GameType: skyrim
Authorization: Bearer {jwt_token}

Response: 400 Bad Request
{
  "error": "GAMETYPE_MISMATCH",
  "message": "Le header X-GameType 'skyrim' ne correspond pas à l'endpoint '/character/dnd'",
  "expectedGameType": "dnd",
  "receivedGameType": "skyrim",
  "suggestion": "Utilisez '/character' avec X-GameType: skyrim pour un personnage Skyrim"
}
```

---

## 🎯 Patterns d'Usage Recommandés

### 1. **Initialisation d'une nouvelle campagne**
1. Créer la campagne (`POST /campaign`)
2. Créer le premier chapitre (`POST /campaign/{id}/chapter`)
3. Ajouter les PNJ essentiels (`POST /chapter/{id}/npc`)
4. Inviter les joueurs
5. Commencer le jeu !

### 2. **Préparation d'un combat**
1. Visualiser le chapitre (`GET /chapter/{id}`)
2. Vérifier les stats des monstres
3. Lancer le combat (`POST /chapter/{id}/combat/start`)
4. Gérer les tours de jeu

### 3. **Gestion multi-campagnes**
1. Lister toutes les campagnes (`GET /campaign?userId={id}`)
2. Basculer entre les rôles selon le contexte
3. Utiliser les bons headers d'authentification

Ces cas d'utilisation montrent la flexibilité et la puissance de l'API pour gérer des scénarios JDR complexes tout en maintenant la sécurité et la cohérence des données.

---

*Retour au [README principal](./README.md)*