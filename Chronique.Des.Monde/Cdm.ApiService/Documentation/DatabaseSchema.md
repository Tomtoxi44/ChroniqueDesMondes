# Schéma de Base de Données - Chronique des Mondes

Ce document présente le schéma de base de données actuel et les évolutions prévues pour les systèmes de sorts et équipements.

## 📊 Vue d'ensemble

### État Actuel vs Prévu

```mermaid
erDiagram
    %% ========================================
    %% TABLES ACTUELLEMENT IMPLÉMENTÉES ✅
    %% ========================================
    
    Users {
        int Id PK
        string UserName
        string UserEmail
        string Password
    }
    
    Characters {
        int Id PK
        int UserId FK
        string Name
        string Picture
        string Background
        int Life
        int Leveling
        string GameType
    }
    
    CharactersDnd {
        int Id PK
        int UserId FK
        string Class
        int ClassArmor
        int Strong
        int AdditionalStrong
        int Dexterity
        int AdditionalDexterity
        int Constitution
        int AdditionalConstitution
        int Intelligence
        int AdditionalIntelligence
        int Wisdoms
        int AdditionalWisdoms
        int Charism
        int AdditionalCharism
    }
    
    %% ========================================
    %% NOUVELLES TABLES À CRÉER 🚧
    %% ========================================
    
    Spells {
        int Id PK
        string Name
        string Description
        string ImageUrl
        string GameType
        int CreatedByUserId FK
        bool IsPublic
        json Tags
        json DndProperties
        json SkyrimProperties
        json GenericProperties
        datetime CreatedAt
        datetime UpdatedAt
    }
    
    Equipment {
        int Id PK
        string Name
        string Description
        string ImageUrl
        string GameType
        int CreatedByUserId FK
        bool IsPublic
        json Tags
        json DndProperties
        json GenericProperties
        datetime CreatedAt
        datetime UpdatedAt
    }
    
    Campaigns {
        int Id PK
        string Name
        string Description
        string GameType
        int GameMasterId FK
        bool IsPublic
        int CurrentChapterId FK
        datetime CreatedAt
        datetime UpdatedAt
    }
    
    Chapters {
        int Id PK
        int CampaignId FK
        int ChapterNumber
        string Title
        string Content
        int OrderIndex
        string Status
        datetime CreatedAt
        datetime UpdatedAt
    }
    
    NPCs {
        int Id PK
        int ChapterId FK
        string Name
        string Description
        string GameType
        json DndProperties
        json GenericProperties
        datetime CreatedAt
    }
    
    CampaignPlayers {
        int CampaignId FK
        int UserId FK
        string Status
        datetime JoinedAt
        datetime InvitedAt
        bool IsActive
    }
    
    %% ========================================
    %% NOUVELLES TABLES SESSIONS 🆕
    %% ========================================
    
    Sessions {
        string SessionId PK
        int CampaignId FK
        int GameMasterId FK
        string Status
        datetime StartedAt
        datetime EndedAt
        int CurrentChapterId FK
        json SessionSettings
        datetime LastSavedAt
    }
    
    SessionParticipants {
        string SessionId FK
        int UserId FK
        int CharacterId FK
        string Status
        datetime JoinedAt
        datetime LastSeenAt
        bool IsOnline
    }
    
    CampaignInvitations {
        int Id PK
        int CampaignId FK
        int InviterId FK
        string InviteeEmail
        int InviteeUserId FK
        string Status
        string Message
        datetime CreatedAt
        datetime ExpiresAt
        datetime RespondedAt
    }
    
    CampaignProgress {
        int CampaignId FK
        int UserId FK
        int CurrentChapterId FK
        int CompletedChapters
        int TotalChapters
        decimal ProgressPercentage
        datetime LastUpdated
    }
    
    SessionSaves {
        int Id PK
        string SessionId FK
        int SaveSlot
        json GameState
        int ChapterId FK
        string Description
        datetime CreatedAt
    }
    
    Combats {
        int Id PK
        string SessionId FK
        int ChapterId FK
        string Status
        int CurrentTurn
        json TurnOrder
        datetime StartedAt
        datetime EndedAt
    }
    
    CombatParticipants {
        int CombatId FK
        int ParticipantId
        string ParticipantType
        int CharacterId FK
        int NpcId FK
        int Initiative
        int CurrentHitPoints
        json StatusEffects
        bool IsActive
    }
    
    Notifications {
        int Id PK
        int UserId FK
        string Type
        string Title
        string Message
        json Data
        bool IsRead
        string DeliveryMethod
        datetime CreatedAt
        datetime ReadAt
        datetime ExpiresAt
    }
    
    PasswordResets {
        int Id PK
        int UserId FK
        string ResetToken
        datetime CreatedAt
        datetime ExpiresAt
        bool IsUsed
        datetime UsedAt
    }
    
    %% ========================================
    %% TABLES STATISTIQUES ET SUCCÈS 🏆
    %% ========================================
    
    PlayerStatistics {
        int Id PK
        int UserId FK
        string StatType
        string StatCategory
        decimal StatValue
        json AdditionalData
        string SessionId FK
        int CharacterId FK
        int CampaignId FK
        datetime RecordedAt
    }
    
    DiceRolls {
        int Id PK
        int UserId FK
        string SessionId FK
        int CharacterId FK
        string DiceType
        int Result
        string Context
        int TargetDC
        bool IsSuccess
        bool IsCritical
        int AdditionalModifiers
        datetime RolledAt
    }
    
    Achievements {
        string Id PK
        string Name
        string Description
        string Icon
        string Category
        string Rarity
        int RequiredValue
        json RequiredData
        bool IsActive
        datetime CreatedAt
    }
    
    PlayerAchievements {
        int UserId FK
        string AchievementId FK
        decimal Progress
        int CurrentValue
        bool IsUnlocked
        datetime UnlockedAt
        int UnlockedWithCharacterId FK
        json UnlockContext
    }
    
    CombatActions {
        int Id PK
        int CombatId FK
        int UserId FK
        int CharacterId FK
        string ActionType
        string TargetType
        int TargetId
        int DamageDealt
        int DamageTaken
        bool IsHit
        bool IsCritical
        int SpellId FK
        int EquipmentId FK
        int RoundNumber
        int ActionOrder
        json ActionData
        datetime ExecutedAt
    }
    
    SessionActivities {
        int Id PK
        string SessionId FK
        int UserId FK
        string ActivityType
        json ActivityData
        int ExperienceGained
        json ItemsGained
        datetime ActivityTime
    }
    
    PlayerReports {
        int Id PK
        int UserId FK
        string ReportType
        string ReportPeriod
        datetime StartDate
        datetime EndDate
        json ReportData
        datetime GeneratedAt
    }
```

