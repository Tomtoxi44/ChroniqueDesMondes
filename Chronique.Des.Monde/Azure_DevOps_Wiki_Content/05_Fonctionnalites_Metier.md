# 🎮 Fonctionnalités Métier JDR

Cette page détaille toutes les fonctionnalités métier innovantes de **Chronique des Mondes**, spécialement conçues pour révolutionner l'expérience du jeu de rôle en ligne.

---

## 🎯 **Vision Métier**

### **🌟 Innovation Multi-Rôles**
**Chronique des Mondes** est la **première plateforme** permettant aux utilisateurs d'être simultanément :
- **🎭 Maître de Jeu** d'une campagne
- **👤 Joueur** dans d'autres campagnes
- **🔄 Transition fluide** entre les rôles selon le contexte

### **🎲 Systèmes JDR Supportés**
- **🐉 D&D 5e** : Calculs automatiques complets, SRD intégré
- **🎭 Générique** : Systèmes personnalisés adaptables
- **🔮 Futur** : Pathfinder, Shadowrun, systèmes communautaires

---

## 🧙‍♂️ **Système de Personnages Multi-GameType**

### **🏗️ Architecture Extensible**
```
┌─────────────────────────────────────────────────────────────┐
│                PERSONNAGES MULTI-SYSTÈMES                  │
├─────────────────────────────────────────────────────────────┤
│ 🎭 PERSONNAGES GÉNÉRIQUES                                  │
│ ├── Attributs dynamiques (Force, Agilité, etc.)           │
│ ├── Compétences personnalisables                           │
│ ├── Inventaire et équipement adaptable                     │
│ └── Progression libre définie par le MJ                    │
├─────────────────────────────────────────────────────────────┤
│ 🐉 PERSONNAGES D&D 5e                                      │
│ ├── 6 Caractéristiques officielles (Force, Dex, etc.)     │
│ ├── Classes avec progression automatique                   │
│ ├── Races avec traits raciaux                              │
│ ├── Calculs automatiques (CA, PV, modificateurs)          │
│ ├── Maîtrises et sauvegardes                              │
│ └── Sorts par classe avec emplacements                     │
├─────────────────────────────────────────────────────────────┤
│ 🔮 EXTENSIONS FUTURES                                      │
│ ├── Pathfinder 2e : Actions multiples, degrés succès      │
│ ├── Shadowrun : Pool de dés d6, Matrice, cyberware        │
│ └── Systèmes communautaires personnalisés                  │
└─────────────────────────────────────────────────────────────┘
```

### **⚡ Calculs Automatiques D&D 5e**
```csharp
// Exemple de calculs automatiques
public class DndCharacterCalculator
{
    public int CalculateArmorClass(CharacterDnd character)
    {
        var baseAC = 10; // CA de base
        var dexModifier = GetModifier(character.TotalDexterity);
        var armorAC = character.Armor?.BaseAC ?? 0;
        var shieldAC = character.Shield?.ACBonus ?? 0;
        
        return baseAC + dexModifier + armorAC + shieldAC;
    }
    
    public int CalculateHitPoints(CharacterDnd character)
    {
        var baseHP = GetClassBaseHP(character.Class);
        var conModifier = GetModifier(character.TotalConstitution);
        var levelBonus = (character.Level - 1) * (GetClassHitDie(character.Class) / 2 + 1);
        
        return baseHP + conModifier + levelBonus;
    }
    
    public int CalculateProficiencyBonus(int level)
    {
        return (level - 1) / 4 + 2; // +2 au niveau 1, +3 au niveau 5, etc.
    }
}
```

