# 🎨 **FRONTEND ROADMAP - CHRONIQUE DES MONDES**
## Interface Combat D&D Temps Réel & Refonte Visuelle Complète

---

## 📋 **CONTEXTE & OBJECTIFS**

### 🎯 **Mission Principale**
Transformer **Chronique des Mondes** en VTT (Virtual Tabletop) de référence avec une interface moderne, immersive et temps réel pour le combat D&D 5e.

### ✅ **Acquis Actuels**
- **Backend Combat** : Système complet avec 33 équipements D&D intégrés
- **API REST** : 12 endpoints combat fonctionnels  
- **Calculs D&D 5e** : Automatiques et conformes au SRD
- **Architecture** : Blazor Server (.NET 9) + Entity Framework
- **Données** : 74 sorts + 33 équipements + système complet D&D

### ❌ **Problèmes à Résoudre**
- **Interface obsolète** : Design générique peu attractif
- **Pas d'UI combat** : Système backend sans interface utilisateur
- **UX confuse** : Navigation peu intuitive
- **Mobile insuffisant** : Responsive design limité
- **Pas de temps réel** : Absence de SignalR pour multijoueur

---

## 🚀 **ROADMAP EN 6 PHASES**

### **📐 PHASE 1 : FOUNDATION - DESIGN SYSTEM MODERNE**
*Durée : 2-3 semaines | Priorité : 🔥 CRITIQUE*

#### **1.1 Setup Framework UI**
```xml
<!-- Package.csproj -->
<PackageReference Include="MudBlazor" Version="6.11.2" />
<PackageReference Include="MudBlazor.ThemeManager" Version="1.0.8" />
```

**Tâches :**
- [ ] **Installation MudBlazor** + configuration thème
- [ ] **Palette D&D** : Rouges sang, ors anciens, noirs profonds
- [ ] **Typographie fantasy** : Headers épiques, texte lisible
- [ ] **Grille responsive** : Mobile-first avec breakpoints optimisés
- [ ] **Système d'icônes** : FontAwesome 6 + icônes D&D custom

#### **1.2 Composants de Base**
```razor
<!-- Exemples de composants à créer -->
@* CDM.Components/Base/CdmButton.razor *@
@* CDM.Components/Base/CdmCard.razor *@
@* CDM.Components/Base/CdmModal.razor *@
```

**Composants Core :**
- [ ] **`CdmButton.razor`** - Boutons avec variantes D&D
- [ ] **`CdmCard.razor`** - Cartes avec effet parchemin
- [ ] **`CdmModal.razor`** - Modales immersives
- [ ] **`CdmToast.razor`** - Notifications thématiques
- [ ] **`CdmSpinner.razor`** - Loading avec dés qui roulent

#### **1.3 Layout Principal**
```razor
@* Cdm.Web/Shared/MainLayout.razor - Refonte complète *@
<div class="cdm-layout">
    <CdmHeader />
    <CdmSidebar />
    <main class="cdm-content">
        @Body
    </main>
    <CdmFooter />
</div>
```

---

### **⚔️ PHASE 2 : COMBAT INTERFACE TEMPS RÉEL**
*Durée : 3-4 semaines | Priorité : 🔥 CRITIQUE*

#### **2.1 Combat Hub SignalR**
```csharp
// Cdm.Web/Hubs/CombatHub.cs
public class CombatHub : Hub
{
    public async Task JoinCombat(int combatId)
    public async Task ExecuteAction(ExecuteCombatActionCommand command)
    public async Task BroadcastActionResult(CombatActionResult result)
    public async Task UpdateCombatState(CombatStateDto state)
}
```

