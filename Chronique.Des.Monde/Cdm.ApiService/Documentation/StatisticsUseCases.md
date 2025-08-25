# Cas d'Utilisation - Statistiques et Succès

Ce document présente des scénarios détaillés pour le système de statistiques, d'analyse de performance et de déblocage de succès.

## 📊 Cas d'Utilisation 1 : Analyse Mensuelle Complète

### Contexte
Lisa consulte ses statistiques de janvier 2024 après un mois particulièrement actif avec plusieurs campagnes.

### Workflow d'Analyse Mensuelle

#### 1. Rapport mensuel global
```http
GET /user/3/stats/reports/monthly?month=2024-01 HTTP/1.1
Authorization: Bearer {jwt_token_lisa}

Response: 200 OK
{
  "reportPeriod": "Janvier 2024",
  "summary": {
    "totalSessions": 15,
    "totalPlaytime": "45h 30min",
    "averageSessionLength": "3h 02min",
    "campaignsActive": 3,
    "charactersPlayed": 2,
    "achievement": "Mois très actif ! 🔥",
    "grade": "A+",
    "improvementVsPreviousMonth": "+25%"
  },
  "highlights": [
    "🐲 Premier dragon vaincu avec Lyralei !",
    "🎯 Record personnel de dégâts : 187 en un coup",
    "🏆 3 nouveaux succès débloqués",
    "🤝 +2 nouveaux compagnons d'aventure",
    "📜 15 nouveaux sorts appris"
  ],
  "personalRecords": [
    {
      "category": "Combat",
      "record": "Plus haut dégât en une attaque",
      "value": 187,
      "previousRecord": 142,
      "improvement": "+31%",
      "context": "Coup critique avec Lame de Vérité vs Dragon Rouge"
    },
    {
      "category": "Chance",
      "record": "Série de jets chanceux",
      "value": "7 jets consécutifs > 15",
      "previousRecord": "4 jets",
      "improvement": "+75%",
      "context": "Combat final du Temple Perdu"
    }
  ],
  "socialStats": {
    "sessionsWithFriends": 12,
    "newPlayersHelped": 2,
    "equipmentShared": 8,
    "complimentsReceived": 23,
    "teamworkRating": 9.4  // Sur 10, évalué par les autres joueurs
  },
  "goals": {
    "completed": [
      "🗺️ Explorer 5 nouveaux lieux (5/5 ✅)",
      "🎲 Maintenir moyenne de dés > 11 (11.8 ✅)"
    ],
    "inProgress": [
      "📜 Apprendre 20 nouveaux sorts (15/20 - 75%)",
      "⚔️ Gagner 10 combats sans KO (7/10 - 70%)"
    ],
    "nextMonthSuggestions": [
      "🐉 Tenter un combat de boss solo",
      "👥 Organiser une session en tant que MJ",
      "🎯 Atteindre 20 coups critiques ce mois"
    ]
  }
}
```