## 🏗️ Détail des Tables

### Tables Actuellement Implémentées ✅

#### **Users**
```sql
CREATE TABLE Users (
    Id int IDENTITY(1,1) PRIMARY KEY,
    UserName nvarchar(20) NOT NULL,
    UserEmail nvarchar(255) NOT NULL,
    Password nvarchar(255) NOT NULL,
    
    CONSTRAINT UQ_Users_UserEmail UNIQUE (UserEmail)
);
```

#### **Characters (Abstrait)**
```sql
-- Note: Cette table existe conceptuellement via l'héritage EF
-- Les propriétés communes sont dans les tables dérivées
```

#### **CharactersDnd**
```sql
CREATE TABLE CharactersDnd (
    Id int IDENTITY(1,1) PRIMARY KEY,
    UserId int NOT NULL,
    Name nvarchar(100) NOT NULL,
    Picture nvarchar(255),
    Background nvarchar(max),
    Life int NOT NULL DEFAULT 0,
    Leveling int NOT NULL DEFAULT 1,
    
    -- Propriétés D&D spécifiques
    Class nvarchar(50) NOT NULL,
    ClassArmor int NOT NULL DEFAULT 10,
    Strong int NOT NULL DEFAULT 10,
    AdditionalStrong int NOT NULL DEFAULT 0,
    Dexterity int NOT NULL DEFAULT 10,
    AdditionalDexterity int NOT NULL DEFAULT 0,
    Constitution int NOT NULL DEFAULT 10,
    AdditionalConstitution int NOT NULL DEFAULT 0,
    Intelligence int NOT NULL DEFAULT 10,
    AdditionalIntelligence int NOT NULL DEFAULT 0,
    Wisdoms int NOT NULL DEFAULT 10,
    AdditionalWisdoms int NOT NULL DEFAULT 0,
    Charism int NOT NULL DEFAULT 10,
    AdditionalCharism int NOT NULL DEFAULT 0,
    
    CONSTRAINT FK_CharactersDnd_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
);
```

### Nouvelles Tables à Créer 🚧

#### **Spells**
```sql
CREATE TABLE Spells (
    Id int IDENTITY(1,1) PRIMARY KEY,
    Name nvarchar(100) NOT NULL,
    Description nvarchar(max) NOT NULL,
    ImageUrl nvarchar(255),
    GameType nvarchar(20) NOT NULL DEFAULT 'generic',
    CreatedByUserId int NOT NULL, -- 0 = Administrateur
    IsPublic bit NOT NULL DEFAULT 0,
    Tags nvarchar(max), -- JSON array
    
    -- Propriétés spécialisées stockées en JSON
    DndProperties nvarchar(max), -- JSON pour propriétés D&D
    SkyrimProperties nvarchar(max), -- JSON pour propriétés Skyrim
    GenericProperties nvarchar(max), -- JSON pour propriétés génériques
    
    CreatedAt datetime2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt datetime2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_Spells_Users FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id),
    CONSTRAINT CK_Spells_GameType CHECK (GameType IN ('generic', 'dnd', 'skyrim'))
);

-- Index pour optimiser les requêtes
CREATE INDEX IX_Spells_GameType_IsPublic ON Spells (GameType, IsPublic);
CREATE INDEX IX_Spells_CreatedByUserId ON Spells (CreatedByUserId);
```

**Exemple de DndProperties JSON :**
```json
{
  "level": 3,
  "school": "Évocation",
  "castingTime": "1 action",
  "range": "45 mètres",
  "duration": "Instantané",
  "components": ["V", "S", "M"],
  "damageFormula": "8d6",
  "requiresAttackRoll": false,
  "requiresSavingThrow": true,
  "savingThrowAbility": "Dextérité"
}
```

#### **Equipment**
```sql
CREATE TABLE Equipment (
    Id int IDENTITY(1,1) PRIMARY KEY,
    Name nvarchar(100) NOT NULL,
    Description nvarchar(max) NOT NULL,
    ImageUrl nvarchar(255),
    GameType nvarchar(20) NOT NULL DEFAULT 'generic',
    CreatedByUserId int NOT NULL, -- 0 = Administrateur
    IsPublic bit NOT NULL DEFAULT 0,
    Tags nvarchar(max), -- JSON array
    
    -- Propriétés spécialisées stockées en JSON
    DndProperties nvarchar(max), -- JSON pour propriétés D&D
    GenericProperties nvarchar(max), -- JSON pour propriétés génériques
    
    CreatedAt datetime2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt datetime2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_Equipment_Users FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id),
    CONSTRAINT CK_Equipment_GameType CHECK (GameType IN ('generic', 'dnd', 'skyrim'))
);

-- Index pour optimiser les requêtes
CREATE INDEX IX_Equipment_GameType_IsPublic ON Equipment (GameType, IsPublic);
CREATE INDEX IX_Equipment_CreatedByUserId ON Equipment (CreatedByUserId);
```

**Exemple de DndProperties JSON :**
```json
{
  "equipmentType": "Weapon",
  "weaponCategory": "Martial",
  "damageFormula": "1d8 + mod",
  "damageType": "Tranchant",
  "properties": ["Versatile (1d10)", "Finesse"],
  "rarity": "Commun",
  "requiresAttunement": false,
  "armorClassBase": null,
  "armorClassDexBonus": null
}
```

#### **Campaigns**
```sql
CREATE TABLE Campaigns (
    Id int IDENTITY(1,1) PRIMARY KEY,
    Name nvarchar(100) NOT NULL,
    Description nvarchar(max),
    GameType nvarchar(20) NOT NULL DEFAULT 'generic',
    GameMasterId int NOT NULL,
    IsPublic bit NOT NULL DEFAULT 0,
    CurrentChapterId int,
    
    CreatedAt datetime2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt datetime2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_Campaigns_GameMaster FOREIGN KEY (GameMasterId) REFERENCES Users(Id),
    CONSTRAINT CK_Campaigns_GameType CHECK (GameType IN ('generic', 'dnd', 'skyrim'))
);
```