#### **2.2 Dashboard MJ (Master Interface)**
```razor
@page "/combat/{combatId:int}/master"
@using Cdm.Business.Common.Models.Combat

<MudContainer MaxWidth="MaxWidth.ExtraExtraLarge">
    <MudGrid>
        <!-- Initiative Tracker -->
        <MudItem xs="12" md="4">
            <InitiativeTracker CombatState="@_combatState" />
        </MudItem>
        
        <!-- Combat Actions -->
        <MudItem xs="12" md="8">
            <CombatActionPanel CombatId="@CombatId" />
        </MudItem>
        
        <!-- Participants Grid -->
        <MudItem xs="12">
            <ParticipantsGrid Participants="@_combatState.Participants" />
        </MudItem>
        
        <!-- Combat Log -->
        <MudItem xs="12">
            <CombatLogPanel ActionLog="@_combatState.ActionLog" />
        </MudItem>
    </MudGrid>
</MudContainer>
```

**Composants MJ à créer :**
- [ ] **`CombatMasterDashboard.razor`** - Vue d'ensemble complète
- [ ] **`InitiativeTracker.razor`** - Ordre des tours avec drag&drop
- [ ] **`ParticipantCard.razor`** - Carte personnage avec stats live
- [ ] **`ActionExecutor.razor`** - Interface d'exécution d'actions
- [ ] **`CombatControls.razor`** - Contrôles de combat (pause, suivant, etc.)
- [ ] **`EnvironmentPanel.razor`** - Gestion environnement de combat

#### **2.3 Interface Joueur**
```razor
@page "/combat/{combatId:int}/player/{playerId:int}"
<div class="player-combat-view">
    <MyCharacterPanel CharacterId="@PlayerId" />
    <ActionButtonsPanel CombatId="@CombatId" PlayerId="@PlayerId" />
    <TargetSelector AvailableTargets="@_availableTargets" />
    <CombatLogPlayer ActionLog="@_recentActions" />
</div>
```

**Composants Joueur à créer :**
- [ ] **`CombatPlayerView.razor`** - Interface épurée joueur
- [ ] **`MyCharacterPanel.razor`** - Fiche personnage combat
- [ ] **`ActionButtonsPanel.razor`** - Actions disponibles
- [ ] **`WeaponSelector.razor`** - Sélection d'armes avec stats
- [ ] **`TargetSelector.razor`** - Sélection de cibles visuelles

#### **2.4 Système de Dés Interactif**
```razor
@* Cdm.Web/Components/Dice/DiceRoller.razor *@
<div class="dice-roller">
    <MudButton OnClick="RollDice" Class="dice-button">
        <i class="fas fa-dice-d20"></i> Lancer @DiceFormula
    </MudButton>
    
    @if (_result != null)
    {
        <DiceResult Result="@_result" ShowAnimation="true" />
    }
</div>
```

**Composants Dés :**
- [ ] **`DiceRoller.razor`** - Interface de lancement
- [ ] **`DiceResult.razor`** - Affichage résultats animés
- [ ] **`DiceAnimation.razor`** - Animation 3D des dés
- [ ] **`AdvantageSelector.razor`** - Sélection avantage/désavantage

---

### **🎲 PHASE 3 : UX IMMERSIVE & ANIMATIONS**
*Durée : 2-3 semaines | Priorité : ⚡ IMPORTANT*

#### **3.1 Animations Combat**
```css
/* Cdm.Web/wwwroot/css/combat-animations.css */
.damage-number {
    animation: damageFloat 1.5s ease-out forwards;
    color: #dc2626;
    font-weight: bold;
}

.critical-hit {
    animation: criticalPulse 0.8s ease-in-out;
    color: #fbbf24;
    text-shadow: 0 0 10px #fbbf24;
}

.health-bar {
    transition: width 0.6s ease-in-out;
}

@keyframes damageFloat {
    0% { transform: translateY(0); opacity: 1; }
    100% { transform: translateY(-50px); opacity: 0; }
}

@keyframes criticalPulse {
    0%, 100% { transform: scale(1); }
    50% { transform: scale(1.2); }
}
```

#### **3.2 Composants Animés**
```razor
@* HealthBar avec animation *@
<div class="health-bar-container">
    <div class="health-bar" style="width: @(HealthPercentage)%"></div>
    @if (_lastDamage > 0)
    {
        <div class="damage-number">-@_lastDamage</div>
    }
</div>
```