#### 2. Analyse détaillée des dés
```http
GET /user/3/stats/dice/performance?timeframe=month&month=2024-01 HTTP/1.1
Authorization: Bearer {jwt_token_lisa}

Response: 200 OK
{
  "monthlyDiceAnalysis": {
    "totalRolls": 567,
    "averageRoll": 11.8,
    "theoreticalAverage": 10.5,
    "luckFactor": 112.4,  // Très chanceux ce mois !
    "confidenceLevel": "very_high",
    "monthRanking": "2ème meilleur mois de l'année"
  },
  "weeklyTrends": [
    {
      "week": "Semaine 1",
      "averageRoll": 9.2,
      "mood": "😐 Démarrage difficile",
      "notableEvents": ["Échec critique lors du premier combat"]
    },
    {
      "week": "Semaine 2", 
      "averageRoll": 12.1,
      "mood": "😊 Retour en force !",
      "notableEvents": ["Série de 5 jets > 15", "Sauvegarde héroïque"]
    },
    {
      "week": "Semaine 3",
      "averageRoll": 13.4,
      "mood": "🔥 En feu !",
      "notableEvents": ["Record de dégâts", "3 critiques dans un combat"]
    },
    {
      "week": "Semaine 4",
      "averageRoll": 12.9,
      "mood": "⭐ Constance au top",
      "notableEvents": ["Aucun jet < 5 de toute la semaine"]
    }
  ],
  "diceSuperstitions": {
    "luckyNumbers": [7, 11, 18, 20],
    "unluckyNumbers": [1, 3],
    "bestDayOfWeek": "samedi",
    "bestTimeOfDay": "21h-22h",
    "luckyCharacter": "Lyralei l'Archimage",
    "luckyDice": "Dé rouge métallique",
    "ritualBeforeRoll": "Souffler sur le dé"
  },
  "contextualPerformance": {
    "combatRolls": {
      "average": 12.3,
      "criticalRate": 6.2,  // % de 20 naturels
      "clutchMoments": 4  // Jets salvateurs dans des situations critiques
    },
    "skillChecks": {
      "average": 11.1,
      "successRate": 73.4,
      "mostSuccessfulSkill": "Arcanes",
      "improvementArea": "Athlétisme"
    },
    "savingThrows": {
      "average": 12.8,
      "successRate": 81.2,
      "deathSaves": 0,  // Aucune mort ce mois !
      "heroicSaves": 3  // Sauvegardes in extremis
    }
  }
}
```

## 🏆 Cas d'Utilisation 2 : Déblocage de Succès Légendaire

### Contexte
Pendant le combat contre le Dragon Rouge, Thomas réalise un exploit qui déclenche plusieurs succès simultanément.

### Workflow de Déblocage Multiple

#### 1. Action héroïque enregistrée
```http
POST /user/2/achievements/check HTTP/1.1
Authorization: Bearer {jwt_token_thomas}
Content-Type: application/json

{
  "actionType": "combat_final_blow",
  "contextData": {
    "combatId": 789,
    "characterId": 15,  // Gorthak le Protecteur
    "enemyType": "ancient_red_dragon",
    "enemyName": "Pyrothos l'Ancien",
    "damageDealt": 187,
    "attackType": "critical_hit",
    "weaponUsed": "Lame de Vérité",
    "survivingHp": 3,  // Thomas était presque mort
    "alliesDown": 2,   // 2 alliés KO
    "combatDuration": 2847,  // 47 minutes de combat épique
    "spectators": ["Lisa", "Paul", "Sophie"],
    "sessionId": "sess_abc123",
    "campaignId": 42,
    "epicMoment": true,
    "soloKill": true,
    "contextualBonus": "Last_stand_against_overwhelming_odds"
  }
}

Response: 200 OK
{
  "achievementsUnlocked": [
    {
      "id": "dragon_slayer",
      "name": "Tueur de Dragons",
      "description": "Vaincre un dragon ancien",
      "icon": "🐲",
      "rarity": "legendary",
      "newlyUnlocked": true,
      "celebrationLevel": "epic",
      "rewards": {
        "title": "Fléau des Dragons",
        "experienceBonus": 1000,
        "specialEffect": "Dragon slayer aura for 30 days",
        "unlockable": "Ancient Dragon Lore access"
      }
    },
    {
      "id": "last_stand",
      "name": "Dernier Rempart",
      "description": "Vaincre un boss en solo avec < 5 HP",
      "icon": "🛡️",
      "rarity": "epic", 
      "newlyUnlocked": true,
      "celebrationLevel": "major"
    },
    {
      "id": "critical_finisher",
      "name": "Finisseur Critique",
      "description": "Terminer un boss avec un coup critique",
      "icon": "⚡",
      "rarity": "rare",
      "newlyUnlocked": true
    }
  ],
  "progressUpdated": [
    {
      "id": "damage_dealer",
      "name": "Machine de Guerre",
      "previousProgress": 78,
      "newProgress": 80,
      "progressGained": 2,
      "note": "Record de dégâts battu !"
    },
    {
      "id": "boss_hunter",
      "name": "Chasseur de Boss",
      "previousProgress": 67,
      "newProgress": 75,
      "progressGained": 8,
      "nearCompletion": "Plus que 3 boss à vaincre"
    }
  ],
  "socialImpact": {
    "witnessesNotified": true,
    "partyAchievement": "Legendary Victory",
    "campaignMilestone": true,
    "storyEntry": "This victory will be remembered in campaign lore"
  },
  "celebrationTriggered": {
    "type": "legendary_unlock",
    "duration": 10000,  // 10 secondes de célébration
    "effects": [
      "golden_confetti",
      "dragon_roar_sound",
      "screen_shake",
      "achievement_fanfare",
      "title_display"
    ],
    "sharePrompt": true,
    "screenshotSuggestion": true
  }
}
```