### **🎨 Interface de Création Wizard**
```razor
@* Wizard de création personnage D&D *@
<div class="character-wizard">
    <div class="wizard-steps">
        <div class="step @(CurrentStep == 1 ? "active" : "")">1. Informations de base</div>
        <div class="step @(CurrentStep == 2 ? "active" : "")">2. Race et Classe</div>
        <div class="step @(CurrentStep == 3 ? "active" : "")">3. Caractéristiques</div>
        <div class="step @(CurrentStep == 4 ? "active" : "")">4. Équipement de départ</div>
        <div class="step @(CurrentStep == 5 ? "active" : "")">5. Finalisation</div>
    </div>
    
    @if (CurrentStep == 3)
    {
        <div class="ability-scores">
            <h3>🎲 Répartition des Caractéristiques</h3>
            <p>Points disponibles : <strong>@RemainingPoints</strong>/27</p>
            
            @foreach (var ability in AbilityScores)
            {
                <div class="ability-row">
                    <label>@ability.Name</label>
                    <div class="score-controls">
                        <button @onclick="() => DecreaseStat(ability)" 
                                disabled="@(ability.Value <= 8)">-</button>
                        <span class="score-value">@ability.Value</span>
                        <button @onclick="() => IncreaseStat(ability)" 
                                disabled="@(RemainingPoints <= 0 || ability.Value >= 15)">+</button>
                    </div>
                    <div class="modifier">
                        (@(GetModifier(ability.Value) >= 0 ? "+" : "")@GetModifier(ability.Value))
                    </div>
                </div>
            }
            
            <div class="racial-bonuses">
                <h4>🎭 Bonus Raciaux</h4>
                @if (SelectedRace != null)
                {
                    @foreach (var bonus in SelectedRace.AbilityBonuses)
                    {
                        <span class="bonus">@bonus.Ability +@bonus.Value</span>
                    }
                }
            </div>
        </div>
    }
</div>
```

---

## 🪄 **Système de Sorts Bi-Niveau**

### **🌟 Architecture Officiel + Privé**
```
📚 SORTS OFFICIELS (SRD D&D 5e)
├── 🔒 Créés par l'administration (CreatedByUserId = 0)
├── 🌍 Visibles par tous les utilisateurs (IsPublic = true)
├── 📖 350+ sorts du System Reference Document
├── ⚡ Calculs automatiques intégrés (DD, dégâts, portée)
└── 🚫 Non modifiables par les utilisateurs

✨ SORTS PRIVÉS (Créations utilisateur)
├── 👤 Créés par les utilisateurs (CreatedByUserId > 0)
├── 🔐 Visibilité contrôlée (IsPublic = true/false)
├── 🎨 Personnalisation complète des propriétés
├── 🛡️ Validation et modération par scoring
└── 📊 Partage avec la communauté (optionnel)
```

### **📖 Système de Grimoires Intelligent**
```csharp
public class SpellbookService
{
    public async Task<List<Spell>> GetAvailableSpellsForClassAsync(
        string characterClass, int characterLevel)
    {
        var query = _context.Spells
            .Where(s => s.IsPublic || s.CreatedByUserId == userId);
            
        // Filtrage par classe D&D
        if (characterClass == "Magicien")
        {
            query = query.Where(s => 
                s.DndProperties.Classes.Contains("Magicien") &&
                s.DndProperties.Level <= GetMaxSpellLevel(characterLevel));
        }
        
        return await query.ToListAsync();
    }
    
    public async Task<bool> CanLearnSpellAsync(CharacterDnd character, Spell spell)
    {
        // Vérifications D&D 5e
        if (!spell.DndProperties.Classes.Contains(character.Class))
            return false;
            
        var maxSpellLevel = GetMaxSpellLevel(character.Level);
        if (spell.DndProperties.Level > maxSpellLevel)
            return false;
            
        // Vérifier les emplacements de sorts disponibles
        var knownSpells = await GetKnownSpellsCountAsync(character.Id);
        var maxKnownSpells = GetMaxKnownSpells(character.Class, character.Level);
        
        return knownSpells < maxKnownSpells;
    }
}
```

