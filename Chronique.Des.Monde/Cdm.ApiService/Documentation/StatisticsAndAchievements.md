# Système de Statistiques et Succès - Chronique des Mondes

Ce document détaille le système complet de statistiques de joueur, d'analyse de performance et de succès/achievements pour enrichir l'expérience JDR.

## 📊 Vue d'ensemble du Système

### Philosophie des Statistiques
Le système de statistiques transforme chaque action de jeu en données mesurables et exploitables, permettant aux joueurs de :
- **Analyser leurs performances** : Comprendre leurs forces et faiblesses
- **Suivre leur progression** : Visualiser l'évolution de leurs personnages
- **Découvrir des patterns** : Identifier leurs habitudes de jeu
- **Débloquer des succès** : Être récompensé pour leurs accomplissements

### Types de Données Collectées
- **Sessions de jeu** : Fréquence, durée, participation
- **Actions de combat** : Attaques, dégâts, sorts lancés
- **Jets de dés** : Résultats, moyennes, chance
- **Progression** : Expérience, niveaux, équipements
- **Interactions sociales** : Échanges, invitations, collaborations

## 🎯 Statistiques de Sessions

### Métriques de Participation

#### **Fréquence de Jeu**
```http
GET /user/{userId}/stats/sessions/frequency HTTP/1.1
Authorization: Bearer {token}

Response: 200 OK
{
  "currentMonth": {
    "sessionsPlayed": 12,
    "totalHours": 36.5,
    "averageSessionDuration": "3h 02min",
    "daysActive": 8,
    "favoriteDay": "samedi",
    "favoriteTimeSlot": "19h-22h"
  },
  "currentYear": {
    "sessionsPlayed": 127,
    "totalHours": 384.2,
    "averageSessionDuration": "3h 01min",
    "campaignsParticipated": 6,
    "campaignsCompleted": 2,
    "mostActivePeriod": "automne"
  },
  "allTime": {
    "sessionsPlayed": 248,
    "totalHours": 742.7,
    "firstSession": "2023-03-15T19:30:00Z",
    "longestSession": "6h 45min",
    "shortestSession": "1h 12min"
  },
  "trends": {
    "monthlyGrowth": "+15%",
    "consistencyScore": 8.2,  // Sur 10
    "burnoutRisk": "low"
  }
}
```

#### **Rôles et Préférences**
```http
GET /user/{userId}/stats/roles HTTP/1.1
Authorization: Bearer {token}

Response: 200 OK
{
  "roleDistribution": {
    "asPlayer": {
      "sessions": 198,
      "percentage": 79.8,
      "averageGroupSize": 4.2,
      "favoriteGameType": "dnd"
    },
    "asGameMaster": {
      "sessions": 50,
      "percentage": 20.2,
      "campaignsCreated": 8,
      "playersManaged": 23,
      "averageCampaignDuration": "4.2 mois"
    }
  },
  "gameTypePreferences": [
    {
      "gameType": "dnd",
      "sessions": 156,
      "percentage": 62.9,
      "averageEnjoyment": 9.1
    },
    {
      "gameType": "generic",
      "sessions": 67,
      "percentage": 27.0,
      "averageEnjoyment": 8.4
    },
    {
      "gameType": "skyrim",
      "sessions": 25,
      "percentage": 10.1,
      "averageEnjoyment": 8.8
    }
  ]
}
```

## 🎲 Statistiques de Dés et Chance

### Analyse des Jets de Dés