#### **Chapters**
```sql
CREATE TABLE Chapters (
    Id int IDENTITY(1,1) PRIMARY KEY,
    CampaignId int NOT NULL,
    ChapterNumber int NOT NULL,
    Title nvarchar(100) NOT NULL,
    Content nvarchar(max),
    OrderIndex int,
    Status nvarchar(20) NOT NULL DEFAULT 'Pending',
    
    CreatedAt datetime2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt datetime2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_Chapters_Campaigns FOREIGN KEY (CampaignId) REFERENCES Campaigns(Id),
    CONSTRAINT CK_Chapters_Status CHECK (Status IN ('Pending', 'Active', 'Completed')),
    CONSTRAINT UQ_Chapters_CampaignId_ChapterNumber UNIQUE (CampaignId, ChapterNumber)
);
```

#### **CharacterSpells (Table de liaison)**
```sql
CREATE TABLE CharacterSpells (
    CharacterId int NOT NULL,
    SpellId int NOT NULL,
    LearnedDate datetime2 NOT NULL DEFAULT GETDATE(),
    IsPrepared bit NOT NULL DEFAULT 1,
    Notes nvarchar(500),
    
    CONSTRAINT PK_CharacterSpells PRIMARY KEY (CharacterId, SpellId),
    CONSTRAINT FK_CharacterSpells_Character FOREIGN KEY (CharacterId) REFERENCES CharactersDnd(Id),
    CONSTRAINT FK_CharacterSpells_Spell FOREIGN KEY (SpellId) REFERENCES Spells(Id)
);
```

#### **CharacterEquipment (Table de liaison)**
```sql
CREATE TABLE CharacterEquipment (
    CharacterId int NOT NULL,
    EquipmentId int NOT NULL,
    Quantity int NOT NULL DEFAULT 1,
    IsEquipped bit NOT NULL DEFAULT 0,
    CustomProperties nvarchar(max), -- JSON pour propriétés custom
    AcquiredDate datetime2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT PK_CharacterEquipment PRIMARY KEY (CharacterId, EquipmentId),
    CONSTRAINT FK_CharacterEquipment_Character FOREIGN KEY (CharacterId) REFERENCES CharactersDnd(Id),
    CONSTRAINT FK_CharacterEquipment_Equipment FOREIGN KEY (EquipmentId) REFERENCES Equipment(Id),
    CONSTRAINT CK_CharacterEquipment_Quantity CHECK (Quantity > 0)
);
```

#### **EquipmentOffers (Propositions MJ → Joueur)**
```sql
CREATE TABLE EquipmentOffers (
    Id int IDENTITY(1,1) PRIMARY KEY,
    CampaignId int NOT NULL,
    GameMasterId int NOT NULL,
    TargetPlayerId int NOT NULL,
    EquipmentId int NOT NULL,
    Quantity int NOT NULL DEFAULT 1,
    Message nvarchar(500),
    Status nvarchar(20) NOT NULL DEFAULT 'Pending',
    
    CreatedAt datetime2 NOT NULL DEFAULT GETDATE(),
    RespondedAt datetime2,
    
    CONSTRAINT FK_EquipmentOffers_Campaign FOREIGN KEY (CampaignId) REFERENCES Campaigns(Id),
    CONSTRAINT FK_EquipmentOffers_GameMaster FOREIGN KEY (GameMasterId) REFERENCES Users(Id),
    CONSTRAINT FK_EquipmentOffers_TargetPlayer FOREIGN KEY (TargetPlayerId) REFERENCES Users(Id),
    CONSTRAINT FK_EquipmentOffers_Equipment FOREIGN KEY (EquipmentId) REFERENCES Equipment(Id),
    CONSTRAINT CK_EquipmentOffers_Status CHECK (Status IN ('Pending', 'Accepted', 'Declined', 'Cancelled'))
);
```

#### **EquipmentTrades (Échanges Joueur → Joueur)**
```sql
CREATE TABLE EquipmentTrades (
    Id int IDENTITY(1,1) PRIMARY KEY,
    CampaignId int NOT NULL,
    FromPlayerId int NOT NULL,
    ToPlayerId int NOT NULL,
    EquipmentId int NOT NULL,
    Quantity int NOT NULL DEFAULT 1,
    Message nvarchar(500),
    Status nvarchar(20) NOT NULL DEFAULT 'Proposed',
    
    CreatedAt datetime2 NOT NULL DEFAULT GETDATE(),
    CompletedAt datetime2,
    
    CONSTRAINT FK_EquipmentTrades_Campaign FOREIGN KEY (CampaignId) REFERENCES Campaigns(Id),
    CONSTRAINT FK_EquipmentTrades_FromPlayer FOREIGN KEY (FromPlayerId) REFERENCES Users(Id),
    CONSTRAINT FK_EquipmentTrades_ToPlayer FOREIGN KEY (ToPlayerId) REFERENCES Users(Id),
    CONSTRAINT FK_EquipmentTrades_Equipment FOREIGN KEY (EquipmentId) REFERENCES Equipment(Id),
    CONSTRAINT CK_EquipmentTrades_Status CHECK (Status IN ('Proposed', 'Accepted', 'Declined', 'Completed', 'Cancelled')),
    CONSTRAINT CK_EquipmentTrades_DifferentPlayers CHECK (FromPlayerId != ToPlayerId)
);
```

#### **Sessions (Gestion des Séances de Jeu)**
```sql
CREATE TABLE Sessions (
    SessionId nvarchar(50) PRIMARY KEY,  -- Format: sess_abc123
    CampaignId int NOT NULL,
    GameMasterId int NOT NULL,
    Status nvarchar(20) NOT NULL DEFAULT 'active',
    StartedAt datetime2 NOT NULL DEFAULT GETDATE(),
    EndedAt datetime2,
    CurrentChapterId int,
    SessionSettings nvarchar(max),  -- JSON: autosave, notifications, etc.
    LastSavedAt datetime2,
    
    CONSTRAINT FK_Sessions_Campaign FOREIGN KEY (CampaignId) REFERENCES Campaigns(Id),
    CONSTRAINT FK_Sessions_GameMaster FOREIGN KEY (GameMasterId) REFERENCES Users(Id),
    CONSTRAINT FK_Sessions_CurrentChapter FOREIGN KEY (CurrentChapterId) REFERENCES Chapters(Id),
    CONSTRAINT CK_Sessions_Status CHECK (Status IN ('active', 'paused', 'ended', 'waiting'))
);

CREATE INDEX IX_Sessions_Campaign_Status ON Sessions (CampaignId, Status);
```