### **🔍 Recherche et Filtrage Avancé**
```razor
<div class="spell-browser">
    <div class="filters">
        <div class="filter-group">
            <label>🎯 Source</label>
            <div class="checkbox-group">
                <input type="checkbox" @bind="ShowOfficialSpells" /> Sorts Officiels
                <input type="checkbox" @bind="ShowPrivateSpells" /> Mes Sorts Privés
                <input type="checkbox" @bind="ShowCommunitySpells" /> Communauté
            </div>
        </div>
        
        <div class="filter-group">
            <label>🏫 École de Magie</label>
            <select @bind="SelectedSchool">
                <option value="">Toutes les écoles</option>
                <option value="Abjuration">🛡️ Abjuration</option>
                <option value="Invocation">🌟 Invocation</option>
                <option value="Divination">🔮 Divination</option>
                <option value="Enchantement">💫 Enchantement</option>
                <option value="Évocation">⚡ Évocation</option>
                <option value="Illusion">🎭 Illusion</option>
                <option value="Nécromancie">💀 Nécromancie</option>
                <option value="Transmutation">🔄 Transmutation</option>
            </select>
        </div>
        
        <div class="filter-group">
            <label>📊 Niveau</label>
            <div class="level-buttons">
                @for (int i = 0; i <= 9; i++)
                {
                    <button class="level-btn @(SelectedLevels.Contains(i) ? "active" : "")"
                            @onclick="() => ToggleLevel(i)">
                        @(i == 0 ? "Cantrip" : i.ToString())
                    </button>
                }
            </div>
        </div>
    </div>
    
    <div class="spell-grid">
        @foreach (var spell in FilteredSpells)
        {
            <div class="spell-card @spell.Source.ToString().ToLower()">
                <div class="spell-header">
                    <h4>@spell.Name</h4>
                    <span class="spell-level">Niveau @spell.DndProperties.Level</span>
                </div>
                <div class="spell-details">
                    <span class="school">@spell.DndProperties.School</span>
                    <span class="casting-time">⏱️ @spell.DndProperties.CastingTime</span>
                    <span class="range">📏 @spell.DndProperties.Range</span>
                </div>
                <div class="spell-description">
                    @spell.Description.Substring(0, Math.Min(150, spell.Description.Length))...
                </div>
                <div class="spell-actions">
                    @if (UserCharacter != null && CanLearnSpell(spell))
                    {
                        <button class="btn btn-primary" @onclick="() => LearnSpell(spell)">
                            📚 Apprendre
                        </button>
                    }
                    <button class="btn btn-outline" @onclick="() => ViewSpellDetails(spell)">
                        👁️ Détails
                    </button>
                </div>
            </div>
        }
    </div>
</div>
```

---

## ⚔️ **Système d'Équipements et Échanges**

### **🛡️ Architecture Multi-Instances**
```
⚔️ ÉQUIPEMENTS OFFICIELS
├── 📊 Base SRD D&D 5e (200+ items)
├── 🏷️ Propriétés standardisées (CA, dégâts, poids)
├── 💰 Valeurs économiques équilibrées
└── 🔄 Instances infinies (copie, pas transfert)

🎨 ÉQUIPEMENTS PERSONNALISÉS
├── 👤 Créations utilisateur avec upload images
├── 🎯 Propriétés personnalisables
├── 🛡️ Validation par l'IA pour équilibrage
└── 💎 Objets uniques avec instances limitées

🤝 SYSTÈME D'ÉCHANGES DUAL
├── 🎁 MJ → Joueurs : COPIE (propositions infinies)
├── 👥 Joueur ↔ Joueur : TRANSFERT (échange réel)
├── 🔔 Notifications temps réel via SignalR
└── 📊 Historique complet des transactions
```

### **💰 Économie Dynamique**
```csharp
public class EquipmentExchangeService
{
    // MJ propose un équipement (COPIE)
    public async Task<EquipmentOffer> CreateOfferAsync(CreateOfferCommand command)
    {
        // Vérifier que l'utilisateur est MJ de la campagne
        var isGM = await _campaignService.IsGameMasterAsync(
            command.GameMasterId, command.CampaignId);
        if (!isGM) throw new UnauthorizedException();
        
        var offer = new EquipmentOffer
        {
            CampaignId = command.CampaignId,
            GameMasterId = command.GameMasterId,
            TargetPlayerId = command.TargetPlayerId,
            EquipmentId = command.EquipmentId,
            Quantity = command.Quantity,
            Message = command.Message,
            Status = OfferStatus.Pending
        };
        
        await _unitOfWork.EquipmentOffers.AddAsync(offer);
        await _unitOfWork.SaveChangesAsync();
        
        // Notification temps réel
        await _notificationService.NotifyEquipmentOfferAsync(offer);
        
        return offer;
    }
    
    // Joueur accepte → COPIE vers inventaire
    public async Task<ServiceResult> AcceptOfferAsync(int offerId, int playerId)
    {
        var offer = await _unitOfWork.EquipmentOffers.GetWithDetailsAsync(offerId);
        if (offer.TargetPlayerId != playerId) 
            return ServiceResult.Failure("Non autorisé");
        
        // COPIER l'équipement vers l'inventaire du joueur
        var characterEquipment = new CharacterEquipment
        {
            CharacterId = offer.TargetCharacterId,
            EquipmentId = offer.EquipmentId,
            Quantity = offer.Quantity,
            AcquiredFrom = "GM_Offer",
            AcquiredAt = DateTime.UtcNow
        };
        
        await _unitOfWork.CharacterEquipment.AddAsync(characterEquipment);
        
        offer.Status = OfferStatus.Accepted;
        offer.RespondedAt = DateTime.UtcNow;
        
        await _unitOfWork.SaveChangesAsync();
        
        return ServiceResult.Success("Équipement ajouté à votre inventaire !");
    }
}
```