#### 2. Notification aux témoins
```javascript
// WebSocket pour tous les participants de la session
{
  "type": "LEGENDARY_ACHIEVEMENT_WITNESSED",
  "achiever": {
    "userName": "Thomas",
    "characterName": "Gorthak le Protecteur"
  },
  "achievement": {
    "name": "Tueur de Dragons",
    "rarity": "legendary",
    "icon": "🐲"
  },
  "context": {
    "epicMoment": "Coup final contre Pyrothos l'Ancien",
    "yourRole": "witness",
    "combatHighlight": "Thomas a vaincu le dragon avec seulement 3 HP restants !"
  },
  "celebration": {
    "sharedCelebration": true,
    "message": "🐲⚔️ EXPLOIT LÉGENDAIRE ! Thomas vient de terrasser un dragon ancien !",
    "effects": ["shared_confetti", "group_cheer_sound"],
    "duration": 8000
  },
  "socialActions": [
    {
      "label": "👏 Féliciter",
      "action": "send_congratulations"
    },
    {
      "label": "📸 Immortaliser",
      "action": "take_group_screenshot"
    },
    {
      "label": "📜 Raconter",
      "action": "write_story_entry"
    }
  ]
}
```

#### 3. Mise à jour du profil héroïque
```http
GET /user/2/achievements/legendary HTTP/1.1
Authorization: Bearer {jwt_token_thomas}

Response: 200 OK
{
  "legendaryStatus": {
    "title": "Fléau des Dragons",
    "titleUnlockedAt": "2024-01-20T22:45:00Z",
    "specialEffects": [
      {
        "name": "Dragon Slayer Aura",
        "description": "Aura dorée visible par tous",
        "duration": "30 jours",
        "expiresAt": "2024-02-19T22:45:00Z"
      }
    ],
    "permanentUnlocks": [
      "Ancient Dragon Lore",
      "Dragon Scale Crafting",
      "Legendary Weapons Access"
    ]
  },
  "legendaryAchievements": [
    {
      "id": "dragon_slayer",
      "name": "Tueur de Dragons",
      "unlockedAt": "2024-01-20T22:45:00Z",
      "witnesses": ["Lisa", "Paul", "Sophie"],
      "context": "Combat épique contre Pyrothos l'Ancien",
      "memoryVideo": "combat_789_final_blow.mp4",
      "storyEntry": "Dans les annales de la campagne 'Les Terres Oubliées', le 20 janvier restera à jamais gravé comme le jour où Gorthak le Protecteur a terrassé le terrible Pyrothos..."
    }
  ],
  "communityRecognition": {
    "globalRank": 45,  // 45ème joueur à obtenir ce succès
    "serverRank": 3,   // 3ème sur ce serveur
    "rarity": "0.3% des joueurs possèdent ce succès",
    "socialShares": 156,
    "congratulations": 47
  }
}
```

## 📈 Cas d'Utilisation 3 : Analyse Comparative entre Personnages

### Contexte
Paul veut comparer les performances de ses trois personnages principaux pour optimiser son style de jeu.

### Workflow de Comparaison Multi-Personnages