#### **SessionParticipants (Participants aux Sessions)**
```sql
CREATE TABLE SessionParticipants (
    SessionId nvarchar(50) NOT NULL,
    UserId int NOT NULL,
    CharacterId int NOT NULL,
    Status nvarchar(20) NOT NULL DEFAULT 'invited',
    JoinedAt datetime2,
    LastSeenAt datetime2,
    IsOnline bit NOT NULL DEFAULT 0,
    
    CONSTRAINT PK_SessionParticipants PRIMARY KEY (SessionId, UserId),
    CONSTRAINT FK_SessionParticipants_Session FOREIGN KEY (SessionId) REFERENCES Sessions(SessionId),
    CONSTRAINT FK_SessionParticipants_User FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT FK_SessionParticipants_Character FOREIGN KEY (CharacterId) REFERENCES CharactersDnd(Id),
    CONSTRAINT CK_SessionParticipants_Status CHECK (Status IN ('invited', 'joined', 'declined', 'disconnected'))
);
```

#### **CampaignInvitations (Invitations aux Campagnes)**
```sql
CREATE TABLE CampaignInvitations (
    Id int IDENTITY(1,1) PRIMARY KEY,
    CampaignId int NOT NULL,
    InviterId int NOT NULL,
    InviteeEmail nvarchar(255),
    InviteeUserId int,  -- Si utilisateur existant
    Status nvarchar(20) NOT NULL DEFAULT 'pending',
    Message nvarchar(500),
    CreatedAt datetime2 NOT NULL DEFAULT GETDATE(),
    ExpiresAt datetime2 NOT NULL,
    RespondedAt datetime2,
    
    CONSTRAINT FK_CampaignInvitations_Campaign FOREIGN KEY (CampaignId) REFERENCES Campaigns(Id),
    CONSTRAINT FK_CampaignInvitations_Inviter FOREIGN KEY (InviterId) REFERENCES Users(Id),
    CONSTRAINT FK_CampaignInvitations_Invitee FOREIGN KEY (InviteeUserId) REFERENCES Users(Id),
    CONSTRAINT CK_CampaignInvitations_Status CHECK (Status IN ('pending', 'accepted', 'declined', 'expired')),
    CONSTRAINT CK_CampaignInvitations_Contact CHECK (InviteeEmail IS NOT NULL OR InviteeUserId IS NOT NULL)
);
```

#### **CampaignProgress (Progression des Campagnes)**
```sql
CREATE TABLE CampaignProgress (
    CampaignId int NOT NULL,
    UserId int NOT NULL,
    CurrentChapterId int,
    CompletedChapters int NOT NULL DEFAULT 0,
    TotalChapters int NOT NULL DEFAULT 0,
    ProgressPercentage decimal(5,2) NOT NULL DEFAULT 0.00,
    LastUpdated datetime2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT PK_CampaignProgress PRIMARY KEY (CampaignId, UserId),
    CONSTRAINT FK_CampaignProgress_Campaign FOREIGN KEY (CampaignId) REFERENCES Campaigns(Id),
    CONSTRAINT FK_CampaignProgress_User FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT FK_CampaignProgress_CurrentChapter FOREIGN KEY (CurrentChapterId) REFERENCES Chapters(Id),
    CONSTRAINT CK_CampaignProgress_Percentage CHECK (ProgressPercentage >= 0 AND ProgressPercentage <= 100)
);
```

#### **SessionSaves (Sauvegardes de Session)**
```sql
CREATE TABLE SessionSaves (
    Id int IDENTITY(1,1) PRIMARY KEY,
    SessionId nvarchar(50) NOT NULL,
    SaveSlot int NOT NULL,
    GameState nvarchar(max) NOT NULL,  -- JSON complet de l'état
    ChapterId int NOT NULL,
    Description nvarchar(200),
    CreatedAt datetime2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_SessionSaves_Session FOREIGN KEY (SessionId) REFERENCES Sessions(SessionId),
    CONSTRAINT FK_SessionSaves_Chapter FOREIGN KEY (ChapterId) REFERENCES Chapters(Id),
    CONSTRAINT UQ_SessionSaves_Slot UNIQUE (SessionId, SaveSlot),
    CONSTRAINT CK_SessionSaves_Slot CHECK (SaveSlot >= 1 AND SaveSlot <= 10)
);
```

#### **Combats (Sessions de Combat)**
```sql
CREATE TABLE Combats (
    Id int IDENTITY(1,1) PRIMARY KEY,
    SessionId nvarchar(50) NOT NULL,
    ChapterId int NOT NULL,
    Status nvarchar(20) NOT NULL DEFAULT 'initiative',
    CurrentTurn int NOT NULL DEFAULT 1,
    TurnOrder nvarchar(max),  -- JSON ordre des tours
    StartedAt datetime2 NOT NULL DEFAULT GETDATE(),
    EndedAt datetime2,
    
    CONSTRAINT FK_Combats_Session FOREIGN KEY (SessionId) REFERENCES Sessions(SessionId),
    CONSTRAINT FK_Combats_Chapter FOREIGN KEY (ChapterId) REFERENCES Chapters(Id),
    CONSTRAINT CK_Combats_Status CHECK (Status IN ('initiative', 'active', 'paused', 'ended'))
);
```

#### **CombatParticipants (Participants aux Combats)**
```sql
CREATE TABLE CombatParticipants (
    CombatId int NOT NULL,
    ParticipantId int NOT NULL,
    ParticipantType nvarchar(10) NOT NULL,
    CharacterId int,  -- Si joueur
    NpcId int,        -- Si PNJ/monstre
    Initiative int NOT NULL,
    CurrentHitPoints int NOT NULL,
    MaxHitPoints int NOT NULL,
    StatusEffects nvarchar(max),  -- JSON effets actifs
    IsActive bit NOT NULL DEFAULT 1,
    
    CONSTRAINT PK_CombatParticipants PRIMARY KEY (CombatId, ParticipantId),
    CONSTRAINT FK_CombatParticipants_Combat FOREIGN KEY (CombatId) REFERENCES Combats(Id),
    CONSTRAINT FK_CombatParticipants_Character FOREIGN KEY (CharacterId) REFERENCES CharactersDnd(Id),
    CONSTRAINT FK_CombatParticipants_Npc FOREIGN KEY (NpcId) REFERENCES NPCs(Id),
    CONSTRAINT CK_CombatParticipants_Type CHECK (ParticipantType IN ('player', 'npc')),
    CONSTRAINT CK_CombatParticipants_Reference CHECK (
        (ParticipantType = 'player' AND CharacterId IS NOT NULL AND NpcId IS NULL) OR
        (ParticipantType = 'npc' AND CharacterId IS NULL AND NpcId IS NOT NULL)
    )
);
```