### **🎒 Interface Inventaire Intelligent**
```razor
<div class="inventory-panel">
    <div class="inventory-header">
        <h3>🎒 Inventaire de @Character.Name</h3>
        <div class="inventory-stats">
            <span>⚖️ Poids: @TotalWeight.ToString("F1")/@MaxWeight kg</span>
            <span>💰 Valeur: @TotalValue po</span>
        </div>
    </div>
    
    <div class="equipment-slots">
        <div class="equipment-grid">
            <!-- Emplacements équipés -->
            <div class="slot main-hand" @ondrop="DropEquipment" @ondragover="AllowDrop">
                @if (Character.MainHandWeapon != null)
                {
                    <EquipmentIcon Equipment="Character.MainHandWeapon" 
                                   OnUnequip="UnequipItem" />
                }
                else
                {
                    <div class="empty-slot">⚔️ Main droite</div>
                }
            </div>
            
            <div class="slot armor" @ondrop="DropEquipment" @ondragover="AllowDrop">
                @if (Character.Armor != null)
                {
                    <EquipmentIcon Equipment="Character.Armor" 
                                   OnUnequip="UnequipItem" />
                }
                else
                {
                    <div class="empty-slot">🛡️ Armure</div>
                }
            </div>
            
            <!-- Autres emplacements... -->
        </div>
        
        <div class="calculated-stats">
            <h4>📊 Statistiques Calculées</h4>
            <div class="stat">CA: @CalculatedAC</div>
            <div class="stat">Vitesse: @CalculatedSpeed m</div>
            <div class="stat">Bonus attaque: +@AttackBonus</div>
        </div>
    </div>
    
    <div class="inventory-items">
        <div class="tabs">
            <button class="tab @(ActiveTab == "weapons" ? "active" : "")" 
                    @onclick="() => SetActiveTab('weapons')">⚔️ Armes</button>
            <button class="tab @(ActiveTab == "armor" ? "active" : "")" 
                    @onclick="() => SetActiveTab('armor')">🛡️ Armures</button>
            <button class="tab @(ActiveTab == "consumables" ? "active" : "")" 
                    @onclick="() => SetActiveTab('consumables')">🧪 Consommables</button>
            <button class="tab @(ActiveTab == "other" ? "active" : "")" 
                    @onclick="() => SetActiveTab('other')">📦 Autre</button>
        </div>
        
        <div class="items-grid">
            @foreach (var item in FilteredInventoryItems)
            {
                <div class="inventory-item" draggable="true" 
                     @ondragstart="() => StartDrag(item)">
                    <div class="item-icon">
                        @if (!string.IsNullOrEmpty(item.Equipment.ImageUrl))
                        {
                            <img src="@item.Equipment.ImageUrl" alt="@item.Equipment.Name" />
                        }
                        else
                        {
                            <div class="default-icon">@GetEquipmentIcon(item.Equipment.Type)</div>
                        }
                    </div>
                    <div class="item-details">
                        <div class="item-name">@item.Equipment.Name</div>
                        <div class="item-quantity">×@item.Quantity</div>
                    </div>
                    <div class="item-actions">
                        @if (CanEquip(item.Equipment))
                        {
                            <button class="btn-icon" @onclick="() => EquipItem(item)" 
                                    title="Équiper">⚡</button>
                        }
                        <button class="btn-icon" @onclick="() => ShowTradeModal(item)" 
                                title="Échanger">🤝</button>
                        <button class="btn-icon" @onclick="() => DropItem(item)" 
                                title="Abandonner">🗑️</button>
                    </div>
                </div>
            }
        </div>
    </div>
</div>
```