#### 1. Vue comparative globale
```http
GET /user/4/stats/characters/comparison HTTP/1.1
Authorization: Bearer {jwt_token_paul}

Response: 200 OK
{
  "charactersCompared": [
    {
      "characterId": 22,
      "name": "Gareth l'Épéiste",
      "class": "Guerrier",
      "level": 5,
      "gameType": "dnd",
      "playtime": "89h 15min",
      "sessions": 34,
      "role": "Tank/DPS"
    },
    {
      "characterId": 28,
      "name": "Zara l'Assassin",
      "class": "Roublard",
      "level": 4,
      "gameType": "dnd",
      "playtime": "45h 30min",
      "sessions": 18,
      "role": "DPS/Stealth"
    },
    {
      "characterId": 35,
      "name": "Capitaine Redbeard",
      "class": "Personnage générique",
      "level": "N/A",
      "gameType": "generic",
      "playtime": "67h 45min",
      "sessions": 25,
      "role": "Leader/Aventurier"
    }
  ],
  "performanceComparison": {
    "combatEfficiency": {
      "winner": "Zara l'Assassin",
      "metrics": {
        "Gareth": {
          "damagePerRound": 24.7,
          "survivability": 92.3,
          "hitRate": 75.2,
          "strength": "Durabilité exceptionnelle"
        },
        "Zara": {
          "damagePerRound": 31.4,
          "survivability": 67.8,
          "hitRate": 82.1,
          "strength": "Dégâts de pointe avec attaques sournoises"
        },
        "Redbeard": {
          "damagePerRound": "18.3 (système générique)",
          "survivability": 78.4,
          "hitRate": "Variable selon MJ",
          "strength": "Polyvalence et leadership"
        }
      }
    },
    "dicePerformance": {
      "luckyCharacter": "Capitaine Redbeard",
      "averages": {
        "Gareth": 10.8,
        "Zara": 11.5,
        "Redbeard": 12.1
      },
      "criticalRates": {
        "Gareth": 4.2,
        "Zara": 6.8,  // Avantage des roublards
        "Redbeard": 5.1
      }
    },
    "socialInteraction": {
      "mostCharismatic": "Capitaine Redbeard",
      "diplomaticSuccesses": {
        "Gareth": 23,
        "Zara": 12,
        "Redbeard": 45
      },
      "leadershipMoments": {
        "Gareth": 15,
        "Zara": 3,
        "Redbeard": 67
      }
    }
  },
  "personalityProfiles": {
    "Gareth": {
      "playstyle": "Protecteur dévoué",
      "riskTaking": 3.2,  // Sur 10
      "creativity": 5.8,
      "teamwork": 9.1,
      "signature": "Charge héroïque pour sauver ses alliés"
    },
    "Zara": {
      "playstyle": "Frappe chirurgicale",
      "riskTaking": 7.8,
      "creativity": 8.4,
      "teamwork": 6.2,
      "signature": "Attaque sournoise dévastatrice depuis les ombres"
    },
    "Redbeard": {
      "playstyle": "Leader d'aventure",
      "riskTaking": 6.1,
      "creativity": 9.2,
      "teamwork": 8.7,
      "signature": "Discours inspirant qui redonne courage à l'équipe"
    }
  },
  "recommendations": {
    "forGareth": [
      "Essayer des tactiques plus offensives occasionnellement",
      "Développer les compétences sociales",
      "Explorer des styles de combat alternatifs"
    ],
    "forZara": [
      "Améliorer la coordination d'équipe",
      "Prendre plus de risques calculés",
      "Développer des compétences utilitaires"
    ],
    "forRedbeard": [
      "Continuer le développement du leadership",
      "Explorer plus de défis personnels",
      "Approfondir les relations avec l'équipage"
    ]
  }
}
```