**Animations à implémenter :**
- [ ] **Dégâts flottants** : Nombres qui montent et disparaissent
- [ ] **Barres de vie animées** : Transitions fluides
- [ ] **Effets critiques** : Flash doré pour coup critique
- [ ] **Status effects** : Particules pour buffs/debuffs
- [ ] **Tour notifications** : Transition entre joueurs

#### **3.3 Thème Visuel D&D**
```scss
// Cdm.Web/Styles/_variables.scss
$cdm-primary: #8B0000;      // Rouge sang
$cdm-secondary: #DAA520;    // Or ancien  
$cdm-dark: #2D1B0B;         // Brun foncé
$cdm-parchment: #F4E4BC;    // Parchemin
$cdm-success: #228B22;      // Vert forêt
$cdm-danger: #DC143C;       // Rouge vif

// Textures
$texture-parchment: url('/images/textures/parchment.jpg');
$texture-leather: url('/images/textures/leather.jpg');
$texture-metal: url('/images/textures/metal.jpg');
```

---

### **📊 PHASE 4 : GESTION CAMPAGNES MODERNE**
*Durée : 2 semaines | Priorité : ⚡ IMPORTANT*

#### **4.1 Dashboard Campagnes**
```razor
@page "/campaigns"
<PageTitle>Mes Campagnes</PageTitle>

<MudContainer MaxWidth="MaxWidth.Large">
    <MudStack Spacing="4">
        <MudPaper Class="pa-6">
            <MudStack Row Justify="Justify.SpaceBetween" AlignItems="Center.Center">
                <MudText Typo="Typo.h4">📚 Mes Campagnes</MudText>
                <MudButton StartIcon="@Icons.Material.Filled.Add" 
                          Variant="Variant.Filled" 
                          Color="Color.Primary"
                          OnClick="CreateNewCampaign">
                    Nouvelle Campagne
                </MudButton>
            </MudStack>
        </MudPaper>
        
        <MudGrid>
            @foreach (var campaign in _campaigns)
            {
                <MudItem xs="12" sm="6" md="4">
                    <CampaignCard Campaign="@campaign" />
                </MudItem>
            }
        </MudGrid>
    </MudStack>
</MudContainer>
```

#### **4.2 Refonte Fiche Personnage**
```razor
@page "/characters/{characterId:int}"
<div class="character-sheet-modern">
    <CharacterHeader Character="@_character" />
    <MudTabs>
        <MudTabPanel Text="🗡️ Combat">
            <CharacterCombatTab Character="@_character" />
        </MudTabPanel>
        <MudTabPanel Text="🎒 Équipement">
            <CharacterEquipmentTab Character="@_character" />
        </MudTabPanel>
        <MudTabPanel Text="📜 Sorts">
            <CharacterSpellsTab Character="@_character" />
        </MudTabPanel>
        <MudTabPanel Text="📊 Stats">
            <CharacterStatsTab Character="@_character" />
        </MudTabPanel>
    </MudTabs>
</div>
```

---

### **🔮 PHASE 5 : SYSTÈME DE SORTS UI**
*Durée : 3 semaines | Priorité : ⭐ OPTIONNEL*

#### **5.1 Interface Sorts Combat**
```razor
@* Intégration des 74 sorts existants *@
<div class="spell-casting-interface">
    <SpellSelector AvailableSpells="@_characterSpells" 
                   OnSpellSelected="@HandleSpellSelection" />
    
    <SpellDetails Spell="@_selectedSpell" />
    
    <SpellSlotTracker Character="@_character" />
    
    <TargetingSystem Spell="@_selectedSpell" 
                     AvailableTargets="@_targets" />
    
    <MudButton OnClick="CastSpell" 
              Disabled="@(!CanCastSpell())"
              Class="cast-spell-button">
        🔮 Lancer le Sort
    </MudButton>
</div>
```