---

## 🏰 **Campagnes et Chapitres Narratifs**

### **📚 Structure Narrative Évolutive**
```
🏰 CAMPAGNE
├── 📊 Métadonnées (Nom, GameType, Difficulté)
├── 👑 Maître de Jeu (Créateur unique)
├── 👥 Joueurs (Invitations et statuts)
├── ⚙️ Paramètres (Max joueurs, règles house)
└── 📖 Chapitres (Structure narrative)

📖 CHAPITRE
├── 📝 Contenu narratif (Texte riche + médias)
├── 🎯 Objectifs (Principaux + secondaires)
├── 🧙‍♂️ PNJ et Monstres contextuels
├── 🎲 Encounters de combat
├── 💰 Récompenses (XP, équipements, sorts)
└── 🔄 Transitions et conditions
```

### **🎭 PNJ Intelligents avec Comportements**
```csharp
public class NPCBehaviorEngine
{
    public NPCReaction GetReactionToPlayer(NPC npc, Character player, string context)
    {
        var behaviorData = JsonSerializer.Deserialize<NPCBehavior>(npc.Behaviors);
        
        // Analyser l'attitude du joueur
        var playerAttitude = AnalyzePlayerAttitude(player, context);
        
        // Déterminer la réaction du PNJ
        var reaction = behaviorData.PlayerAttitude.ToLower() switch
        {
            "friendly" when playerAttitude == PlayerAttitude.Friendly => 
                new NPCReaction { Type = "Enthusiastic", DialogueOptions = GetFriendlyDialogue(npc) },
            "suspicious" when playerAttitude == PlayerAttitude.Aggressive => 
                new NPCReaction { Type = "Hostile", DialogueOptions = GetHostileDialogue(npc) },
            "neutral" => 
                new NPCReaction { Type = "Cautious", DialogueOptions = GetNeutralDialogue(npc) },
            _ => new NPCReaction { Type = "Default", DialogueOptions = GetDefaultDialogue(npc) }
        };
        
        return reaction;
    }
}

public class NPCBehavior
{
    public string PlayerAttitude { get; set; } = "neutral"; // friendly, neutral, hostile, suspicious
    public string NPCResponse { get; set; } = string.Empty;
    public string BackgroundContext { get; set; } = string.Empty;
    public List<string> DialogueOptions { get; set; } = new();
    public Dictionary<string, object> Actions { get; set; } = new();
}
```

### **⚡ Progression Automatique et Sauvegarde**
```csharp
public class ChapterProgressionService
{
    public async Task<ChapterProgressResult> AdvanceChapterAsync(
        string sessionId, int chapterId, string progressionType)
    {
        var session = await _sessionService.GetActiveSessionAsync(sessionId);
        var chapter = await _chapterService.GetChapterAsync(chapterId);
        
        // Vérifier les conditions de progression
        var canAdvance = await CheckProgressionConditionsAsync(chapter, session);
        if (!canAdvance.Success)
            return ChapterProgressResult.Failure(canAdvance.ErrorMessage);
        
        // Calculer récompenses XP
        var xpRewards = CalculateChapterXPRewards(chapter, session.Participants);
        
        // Distribuer récompenses
        await DistributeRewardsAsync(chapter.Rewards, session.Participants);
        
        // Sauvegarder l'état
        var saveState = new SessionSave
        {
            SessionId = sessionId,
            ChapterId = chapterId,
            SaveDescription = $"Fin du chapitre: {chapter.Title}",
            GameState = JsonSerializer.Serialize(session.CurrentState),
            SavedAt = DateTime.UtcNow
        };
        
        await _unitOfWork.SessionSaves.AddAsync(saveState);
        
        // Déverrouiller le chapitre suivant
        var nextChapter = await GetNextChapterAsync(chapter);
        if (nextChapter != null)
        {
            nextChapter.Status = "Active";
            session.CurrentChapterId = nextChapter.Id;
        }
        
        await _unitOfWork.SaveChangesAsync();
        
        // Notifier tous les participants via SignalR
        await _sessionHub.Clients.Group($"session_{sessionId}")
            .SendAsync("ChapterCompleted", new
            {
                CompletedChapter = chapter.Title,
                XPGained = xpRewards,
                NextChapter = nextChapter?.Title,
                Rewards = chapter.Rewards
            });
        
        return ChapterProgressResult.Success(nextChapter);
    }
}
```