#### **Performance Globale**
```http
GET /user/{userId}/stats/dice/performance HTTP/1.1
Authorization: Bearer {token}

Response: 200 OK
{
  "overallStats": {
    "totalRolls": 1547,
    "averageRoll": 11.2,
    "theoreticalAverage": 10.5,
    "luckFactor": 106.7,  // Pourcentage de chance par rapport à la théorie
    "confidenceLevel": "high"  // Basé sur le nombre de lancers
  },
  "diceBreakdown": {
    "d20": {
      "totalRolls": 623,
      "average": 10.8,
      "criticalSuccesses": 31,  // 20 naturels
      "criticalFailures": 29,   // 1 naturels
      "distribution": [65, 68, 72, 75, 79, 82, 85, 88, 91, 94, 97, 89, 86, 83, 80, 77, 74, 71, 69, 78],
      "mostFrequentResult": 11,
      "leastFrequentResult": 1,
      "streaks": {
        "longestHotStreak": 7,  // 7 jets consécutifs > 15
        "longestColdStreak": 5  // 5 jets consécutifs < 6
      }
    },
    "d6": {
      "totalRolls": 412,
      "average": 3.7,
      "maxRoll": 6,
      "distribution": [68, 72, 65, 71, 69, 67]
    },
    "d8": {
      "totalRolls": 289,
      "average": 4.6,
      "maxRoll": 8,
      "favoriteNumber": 7
    },
    "d12": {
      "totalRolls": 156,
      "average": 6.8,
      "maxRoll": 12
    }
  },
  "monthlyTrend": {
    "currentMonth": {
      "average": 11.8,
      "luckRating": "très chanceux",
      "improvement": "+5.4% vs mois dernier"
    },
    "bestMonth": {
      "date": "2024-11",
      "average": 12.3,
      "note": "Mois le plus chanceux de l'année"
    },
    "worstMonth": {
      "date": "2024-06", 
      "average": 9.1,
      "note": "Période difficile, mais ça s'est amélioré !"
    }
  },
  "superstitions": {
    "luckyNumbers": [7, 11, 18],
    "unluckyNumbers": [1, 3, 13],
    "bestDayOfWeek": "samedi",
    "bestTimeOfDay": "21h-22h"
  }
}
```

#### **Contexte des Jets**
```http
GET /user/{userId}/stats/dice/context HTTP/1.1
Authorization: Bearer {token}

Response: 200 OK
{
  "combatRolls": {
    "attackRolls": {
      "total": 287,
      "hits": 201,
      "hitRate": 70.0,
      "criticalHits": 14,
      "criticalHitRate": 4.9
    },
    "savingThrows": {
      "total": 156,
      "successes": 89,
      "successRate": 57.1,
      "clutchSaves": 12  // Sauvegarde réussie de justesse
    },
    "skillChecks": {
      "total": 234,
      "successes": 167,
      "successRate": 71.4,
      "averageDifficulty": 14.2
    }
  },
  "spellcasting": {
    "spellAttackRolls": 89,
    "spellHits": 67,
    "spellHitRate": 75.3,
    "concentrationSaves": 23,
    "concentrationMaintained": 18
  },
  "socialEncounters": {
    "persuasionRolls": 45,
    "persuasionSuccesses": 32,
    "intimidationRolls": 12,
    "deceptionRolls": 23
  }
}
```

## ⚔️ Statistiques de Combat

### Performance de Combat

#### **Métriques de Combat Globales**
```http
GET /user/{userId}/stats/combat/overview HTTP/1.1
Authorization: Bearer {token}

Response: 200 OK
{
  "combatExperience": {
    "totalCombats": 127,
    "victoriesAsPlayer": 89,
    "defeatsAsPlayer": 23,
    "drawsAndRetreat": 15,
    "winRateAsPlayer": 70.1,
    "averageCombatDuration": "12.4 minutes",
    "longestCombat": "47 minutes",
    "shortestCombat": "2 minutes"
  },
  "combatRoles": {
    "asTank": {
      "combats": 45,
      "damageAbsorbed": 2340,
      "averageDamagePerCombat": 52.0,
      "survivabilityRate": 93.3
    },
    "asDPS": {
      "combats": 67,
      "totalDamageDealt": 4567,
      "averageDamagePerCombat": 68.2,
      "killParticipations": 156
    },
    "asSupport": {
      "combats": 15,
      "alliesHealed": 43,
      "totalHealingDone": 1256,
      "buffsCast": 89
    }
  },
  "enemyTypes": {
    "undead": {"faced": 34, "defeated": 31, "winRate": 91.2},
    "beasts": {"faced": 28, "defeated": 25, "winRate": 89.3},
    "humanoids": {"faced": 45, "defeated": 30, "winRate": 66.7},
    "dragons": {"faced": 3, "defeated": 2, "winRate": 66.7},
    "fiends": {"faced": 17, "defeated": 11, "winRate": 64.7}
  }
}
```