#### **5.2 Composants Sorts Avancés**
- [ ] **`SpellCard.razor`** - Carte sort avec détails complets
- [ ] **`SpellSlotTracker.razor`** - Gestion emplacements visuels
- [ ] **`SpellEffectPreview.razor`** - Prévisualisation effets
- [ ] **`ConcentrationManager.razor`** - Gestion de la concentration

---

### **🏆 PHASE 6 : FONCTIONNALITÉS AVANCÉES**
*Durée : 4 semaines | Priorité : ⭐ OPTIONNEL*

#### **6.1 Cartes Tactiques**
```razor
@page "/combat/{combatId:int}/map"
<div class="tactical-map">
    <TacticalGrid CombatId="@CombatId" />
    <TokenManager Participants="@_participants" />
    <MovementCalculator CurrentParticipant="@_currentParticipant" />
</div>
```

#### **6.2 Système de Macros**
```razor
<div class="macro-panel">
    <MacroButton Macro="@attackMacro" OnExecute="@ExecuteMacro" />
    <MacroEditor OnSave="@SaveMacro" />
</div>
```

---

## 🛠️ **STRUCTURE TECHNIQUE DÉTAILLÉE**

### **📁 Organisation des Fichiers**
```
Cdm.Web/
├── Components/
│   ├── Combat/
│   │   ├── CombatMasterDashboard.razor
│   │   ├── InitiativeTracker.razor
│   │   ├── ParticipantCard.razor
│   │   ├── ActionExecutor.razor
│   │   └── CombatPlayerView.razor
│   ├── Dice/
│   │   ├── DiceRoller.razor
│   │   ├── DiceResult.razor
│   │   └── DiceAnimation.razor
│   ├── Characters/
│   │   ├── CharacterSheet.razor
│   │   ├── CharacterCombatTab.razor
│   │   └── CharacterCreator.razor
│   └── Shared/
│       ├── CdmButton.razor
│       ├── CdmCard.razor
│       └── CdmModal.razor
├── Pages/
│   ├── Combat/
│   │   ├── CombatMaster.razor
│   │   ├── CombatPlayer.razor
│   │   └── CombatSetup.razor
│   ├── Campaigns/
│   │   ├── CampaignList.razor
│   │   ├── CampaignDetails.razor
│   │   └── CampaignCreator.razor
│   └── Characters/
│       ├── CharacterList.razor
│       └── CharacterDetails.razor
├── Services/
│   ├── CombatSignalRService.cs
│   ├── CombatStateService.cs
│   └── DiceAnimationService.cs
├── Hubs/
│   └── CombatHub.cs
└── wwwroot/
    ├── css/
    │   ├── combat-theme.css
    │   ├── animations.css
    │   └── dnd-components.css
    ├── js/
    │   ├── combat-signalr.js
    │   ├── dice-animations.js
    │   └── audio-manager.js
    └── sounds/
        ├── dice-roll.mp3
        ├── sword-clash.mp3
        └── spell-cast.mp3
```

### **📦 Packages NuGet Requis**
```xml
<!-- Cdm.Web.csproj -->
<PackageReference Include="MudBlazor" Version="6.11.2" />
<PackageReference Include="MudBlazor.ThemeManager" Version="1.0.8" />
<PackageReference Include="Microsoft.AspNetCore.SignalR.Client" Version="8.0.0" />
<PackageReference Include="Blazor.Extensions.Canvas" Version="1.1.1" />
<PackageReference Include="BlazorAnimate" Version="3.0.0" />
```

### **🔧 Configuration SignalR**
```csharp
// Program.cs
builder.Services.AddSignalR();
builder.Services.AddScoped<CombatSignalRService>();

// Dans Configure
app.MapHub<CombatHub>("/combatHub");
```

---

## 📅 **PLANNING DÉTAILLÉ AVEC MILESTONES**

### **🗓️ Semaines 1-3 : Foundation**
**Milestone 1 :** Design system opérationnel
- ✅ MudBlazor configuré avec thème D&D
- ✅ Composants de base créés et testés
- ✅ Layout principal refait
- ✅ Responsive mobile fonctionnel