---

## ⚔️ **Combat Temps Réel Collaboratif**

### **🎯 Initiative et Gestion des Tours**
```csharp
[Authorize]
public class CombatHub : Hub
{
    public async Task RollInitiative(int combatId, int characterId, int initiativeRoll)
    {
        var userId = Context.User.GetUserId();
        
        // Valider que le joueur peut agir pour ce personnage
        var canAct = await _combatService.CanActForCharacterAsync(userId, characterId);
        if (!canAct) return;
        
        // Enregistrer l'initiative
        var participant = await _combatService.SetInitiativeAsync(combatId, characterId, initiativeRoll);
        
        // Recalculer l'ordre des tours
        var turnOrder = await _combatService.CalculateTurnOrderAsync(combatId);
        
        // Diffuser à tous les participants
        await Clients.Group($"combat_{combatId}")
            .SendAsync("InitiativeUpdated", new
            {
                CharacterId = characterId,
                Initiative = initiativeRoll,
                TurnOrder = turnOrder,
                NextToAct = turnOrder.FirstOrDefault()
            });
        
        // Si c'est la dernière initiative, démarrer le combat
        var allInitiativesRolled = await _combatService.AllInitiativesRolledAsync(combatId);
        if (allInitiativesRolled)
        {
            await Clients.Group($"combat_{combatId}")
                .SendAsync("CombatStarted", new
                {
                    TurnOrder = turnOrder,
                    CurrentTurn = turnOrder.First(),
                    Message = "🎯 Le combat commence ! À toi de jouer, " + turnOrder.First().CharacterName
                });
        }
    }
    
    public async Task ExecuteAction(int combatId, CombatActionDto actionDto)
    {
        var userId = Context.User.GetUserId();
        
        try
        {
            // Valider que c'est le tour du joueur
            var canAct = await _combatService.CanPlayerActAsync(combatId, userId);
            if (!canAct.Success)
            {
                await Clients.Caller.SendAsync("ActionError", canAct.ErrorMessage);
                return;
            }
            
            // Exécuter l'action avec calculs D&D
            var result = await _combatService.ExecuteActionAsync(new ExecuteActionCommand
            {
                CombatId = combatId,
                UserId = userId,
                ActionType = actionDto.ActionType,
                TargetId = actionDto.TargetId,
                SpellId = actionDto.SpellId,
                EquipmentId = actionDto.EquipmentId
            });
            
            // Diffuser le résultat avec animations
            await Clients.Group($"combat_{combatId}")
                .SendAsync("ActionExecuted", new
                {
                    Action = result.ActionDescription,
                    AttackerName = result.AttackerName,
                    TargetName = result.TargetName,
                    DamageDealt = result.DamageDealt,
                    IsCritical = result.IsCritical,
                    RollDetails = result.RollDetails,
                    Animation = GetActionAnimation(actionDto.ActionType)
                });
            
            // Effets spéciaux pour les coups critiques
            if (result.IsCritical)
            {
                await Clients.Group($"combat_{combatId}")
                    .SendAsync("CriticalHit", new
                    {
                        Message = $"💥 COUP CRITIQUE ! {result.AttackerName} inflige {result.DamageDealt} dégâts !",
                        Sound = "critical-hit.mp3",
                        Animation = "critical-explosion"
                    });
            }
            
            // Vérifier si la cible est vaincue
            if (result.TargetDefeated)
            {
                await Clients.Group($"combat_{combatId}")
                    .SendAsync("TargetDefeated", new
                    {
                        DefeatedName = result.TargetName,
                        Message = $"💀 {result.TargetName} est vaincu(e) !",
                        RemainingEnemies = result.RemainingEnemies
                    });
            }
            
            // Passer au tour suivant
            var nextTurn = await _combatService.AdvanceToNextTurnAsync(combatId);
            if (nextTurn != null)
            {
                var nextPlayerUserId = await _combatService.GetPlayerUserIdAsync(nextTurn.CharacterId);
                
                // Notifier le joueur suivant
                await Clients.User(nextPlayerUserId.ToString())
                    .SendAsync("YourTurn", new
                    {
                        CharacterName = nextTurn.CharacterName,
                        TurnTimeLimit = result.TurnTimeLimit,
                        AvailableActions = nextTurn.AvailableActions,
                        Message = $"🎯 À ton tour, {nextTurn.CharacterName} !",
                        Sound = "turn-notification.mp3"
                    });
                
                // Mettre à jour l'interface pour tous
                await Clients.Group($"combat_{combatId}")
                    .SendAsync("TurnChanged", nextTurn);
            }
            
            // Vérifier si le combat est terminé
            if (result.CombatEnded)
            {
                await Clients.Group($"combat_{combatId}")
                    .SendAsync("CombatEnded", new
                    {
                        Winner = result.Winner,
                        Duration = result.CombatDuration,
                        XPGained = result.XPRewards,
                        Loot = result.LootDrops,
                        Summary = result.CombatSummary
                    });
            }
            
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("ActionError", 
                "Une erreur est survenue lors de l'exécution de l'action");
        }
    }
}
```

