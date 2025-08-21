# Cas d'Utilisation Sessions et Combat Temps Réel

Ce document présente des scénarios détaillés pour le nouveau système de sessions de jeu avec notifications temps réel et combat synchronisé.

## 🎮 Cas d'Utilisation 1 : Lancement de Session Complète

### Contexte
Thomas a créé une campagne D&D "Les Terres Oubliées" et veut lancer une session avec ses amis. Lisa et Paul ont déjà accepté de rejoindre, Sophie est invitée mais pas encore répondu.

### Workflow Complet de Session

#### 1. Thomas consulte ses campagnes disponibles
```http
GET /user/2/campaigns/available HTTP/1.1
Authorization: Bearer {jwt_token_thomas}

Response: 200 OK
{
  "ownedCampaigns": [
    {
      "id": 42,
      "name": "Les Terres Oubliées",
      "gameType": "dnd",
      "role": "creator",
      "currentChapter": 3,
      "totalChapters": 8,
      "progressPercentage": 37.5,
      "invitedPlayers": [
        {
          "userId": 3,
          "userName": "Lisa",
          "status": "accepted",
          "characterId": 16,
          "characterName": "Lyralei l'Archimage",
          "lastSeen": "2024-01-20T18:45:00Z"
        },
        {
          "userId": 4,
          "userName": "Paul",
          "status": "accepted", 
          "characterId": 22,
          "characterName": "Gareth l'Épéiste",
          "lastSeen": "2024-01-20T19:00:00Z"
        },
        {
          "userId": 5,
          "userName": "Sophie",
          "status": "pending",
          "characterId": null,
          "invitedAt": "2024-01-18T10:00:00Z"
        }
      ],
      "canLaunchSession": true,
      "estimatedSessionTime": "2-3 heures"
    }
  ],
  "joinedCampaigns": [
    {
      "id": 58,
      "name": "Pirates des Mers du Sud",
      "gameType": "generic",
      "role": "player",
      "gameMaster": "Marie",
      "myCharacterId": 35,
      "nextSession": "2024-01-22T20:00:00Z"
    }
  ]
}
```

#### 2. Thomas lance la session
```http
POST /campaign/42/session/start HTTP/1.1
Authorization: Bearer {jwt_token_thomas}
Content-Type: application/json

{
  "sessionSettings": {
    "allowLateJoin": true,
    "notifyAbsentPlayers": true,
    "autoSaveInterval": 300,
    "maxSessionDuration": 10800,  // 3 heures
    "enableCombatNotifications": true,
    "turnTimeLimit": 120  // 2 minutes par tour
  },
  "welcomeMessage": "Prêts pour explorer le Temple Perdu ? 🏛️"
}

Response: 201 Created
{
  "sessionId": "sess_abc123",
  "campaignId": 42,
  "status": "active",
  "startedAt": "2024-01-20T19:30:00Z",
  "gameMasterId": 2,
  "currentChapter": {
    "id": 104,
    "number": 4,
    "title": "Le Temple Perdu",
    "description": "Les aventuriers découvrent un ancien temple caché dans la forêt"
  },
  "participants": [
    {
      "userId": 3,
      "userName": "Lisa",
      "characterId": 16,
      "status": "invited",
      "notificationSent": true
    },
    {
      "userId": 4,
      "userName": "Paul", 
      "characterId": 22,
      "status": "invited",
      "notificationSent": true
    },
    {
      "userId": 5,
      "userName": "Sophie",
      "characterId": null,
      "status": "campaign_pending",
      "notificationSent": false
    }
  ],
  "notifications": {
    "websocketSent": 2,  // Lisa et Paul connectés
    "emailSent": 0,      // Tous connectés
    "pendingInvitations": 1  // Sophie
  }
}
```

#### 3. Notifications automatiques envoyées