#### **Analyse des Dégâts**
```http
GET /user/{userId}/stats/combat/damage?timeframe=last30combats HTTP/1.1
Authorization: Bearer {token}

Response: 200 OK
{
  "damageDealt": {
    "totalDamage": 2456,
    "averagePerCombat": 81.9,
    "averagePerRound": 23.4,
    "highestSingleHit": 187,
    "damageTypes": {
      "slashing": 892,
      "fire": 567,
      "piercing": 345,
      "bludgeoning": 312,
      "magical": 340
    },
    "weaponBreakdown": {
      "longsword": {"damage": 678, "hits": 45, "avgDamage": 15.1},
      "fireball": {"damage": 423, "casts": 12, "avgDamage": 35.3},
      "bow": {"damage": 234, "shots": 23, "avgDamage": 10.2}
    }
  },
  "damageTaken": {
    "totalDamage": 1234,
    "averagePerCombat": 41.1,
    "fatalBlows": 3,
    "nearDeathExperiences": 8,  // < 5 HP
    "damageReduced": 567,  // Via armure, sorts, etc.
    "mostDangerousEnemy": "Balor"
  },
  "efficiency": {
    "damagePerAction": 28.7,
    "criticalDamageBonus": 45.2,
    "overkillDamage": 234,  // Dégâts en trop sur ennemis morts
    "wastedActions": 7  // Actions qui n'ont pas touché
  },
  "trends": {
    "improvementRate": "+12% vs 30 combats précédents",
    "consistency": "stable",
    "peakPerformance": "Combat vs Dragon Rouge"
  }
}
```

## 🧙‍♂️ Statistiques par Personnage

### Progression et Utilisation

#### **Performance par Personnage**
```http
GET /user/{userId}/stats/characters/performance HTTP/1.1
Authorization: Bearer {token}

Response: 200 OK
{
  "characters": [
    {
      "characterId": 16,
      "name": "Gorthak le Protecteur",
      "class": "Guerrier",
      "level": 5,
      "gameType": "dnd",
      "usage": {
        "sessionsPlayed": 34,
        "totalPlaytime": "102h 15min",
        "averageSessionLength": "3h 00min",
        "lastPlayed": "2024-01-20T19:30:00Z",
        "daysSinceCreation": 127
      },
      "combatStats": {
        "combatsParticipated": 67,
        "damageDealt": 3456,
        "damageTaken": 2890,
        "knockouts": 2,
        "rescues": 12,  // Fois où il a sauvé un allié
        "signature": "Charge héroïque"
      },
      "progression": {
        "experienceGained": 6500,
        "levelsGained": 4,
        "spellsLearned": 0,
        "equipmentAcquired": 23,
        "questsCompleted": 15
      },
      "socialStats": {
        "npcInteractions": 156,
        "persuasionAttempts": 23,
        "intimidationSuccess": 18,
        "alliancesMade": 7
      },
      "achievements": [
        "Gardien Infatigable",
        "Mur Vivant", 
        "Héros du Peuple"
      ],
      "personalityTraits": {
        "riskTaking": "prudent",
        "leadership": "high",
        "teamPlayer": "excellent"
      }
    },
    {
      "characterId": 22,
      "name": "Lyralei l'Archimage",
      "class": "Mage",
      "level": 5,
      "gameType": "dnd",
      "usage": {
        "sessionsPlayed": 28,
        "totalPlaytime": "84h 30min",
        "averageSessionLength": "3h 01min"
      },
      "combatStats": {
        "combatsParticipated": 45,
        "spellsCast": 234,
        "damageDealt": 4123,
        "manaConsumed": 1567,
        "concentrationBroken": 8,
        "signature": "Boule de feu dévastatrice"
      },
      "spellcasting": {
        "favoriteSchool": "Évocation",
        "mostUsedSpell": "Projectile Magique",
        "deadliestSpell": "Boule de Feu",
        "utilitySpells": 67,
        "creativeUses": 23  // Utilisation créative de sorts
      }
    }
  ],
  "characterComparison": {
    "mostPlayed": "Gorthak le Protecteur",
    "mostEfficient": "Lyralei l'Archimage",
    "mostVersatile": "Aranis Chassevent",
    "favoriteClass": "Guerrier",
    "preferredGameType": "dnd"
  }
}
```