### **🗓️ Semaines 4-7 : Combat Core**
**Milestone 2 :** Interface combat fonctionnelle
- ✅ SignalR Hub implémenté
- ✅ Dashboard MJ opérationnel
- ✅ Interface joueur basique
- ✅ Actions de combat exécutables

### **🗓️ Semaines 8-10 : UX & Polish**
**Milestone 3 :** Expérience immersive
- ✅ Animations combat fluides
- ✅ Sons et effets visuels
- ✅ Mobile optimisé
- ✅ Tests utilisateur positifs

### **🗓️ Semaines 11-12 : Campagnes**
**Milestone 4 :** Gestion modernisée
- ✅ Dashboard campagnes refait
- ✅ Fiches personnages modernes
- ✅ Navigation intuitive

### **🗓️ Semaines 13-15 : Sorts**
**Milestone 5 :** Sorts intégrés
- ✅ 74 sorts dans l'interface
- ✅ Casting visuel fonctionnel
- ✅ Effets de sorts

### **🗓️ Semaines 16-19 : Avancé**
**Milestone 6 :** Fonctionnalités premium
- ✅ Cartes tactiques
- ✅ Macros système
- ✅ IA interface

---

## 🎯 **MÉTRIQUES DE SUCCÈS**

### **📊 KPIs Techniques**
- **Temps de chargement** : < 2s pour interface combat
- **Temps de réponse** : < 200ms pour actions SignalR
- **Responsive** : 100% fonctionnel sur mobile/tablette
- **Accessibilité** : Score A+ sur tests d'accessibilité

### **👥 KPIs Utilisateur**
- **Courbe d'apprentissage** : < 5 minutes pour nouvelle interface
- **Satisfaction** : > 4.5/5 sur tests utilisateur
- **Adoption** : 80% des utilisateurs utilisent nouvelle interface
- **Rétention** : Diminution de 0% du taux de rebond

### **⚔️ KPIs Combat**
- **Actions/minute** : > 20 actions de combat par minute possible
- **Simultané** : Support de 8 joueurs simultanés minimum
- **Synchronisation** : < 100ms de délai entre joueurs
- **Erreurs** : 0 désynchronisation d'état de combat

---

## 🎨 **GUIDES DE STYLE & RÉFÉRENCES**

### **🎨 Inspiration Visuelle**
- **D&D Beyond** : Ergonomie et organisation
- **Roll20** : Interface de combat
- **Foundry VTT** : Animations et effets
- **Discord** : UX moderne et responsive

### **🎨 Palette de Couleurs**
```scss
// Primaires
$blood-red: #8B0000;       // Actions offensives
$ancient-gold: #DAA520;    // Actions importantes
$forest-green: #228B22;    // Actions positives
$deep-brown: #2D1B0B;      // Backgrounds

// Secondaires  
$parchment: #F4E4BC;       // Textes/cards
$steel-gray: #4F4F4F;      // UI elements
$flame-orange: #FF6B35;    // Alertes
$mystic-purple: #6A5ACD;   // Magie/sorts
```

### **🎨 Typographie**
```scss
// Headers épiques
$font-headers: 'Cinzel', 'Trajan Pro', serif;

// Corps de texte lisible  
$font-body: 'Source Sans Pro', 'Segoe UI', sans-serif;

// Interface/UI
$font-ui: 'Inter', 'Roboto', sans-serif;

// Monospace pour stats
$font-mono: 'JetBrains Mono', 'Consolas', monospace;
```

---

## 🚀 **DÉPLOIEMENT & TESTS**

### **🧪 Stratégie de Tests**
```csharp
// Tests composants Blazor
[Test]
public void CombatMasterDashboard_ShouldDisplayParticipants()
{
    // Arrange
    var combat = CreateTestCombat();
    
    // Act
    var component = RenderComponent<CombatMasterDashboard>(
        parameters => parameters.Add(p => p.CombatState, combat)
    );
    
    // Assert
    Assert.That(component.FindAll(".participant-card").Count, Is.EqualTo(4));
}
```