#### **Notifications (Système de Notifications)**
```sql
CREATE TABLE Notifications (
    Id int IDENTITY(1,1) PRIMARY KEY,
    UserId int NOT NULL,
    Type nvarchar(50) NOT NULL,
    Title nvarchar(100) NOT NULL,
    Message nvarchar(500) NOT NULL,
    Data nvarchar(max),  -- JSON données contextuelles
    IsRead bit NOT NULL DEFAULT 0,
    DeliveryMethod nvarchar(20) NOT NULL DEFAULT 'websocket',
    CreatedAt datetime2 NOT NULL DEFAULT GETDATE(),
    ReadAt datetime2,
    ExpiresAt datetime2,
    
    CONSTRAINT FK_Notifications_User FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT CK_Notifications_Type CHECK (Type IN ('session_started', 'your_turn', 'combat_invite', 'campaign_invite', 'equipment_offer')),
    CONSTRAINT CK_Notifications_DeliveryMethod CHECK (DeliveryMethod IN ('websocket', 'email', 'both'))
);

-- Index pour les notifications non lues
CREATE INDEX IX_Notifications_User_Unread ON Notifications (UserId, IsRead, CreatedAt);
```

#### **PasswordResets (Réinitialisation Mots de Passe)**
```sql
CREATE TABLE PasswordResets (
    Id int IDENTITY(1,1) PRIMARY KEY,
    UserId int NOT NULL,
    ResetToken nvarchar(100) NOT NULL,
    CreatedAt datetime2 NOT NULL DEFAULT GETDATE(),
    ExpiresAt datetime2 NOT NULL,
    IsUsed bit NOT NULL DEFAULT 0,
    UsedAt datetime2,
    
    CONSTRAINT FK_PasswordResets_User FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT UQ_PasswordResets_Token UNIQUE (ResetToken)
);

-- Index pour nettoyage automatique des tokens expirés
CREATE INDEX IX_PasswordResets_Expiry ON PasswordResets (ExpiresAt, IsUsed);
```

#### **PlayerStatistics (Statistiques Joueur)** ✨ NOUVEAU
```sql
CREATE TABLE PlayerStatistics (
    Id int IDENTITY(1,1) PRIMARY KEY,
    UserId int NOT NULL,
    StatType nvarchar(50) NOT NULL,
    StatCategory nvarchar(30) NOT NULL,
    StatValue decimal(18,2) NOT NULL,
    AdditionalData nvarchar(max),  -- JSON pour données contextuelles
    SessionId nvarchar(50),
    CharacterId int,
    CampaignId int,
    RecordedAt datetime2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_PlayerStatistics_User FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT FK_PlayerStatistics_Session FOREIGN KEY (SessionId) REFERENCES Sessions(SessionId),
    CONSTRAINT FK_PlayerStatistics_Character FOREIGN KEY (CharacterId) REFERENCES CharactersDnd(Id),
    CONSTRAINT FK_PlayerStatistics_Campaign FOREIGN KEY (CampaignId) REFERENCES Campaigns(Id),
    CONSTRAINT CK_PlayerStatistics_Category CHECK (StatCategory IN ('session', 'combat', 'dice', 'social', 'progression'))
);

-- Index pour requêtes de stats par utilisateur et type
CREATE INDEX IX_PlayerStatistics_User_Type ON PlayerStatistics (UserId, StatType, RecordedAt);
CREATE INDEX IX_PlayerStatistics_Character ON PlayerStatistics (CharacterId, StatCategory);
```

#### **DiceRolls (Historique des Jets de Dés)**
```sql
CREATE TABLE DiceRolls (
    Id int IDENTITY(1,1) PRIMARY KEY,
    UserId int NOT NULL,
    SessionId nvarchar(50),
    CharacterId int,
    DiceType nvarchar(10) NOT NULL,  -- d20, d6, d8, d12, etc.
    Result int NOT NULL,
    Context nvarchar(50),  -- attack, save, skill, spell, etc.
    TargetDC int,  -- Classe de difficulté visée
    IsSuccess bit,
    IsCritical bit NOT NULL DEFAULT 0,
    AdditionalModifiers int DEFAULT 0,
    RolledAt datetime2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_DiceRolls_User FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT FK_DiceRolls_Session FOREIGN KEY (SessionId) REFERENCES Sessions(SessionId),
    CONSTRAINT FK_DiceRolls_Character FOREIGN KEY (CharacterId) REFERENCES CharactersDnd(Id),
    CONSTRAINT CK_DiceRolls_DiceType CHECK (DiceType IN ('d4', 'd6', 'd8', 'd10', 'd12', 'd20', 'd100')),
    CONSTRAINT CK_DiceRolls_Context CHECK (Context IN ('attack', 'damage', 'save', 'skill', 'spell', 'initiative', 'other'))
);

-- Index pour analyse statistique des dés
CREATE INDEX IX_DiceRolls_User_Type ON DiceRolls (UserId, DiceType, RolledAt);
CREATE INDEX IX_DiceRolls_Character_Context ON DiceRolls (CharacterId, Context, RolledAt);
```

#### **Achievements (Système de Succès)**
```sql
CREATE TABLE Achievements (
    Id nvarchar(50) PRIMARY KEY,  -- Format: dragon_slayer, critical_master
    Name nvarchar(100) NOT NULL,
    Description nvarchar(500) NOT NULL,
    Icon nvarchar(10) NOT NULL,
    Category nvarchar(30) NOT NULL,
    Rarity nvarchar(20) NOT NULL,
    RequiredValue int,
    RequiredData nvarchar(max),  -- JSON critères complexes
    IsActive bit NOT NULL DEFAULT 1,
    CreatedAt datetime2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT CK_Achievements_Category CHECK (Category IN ('combat', 'exploration', 'social', 'mastery', 'collection', 'luck', 'progression')),
    CONSTRAINT CK_Achievements_Rarity CHECK (Rarity IN ('common', 'uncommon', 'rare', 'epic', 'legendary'))
);
```

#### **PlayerAchievements (Succès des Joueurs)**
```sql
CREATE TABLE PlayerAchievements (
    UserId int NOT NULL,
    AchievementId nvarchar(50) NOT NULL,
    Progress decimal(5,2) NOT NULL DEFAULT 0.00,
    CurrentValue int NOT NULL DEFAULT 0,
    IsUnlocked bit NOT NULL DEFAULT 0,
    UnlockedAt datetime2,
    UnlockedWithCharacterId int,
    UnlockContext nvarchar(max),  -- JSON circonstances du déblocage
    
    CONSTRAINT PK_PlayerAchievements PRIMARY KEY (UserId, AchievementId),
    CONSTRAINT FK_PlayerAchievements_User FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT FK_PlayerAchievements_Achievement FOREIGN KEY (AchievementId) REFERENCES Achievements(Id),
    CONSTRAINT FK_PlayerAchievements_Character FOREIGN KEY (UnlockedWithCharacterId) REFERENCES CharactersDnd(Id),
    CONSTRAINT CK_PlayerAchievements_Progress CHECK (Progress >= 0 AND Progress <= 100)
);

-- Index pour dashboard des succès
CREATE INDEX IX_PlayerAchievements_User_Unlocked ON PlayerAchievements (UserId, IsUnlocked, UnlockedAt);
```