#### **Évolution Temporelle**
```http
GET /character/{characterId}/stats/evolution HTTP/1.1
Authorization: Bearer {token}

Response: 200 OK
{
  "characterId": 16,
  "name": "Gorthak le Protecteur",
  "evolution": {
    "levels": [
      {
        "level": 1,
        "reachedAt": "2023-08-15T19:30:00Z",
        "sessionNumber": 1,
        "location": "Village de Braelynn"
      },
      {
        "level": 2,
        "reachedAt": "2023-08-22T21:15:00Z",
        "sessionNumber": 3,
        "location": "Caves Hantées",
        "notableEvent": "Premier boss vaincu"
      },
      {
        "level": 5,
        "reachedAt": "2024-01-15T20:45:00Z",
        "sessionNumber": 32,
        "location": "Temple Perdu",
        "notableEvent": "Artefact légendaire découvert"
      }
    ],
    "equipmentMilestones": [
      {
        "date": "2023-09-05T19:00:00Z",
        "event": "Première armure magique",
        "item": "Armure de Plates +1"
      },
      {
        "date": "2024-01-15T20:30:00Z",
        "event": "Arme légendaire obtenue",
        "item": "Lame de Vérité"
      }
    ],
    "performanceEvolution": {
      "damagePerSession": [45, 52, 67, 71, 68, 89, 92],
      "survivalRate": [85, 90, 88, 95, 97, 93, 98],
      "teamworkScore": [7.2, 7.8, 8.1, 8.5, 8.9, 9.2, 9.4]
    }
  }
}
```

## 🏆 Système de Succès/Achievements

### Catégories de Succès

#### **Succès de Combat** ⚔️
```http
GET /user/{userId}/achievements/combat HTTP/1.1
Authorization: Bearer {token}

Response: 200 OK
{
  "combatAchievements": [
    {
      "id": "first_blood",
      "name": "Premier Sang",
      "description": "Remporter votre premier combat",
      "icon": "🗡️",
      "rarity": "common",
      "isUnlocked": true,
      "unlockedAt": "2023-08-15T20:15:00Z",
      "unlockedWith": "Gorthak le Protecteur",
      "progress": 100
    },
    {
      "id": "critical_master",
      "name": "Maître du Critique",
      "description": "Obtenir 10 coups critiques consécutifs",
      "icon": "🎯",
      "rarity": "rare",
      "isUnlocked": true,
      "unlockedAt": "2024-01-08T21:30:00Z",
      "unlockedWith": "Lyralei l'Archimage",
      "progress": 100,
      "specialNote": "Exploit réalisé contre un dragon !"
    },
    {
      "id": "untouchable",
      "name": "Intouchable",
      "description": "Terminer 5 combats sans subir de dégâts",
      "icon": "🛡️",
      "rarity": "epic",
      "isUnlocked": false,
      "progress": 60,
      "currentStreak": 3,
      "requirement": 5
    },
    {
      "id": "dragon_slayer",
      "name": "Tueur de Dragons",
      "description": "Vaincre un dragon ancien en solo",
      "icon": "🐲",
      "rarity": "legendary",
      "isUnlocked": true,
      "unlockedAt": "2024-01-15T22:45:00Z",
      "unlockedWith": "Gorthak le Protecteur",
      "progress": 100,
      "witnesses": ["Lisa", "Paul", "Sophie"],
      "epicMoment": true
    },
    {
      "id": "damage_dealer",
      "name": "Machine de Guerre",
      "description": "Infliger 10 000 points de dégâts au total",
      "icon": "💥",
      "rarity": "rare",
      "isUnlocked": false,
      "progress": 78,
      "currentValue": 7834,
      "requirement": 10000
    }
  ]
}
```

