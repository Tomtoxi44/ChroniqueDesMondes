# Système de Sessions et Notifications - Chronique des Mondes

Ce document détaille le système complet de gestion des sessions de jeu, des notifications en temps réel et de la progression des campagnes.

## 🎮 Système de Sessions

### Vue d'ensemble des Sessions
Une **session** représente une séance de jeu active d'une campagne. Elle permet au MJ et aux joueurs de jouer ensemble en temps réel avec notifications et synchronisation.

### Cycle de Vie d'une Session

#### **1. Pré-Session : Préparation et Invitations**
- **Création de campagne** par un utilisateur (devient MJ)
- **Ajout de campagnes** publiques à sa liste de favoris
- **Invitations de joueurs** avant le lancement de la session
- **Acceptation des invitations** par les joueurs

#### **2. Lancement de Session**
- **Activation** de la session par le MJ
- **Notifications automatiques** à tous les joueurs invités
- **Transformation** du créateur en MJ actif
- **Synchronisation** de l'état du jeu pour tous les participants

#### **3. Session Active**
- **Progression par chapitres** avec sauvegarde automatique
- **Système de notifications** pour les actions importantes
- **Combat en temps réel** avec gestion des tours
- **Invitations dynamiques** en cours de session

#### **4. Fin de Session**
- **Sauvegarde** automatique de la progression
- **Mise à jour** du chapitre courant
- **Historique** des actions et événements

## 🚀 Lancement et Gestion de Sessions

### Workflow de Lancement

#### **1. Campagnes Disponibles pour un Utilisateur**
```http
GET /user/{userId}/campaigns/available HTTP/1.1
Authorization: Bearer {token}

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
      "invitedPlayers": [
        {
          "userId": 3,
          "userName": "Lisa",
          "status": "accepted",
          "characterId": 16
        },
        {
          "userId": 4,
          "userName": "Paul", 
          "status": "pending",
          "characterId": null
        }
      ],
      "canLaunchSession": true
    }
  ],
  "joinedCampaigns": [
    {
      "id": 58,
      "name": "Pirates des Mers du Sud",
      "gameType": "generic",
      "role": "player",
      "gameMaster": "Thomas",
      "myCharacterId": 45,
      "currentChapter": 2
    }
  ],
  "publicCampaigns": [
    {
      "id": 73,
      "name": "Exploration Spatiale",
      "gameType": "generic", 
      "gameMaster": "Sophie",
      "description": "Campagne sci-fi ouverte à tous",
      "playerCount": 2,
      "maxPlayers": 6,
      "canJoin": true
    }
  ]
}
```

#### **2. Lancement d'une Session**
```http
POST /campaign/{campaignId}/session/start HTTP/1.1
Authorization: Bearer {token}

{
  "sessionSettings": {
    "allowLateJoin": true,
    "notifyAbsentPlayers": true,
    "autoSaveInterval": 300,  // 5 minutes
    "maxSessionDuration": 14400  // 4 heures
  }
}

Response: 201 Created
{
  "sessionId": "sess_abc123",
  "campaignId": 42,
  "status": "active",
  "startedAt": "2024-01-20T19:00:00Z",
  "gameMasterId": 2,
  "currentChapter": {
    "id": 103,
    "number": 3,
    "title": "La Forêt Enchantée"
  },
  "participants": [
    {
      "userId": 3,
      "userName": "Lisa",
      "characterId": 16,
      "status": "online",
      "lastSeen": "2024-01-20T19:00:00Z"
    },
    {
      "userId": 4,
      "userName": "Paul",
      "characterId": 22,
      "status": "invited",
      "lastSeen": null
    }
  ],
  "notifications": {
    "sentToPlayers": 2,
    "deliveredInApp": 1,
    "deliveredByEmail": 1
  }
}
```