**WebSocket pour Lisa (connectée) :**
```javascript
{
  "type": "SESSION_STARTED",
  "sessionId": "sess_abc123",
  "campaign": {
    "id": 42,
    "name": "Les Terres Oubliées",
    "gameMaster": "Thomas"
  },
  "currentChapter": {
    "title": "Le Temple Perdu",
    "description": "Les aventuriers découvrent un ancien temple caché dans la forêt"
  },
  "message": "🎲 Thomas a lancé une session ! Rejoignez la partie.",
  "welcomeMessage": "Prêts pour explorer le Temple Perdu ? 🏛️",
  "urgency": "high",
  "actions": [
    {
      "label": "Rejoindre Maintenant",
      "style": "primary",
      "endpoint": "/session/sess_abc123/join"
    },
    {
      "label": "Rejoindre Plus Tard",
      "style": "secondary",
      "endpoint": "/session/sess_abc123/join-later"
    }
  ],
  "autoJoin": false,
  "playSound": "session_start.mp3"
}
```

**Email pour Paul (déconnecté) :**
```html
<!DOCTYPE html>
<html>
<head>
    <title>Session JDR Maintenant - Les Terres Oubliées</title>
</head>
<body>
    <h1>🎲 Session en Cours !</h1>
    <p>Salut Paul,</p>
    <p><strong>Thomas</strong> vient de lancer une session de :</p>
    
    <div style="border:2px solid #4CAF50; padding:20px; margin:20px 0; background:#f9f9f9;">
        <h2>📖 Les Terres Oubliées</h2>
        <p><strong>Chapitre :</strong> Le Temple Perdu</p>
        <p><strong>Démarrée :</strong> 20 janvier 2024 à 19h30</p>
        <p><em>"Prêts pour explorer le Temple Perdu ? 🏛️"</em></p>
    </div>
    
    <div style="text-align:center; margin:30px 0;">
        <a href="https://chronique-des-mondes.fr/session/sess_abc123" 
           style="background:#4CAF50; color:white; padding:15px 30px; text-decoration:none; border-radius:5px; font-size:18px;">
           🚀 Rejoindre la Session
        </a>
    </div>
    
    <p>Votre personnage : <strong>Gareth l'Épéiste</strong></p>
    <p>Session estimée : 2-3 heures</p>
</body>
</html>
```

#### 4. Lisa rejoint la session
```http
PUT /session/sess_abc123/join HTTP/1.1
Authorization: Bearer {jwt_token_lisa}
Content-Type: application/json

{
  "characterId": 16,
  "joinMessage": "Lyralei est prête pour l'aventure !",
  "deviceInfo": {
    "type": "desktop",
    "browser": "Chrome",
    "hasAudio": true
  }
}

Response: 200 OK
{
  "sessionJoined": true,
  "welcomeBack": {
    "lastSession": "2024-01-15T20:30:00Z",
    "chaptersCompletedSince": 1,
    "newEquipmentReceived": [
      {
        "name": "Potion de Soins Supérieure",
        "source": "reward",
        "addedAt": "2024-01-15T21:00:00Z"
      }
    ]
  },
  "currentGameState": {
    "chapter": "Le Temple Perdu",
    "location": "Entrée du temple",
    "partyStatus": {
      "hitPoints": {"Lyralei": 18, "Unknown": "Waiting for other players"},
      "resources": {"spellSlots": {"1": 3, "2": 2}}
    }
  },
  "otherParticipants": [
    {
      "userName": "Paul",
      "status": "invited",
      "expectedJoin": "Notifié par email"
    },
    {
      "userName": "Thomas",
      "status": "gm",
      "role": "gamemaster"
    }
  ]
}
```

## 🔥 Cas d'Utilisation 2 : Combat avec Invitations Dynamiques

### Contexte
La session est en cours. Thomas (MJ), Lisa et Paul explorent le temple. Ils entrent en combat contre des gardiens squelettes. En cours de combat, Thomas veut inviter Sophie qui vient de se connecter.

### Workflow Combat Temps Réel