#### **CombatActions (Actions de Combat)** 
```sql
CREATE TABLE CombatActions (
    Id int IDENTITY(1,1) PRIMARY KEY,
    CombatId int NOT NULL,
    UserId int NOT NULL,
    CharacterId int NOT NULL,
    ActionType nvarchar(30) NOT NULL,
    TargetType nvarchar(20),
    TargetId int,
    DamageDealt int DEFAULT 0,
    DamageTaken int DEFAULT 0,
    IsHit bit DEFAULT 1,
    IsCritical bit DEFAULT 0,
    SpellId int,
    EquipmentId int,
    RoundNumber int NOT NULL,
    ActionOrder int NOT NULL,
    ActionData nvarchar(max),  -- JSON données détaillées
    ExecutedAt datetime2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_CombatActions_Combat FOREIGN KEY (CombatId) REFERENCES Combats(Id),
    CONSTRAINT FK_CombatActions_User FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT FK_CombatActions_Character FOREIGN KEY (CharacterId) REFERENCES CharactersDnd(Id),
    CONSTRAINT FK_CombatActions_Spell FOREIGN KEY (SpellId) REFERENCES Spells(Id),
    CONSTRAINT FK_CombatActions_Equipment FOREIGN KEY (EquipmentId) REFERENCES Equipment(Id),
    CONSTRAINT CK_CombatActions_ActionType CHECK (ActionType IN ('attack', 'spell', 'move', 'dodge', 'help', 'hide', 'dash', 'ready')),
    CONSTRAINT CK_CombatActions_TargetType CHECK (TargetType IN ('player', 'npc', 'environment', 'self', 'area'))
);

-- Index pour analyse de performance combat
CREATE INDEX IX_CombatActions_User_Combat ON CombatActions (UserId, CombatId, RoundNumber);
```

#### **SessionActivities (Activités de Session)**
```sql
CREATE TABLE SessionActivities (
    Id int IDENTITY(1,1) PRIMARY KEY,
    SessionId nvarchar(50) NOT NULL,
    UserId int NOT NULL,
    ActivityType nvarchar(30) NOT NULL,
    ActivityData nvarchar(max),  -- JSON données spécifiques
    ExperienceGained int DEFAULT 0,
    ItemsGained nvarchar(max),  -- JSON liste objets
    ActivityTime datetime2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_SessionActivities_Session FOREIGN KEY (SessionId) REFERENCES Sessions(SessionId),
    CONSTRAINT FK_SessionActivities_User FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT CK_SessionActivities_Type CHECK (ActivityType IN ('quest_complete', 'boss_defeat', 'treasure_find', 'level_up', 'skill_use', 'social_encounter'))
);
```

#### **PlayerReports (Rapports Personnalisés)**
```sql
CREATE TABLE PlayerReports (
    Id int IDENTITY(1,1) PRIMARY KEY,
    UserId int NOT NULL,
    ReportType nvarchar(20) NOT NULL,
    ReportPeriod nvarchar(20) NOT NULL,  -- monthly, yearly, custom
    StartDate datetime2 NOT NULL,
    EndDate datetime2 NOT NULL,
    ReportData nvarchar(max) NOT NULL,  -- JSON rapport complet
    GeneratedAt datetime2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_PlayerReports_User FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT CK_PlayerReports_Type CHECK (ReportType IN ('performance', 'progression', 'social', 'achievements')),
    CONSTRAINT CK_PlayerReports_Period CHECK (ReportPeriod IN ('weekly', 'monthly', 'yearly', 'custom'))
);

-- Index pour récupération des rapports
CREATE INDEX IX_PlayerReports_User_Period ON PlayerReports (UserId, ReportType, StartDate DESC);
```

## 🔄 Migrations Prévues

### Migration 1 : Tables de Base
```bash
dotnet ef migrations add CreateSpellsAndEquipmentTables
```
- Création des tables `Spells` et `Equipment`
- Index pour optimisation des requêtes

### Migration 2 : Système d'Apprentissage
```bash
dotnet ef migrations add CreateCharacterSpellsRelation
```
- Création de la table `CharacterSpells`
- Relations avec validation

### Migration 3 : Système d'Inventaire
```bash
dotnet ef migrations add CreateCharacterEquipmentRelation
```
- Création de la table `CharacterEquipment`
- Support des quantités multiples

### Migration 4 : Système d'Échanges
```bash
dotnet ef migrations add CreateEquipmentExchangeSystem
```
- Création des tables `EquipmentOffers` et `EquipmentTrades`
- Contraintes de validation des échanges

### Migration 5 : Campagnes et Chapitres
```bash
dotnet ef migrations add CreateCampaignsAndChapters
```
- Système complet de gestion des campagnes
- Tables `Campaigns`, `Chapters`, `NPCs`, `CampaignPlayers`

### Migration 6 : Système de Sessions ✨ NOUVEAU
```bash
dotnet ef migrations add CreateSessionsSystem
```
- Création des tables `Sessions` et `SessionParticipants`
- Support du lancement et gestion des sessions temps réel

### Migration 7 : Système de Notifications
```bash
dotnet ef migrations add CreateNotificationsSystem  
```
- Création de la table `Notifications`
- Support WebSocket et email pour alertes en temps réel

### Migration 8 : Invitations et Progression
```bash
dotnet ef migrations add CreateInvitationsAndProgress
```
- Création des tables `CampaignInvitations` et `CampaignProgress`
- Système complet d'invitations et suivi de progression

### Migration 9 : Combat Temps Réel
```bash
dotnet ef migrations add CreateRealTimeCombat
```
- Création des tables `Combats` et `CombatParticipants`
- Support des combats synchronisés avec invitations dynamiques

### Migration 10 : Authentification Avancée
```bash
dotnet ef migrations add CreatePasswordResets
```
- Création de la table `PasswordResets`
- Système complet de réinitialisation de mots de passe