#### **3. Notifications Automatiques aux Joueurs**
```javascript
// Notification WebSocket en temps réel pour joueurs connectés
{
  "type": "SESSION_STARTED",
  "sessionId": "sess_abc123",
  "campaign": {
    "id": 42,
    "name": "Les Terres Oubliées",
    "gameMaster": "Thomas"
  },
  "message": "Thomas a lancé une session ! Rejoignez la partie.",
  "actions": [
    {
      "label": "Rejoindre",
      "endpoint": "/session/sess_abc123/join"
    },
    {
      "label": "Refuser",
      "endpoint": "/session/sess_abc123/decline"
    }
  ],
  "timestamp": "2024-01-20T19:00:00Z"
}

// Email automatique pour joueurs hors ligne
{
  "to": "paul@example.com",
  "subject": "Session JDR : Les Terres Oubliées - Maintenant !",
  "template": "session_invitation",
  "data": {
    "playerName": "Paul",
    "campaignName": "Les Terres Oubliées", 
    "gameMaster": "Thomas",
    "sessionUrl": "https://chronique-des-mondes.fr/session/sess_abc123",
    "startTime": "2024-01-20T19:00:00Z"
  }
}
```

## 📈 Système de Progression

### Sauvegarde et Progression par Chapitres

#### **1. Progression Automatique**
```http
PUT /session/{sessionId}/chapter/complete HTTP/1.1
Authorization: Bearer {token}

{
  "chapterId": 103,
  "completionStatus": "completed",
  "playerNotes": "Combat contre les loups terminé, artefact récupéré",
  "nextChapterId": 104
}

Response: 200 OK
{
  "campaignProgress": {
    "campaignId": 42,
    "currentChapter": 104,
    "previousChapter": 103,
    "completedChapters": [101, 102, 103],
    "totalChapters": 8,
    "progressPercentage": 37.5,  // 3/8 chapitres
    "progressBar": {
      "completed": "███",
      "remaining": "▓▓▓▓▓",
      "current": "→"
    }
  },
  "autoSaved": true,
  "lastSave": "2024-01-20T20:30:00Z",
  "sessionDuration": "01:30:00"
}
```

#### **2. Interface de Progression**
```http
GET /campaign/{campaignId}/progress HTTP/1.1
Authorization: Bearer {token}

Response: 200 OK
{
  "campaign": {
    "id": 42,
    "name": "Les Terres Oubliées",
    "currentChapter": 4,
    "totalChapters": 8
  },
  "progressDetails": {
    "overallProgress": 50.0,
    "chaptersCompleted": 4,
    "chaptersRemaining": 4,
    "estimatedTimeRemaining": "6-8 sessions",
    "lastSessionDate": "2024-01-20T19:00:00Z"
  },
  "chapterStatus": [
    {
      "chapterId": 101,
      "title": "L'Arrivée au Village",
      "status": "completed",
      "completedDate": "2024-01-10T21:00:00Z"
    },
    {
      "chapterId": 102,
      "title": "Les Caves Hantées", 
      "status": "completed",
      "completedDate": "2024-01-15T22:15:00Z"
    },
    {
      "chapterId": 103,
      "title": "La Forêt Enchantée",
      "status": "completed", 
      "completedDate": "2024-01-20T20:30:00Z"
    },
    {
      "chapterId": 104,
      "title": "Le Temple Perdu",
      "status": "current",
      "startedDate": "2024-01-20T20:30:00Z"
    },
    {
      "chapterId": 105,
      "title": "La Cité Souterraine",
      "status": "locked",
      "estimatedStart": null
    }
  ],
  "visualProgress": {
    "barSvg": "<svg>...</svg>",
    "textRepresentation": "████▓▓▓▓ 50%"
  }
}
```

## 👥 Système d'Invitations Avancé

### Invitations Pré-Session