#### **Succès d'Exploration** 🗺️
```http
GET /user/{userId}/achievements/exploration HTTP/1.1
Authorization: Bearer {token}

Response: 200 OK
{
  "explorationAchievements": [
    {
      "id": "cartographer",
      "name": "Cartographe",
      "description": "Découvrir 50 lieux secrets",
      "icon": "🗺️",
      "rarity": "uncommon",
      "isUnlocked": false,
      "progress": 76,
      "currentValue": 38,
      "requirement": 50,
      "recentDiscoveries": [
        "Temple Perdu - Salle Secrète",
        "Forêt Enchantée - Clairière Mystique",
        "Caves Hantées - Passage Caché"
      ]
    },
    {
      "id": "treasure_hunter",
      "name": "Chasseur de Trésors",
      "description": "Trouver 25 trésors légendaires",
      "icon": "💎",
      "rarity": "epic",
      "isUnlocked": false,
      "progress": 32,
      "currentValue": 8,
      "requirement": 25,
      "latestTreasure": "Amulette des Anciens"
    },
    {
      "id": "dungeon_master",
      "name": "Maître des Donjons",
      "description": "Compléter 10 donjons différents",
      "icon": "🏰",
      "rarity": "rare",
      "isUnlocked": false,
      "progress": 70,
      "currentValue": 7,
      "requirement": 10,
      "dungeonsList": [
        "Caves Hantées",
        "Temple Perdu", 
        "Crypte du Roi Liche",
        "Forteresse Abandonnée",
        "Labyrinthe Mystique",
        "Tour du Mage Fou",
        "Cavernes de Cristal"
      ]
    }
  ]
}
```

#### **Succès Sociaux** 👥
```http
GET /user/{userId}/achievements/social HTTP/1.1
Authorization: Bearer {token}

Response: 200 OK
{
  "socialAchievements": [
    {
      "id": "team_player",
      "name": "Esprit d'Équipe",
      "description": "Participer à 100 sessions multijoueurs",
      "icon": "🤝",
      "rarity": "common",
      "isUnlocked": true,
      "unlockedAt": "2024-01-10T19:00:00Z",
      "progress": 100,
      "currentValue": 127
    },
    {
      "id": "mentor",
      "name": "Mentor",
      "description": "Aider 5 nouveaux joueurs à créer leur premier personnage",
      "icon": "👨‍🏫",
      "rarity": "rare",
      "isUnlocked": false,
      "progress": 40,
      "currentValue": 2,
      "requirement": 5,
      "helpedPlayers": ["Marie", "Alexandre"]
    },
    {
      "id": "diplomat",
      "name": "Diplomate",
      "description": "Résoudre 10 conflits sans violence",
      "icon": "🕊️",
      "rarity": "uncommon",
      "isUnlocked": false,
      "progress": 80,
      "currentValue": 8,
      "requirement": 10
    },
    {
      "id": "generous_soul",
      "name": "Âme Généreuse",
      "description": "Donner 100 objets à d'autres joueurs",
      "icon": "🎁",
      "rarity": "uncommon",
      "isUnlocked": false,
      "progress": 45,
      "currentValue": 45,
      "requirement": 100
    }
  ]
}
```