#### 2. Évolution temporelle détaillée
```http
GET /user/4/stats/characters/evolution?characterIds=22,28,35 HTTP/1.1
Authorization: Bearer {jwt_token_paul}

Response: 200 OK
{
  "evolutionTimeline": {
    "timeRange": "6 derniers mois",
    "dataPoints": [
      {
        "month": "2023-08",
        "Gareth": {
          "averageDamage": 18.2,
          "survivabilityRate": 87.1,
          "sessionsPlayed": 6,
          "milestone": "Création du personnage"
        },
        "Zara": null,  // Pas encore créée
        "Redbeard": {
          "averageDamage": 15.1,
          "survivabilityRate": 82.3,
          "sessionsPlayed": 4,
          "milestone": "Première aventure maritime"
        }
      },
      {
        "month": "2023-09",
        "Gareth": {
          "averageDamage": 21.5,
          "survivabilityRate": 91.2,
          "sessionsPlayed": 5,
          "milestone": "Première armure magique obtenue"
        },
        "Zara": {
          "averageDamage": 28.7,
          "survivabilityRate": 65.4,
          "sessionsPlayed": 3,
          "milestone": "Création - Style agressif immédiat"
        },
        "Redbeard": {
          "averageDamage": 16.8,
          "survivabilityRate": 76.9,
          "sessionsPlayed": 5,
          "milestone": "Premier commandement d'équipage"
        }
      },
      {
        "month": "2024-01",
        "Gareth": {
          "averageDamage": 24.7,
          "survivabilityRate": 92.3,
          "sessionsPlayed": 7,
          "milestone": "Maîtrise tactique aboutie"
        },
        "Zara": {
          "averageDamage": 31.4,
          "survivabilityRate": 67.8,
          "sessionsPlayed": 4,
          "milestone": "Première mission solo réussie"
        },
        "Redbeard": {
          "averageDamage": 18.3,
          "survivabilityRate": 78.4,
          "sessionsPlayed": 6,
          "milestone": "Légende naissante des mers"
        }
      }
    ],
    "trends": {
      "Gareth": {
        "direction": "progression_stable",
        "strength": "Amélioration constante de la survie",
        "prediction": "Évolution vers tank ultime"
      },
      "Zara": {
        "direction": "progression_rapide",
        "strength": "Courbe d'apprentissage impressionnante",
        "prediction": "Potentiel DPS légendaire"
      },
      "Redbeard": {
        "direction": "progression_sociale",
        "strength": "Leadership de plus en plus naturel",
        "prediction": "Futur grand capitaine"
      }
    }
  }
}
```

## 🎯 Cas d'Utilisation 4 : Prédictions et Recommandations IA

### Contexte
Le système analyse les données de Sophie pour lui proposer des recommandations personnalisées et des prédictions.

### Workflow d'Analyse Prédictive