#### **1. Invitation de Joueurs à une Campagne**
```http
POST /campaign/{campaignId}/invite HTTP/1.1
Authorization: Bearer {token}

{
  "invitees": [
    {
      "userEmail": "paul@example.com",
      "characterId": null,  // Le joueur choisira
      "role": "player",
      "message": "Rejoins-nous pour une aventure épique !"
    },
    {
      "userName": "Sophie",  // Utilisateur existant
      "characterId": 28,
      "role": "player",
      "message": "Ta voleuse serait parfaite pour cette campagne"
    }
  ],
  "sessionStartTime": "2024-01-25T19:00:00Z",  // Optionnel
  "campaignInfo": {
    "shortDescription": "Campagne D&D niveau 3-5",
    "expectedDuration": "8-10 sessions",
    "schedule": "Jeudis soirs 19h-22h"
  }
}

Response: 201 Created
{
  "invitationsSent": 2,
  "invitations": [
    {
      "invitationId": "inv_001",
      "recipientEmail": "paul@example.com",
      "status": "sent",
      "expiresAt": "2024-02-10T23:59:59Z"
    },
    {
      "invitationId": "inv_002", 
      "recipientUser": "Sophie",
      "status": "pending",
      "expiresAt": "2024-02-10T23:59:59Z"
    }
  ]
}
```

#### **2. Réponse à une Invitation**
```http
PUT /invitation/{invitationId}/respond HTTP/1.1
Authorization: Bearer {token}

{
  "response": "accepted",
  "characterId": 22,  // Personnage choisi
  "playerMessage": "Hâte de commencer l'aventure !",
  "availability": {
    "preferredDays": ["thursday", "friday"],
    "preferredHours": "19:00-22:00"
  }
}

Response: 200 OK
{
  "invitationStatus": "accepted",
  "addedToCampaign": true,
  "campaignInfo": {
    "id": 42,
    "name": "Les Terres Oubliées",
    "currentPlayers": 3,
    "nextSession": "2024-01-25T19:00:00Z"
  },
  "characterValidation": {
    "characterId": 22,
    "name": "Gareth l'Épéiste",
    "gameTypeCompatible": true,
    "levelAppropriate": true
  },
  "gmNotified": true
}
```

### Notifications Multi-Canal

#### **1. Notification WebSocket (Temps Réel)**
```javascript
// Pour joueurs connectés
{
  "type": "CAMPAIGN_INVITATION",
  "invitationId": "inv_002",
  "campaign": {
    "id": 42,
    "name": "Les Terres Oubliées",
    "gameType": "dnd",
    "gameMaster": "Thomas"
  },
  "message": "Thomas vous invite à rejoindre sa campagne D&D",
  "urgency": "normal",
  "actions": [
    {
      "label": "Accepter",
      "style": "primary",
      "endpoint": "/invitation/inv_002/respond"
    },
    {
      "label": "Refuser",
      "style": "secondary", 
      "endpoint": "/invitation/inv_002/respond"
    },
    {
      "label": "Voir détails",
      "style": "info",
      "endpoint": "/campaign/42/preview"
    }
  ]
}
```

#### **2. Notification Email (Joueurs Hors Ligne)**
```html
<!DOCTYPE html>
<html>
<head>
    <title>Invitation Campagne JDR</title>
</head>
<body>
    <h1>🎲 Invitation à une Campagne JDR</h1>
    <p>Salut Sophie,</p>
    <p><strong>Thomas</strong> t'invite à rejoindre sa campagne :</p>
    
    <div style="border:1px solid #ccc; padding:20px; margin:20px 0;">
        <h2>📖 Les Terres Oubliées</h2>
        <p><strong>Type :</strong> Dungeons & Dragons</p>
        <p><strong>Description :</strong> Campagne D&D niveau 3-5</p>
        <p><strong>Prochaine session :</strong> Jeudi 25 janvier à 19h00</p>
        <p><strong>Fréquence :</strong> Jeudis soirs 19h-22h</p>
    </div>
    
    <p><em>"Ta voleuse serait parfaite pour cette campagne"</em></p>
    
    <div style="text-align:center; margin:30px 0;">
        <a href="https://chronique-des-mondes.fr/invitation/inv_002" 
           style="background:#4CAF50; color:white; padding:15px 30px; text-decoration:none; border-radius:5px;">
           Répondre à l'invitation
        </a>
    </div>
    
    <p>Cette invitation expire le 10 février 2024.</p>
</body>
</html>
```

## ⚔️ Système de Combat en Temps Réel

### Invitations Dynamiques en Combat