#### **Succès de Maîtrise** 🎭
```http
GET /user/{userId}/achievements/mastery HTTP/1.1
Authorization: Bearer {token}

Response: 200 OK
{
  "masteryAchievements": [
    {
      "id": "gm_apprentice",
      "name": "Apprenti MJ",
      "description": "Créer votre première campagne",
      "icon": "📚",
      "rarity": "common",
      "isUnlocked": true,
      "unlockedAt": "2023-09-01T18:00:00Z",
      "progress": 100
    },
    {
      "id": "storyteller",
      "name": "Conteur",
      "description": "Mener 10 campagnes à leur terme",
      "icon": "📖",
      "rarity": "epic",
      "isUnlocked": false,
      "progress": 20,
      "currentValue": 2,
      "requirement": 10,
      "completedCampaigns": [
        "Les Terres Oubliées",
        "Pirates des Mers du Sud"
      ]
    },
    {
      "id": "world_builder",
      "name": "Créateur de Mondes",
      "description": "Créer 50 PNJ personnalisés",
      "icon": "🌍",
      "rarity": "rare",
      "isUnlocked": false,
      "progress": 68,
      "currentValue": 34,
      "requirement": 50
    }
  ]
}
```

#### **Succès de Collection** 💎
```http
GET /user/{userId}/achievements/collection HTTP/1.1
Authorization: Bearer {token}

Response: 200 OK
{
  "collectionAchievements": [
    {
      "id": "spell_collector",
      "name": "Collectionneur de Sorts",
      "description": "Apprendre 100 sorts différents",
      "icon": "📜",
      "rarity": "rare",
      "isUnlocked": false,
      "progress": 67,
      "currentValue": 67,
      "requirement": 100,
      "schoolBreakdown": {
        "Évocation": 23,
        "Enchantement": 15,
        "Illusion": 12,
        "Nécromancie": 8,
        "Divination": 9
      }
    },
    {
      "id": "equipment_hoarder",
      "name": "Accumulateur",
      "description": "Posséder 500 objets au total",
      "icon": "📦",
      "rarity": "uncommon",
      "isUnlocked": false,
      "progress": 43,
      "currentValue": 215,
      "requirement": 500
    },
    {
      "id": "legendary_collector",
      "name": "Collectionneur Légendaire",
      "description": "Posséder 10 objets légendaires simultanément",
      "icon": "⭐",
      "rarity": "legendary",
      "isUnlocked": false,
      "progress": 30,
      "currentValue": 3,
      "requirement": 10,
      "legendaryItems": [
        "Lame de Vérité",
        "Amulette des Anciens",
        "Anneau de Téléportation"
      ]
    }
  ]
}
```

#### **Succès de Chance** 🎲
```http
GET /user/{userId}/achievements/luck HTTP/1.1
Authorization: Bearer {token}

Response: 200 OK
{
  "luckAchievements": [
    {
      "id": "natural_20",
      "name": "Coup du Destin",
      "description": "Obtenir un 20 naturel",
      "icon": "🎯",
      "rarity": "common",
      "isUnlocked": true,
      "unlockedAt": "2023-08-15T19:45:00Z",
      "progress": 100,
      "totalNatural20s": 78
    },
    {
      "id": "streak_master",
      "name": "Série Chanceuse",
      "description": "Obtenir 5 jets de 15+ consécutifs",
      "icon": "🔥",
      "rarity": "rare",
      "isUnlocked": true,
      "unlockedAt": "2024-01-08T20:30:00Z",
      "progress": 100,
      "bestStreak": 7
    },
    {
      "id": "unlikely_hero",
      "name": "Héros Improbable",
      "description": "Gagner un combat avec moins de 5% de chance",
      "icon": "🍀",
      "rarity": "legendary",
      "isUnlocked": true,
      "unlockedAt": "2024-01-15T22:00:00Z",
      "progress": 100,
      "context": "Victoire contre Dragon Rouge avec 3 HP restants"
    },
    {
      "id": "lucky_month",
      "name": "Mois Béni",
      "description": "Avoir une moyenne de dés > 15 sur un mois",
      "icon": "🌟",
      "rarity": "epic",
      "isUnlocked": false,
      "progress": 87,
      "currentMonthAverage": 14.7,
      "requirement": 15.0
    }
  ]
}
```

### Système de Progression des Succès

#### **Niveaux de Rareté**
- **Common** (Commun) 🟢 : Succès de base, obtenus naturellement
- **Uncommon** (Peu Commun) 🔵 : Nécessitent un effort modéré
- **Rare** (Rare) 🟣 : Défis significatifs, reconnaissance notable
- **Epic** (Épique) 🟠 : Accomplissements remarquables
- **Legendary** (Légendaire) 🟡 : Exploits exceptionnels, très rares