### Migration 11 : Système de Statistiques ✨ NOUVEAU
```bash
dotnet ef migrations add CreatePlayerStatistics
```
- Création des tables `PlayerStatistics` et `DiceRolls`
- Collecte automatique des métriques de performance

### Migration 12 : Système de Succès
```bash
dotnet ef migrations add CreateAchievementsSystem
```
- Création des tables `Achievements` et `PlayerAchievements`
- Framework complet de déblocage de succès

### Migration 13 : Actions de Combat Détaillées
```bash
dotnet ef migrations add CreateCombatActions
```
- Création de la table `CombatActions`
- Enregistrement détaillé de toutes les actions de combat

### Migration 14 : Activités et Rapports
```bash
dotnet ef migrations add CreateActivitiesAndReports
```
- Création des tables `SessionActivities` et `PlayerReports`
- Système complet de suivi et d'analyse

## 📈 Données de Test

### Scripts d'Injection Administrative

#### **Sorts D&D Officiels**
```sql
-- Sorts de niveau 1
INSERT INTO Spells (Name, Description, GameType, CreatedByUserId, IsPublic, DndProperties)
VALUES 
('Projectile Magique', 'Trois projectiles d''énergie pure frappent automatiquement leurs cibles.', 'dnd', 0, 1, 
 '{"level":1,"school":"Évocation","castingTime":"1 action","range":"36 mètres","duration":"Instantané","components":["V","S"],"damageFormula":"1d4+1","requiresAttackRoll":true}'),

('Soin', 'Restaure instantanément les points de vie de la cible.', 'dnd', 0, 1,
 '{"level":1,"school":"Évocation","castingTime":"1 action","range":"Contact","duration":"Instantané","components":["V","S"],"damageFormula":"1d8+mod","requiresAttackRoll":false}');

-- Sorts de niveau 3
INSERT INTO Spells (Name, Description, GameType, CreatedByUserId, IsPublic, DndProperties)
VALUES 
('Boule de Feu', 'Une explosion de flammes dévastatrice dans une zone de 6 mètres de rayon.', 'dnd', 0, 1,
 '{"level":3,"school":"Évocation","castingTime":"1 action","range":"45 mètres","duration":"Instantané","components":["V","S","M"],"damageFormula":"8d6","requiresSavingThrow":true,"savingThrowAbility":"Dextérité"}');
```

#### **Équipements D&D Officiels**
```sql
-- Armes
INSERT INTO Equipment (Name, Description, GameType, CreatedByUserId, IsPublic, DndProperties)
VALUES 
('Épée Longue', 'Arme martiale polyvalente à une main.', 'dnd', 0, 1,
 '{"equipmentType":"Weapon","weaponCategory":"Martial","damageFormula":"1d8","damageType":"Tranchant","properties":["Versatile (1d10)"],"rarity":"Commun"}'),

('Dague', 'Arme simple légère et précise.', 'dnd', 0, 1,
 '{"equipmentType":"Weapon","weaponCategory":"Simple","damageFormula":"1d4","damageType":"Perforant","properties":["Finesse","Light","Thrown (6/18)"],"rarity":"Commun"}');

-- Armures
INSERT INTO Equipment (Name, Description, GameType, CreatedByUserId, IsPublic, DndProperties)
VALUES 
('Armure de Cuir', 'Armure légère flexible et silencieuse.', 'dnd', 0, 1,
 '{"equipmentType":"Armor","armorCategory":"Light","armorClassBase":11,"armorClassDexBonus":10,"rarity":"Commun"}'),

('Cotte de Mailles', 'Armure intermédiaire offrant une bonne protection.', 'dnd', 0, 1,
 '{"equipmentType":"Armor","armorCategory":"Medium","armorClassBase":13,"armorClassDexBonus":2,"rarity":"Commun"}');

-- Consommables
INSERT INTO Equipment (Name, Description, GameType, CreatedByUserId, IsPublic, DndProperties)
VALUES 
('Potion de Soins', 'Récupère des points de vie instantanément.', 'dnd', 0, 1,
 '{"equipmentType":"Consumable","properties":["Healing 2d4+2"],"rarity":"Commun"}');
```

#### **Campagnes et Chapitres Initiaux**
```sql
-- Campagne Test
INSERT INTO Campaigns (Name, Description, GameType, GameMasterId, IsPublic, CreatedAt, UpdatedAt)
VALUES 
('Campagne de Test', 'Une campagne passionnante pour tester les fonctionnalités.', 'dnd', 1, 1, GETDATE(), GETDATE());

-- Récupérer l''Id de la campagne créée
DECLARE @CampaignId int = SCOPE_IDENTITY();

-- Chapitre 1
INSERT INTO Chapters (CampaignId, ChapterNumber, Title, Content, OrderIndex, Status, CreatedAt, UpdatedAt)
VALUES 
(@CampaignId, 1, 'Chapitre d''Introduction', 'Contenu du chapitre d''introduction...', 1, 'Active', GETDATE(), GETDATE());

-- Chapitre 2
INSERT INTO Chapters (CampaignId, ChapterNumber, Title, Content, OrderIndex, Status, CreatedAt, UpdatedAt)
VALUES 
(@CampaignId, 2, 'Chapitre de Développement', 'Contenu du chapitre de développement...', 2, 'Pending', GETDATE(), GETDATE());
```

#### **Initialisation des Succès par Défaut**