#### 1. Déclenchement du combat
```http
POST /session/sess_abc123/combat/start HTTP/1.1
Authorization: Bearer {jwt_token_thomas}
Content-Type: application/json

{
  "chapterId": 104,
  "combatName": "Gardiens du Temple",
  "participants": [
    {
      "type": "player",
      "characterId": 16,  // Lisa - Lyralei
      "userId": 3
    },
    {
      "type": "player",
      "characterId": 22,  // Paul - Gareth  
      "userId": 4
    },
    {
      "type": "npc",
      "npcId": 405,       // Squelettes gardiens
      "quantity": 3,
      "names": ["Gardien Alpha", "Gardien Beta", "Gardien Gamma"]
    }
  ],
  "environment": {
    "lighting": "dim",
    "terrain": "stone_floor",
    "specialFeatures": ["ancient_runes", "crumbling_pillars"]
  }
}

Response: 201 Created
{
  "combatId": "combat_789",
  "status": "initiative_phase",
  "turnOrder": [
    {
      "participantId": 1,
      "name": "Lyralei l'Archimage",
      "type": "player",
      "userId": 3,
      "initiative": 17,
      "hitPoints": 18,
      "armorClass": 12
    },
    {
      "participantId": 2,
      "name": "Gardien Alpha",
      "type": "npc",
      "npcId": 405,
      "initiative": 14,
      "hitPoints": 13,
      "armorClass": 13
    },
    {
      "participantId": 3,
      "name": "Gareth l'Épéiste",
      "type": "player", 
      "userId": 4,
      "initiative": 12,
      "hitPoints": 24,
      "armorClass": 18
    },
    {
      "participantId": 4,
      "name": "Gardien Beta",
      "type": "npc",
      "initiative": 10
    },
    {
      "participantId": 5,
      "name": "Gardien Gamma", 
      "type": "npc",
      "initiative": 8
    }
  ],
  "currentTurn": {
    "participantId": 1,
    "name": "Lyralei l'Archimage",
    "type": "player",
    "userId": 3,
    "timeLimit": 120
  },
  "allPlayersNotified": true
}
```

#### 2. Notification "À votre tour" pour Lisa
```javascript
// WebSocket notification
{
  "type": "YOUR_TURN",
  "combatId": "combat_789", 
  "participant": {
    "characterId": 16,
    "characterName": "Lyralei l'Archimage",
    "initiative": 17
  },
  "turnInfo": {
    "turnNumber": 1,
    "roundNumber": 1,
    "timeLimit": 120,
    "startTime": "2024-01-20T20:15:00Z"
  },
  "combatContext": {
    "enemiesInRange": [
      {
        "name": "Gardien Alpha",
        "distance": "30 feet",
        "status": "healthy",
        "vulnerabilities": ["bludgeoning"]
      }
    ],
    "alliesVisible": [
      {
        "name": "Gareth l'Épéiste", 
        "distance": "15 feet",
        "status": "healthy",
        "canSupport": true
      }
    ],
    "environment": {
      "cover": "partial (pillar)",
      "lighting": "dim (-2 to perception)"
    }
  },
  "availableActions": [
    {
      "type": "spell",
      "name": "Boule de Feu",
      "canTarget": ["Gardien Alpha", "Gardien Beta"],
      "spellSlotRequired": 3
    },
    {
      "type": "spell", 
      "name": "Projectile Magique",
      "canTarget": ["Gardien Alpha"],
      "autoHit": true
    },
    {
      "type": "movement",
      "maxDistance": "30 feet",
      "safePositions": ["behind_pillar", "near_ally"]
    }
  ],
  "uiUpdates": {
    "highlightCharacter": true,
    "showActionPanel": true,
    "borderColor": "#4CAF50",
    "playSound": "your_turn.mp3",
    "showTimer": true
  }
}
```

#### 3. Sophie se connecte et Thomas l'invite au combat
```http
POST /session/sess_abc123/combat/combat_789/invite-player HTTP/1.1
Authorization: Bearer {jwt_token_thomas}
Content-Type: application/json

{
  "invitedUserId": 5,
  "characterId": 28,  // Zara l'Assassin
  "invitationMessage": "🆘 Nous avons besoin de renfort ! Les gardiens sont coriaces !",
  "rollInitiative": true,
  "entryPoint": "temple_entrance",
  "urgency": "high"
}

Response: 200 OK
{
  "invitationSent": true,
  "invitee": {
    "userId": 5,
    "userName": "Sophie", 
    "characterName": "Zara l'Assassin"
  },
  "combatStatus": {
    "combatId": "combat_789",
    "currentTurn": 1,
    "currentParticipant": "Lyralei l'Archimage",
    "totalParticipants": 5,
    "awaitingResponse": true
  },
  "notificationDelivered": {
    "websocket": true,
    "email": false,
    "urgency": "high",
    "deliveryTime": "2024-01-20T20:16:00Z"
  }
}
```