#### **1. Ajout de Joueur en Cours de Combat**
```http
POST /session/{sessionId}/combat/{combatId}/invite-player HTTP/1.1
Authorization: Bearer {token}

{
  "invitedUserId": 5,
  "characterId": 30,
  "invitationMessage": "Nous avons besoin de renfort ! Rejoins le combat !",
  "rollInitiative": true
}

Response: 200 OK
{
  "invitationSent": true,
  "combatStatus": {
    "combatId": "combat_789",
    "currentTurn": 3,
    "totalParticipants": 5,  // 4 existants + 1 invité
    "awaitingInitiative": [
      {
        "userId": 5,
        "characterName": "Zara l'Assassin",
        "status": "rolling_initiative"
      }
    ]
  },
  "notificationDelivered": {
    "websocket": true,
    "email": false,  // Joueur connecté
    "urgency": "high"
  }
}
```

#### **2. Réponse à l'Invitation de Combat**
```http
PUT /session/{sessionId}/combat/{combatId}/join HTTP/1.1
Authorization: Bearer {token}

{
  "response": "accepted",
  "characterId": 30,
  "initiativeRoll": 16  // 1d20 + modificateur
}

Response: 200 OK
{
  "joinedCombat": true,
  "combatUpdated": {
    "newTurnOrder": [
      { "initiative": 18, "name": "Lisa", "type": "player", "isCurrent": false },
      { "initiative": 16, "name": "Zara", "type": "player", "isCurrent": false },
      { "initiative": 15, "name": "Gobelin 1", "type": "npc", "isCurrent": true },
      { "initiative": 12, "name": "Thomas", "type": "player", "isCurrent": false }
    ],
    "currentTurn": {
      "participantName": "Gobelin 1",
      "type": "npc",
      "initiative": 15
    }
  },
  "allPlayersNotified": true
}
```

### Notifications de Tour de Jeu

#### **1. Notification de Tour Actif**
```javascript
// WebSocket pour le joueur dont c'est le tour
{
  "type": "YOUR_TURN",
  "combatId": "combat_789",
  "participant": {
    "characterId": 16,
    "characterName": "Lyralei",
    "initiative": 18
  },
  "turnInfo": {
    "turnNumber": 4,
    "timeLimit": 120,  // 2 minutes pour jouer
    "availableActions": ["attack", "cast_spell", "move", "dodge", "help"]
  },
  "combatContext": {
    "enemiesInRange": [
      {
        "name": "Gobelin 1",
        "distance": "melee",
        "status": "wounded"
      }
    ],
    "alliesVisible": [
      {
        "name": "Thomas",
        "distance": "close",
        "status": "healthy"
      }
    ]
  },
  "uiUpdates": {
    "highlightCharacter": true,
    "showActionPanel": true,
    "playSound": "turn_start.mp3"
  }
}
```

#### **2. Interface Visuelle de Tour**
```css
/* Style CSS pour indiquer le tour actif */
.character-frame {
  border: 2px solid #ccc;
  transition: all 0.3s ease;
}

.character-frame.active-turn {
  border: 3px solid #4CAF50;
  box-shadow: 0 0 15px rgba(76, 175, 80, 0.5);
  animation: pulse 2s infinite;
}

.character-frame.waiting-turn {
  border: 2px solid #ffc107;
  opacity: 0.8;
}

@keyframes pulse {
  0% { box-shadow: 0 0 15px rgba(76, 175, 80, 0.5); }
  50% { box-shadow: 0 0 25px rgba(76, 175, 80, 0.8); }
  100% { box-shadow: 0 0 15px rgba(76, 175, 80, 0.5); }
}
```

#### **3. Pop-up de Notification**
```javascript
// Notification pop-up discrète
{
  "type": "TURN_NOTIFICATION",
  "style": "toast",
  "position": "top-right",
  "duration": 5000,
  "content": {
    "title": "🎯 À votre tour !",
    "message": "C'est à Lyralei de jouer",
    "actions": [
      {
        "label": "Voir options",
        "action": "show_combat_panel"
      }
    ]
  },
  "autoHide": false,
  "priority": "high"
}
```