#### **Mécaniques de Déblocage**
```http
POST /user/{userId}/achievements/check HTTP/1.1
Authorization: Bearer {token}
Content-Type: application/json

{
  "actionType": "combat_victory",
  "contextData": {
    "combatId": 789,
    "enemyType": "dragon",
    "damageDealt": 1247,
    "criticalHits": 3,
    "survivingHp": 3,
    "alliesInvolved": ["Lisa", "Paul"],
    "combatDuration": 2847  // secondes
  }
}

Response: 200 OK
{
  "achievementsUnlocked": [
    {
      "id": "dragon_slayer",
      "name": "Tueur de Dragons",
      "rarity": "legendary",
      "newlyUnlocked": true,
      "celebrationTrigger": true
    }
  ],
  "progressUpdated": [
    {
      "id": "damage_dealer",
      "name": "Machine de Guerre",
      "previousProgress": 75,
      "newProgress": 78,
      "progressGained": 3
    },
    {
      "id": "critical_master",
      "name": "Maître du Critique",
      "previousProgress": 7,
      "newProgress": 10,
      "unlocked": true
    }
  ],
  "nearMisses": [
    {
      "id": "untouchable",
      "name": "Intouchable",
      "progress": 60,
      "reasonMissed": "Dégâts subis pendant le combat"
    }
  ]
}
```

## 📈 Statistiques Avancées et Analyses

### Patterns et Tendances

#### **Analyse Comportementale**
```http
GET /user/{userId}/stats/patterns HTTP/1.1
Authorization: Bearer {token}

Response: 200 OK
{
  "playingPatterns": {
    "preferredTimes": {
      "weekdays": ["19:00-22:00", "20:00-23:00"],
      "weekends": ["14:00-17:00", "19:00-23:00"],
      "mostActiveDay": "samedi",
      "leastActiveDay": "mardi"
    },
    "sessionPreferences": {
      "idealSessionLength": "3h 15min",
      "attentionSpan": "2h 45min",
      "performanceDecline": "après 4h",
      "breakFrequency": "toutes les 90min"
    },
    "gameplayStyle": {
      "aggression": 7.2,  // Sur 10
      "caution": 6.8,
      "creativity": 8.4,
      "teamwork": 9.1,
      "leadership": 6.3,
      "riskTaking": 5.9
    }
  },
  "socialPatterns": {
    "preferredGroupSize": 4,
    "compatibilityWith": {
      "Lisa": 9.2,
      "Paul": 8.7,
      "Sophie": 8.1,
      "Marie": 7.8
    },
    "rolePreference": {
      "asPlayer": 80,
      "asGM": 20
    }
  },
  "performancePatterns": {
    "bestPerformanceTime": "21:00-22:00",
    "worstPerformanceTime": "14:00-15:00",
    "luckCycles": {
      "currentPhase": "ascending",
      "predictedPeak": "dans 3-5 sessions",
      "lastPeak": "2024-01-08"
    }
  }
}
```

#### **Comparaisons et Classements**
```http
GET /user/{userId}/stats/leaderboards HTTP/1.1
Authorization: Bearer {token}

Response: 200 OK
{
  "globalRankings": {
    "overallScore": {
      "rank": 156,
      "percentile": 85.2,
      "score": 47892,
      "category": "Aventurier Expérimenté"
    },
    "combatEfficiency": {
      "rank": 89,
      "percentile": 91.3,
      "averageDamage": 68.2
    },
    "luckFactor": {
      "rank": 234,
      "percentile": 76.1,
      "score": 106.7
    },
    "achievementCount": {
      "rank": 67,
      "percentile": 94.2,
      "total": 78,
      "legendary": 3
    }
  },
  "friendRankings": {
    "amongFriends": {
      "rank": 2,
      "outOf": 12,
      "strongestAreas": ["Combat", "Teamwork"],
      "improvementAreas": ["Exploration", "Chance"]
    }
  },
  "gameTypeRankings": {
    "dnd": {
      "rank": 145,
      "playerBase": 15420,
      "specialization": "Tank/Fighter"
    },
    "generic": {
      "rank": 67,
      "playerBase": 8320,
      "specialization": "Versatile Player"
    }
  }
}
```