#### 4. Notification d'invitation au combat pour Sophie
```javascript
{
  "type": "COMBAT_INVITATION",
  "combatId": "combat_789",
  "sessionId": "sess_abc123", 
  "inviter": {
    "userName": "Thomas",
    "role": "gamemaster"
  },
  "combatInfo": {
    "name": "Gardiens du Temple",
    "location": "Temple Perdu - Salle Principale",
    "currentRound": 1,
    "participants": ["Lyralei", "Gareth", "3 Gardiens Squelettes"],
    "dangerLevel": "medium",
    "estimatedDuration": "15-20 minutes"
  },
  "character": {
    "id": 28,
    "name": "Zara l'Assassin",
    "readyForCombat": true
  },
  "message": "🆘 Nous avons besoin de renfort ! Les gardiens sont coriaces !",
  "urgency": "high",
  "actions": [
    {
      "label": "Rejoindre le Combat",
      "style": "danger",
      "endpoint": "/session/sess_abc123/combat/combat_789/join"
    },
    {
      "label": "Refuser",
      "style": "secondary",
      "endpoint": "/session/sess_abc123/combat/combat_789/decline"
    }
  ],
  "autoJoinTimer": 30,  // 30 secondes avant refus automatique
  "combatSnapshot": {
    "allies": [
      {"name": "Lyralei", "status": "healthy", "turn": "current"},
      {"name": "Gareth", "status": "healthy", "turn": "waiting"}
    ],
    "enemies": [
      {"name": "Gardiens", "count": 3, "status": "healthy"}
    ]
  }
}
```

#### 5. Sophie accepte et rejoint le combat
```http
PUT /session/sess_abc123/combat/combat_789/join HTTP/1.1
Authorization: Bearer {jwt_token_sophie}
Content-Type: application/json

{
  "response": "accepted",
  "characterId": 28,
  "initiativeRoll": 15,  // 1d20 + 3 (Dex) = 15
  "entryMessage": "Zara surgit des ombres, prête à frapper !",
  "preferredPosition": "flanking"
}

Response: 200 OK
{
  "joinedCombat": true,
  "combatUpdated": {
    "newTurnOrder": [
      { "initiative": 17, "name": "Lyralei", "type": "player", "isCurrent": true },
      { "initiative": 15, "name": "Zara", "type": "player", "isNew": true },
      { "initiative": 14, "name": "Gardien Alpha", "type": "npc" },
      { "initiative": 12, "name": "Gareth", "type": "player" },
      { "initiative": 10, "name": "Gardien Beta", "type": "npc" },
      { "initiative": 8, "name": "Gardien Gamma", "type": "npc" }
    ],
    "currentTurn": {
      "participantName": "Lyralei l'Archimage",
      "stillActive": true,
      "timeRemaining": 87  // seconds
    },
    "newParticipant": {
      "name": "Zara l'Assassin",
      "position": "flanking_east",
      "nextTurn": 2,
      "specialAbilities": ["Sneak Attack", "Cunning Action"]
    }
  },
  "allPlayersNotified": true,
  "combatLog": [
    {
      "timestamp": "2024-01-20T20:17:00Z",
      "message": "Zara l'Assassin rejoint le combat ! Initiative: 15",
      "type": "player_joined"
    }
  ]
}
```

#### 6. Notification visuelle pour tous les participants
```javascript
// Notification pour tous les joueurs
{
  "type": "PLAYER_JOINED_COMBAT",
  "combatId": "combat_789",
  "newPlayer": {
    "userName": "Sophie",
    "characterName": "Zara l'Assassin",
    "initiative": 15
  },
  "message": "🎭 Sophie a rejoint le combat avec Zara l'Assassin !",
  "combatUpdate": {
    "newTurnOrder": "Voir ordre mis à jour",
    "currentTurnStillActive": true,
    "nextAvailableTurn": 2
  },
  "uiUpdates": {
    "addCharacterFrame": {
      "characterId": 28,
      "position": "flanking_east",
      "borderColor": "#9C27B0",  // Violet pour nouvel arrivant
      "animation": "slide_in_from_right"
    },
    "updateInitiativeOrder": true,
    "showJoinMessage": {
      "text": "Zara surgit des ombres, prête à frapper !",
      "duration": 3000
    }
  }
}
```