#### 1. Analyse comportementale avancée
```http
GET /user/5/stats/ai-insights HTTP/1.1
Authorization: Bearer {jwt_token_sophie}

Response: 200 OK
{
  "playerProfile": {
    "archetype": "Exploratrice Créative",
    "confidence": 0.89,
    "traits": {
      "exploration": 9.2,  // Sur 10
      "creativity": 8.7,
      "socialInteraction": 7.8,
      "combatFocus": 5.4,
      "ruleAdherence": 6.1,
      "riskTaking": 7.3
    },
    "playingMotivations": [
      "Découverte de nouveaux lieux et mystères",
      "Développement narratif et roleplay",
      "Résolution créative de problèmes",
      "Interaction sociale avec autres joueurs"
    ]
  },
  "performancePredictions": {
    "dicePerformance": {
      "nextSessionPrediction": "Légèrement au-dessus de la moyenne",
      "confidence": 0.72,
      "reasoning": "Pattern cyclique détecté, actuellement en phase ascendante",
      "expectedAverage": "11.3 ± 1.2"
    },
    "combatEfficiency": {
      "trend": "amélioration_graduelle",
      "predictedImprovement": "+8% sur les 3 prochaines sessions",
      "keyFactors": [
        "Meilleure coordination d'équipe observée",
        "Apprentissage de nouvelles tactiques"
      ]
    },
    "achievementProgression": {
      "nearTermUnlocks": [
        {
          "achievementId": "cartographer",
          "name": "Cartographe",
          "estimatedUnlock": "2-3 sessions",
          "confidence": 0.94,
          "reasoning": "Tendance exploration très marquée"
        },
        {
          "achievementId": "creative_problem_solver",
          "name": "Résolveur Créatif",
          "estimatedUnlock": "1-2 sessions",
          "confidence": 0.78,
          "reasoning": "Pattern de solutions non-conventionnelles"
        }
      ],
      "longTermGoals": [
        {
          "achievementId": "master_explorer",
          "name": "Maître Explorateur",
          "estimatedUnlock": "3-4 mois",
          "requirements": "Continue focus on exploration + leadership development"
        }
      ]
    }
  },
  "personalizedRecommendations": {
    "characterDevelopment": [
      {
        "suggestion": "Développer les compétences de leadership",
        "reasoning": "Votre créativité naturelle pourrait inspirer l'équipe",
        "impact": "high",
        "effort": "medium"
      },
      {
        "suggestion": "Explorer plus de sorts utilitaires",
        "reasoning": "Aligné avec votre style résolution créative",
        "impact": "medium",
        "effort": "low"
      }
    ],
    "gameplay": [
      {
        "suggestion": "Proposer des solutions alternatives en combat",
        "reasoning": "Votre créativité peut transformer les défis",
        "examples": ["Négociation", "Manipulation d'environnement", "Tactiques non-conventionnelles"]
      },
      {
        "suggestion": "Initier plus d'interactions sociales",
        "reasoning": "Score social en progression, potentiel à exploiter",
        "benefits": ["Achievements sociaux", "Amélioration teamwork", "Enrichissement narratif"]
      }
    ],
    "campaignPreferences": [
      {
        "recommendation": "Campagnes exploration/mystère",
        "match": "95%",
        "examples": ["Archaeological expeditions", "Ancient mysteries", "Uncharted territories"]
      },
      {
        "recommendation": "Rôle de scout/éclaireur",
        "match": "87%",
        "benefits": ["Utilise vos forces naturelles", "Responsabilités leadership", "Découvertes en avant-première"]
      }
    ]
  },
  "risksAndOpportunities": {
    "risks": [
      {
        "risk": "Possible lassitude en combats répétitifs",
        "probability": 0.34,
        "mitigation": "Varier les types d'ennemis et tactiques"
      }
    ],
    "opportunities": [
      {
        "opportunity": "Potentiel MJ exceptionnel",
        "probability": 0.78,
        "reasoning": "Créativité + exploration + social = profil MJ idéal",
        "suggestion": "Considérer créer une campagne d'exploration"
      }
    ]
  }
}
```

#### 2. Recommandations de formation d'équipe
```http
GET /user/5/stats/team-compatibility HTTP/1.1
Authorization: Bearer {jwt_token_sophie}

Response: 200 OK
{
  "teamSynergy": {
    "currentTeam": {
      "members": ["Thomas (Tank)", "Lisa (DPS Mage)", "Paul (DPS Melee)"],
      "sophieRole": "Support/Exploration",
      "synergyScore": 8.7,
      "strengths": [
        "Équilibre parfait des rôles de combat",
        "Complémentarité exploration-combat",
        "Excellent moral d'équipe"
      ],
      "improvementAreas": [
        "Développer coordination tactique",
        "Améliorer communication en combat"
      ]
    },
    "optimalTeammates": [
      {
        "playerName": "Marie",
        "compatibility": 0.94,
        "reasons": [
          "Style narratif complémentaire",
          "Même passion pour l'exploration",
          "Compétences sociales similaires"
        ],
        "potentialRole": "Co-explorer/Diplomat"
      },
      {
        "playerName": "Alexandre",
        "compatibility": 0.87,
        "reasons": [
          "Approche créative des défis",
          "Flexibilité de gameplay",
          "Bon équilibre risk/prudence"
        ],
        "potentialRole": "Utility/Support"
      }
    ],
    "campaignRecommendations": [
      {
        "campaignType": "Exploration mystérieuse",
        "teamSize": "4-5 joueurs",
        "idealComposition": [
          "Sophie (Leader/Scout)",
          "Marie (Diplomat/Lore)",
          "Paul (Protection)",
          "Lisa (Magical support)",
          "Nouveau joueur (Wild card)"
        ],
        "successProbability": 0.91
      }
    ]
  }
}
```

---

*Ces cas d'utilisation montrent comment le système de statistiques peut enrichir profondément l'expérience JDR en fournissant des insights personnalisés et des recommandations intelligentes. Retour aux [Statistiques et Succès](./StatisticsAndAchievements.md)*