### **🎮 Interface Combat Immersive**
```razor
<div class="combat-interface">
    <div class="combat-header">
        <h2>⚔️ Combat - Round @CurrentRound</h2>
        <div class="turn-indicator">
            @if (IsMyTurn)
            {
                <span class="my-turn">🎯 À ton tour !</span>
                @if (TurnTimeLimit > 0)
                {
                    <div class="turn-timer">⏱️ @RemainingTime secondes</div>
                }
            }
            else
            {
                <span class="waiting">⏳ Tour de @CurrentPlayerName</span>
            }
        </div>
    </div>
    
    <div class="combat-main">
        <div class="participants-panel">
            <h3>👥 Participants</h3>
            <div class="turn-order">
                @foreach (var participant in TurnOrder)
                {
                    <div class="participant @(participant.IsCurrent ? "current-turn" : "") @(participant.IsPlayer ? "player" : "npc")">
                        <div class="initiative">@participant.Initiative</div>
                        <div class="character-info">
                            <div class="name">@participant.Name</div>
                            <div class="health-bar">
                                <div class="health-fill" style="width: @(participant.HealthPercentage)%"></div>
                                <span class="health-text">@participant.CurrentHP/@participant.MaxHP</span>
                            </div>
                        </div>
                        <div class="status-effects">
                            @foreach (var effect in participant.StatusEffects)
                            {
                                <span class="status-effect @effect.Type">@effect.Icon</span>
                            }
                        </div>
                    </div>
                }
            </div>
        </div>
        
        @if (IsMyTurn && MyCharacter != null)
        {
            <div class="action-panel">
                <h3>⚡ Actions Disponibles</h3>
                <div class="action-buttons">
                    <button class="action-btn attack" @onclick="PrepareAttack">
                        ⚔️ Attaquer
                    </button>
                    <button class="action-btn spell" @onclick="OpenSpellModal">
                        🪄 Lancer un Sort
                    </button>
                    <button class="action-btn move" @onclick="Move">
                        🏃 Se Déplacer
                    </button>
                    <button class="action-btn dodge" @onclick="Dodge">
                        🛡️ Esquiver
                    </button>
                    <button class="action-btn help" @onclick="Help">
                        🤝 Aider
                    </button>
                    <button class="action-btn end-turn" @onclick="EndTurn">
                        ✅ Terminer le Tour
                    </button>
                </div>
            </div>
        }
        
        <div class="combat-log">
            <h3>📜 Historique du Combat</h3>
            <div class="log-entries">
                @foreach (var entry in CombatLog.TakeLast(10))
                {
                    <div class="log-entry @entry.Type">
                        <span class="timestamp">[@entry.Timestamp.ToString("HH:mm:ss")]</span>
                        <span class="message">@entry.Message</span>
                    </div>
                }
            </div>
        </div>
    </div>
</div>

@* Modal de sélection de sorts *@
@if (ShowSpellModal)
{
    <div class="modal spell-modal">
        <div class="modal-content">
            <h3>🪄 Choisir un Sort</h3>
            <div class="spell-slots">
                @for (int level = 1; level <= 9; level++)
                {
                    var slotsRemaining = GetSpellSlotsRemaining(level);
                    if (slotsRemaining > 0)
                    {
                        <div class="slot-level">
                            <h4>Niveau @level (@slotsRemaining emplacements)</h4>
                            <div class="spells-list">
                                @foreach (var spell in GetKnownSpellsByLevel(level))
                                {
                                    <button class="spell-option" @onclick="() => CastSpell(spell, level)">
                                        <div class="spell-name">@spell.Name</div>
                                        <div class="spell-details">
                                            @spell.DndProperties.School | @spell.DndProperties.CastingTime
                                        </div>
                                    </button>
                                }
                            </div>
                        </div>
                    }
                }
            </div>
            <button class="btn btn-secondary" @onclick="CloseSpellModal">Annuler</button>
        </div>
    </div>
}
```