## 📊 Cas d'Utilisation 3 : Progression et Sauvegarde Automatique

### Contexte
Le combat est terminé. L'équipe a vaincu les gardiens et termine le chapitre 4 "Le Temple Perdu". Le système doit sauvegarder automatiquement et faire progresser vers le chapitre 5.

### Workflow de Progression

#### 1. Fin du combat et fin de chapitre
```http
PUT /session/sess_abc123/chapter/complete HTTP/1.1
Authorization: Bearer {jwt_token_thomas}
Content-Type: application/json

{
  "chapterId": 104,
  "completionStatus": "completed",
  "gmNotes": "Combat excellent ! Les joueurs ont utilisé la tactique et le terrain intelligemment.",
  "playerRewards": [
    {
      "type": "experience",
      "amount": 300,
      "description": "Défaite des gardiens du temple"
    },
    {
      "type": "treasure",
      "items": [
        {
          "equipmentId": 156,
          "name": "Amulette Ancienne",
          "assignedTo": 16  // Lisa
        },
        {
          "equipmentId": 89,
          "name": "Potion de Soins",
          "quantity": 3,
          "assignedTo": "party"  // À répartir
        }
      ]
    }
  ],
  "nextChapterId": 105,
  "chapterSummary": "L'équipe a découvert l'amulette cachée et déverrouillé l'accès aux tunnels souterrains."
}

Response: 200 OK
{
  "chapterCompleted": true,
  "campaignProgress": {
    "campaignId": 42,
    "previousChapter": 104,
    "currentChapter": 105,
    "completedChapters": [101, 102, 103, 104],
    "totalChapters": 8,
    "progressPercentage": 50.0,  // 4/8 chapitres
    "progressVisualization": {
      "completed": "████",
      "current": "▶",
      "remaining": "▓▓▓",
      "textBar": "████▶▓▓▓ 50%"
    }
  },
  "autoSaveTriggered": {
    "saveSlot": 4,
    "saveTime": "2024-01-20T21:45:00Z",
    "description": "Fin Chapitre 4 - Temple Perdu Complété"
  },
  "rewardsDistributed": {
    "experienceGained": 300,
    "itemsAdded": [
      {
        "characterId": 16,
        "items": ["Amulette Ancienne"]
      }
    ],
    "partyTreasure": ["3x Potion de Soins"]
  },
  "nextChapterPreview": {
    "id": 105,
    "title": "Les Tunnels Souterrains",
    "description": "Un réseau de tunnels s'étend sous le temple...",
    "estimatedDuration": "1-2 sessions",
    "recommendedLevel": 4
  },
  "sessionStats": {
    "duration": "02:15:00",
    "combatsCompleted": 1,
    "experienceGained": 300,
    "treasureFound": 4
  }
}
```

#### 2. Visualisation de progression pour les joueurs
```http
GET /campaign/42/progress HTTP/1.1
Authorization: Bearer {jwt_token_lisa}

Response: 200 OK
{
  "campaign": {
    "id": 42,
    "name": "Les Terres Oubliées",
    "currentChapter": 5,
    "totalChapters": 8
  },
  "progressDetails": {
    "overallProgress": 50.0,
    "chaptersCompleted": 4,
    "chaptersRemaining": 4,
    "estimatedTimeRemaining": "4-6 sessions",
    "lastSessionDate": "2024-01-20T19:30:00Z",
    "averageSessionDuration": "2h 30min"
  },
  "recentAchievements": [
    {
      "type": "chapter_completed",
      "title": "Temple Perdu Exploré",
      "description": "Première exploration complète d'un donjon ancien",
      "unlockedAt": "2024-01-20T21:45:00Z"
    },
    {
      "type": "treasure_found", 
      "title": "Amulette Ancienne",
      "description": "Artefact mystérieux aux pouvoirs inconnus",
      "unlockedAt": "2024-01-20T21:40:00Z"
    }
  ],
  "chapterStatus": [
    {
      "chapterId": 101,
      "title": "L'Arrivée au Village",
      "status": "completed",
      "completedDate": "2024-01-10T21:00:00Z",
      "duration": "2h 00min"
    },
    {
      "chapterId": 102,
      "title": "Les Caves Hantées",
      "status": "completed", 
      "completedDate": "2024-01-15T22:15:00Z",
      "duration": "2h 45min"
    },
    {
      "chapterId": 103,
      "title": "La Forêt Enchantée",
      "status": "completed",
      "completedDate": "2024-01-18T20:30:00Z", 
      "duration": "2h 00min"
    },
    {
      "chapterId": 104,
      "title": "Le Temple Perdu",
      "status": "completed",
      "completedDate": "2024-01-20T21:45:00Z",
      "duration": "2h 15min",
      "highlights": ["Combat épique", "Amulette découverte"]
    },
    {
      "chapterId": 105,
      "title": "Les Tunnels Souterrains", 
      "status": "current",
      "startedDate": "2024-01-20T21:45:00Z",
      "estimatedDuration": "2-3h"
    },
    {
      "chapterId": 106,
      "title": "La Cité Souterraine",
      "status": "locked",
      "prerequisites": ["Chapter 105 completed"]
    }
  ],
  "partyOverview": {
    "averageLevel": 3.7,
    "totalExperience": 2850,
    "strongestMember": "Gareth l'Épéiste",
    "magicalPower": "Lyralei l'Archimage",
    "stealthExpert": "Zara l'Assassin"
  }
}
```