## 🔄 Système de Sauvegarde Automatique

### Sauvegarde Continue

#### **1. Configuration de Sauvegarde**
```http
PUT /session/{sessionId}/settings/autosave HTTP/1.1
Authorization: Bearer {token}

{
  "enabled": true,
  "intervalSeconds": 300,  // Toutes les 5 minutes
  "saveOnChapterChange": true,
  "saveOnCombatEnd": true,
  "saveOnPlayerDisconnect": true,
  "maxSaveSlots": 10
}

Response: 200 OK
{
  "autosaveEnabled": true,
  "nextSave": "2024-01-20T19:05:00Z",
  "lastSave": "2024-01-20T19:00:00Z",
  "saveSlots": 3  // Utilisés sur 10
}
```

#### **2. État de Sauvegarde**
```http
GET /session/{sessionId}/save-status HTTP/1.1
Authorization: Bearer {token}

Response: 200 OK
{
  "lastSave": {
    "timestamp": "2024-01-20T20:30:00Z",
    "chapterId": 104,
    "saveSlot": 3,
    "playerStates": [
      {
        "userId": 3,
        "characterId": 16,
        "hitPoints": 15,
        "spellSlots": {"1": 2, "2": 1},
        "inventory": [
          {"equipmentId": 89, "quantity": 3}
        ]
      }
    ],
    "combatState": null,  // Pas en combat
    "chapterProgress": "in_progress"
  },
  "saveHistory": [
    {
      "slot": 1,
      "timestamp": "2024-01-20T19:00:00Z",
      "description": "Début session - Chapitre 3"
    },
    {
      "slot": 2,
      "timestamp": "2024-01-20T19:45:00Z", 
      "description": "Après combat gobelins"
    },
    {
      "slot": 3,
      "timestamp": "2024-01-20T20:30:00Z",
      "description": "Fin Chapitre 3 - Début Chapitre 4"
    }
  ]
}
```

## 📧 Système de Réinitialisation de Mot de Passe

### Workflow de Reset Password

#### **1. Demande de Réinitialisation**
```http
POST /auth/password/reset-request HTTP/1.1
Content-Type: application/json

{
  "email": "user@example.com"
}

Response: 200 OK
{
  "resetRequested": true,
  "message": "Un email de réinitialisation a été envoyé",
  "emailSent": true,
  "tokenExpiry": "2024-01-20T22:00:00Z"  // 2 heures
}
```

#### **2. Email de Réinitialisation**
```html
<!DOCTYPE html>
<html>
<head>
    <title>Réinitialisation Mot de Passe</title>
</head>
<body>
    <h1>🔐 Réinitialisation de Mot de Passe</h1>
    <p>Bonjour,</p>
    <p>Une demande de réinitialisation de mot de passe a été effectuée pour votre compte <strong>Chronique des Mondes</strong>.</p>
    
    <div style="text-align:center; margin:30px 0;">
        <a href="https://chronique-des-mondes.fr/reset-password?token=abc123def456" 
           style="background:#2196F3; color:white; padding:15px 30px; text-decoration:none; border-radius:5px;">
           Réinitialiser mon mot de passe
        </a>
    </div>
    
    <p><strong>Ce lien expire dans 2 heures.</strong></p>
    <p>Si vous n'avez pas demandé cette réinitialisation, ignorez ce message.</p>
</body>
</html>
```

#### **3. Confirmation du Nouveau Mot de Passe**
```http
POST /auth/password/reset-confirm HTTP/1.1
Content-Type: application/json

{
  "resetToken": "abc123def456",
  "newPassword": "nouveauMotDePasse123!",
  "confirmPassword": "nouveauMotDePasse123!"
}

Response: 200 OK
{
  "passwordReset": true,
  "message": "Mot de passe mis à jour avec succès",
  "autoLogin": true,
  "newToken": "jwt_token_new_session"
}
```

---

*Ce système de sessions et notifications transforme l'expérience JDR en permettant une coordination fluide entre MJ et joueurs. Retour au [README principal](../README.md)*