---

## 📊 **Métriques Métier et Analytics**

### **🎯 Tracking Comportemental JDR**
```csharp
public class GameplayAnalyticsService
{
    public async Task TrackPlayerActionAsync(PlayerActionEvent actionEvent)
    {
        // Enregistrer l'action pour analytics
        var trackingData = new PlayerActivity
        {
            UserId = actionEvent.UserId,
            ActionType = actionEvent.ActionType,
            Context = JsonSerializer.Serialize(actionEvent.Context),
            Timestamp = DateTime.UtcNow,
            SessionId = actionEvent.SessionId,
            CampaignId = actionEvent.CampaignId
        };
        
        await _unitOfWork.PlayerActivities.AddAsync(trackingData);
        
        // Calculer métriques en temps réel
        await UpdatePlayerStatsAsync(actionEvent.UserId, actionEvent);
        
        await _unitOfWork.SaveChangesAsync();
    }
    
    public async Task<PlayerEngagementReport> GenerateEngagementReportAsync(int userId, TimeSpan period)
    {
        var activities = await _unitOfWork.PlayerActivities
            .Where(a => a.UserId == userId && a.Timestamp >= DateTime.UtcNow.Subtract(period))
            .ToListAsync();
            
        return new PlayerEngagementReport
        {
            TotalSessions = activities.Select(a => a.SessionId).Distinct().Count(),
            TotalPlayTime = CalculateTotalPlayTime(activities),
            FavoriteRole = GetMostFrequentRole(activities),
            PreferredGameType = GetPreferredGameType(activities),
            CombatParticipation = CalculateCombatParticipation(activities),
            SocialInteractions = CalculateSocialScore(activities),
            CreativityScore = CalculateCreativityScore(activities),
            EngagementTrend = CalculateEngagementTrend(activities)
        };
    }
}
```

### **🏆 Système de Succès Gamifié**
```csharp
public class AchievementEngine
{
    public async Task CheckAchievementsAsync(PlayerActionEvent actionEvent)
    {
        var unlockedAchievements = new List<Achievement>();
        
        // Vérifier les succès selon l'action
        switch (actionEvent.ActionType)
        {
            case "character_created":
                var isFirstCharacter = await IsFirstCharacterAsync(actionEvent.UserId);
                if (isFirstCharacter)
                {
                    await UnlockAchievementAsync(actionEvent.UserId, "first_character");
                }
                break;
                
            case "critical_hit":
                var criticalCount = await GetCriticalHitCountAsync(actionEvent.UserId);
                if (criticalCount == 1)
                {
                    await UnlockAchievementAsync(actionEvent.UserId, "first_critical");
                }
                else if (criticalCount == 10)
                {
                    await UnlockAchievementAsync(actionEvent.UserId, "critical_master");
                }
                break;
                
            case "spell_learned":
                var spellCount = await GetLearnedSpellCountAsync(actionEvent.UserId);
                if (spellCount == 5)
                {
                    await UnlockAchievementAsync(actionEvent.UserId, "spell_learner");
                }
                else if (spellCount == 25)
                {
                    await UnlockAchievementAsync(actionEvent.UserId, "spell_master");
                }
                break;
        }
    }
}
```

---

Cette documentation détaillée des **fonctionnalités métier** montre comment **Chronique des Mondes** révolutionne l'expérience JDR avec des mécaniques automatisées intelligentes, des échanges économiques dynamiques, et une collaboration temps réel immersive ! 🎲⚔️

**Prêt pour la page suivante ?** 🚀