**Types de tests :**
- [ ] **Tests unitaires** : Composants Blazor isolés
- [ ] **Tests d'intégration** : SignalR et API
- [ ] **Tests E2E** : Playwright pour scénarios complets
- [ ] **Tests de performance** : Charge et stress
- [ ] **Tests d'accessibilité** : ARIA et navigation clavier

### **📱 Tests Responsive**
- [ ] **Mobile** : iPhone 12/13, Samsung Galaxy
- [ ] **Tablettes** : iPad, Surface Pro
- [ ] **Desktop** : 1920x1080, 2560x1440, 4K
- [ ] **Navigation** : Touch, clavier, souris

### **🌐 Tests Cross-Browser**
- [ ] **Chrome** : Dernières versions
- [ ] **Firefox** : Support complet
- [ ] **Safari** : iOS et macOS
- [ ] **Edge** : Chromium base

---

## 💡 **RECOMMANDATIONS FINALES**

### **🎯 Priorisation Recommandée**
1. **CRITIQUE** : Design System + Combat Interface (Phases 1-2)
2. **IMPORTANT** : Animations + Campagnes (Phases 3-4)  
3. **OPTIONNEL** : Sorts + Avancé (Phases 5-6)

### **⚡ Quick Wins**
- **MudBlazor** : Setup rapide avec composants prêts
- **SignalR** : Intégration .NET native
- **CSS Animations** : Effets impressionnants avec peu de code
- **API existante** : Backend combat déjà fonctionnel

### **🎨 Points d'Attention Design**
- **Lisibilité avant spectacle** : Stats doivent rester lisibles
- **Performances** : Animations fluides même sur mobiles
- **Accessibilité** : Navigation clavier pour tous les éléments
- **Cohérence** : Design system appliqué partout

### **⚔️ Points d'Attention Combat**
- **Temps réel critique** : Synchronisation parfaite requise
- **États complexes** : Gestion des conflits de modification
- **Rollback** : Possibilité d'annuler les actions
- **Offline** : Que faire en cas de déconnexion

---

## 🏆 **VISION FINALE**

### **🎯 Objectif à Atteindre**
**Créer LE VTT de référence pour D&D 5e** avec :
- ✅ Interface **plus intuitive que D&D Beyond**
- ✅ Combat **plus fluide que Roll20**  
- ✅ Animations **plus immersives que Foundry**
- ✅ Mobile **meilleur que tous les concurrents**

### **📈 Résultats Attendus**
- **Adoption massive** : Interface préférée des utilisateurs
- **Performance excellente** : Aucun lag en combat temps réel
- **Expérience immersive** : Sensation de vraie table de jeu
- **Évolutivité** : Base solide pour futures fonctionnalités

---

## 🎊 **CONCLUSION**

Cette roadmap transformera **Chronique des Mondes** en VTT de niveau AAA avec :

**19 semaines de développement** pour une interface **révolutionnaire** qui combine la **puissance technique** du backend existant avec une **expérience utilisateur** moderne et immersive.

**Résultat attendu : L'application D&D la plus avancée du marché !** ⚔️🎲✨

---

## 📚 **RÉFÉRENCES & LIENS**

### **🔗 Documentation Technique**
- [Backend Combat System](./COMBAT_SYSTEM.md) - Architecture et services
- [Tests Combat](./COMBAT_TESTING.md) - Guide de test du système
- [Changelog Combat](./CHANGELOG.md) - Historique des versions

### **🔗 Ressources Design**
- [MudBlazor Documentation](https://mudblazor.com/)
- [D&D 5e SRD](https://dnd.wizards.com/resources/systems-reference-document)
- [Material Design Guidelines](https://material.io/design)

### **🔗 Outils de Développement**
- [Blazor Documentation](https://docs.microsoft.com/aspnet/core/blazor)
- [SignalR Documentation](https://docs.microsoft.com/aspnet/core/signalr)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)

---

*Roadmap Frontend créée pour maximiser l'impact du système de combat existant avec une interface utilisateur de niveau professionnel. Version 1.0 - Décembre 2024*