```sql
-- Script d'injection des succès de base
INSERT INTO Achievements (Id, Name, Description, Icon, Category, Rarity, RequiredValue, RequiredData) VALUES

-- SUCCÈS DE COMBAT ⚔️
('first_blood', 'Premier Sang', 'Remporter votre premier combat', '🗡️', 'combat', 'common', 1, '{"combatType": "any"}'),
('critical_master', 'Maître du Critique', 'Obtenir 10 coups critiques consécutifs', '🎯', 'combat', 'rare', 10, '{"consecutive": true, "rollType": "attack"}'),
('untouchable', 'Intouchable', 'Terminer 5 combats sans subir de dégâts', '🛡️', 'combat', 'epic', 5, '{"condition": "no_damage_taken"}'),
('dragon_slayer', 'Tueur de Dragons', 'Vaincre un dragon ancien', '🐲', 'combat', 'legendary', 1, '{"enemyType": "ancient_dragon"}'),
('damage_dealer', 'Machine de Guerre', 'Infliger 10 000 points de dégâts au total', '💥', 'combat', 'rare', 10000, '{"cumulative": true}'),
('last_stand', 'Dernier Rempart', 'Vaincre un boss avec moins de 5 HP', '🛡️', 'combat', 'epic', 1, '{"condition": "low_hp_boss_kill", "hpThreshold": 5}'),
('berserker', 'Berserker', 'Infliger plus de 100 dégâts en un seul round', '🪓', 'combat', 'rare', 100, '{"timeframe": "single_round"}'),

-- SUCCÈS D'EXPLORATION 🗺️
('cartographer', 'Cartographe', 'Découvrir 50 lieux secrets', '🗺️', 'exploration', 'uncommon', 50, '{"locationType": "secret"}'),
('treasure_hunter', 'Chasseur de Trésors', 'Trouver 25 trésors légendaires', '💎', 'exploration', 'epic', 25, '{"rarity": "legendary"}'),
('dungeon_master', 'Maître des Donjons', 'Compléter 10 donjons différents', '🏰', 'exploration', 'rare', 10, '{"unique": true}'),
('pathfinder', 'Éclaireur', 'Être le premier à entrer dans 20 lieux', '🧭', 'exploration', 'uncommon', 20, '{"condition": "first_entry"}'),
('completionist', 'Perfectionniste', 'Compléter une campagne à 100%', '📜', 'exploration', 'epic', 1, '{"completion": 100}'),

-- SUCCÈS SOCIAUX 👥
('team_player', 'Esprit d\'Équipe', 'Participer à 100 sessions multijoueurs', '🤝', 'social', 'common', 100, '{"sessionType": "multiplayer"}'),
('mentor', 'Mentor', 'Aider 5 nouveaux joueurs', '👨‍🏫', 'social', 'rare', 5, '{"action": "help_new_player"}'),
('diplomat', 'Diplomate', 'Résoudre 10 conflits sans violence', '🕊️', 'social', 'uncommon', 10, '{"method": "peaceful"}'),
('generous_soul', 'Âme Généreuse', 'Donner 100 objets à d\'autres joueurs', '🎁', 'social', 'uncommon', 100, '{"action": "give_item"}'),
('party_leader', 'Chef de Groupe', 'Diriger 25 sessions avec succès', '👑', 'social', 'rare', 25, '{"role": "leader", "outcome": "success"}'),

-- SUCCÈS DE MAÎTRISE 🎭
('gm_apprentice', 'Apprenti MJ', 'Créer votre première campagne', '📚', 'mastery', 'common', 1, '{"action": "create_campaign"}'),
('storyteller', 'Conteur', 'Mener 10 campagnes à leur terme', '📖', 'mastery', 'epic', 10, '{"completion": true}'),
('world_builder', 'Créateur de Mondes', 'Créer 50 PNJ personnalisés', '🌍', 'mastery', 'rare', 50, '{"content": "npc"}'),
('rule_master', 'Maître des Règles', 'Gérer 100 combats sans erreur', '⚖️', 'mastery', 'rare', 100, '{"accuracy": "perfect"}'),
('crowd_pleaser', 'Meneur de Foule', 'Avoir 50+ joueurs dans vos campagnes', '🎪', 'mastery', 'epic', 50, '{"metric": "total_players"}'),

-- SUCCÈS DE COLLECTION 💎
('spell_collector', 'Collectionneur de Sorts', 'Apprendre 100 sorts différents', '📜', 'collection', 'rare', 100, '{"content": "spells", "unique": true}'),
('equipment_hoarder', 'Accumulateur', 'Posséder 500 objets au total', '📦', 'collection', 'uncommon', 500, '{"content": "equipment"}'),
('legendary_collector', 'Collectionneur Légendaire', 'Posséder 10 objets légendaires simultanément', '⭐', 'collection', 'legendary', 10, '{"rarity": "legendary", "simultaneous": true}'),
('library_owner', 'Propriétaire de Bibliothèque', 'Connaître des sorts de toutes les écoles', '📚', 'collection', 'epic', 8, '{"content": "spell_schools", "complete": true}'),

-- SUCCÈS DE CHANCE 🎲
('natural_20', 'Coup du Destin', 'Obtenir un 20 naturel', '🎯', 'luck', 'common', 1, '{"dice": "d20", "result": 20}'),
('streak_master', 'Série Chanceuse', 'Obtenir 5 jets de 15+ consécutifs', '🔥', 'luck', 'rare', 5, '{"consecutive": true, "threshold": 15}'),
('unlikely_hero', 'Héros Improbable', 'Gagner avec moins de 5% de chance', '🍀', 'luck', 'legendary', 1, '{"probability": 0.05}'),
('lucky_month', 'Mois Béni', 'Avoir une moyenne de dés > 15 sur un mois', '🌟', 'luck', 'epic', 15, '{"timeframe": "month", "average": true}'),
('miracle_save', 'Sauvegarde Miraculeuse', 'Réussir une sauvegarde de mort avec un 20', '💫', 'luck', 'rare', 1, '{"saveType": "death", "roll": 20}'),

-- SUCCÈS DE PROGRESSION ⚡
('level_up', 'Montée en Puissance', 'Atteindre le niveau 5', '⚡', 'progression', 'common', 5, '{"metric": "character_level"}'),
('veteran', 'Vétéran', 'Survivre à 200 combats', '🏆', 'progression', 'rare', 200, '{"metric": "combats_survived"}'),
('experience_master', 'Maître de l\'Expérience', 'Gagner 50 000 XP au total', '🎯', 'progression', 'epic', 50000, '{"metric": "total_experience"}'),
('skill_master', 'Maître des Compétences', 'Maximiser 5 compétences', '📈', 'progression', 'rare', 5, '{"metric": "maxed_skills"}'),
('multi_class', 'Polyvalent', 'Jouer 3 classes différentes', '🎭', 'progression', 'uncommon', 3, '{"metric": "different_classes"}');

-- Vérification des succès créés
SELECT Category, COUNT(*) as Count, 
       STRING_AGG(Rarity, ', ') as Rarities
FROM Achievements 
GROUP BY Category 
ORDER BY Category;
```

### Structure JSON des RequiredData

Les différents types de conditions pour les succès :

```json
// Succès simple avec compteur
{
  "cumulative": true,
  "resetOnDeath": false
}

// Succès avec condition temporelle
{
  "timeframe": "month",
  "consecutive": false,
  "resetPeriod": "monthly"
}

// Succès avec conditions multiples
{
  "conditions": [
    {"type": "enemy_type", "value": "dragon"},
    {"type": "damage_threshold", "value": 100},
    {"type": "team_size", "max": 4}
  ],
  "requireAll": true
}

// Succès avec probabilité
{
  "probability_calculation": true,
  "success_threshold": 0.05,
  "context_aware": true
}