#### 3. Sauvegarde automatique détaillée
```http
GET /session/sess_abc123/save-status HTTP/1.1
Authorization: Bearer {jwt_token_thomas}

Response: 200 OK
{
  "currentSave": {
    "slot": 4,
    "timestamp": "2024-01-20T21:45:00Z",
    "chapterId": 105,
    "description": "Fin Chapitre 4 - Temple Perdu Complété",
    "gameState": {
      "campaign": {
        "currentChapter": 105,
        "completedChapters": [101, 102, 103, 104]
      },
      "party": {
        "location": "Entrance to Underground Tunnels",
        "inventoryShared": ["3x Potion de Soins"],
        "partyFunds": 250  // Gold pieces
      },
      "characters": [
        {
          "characterId": 16,
          "name": "Lyralei l'Archimage",
          "currentHitPoints": 18,
          "maxHitPoints": 18,
          "experience": 1100,
          "level": 3,
          "spellSlots": {"1": 4, "2": 2},
          "inventory": [
            {"equipmentId": 156, "name": "Amulette Ancienne", "quantity": 1},
            {"equipmentId": 89, "name": "Potion de Soins", "quantity": 2}
          ]
        },
        {
          "characterId": 22,
          "name": "Gareth l'Épéiste", 
          "currentHitPoints": 24,
          "maxHitPoints": 24,
          "experience": 950,
          "level": 3
        },
        {
          "characterId": 28,
          "name": "Zara l'Assassin",
          "currentHitPoints": 16,
          "maxHitPoints": 16,
          "experience": 850,
          "level": 3,
          "sneakAttackDice": "2d6"
        }
      ]
    }
  },
  "saveHistory": [
    {
      "slot": 1,
      "timestamp": "2024-01-20T19:30:00Z",
      "description": "Début session - Chapitre 4"
    },
    {
      "slot": 2, 
      "timestamp": "2024-01-20T20:00:00Z",
      "description": "Auto-save - Exploration temple"
    },
    {
      "slot": 3,
      "timestamp": "2024-01-20T20:15:00Z",
      "description": "Début combat gardiens"
    },
    {
      "slot": 4,
      "timestamp": "2024-01-20T21:45:00Z",
      "description": "Fin Chapitre 4 - Temple Perdu Complété"
    }
  ],
  "autoSaveSettings": {
    "enabled": true,
    "intervalSeconds": 300,
    "nextAutoSave": "2024-01-20T21:50:00Z",
    "maxSlots": 10,
    "usedSlots": 4
  },
  "backupInfo": {
    "cloudBackup": true,
    "lastCloudSync": "2024-01-20T21:45:00Z",
    "localBackup": true
  }
}
```

Ces cas d'utilisation montrent comment le système de sessions transforme complètement l'expérience JDR en permettant :

- **Coordination fluide** entre MJ et joueurs
- **Notifications contextuelles** en temps réel
- **Combat synchronisé** avec invitations dynamiques
- **Progression automatique** avec sauvegarde intelligente
- **Interface visuelle** riche et réactive

---

*Retour aux [Sessions et Notifications](./SessionsAndNotifications.md)*