## 🎯 Interface et Visualisations

### Dashboard Personnel

#### **Widgets Statistiques**
```typescript
interface StatsWidget {
  type: 'metric' | 'chart' | 'progress' | 'achievement';
  title: string;
  data: any;
  size: 'small' | 'medium' | 'large';
  refreshRate: number;
}

const defaultWidgets: StatsWidget[] = [
  {
    type: 'metric',
    title: 'Sessions ce Mois',
    data: { value: 12, change: '+3', trend: 'up' },
    size: 'small',
    refreshRate: 3600
  },
  {
    type: 'chart',
    title: 'Évolution des Dégâts',
    data: { chartType: 'line', values: [45, 52, 67, 71, 68, 89, 92] },
    size: 'medium',
    refreshRate: 1800
  },
  {
    type: 'progress',
    title: 'Prochain Succès',
    data: { name: 'Maître des Donjons', progress: 70, eta: '2 sessions' },
    size: 'small',
    refreshRate: 600
  },
  {
    type: 'achievement',
    title: 'Dernier Succès',
    data: { name: 'Tueur de Dragons', icon: '🐲', rarity: 'legendary' },
    size: 'medium',
    refreshRate: 86400
  }
];
```

### Notifications de Succès

#### **Célébrations de Déblocage**
```javascript
// Animation de succès débloqué
{
  "type": "ACHIEVEMENT_UNLOCKED",
  "achievement": {
    "id": "dragon_slayer",
    "name": "Tueur de Dragons",
    "icon": "🐲",
    "rarity": "legendary"
  },
  "animation": {
    "type": "epic_celebration",
    "duration": 5000,
    "effects": ["confetti", "golden_border", "sound_fanfare"],
    "shareOptions": true
  },
  "rewards": {
    "title": "Champion Légendaire",
    "badge": "dragon_slayer_badge",
    "experience": 1000,
    "specialEffect": "Golden nameplate for 7 days"
  },
  "socialSharing": {
    "message": "Je viens de terrasser un dragon ancien ! 🐲⚔️",
    "platforms": ["discord", "social_feed"],
    "visibility": "friends"
  }
}
```

### Rapports Personnalisés

#### **Rapport Mensuel**
```http
GET /user/{userId}/stats/reports/monthly?month=2024-01 HTTP/1.1
Authorization: Bearer {token}

Response: 200 OK
{
  "reportPeriod": "Janvier 2024",
  "summary": {
    "totalSessions": 15,
    "totalPlaytime": "45h 30min",
    "averageSessionLength": "3h 02min",
    "achievement": "Mois très actif !",
    "grade": "A+"
  },
  "highlights": [
    "🐲 Premier dragon vaincu !",
    "🎯 Record personnel de dégâts : 187 en un coup",
    "🏆 3 nouveaux succès débloqués",
    "🤝 +2 nouveaux compagnons d'aventure"
  ],
  "improvements": [
    "📊 Chance aux dés : +12% vs mois précédent",
    "⚔️ Efficacité combat : +8% vs moyenne",
    "👥 Score travail d'équipe : +15%"
  ],
  "goals": [
    "🗺️ Explorer 5 nouveaux lieux (3/5 complété)",
    "📜 Apprendre 10 nouveaux sorts (7/10 complété)",
    "🎲 Maintenir moyenne de dés > 11 (Réussi !)"
  ],
  "nextMonthPredictions": {
    "estimatedSessions": 12,
    "achievementsNearCompletion": 3,
    "upcomingMilestones": ["Niveau 6", "100ème session"]
  }
}
```

---

*Ce système de statistiques et succès transforme chaque session JDR en une expérience mesurable et gratifiante. Retour au [README principal](./README.md)*