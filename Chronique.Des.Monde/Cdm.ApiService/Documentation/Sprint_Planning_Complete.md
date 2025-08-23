# 🚀 **PLANIFICATION COMPLÈTE DES SPRINTS - CHRONIQUE DES MONDES**

*Document basé sur la Roadmap et l'audit complet du backlog*

---

## 📋 **VUE D'ENSEMBLE**

**Total User Stories :** 67 US  
**Total Tasks :** 77+ Tasks  
**Durée estimée :** 37 semaines (9 mois)  
**Sprints prévus :** 19 sprints de 2 semaines  

---

## 🎯 **SPRINT 1-2 : FONDATIONS DB (Semaines 1-4)**  
**Objectif :** Infrastructure de données et modèles fondamentaux  
**Durée :** 4 semaines | **Priorité :** CRITIQUE  

### **📊 User Stories assignées**

#### **US #144 - Entités et relations pour multi-rôles utilisateur** 
- **Task #252** - Entités CampaignParticipant avec relations Many-to-Many *(8h)*
- **Task #253** - API UserRoleController avec endpoints permissions *(10h)*
**Estimation :** 18h

#### **US #156 - Entités et modèle de données pour campagnes multi-jeux**
- **Task #254** - Entités Campaign avec GameType et settings JSON *(8h)*
- **Task #255** - CampaignService avec logique business et validation *(12h)*  
**Estimation :** 20h

#### **US #171 - Entités de base pour personnages multi-GameType**
- **Task #246** - Architecture entités Character avec pattern Strategy *(10h)*
- **Task #247** - Interface création personnage générique avec attributs dynamiques *(12h)*
**Estimation :** 22h

#### **US #173 - Entités D&D pour stats, races et classes**
- **Task #258** - Entités D&D spécialisées avec races et classes *(15h)*
- **Task #259** - Services business D&D avec calculs automatiques *(12h)*
**Estimation :** 27h

#### **US #182 - Architecture bi-niveau pour sorts officiels et privés**
- **Task #249** - Architecture entités Spell bi-niveau avec enums et SRD *(10h)*
**Estimation :** 10h

#### **US #192 - Architecture multi-instances pour équipements officiels**  
- **Task #198** - Créer entités Equipment et architecture multi-instances *(12h)*
**Estimation :** 12h

#### **Tasks Infrastructure générales**
- **Task #161** - Créer entités Campaign, Chapter et migrations *(8h)*
- **Task #147** - Créer entité CampaignParticipant et migrations *(6h)*
- **Task #178** - Créer entités Character et architecture multi-GameType *(10h)*
- **Task #179** - Entités D&D complètes avec races et classes *(15h)*
- **Task #188** - Créer entités Spell et architecture bi-niveau *(12h)*

**TOTAL SPRINT 1-2 :** ~150 heures *(2 développeurs × 4 semaines)*

---

## 🔮 **SPRINT 3-4 : SORTS CORE (Semaines 5-8)**  
**Objectif :** Système de sorts complet avec logique business  
**Durée :** 4 semaines | **Priorité :** HAUTE

### **📊 User Stories assignées**

#### **US #183 - Interface d'administration des sorts officiels**
- Endpoints CRUD administrateur
- Import/Export JSON SRD
**Estimation :** 16h

#### **US #184 - Interface de création de sorts personnalisés**  
- Formulaire complet création sorts
- Validation règles business
**Estimation :** 20h

#### **US #185 - Moteur de calculs automatiques pour sorts D&D**
- **Task #250** - Moteur calculs sorts D&D 5e avec surincantation *(12h)*
- **Task #189** - Service SpellCalculationService avec règles D&D 5e *(10h)*
**Estimation :** 22h

#### **US #186 - Système de grimoires et apprentissage de sorts**
- **Task #190** - Interface grimoire avec gestion sorts connus/préparés *(15h)*
**Estimation :** 15h

#### **US #187 - Système de validation et modération des sorts**
- Workflow de modération
- Système de scoring abus
**Estimation :** 18h

#### **Tasks API et Services**
- **Task #191** - API SpellController avec permissions bi-niveau *(12h)*

**TOTAL SPRINT 3-4 :** ~103 heures

---

## ⚔️ **SPRINT 5-6 : SORTS INTERFACE (Semaines 9-12)**  
**Objectif :** Interfaces utilisateur pour sorts et grimoires  
**Durée :** 4 semaines | **Priorité :** HAUTE

### **📊 User Stories assignées**

#### **Interface et UX Sorts**
- Pages Blazor création/édition sorts
- Interface grimoire interactif
- Système apprentissage sorts par classe
- Tests d'intégration complets

**TOTAL SPRINT 5-6 :** ~80 heures

---

## 🛡️ **SPRINT 7-8 : ÉQUIPEMENTS CORE (Semaines 13-16)**  
**Objectif :** Système d'équipements avec inventaires  
**Durée :** 4 semaines | **Priorité :** HAUTE

### **📊 User Stories assignées**

#### **US #193 - Interface d'administration des équipements officiels**
- Interface admin équipements
- Import SRD équipements D&D
**Estimation :** 16h

#### **US #194 - Interface de création d'équipements personnalisés** 
- Formulaire création équipements
- Upload images équipements uniques
**Estimation :** 18h

#### **US #197 - Interface de gestion d'inventaire avec équipement auto**
- **Task #200** - Interface inventaire avec drag & drop et calculs automatiques *(15h)*
**Estimation :** 15h

#### **Tasks Services**
- **Task #201** - API EquipmentController et TradeController complets *(12h)*

**TOTAL SPRINT 7-8 :** ~80 heures

---

## 🤝 **SPRINT 9-10 : ÉQUIPEMENTS ÉCHANGES (Semaines 17-20)**  
**Objectif :** Système complet d'échanges d'équipements  
**Durée :** 4 semaines | **Priorité :** MOYENNE

### **📊 User Stories assignées**

#### **US #195 - Système de propositions d'équipements MJ vers joueurs**
- Interface MJ propositions
- Notifications équipements
**Estimation :** 18h

#### **US #196 - Système d'échanges d'équipements entre joueurs**
- Interface échange joueur-joueur
- Validation MJ optionnelle
**Estimation :** 20h

#### **Tasks Infrastructure**
- **Task #199** - Système d'échanges avec entités TradeOffer et notifications *(15h)*

**TOTAL SPRINT 9-10 :** ~70 heures

---

## 🏰 **SPRINT 11-12 : CAMPAGNES STRUCTURE (Semaines 21-24)**  
**Objectif :** Architecture campagnes et chapitres  
**Durée :** 4 semaines | **Priorité :** MOYENNE

### **📊 User Stories assignées**

#### **US #157 - Interface de création de campagne Blazor**
- **Task #162** - Page CreateCampaign.razor avec sélecteur GameType *(12h)*
**Estimation :** 12h

#### **US #158 - Structure des chapitres et navigation séquentielle**
- **Task #163** - Éditeur de chapitres avec drag & drop et rich text *(15h)*
**Estimation :** 15h

#### **US #159 - Gestion des PNJ et monstres par chapitre**
- **Task #260** - Entités NPC et Monster avec comportements *(10h)*
- **Task #261** - Interface édition PNJ avec drag & drop *(12h)*
**Estimation :** 22h

#### **US #160 - Système d'invitations et gestion des joueurs**
- **Task #168** - Système complet d'invitations et notifications *(15h)*
**Estimation :** 15h

#### **Tasks API**
- **Task #169** - API CampaignController avec endpoints CRUD *(10h)*
- **Task #170** - API ChapterController pour gestion des chapitres *(8h)*
- **Task #166** - Créer entités NPC, Monster et comportements *(8h)*
- **Task #167** - Interface d'édition PNJ/Monstres dans chapitres *(12h)*

**TOTAL SPRINT 11-12 :** ~112 heures

---

## 🎭 **SPRINT 13-14 : CAMPAGNES INTERFACE (Semaines 25-28)**  
**Objectif :** Interfaces complètes gestion campagnes  
**Durée :** 4 semaines | **Priorité :** MOYENNE

### **📊 User Stories assignées**

#### **US #146 - Interface Blazor de sélection de rôle**
- **Task #149** - Composant Blazor RoleSwitcher et State Management *(8h)*
**Estimation :** 8h

#### **US #172 - Interface de création de personnage générique**
- Interface création personnages simples
**Estimation :** 15h

#### **US #174 - Interface de création personnage D&D avancée**
- **Task #248** - Wizard création personnage D&D avec calculs 5e *(20h)*
- **Task #180** - Wizard création personnage D&D multi-étapes *(15h)*
**Estimation :** 35h

#### **US #177 - Interface de gestion et modification de personnages**
- Pages édition et level-up
**Estimation :** 18h

#### **Tasks Business Logic**
- **Task #148** - Implémenter UserRoleService pour gestion des rôles *(10h)*
- **Task #245** - Service UserRoleService avec logique multi-rôles complexe *(12h)*
- **Task #181** - API CharacterController avec endpoints CRUD *(10h)*

**TOTAL SPRINT 13-14 :** ~118 heures

---

## ⚔️ **SPRINT 15-16 : COMBAT SYSTÈME (Semaines 29-32)**  
**Objectif :** Système de combat D&D complet  
**Durée :** 4 semaines | **Priorité :** MOYENNE

### **📊 User Stories assignées**

#### **US #202 - Interface MJ complète de gestion des combats**
- **Task #209** - Interface MJ combat avec gestion complète *(20h)*
**Estimation :** 20h

#### **US #203 - Système d'invitations dynamiques pour combats**
- Invitations temps réel WebSocket
**Estimation :** 15h

#### **US #204 - Moteur d'initiative automatique et gestion des tours**
- Calculs initiative D&D 5e
**Estimation :** 18h

#### **US #205 - Interface combat temps réel avec notifications visuelles**
- Animations et feedback visuel
**Estimation :** 20h

#### **US #206 - Actions contextuelles combat avec timer optionnel**
- Interface actions combat
**Estimation :** 16h

#### **Tasks Infrastructure**
- **Task #207** - Entités Combat et architecture temps réel *(15h)*
- **Task #208** - SignalR CombatHub pour synchronisation temps réel *(12h)*
- **Task #210** - Interface joueur combat avec actions et animations *(18h)*

**TOTAL SPRINT 15-16 :** ~134 heures

---

## 🎪 **SPRINT 17-18 : SESSIONS INFRASTRUCTURE (Semaines 33-36)**  
**Objectif :** Architecture sessions temps réel  
**Durée :** 4 semaines | **Priorité :** FAIBLE

### **📊 User Stories assignées**

#### **US #211 - Lancement de session depuis campagnes multi-sources**
- Interface lancement sessions
**Estimation :** 15h

#### **US #212 - Système d'invitations pré-session avec notifications multi-canal**
- Notifications WebSocket + Email
**Estimation :** 20h

#### **US #213 - Progression automatique des chapitres avec sauvegarde**
- Sauvegarde automatique
**Estimation :** 18h

#### **US #214 - Synchronisation temps réel avec gestion robuste des déconnexions**
- Reconnexion automatique
**Estimation :** 22h

#### **US #215 - Historique complet avec restauration d'états**
- Timeline et export rapports
**Estimation :** 25h

#### **Tasks Infrastructure**
- **Task #216** - Entités Session et architecture multi-sources *(15h)*
- **Task #217** - SignalR SessionHub avec synchronisation état temps réel *(18h)*
- **Task #218** - BackgroundService pour notifications et sauvegardes automatiques *(12h)*
- **Task #219** - Interfaces Blazor sessions et historique avec timeline *(20h)*

**TOTAL SPRINT 17-18 :** ~165 heures

---

## 📊 **SPRINT 19-20 : SESSIONS INTERFACE (Semaines 37-40)**  
**Objectif :** Interfaces sessions utilisateur  
**Durée :** 4 semaines | **Priorité :** FAIBLE

### **📊 User Stories assignées**

#### **Interfaces Sessions**
- Pages lancement sessions
- Historique avec timeline
- Gestion des participants

**TOTAL SPRINT 19-20 :** ~80 heures

---

## 📈 **SPRINTS ADDITIONNELS : STATISTIQUES ET SUCCÈS**  
**Objectif :** Gamification et analytics avancées  
**Durée :** 8-10 semaines | **Priorité :** TRÈS FAIBLE

### **📊 User Stories assignées**

#### **US #220 - Collecte automatique de métriques gaming en temps réel**
- **Task #226** - BackgroundService collecte métriques temps réel *(15h)*
**Estimation :** 15h

#### **US #221 - Tableaux de bord statistiques Blazor interactifs avec graphiques**
- **Task #227** - Tableaux de bord Blazor avec graphiques interactifs *(20h)*
**Estimation :** 20h

#### **US #222 - Système de succès gamifiés avec 5 niveaux de rareté**
- Système achievements complet
**Estimation :** 25h

#### **US #223 - Analytics comportementales avec ML.NET pour détection patterns**
- **Task #228** - ML.NET analytics comportementales et leaderboards *(25h)*
**Estimation :** 25h

#### **US #224 - Leaderboards communautaires avec classements multiples**
- Classements temps réel
**Estimation :** 18h

#### **Tasks Infrastructure**
- **Task #225** - Entités Statistiques et architecture métriques *(12h)*

---

## 🔧 **SPRINTS TRANSVERSAUX : SÉCURITÉ ET UX**

### **📊 User Stories Sécurité (Priorité CRITIQUE)**

#### **US #128 - Endpoint d'inscription utilisateur avec validation**
- **Task #135** - Améliorer validation endpoint /register *(6h)*
**Estimation :** 6h

#### **US #129 - Endpoint de connexion avec génération de token JWT**
- **Task #136** - Améliorer sécurité endpoint /login *(8h)*
**Estimation :** 8h

#### **US #130 - Middleware d'authentification JWT pour API**
- **Task #137** - Configurer authentification JWT Bearer dans Program.cs *(6h)*
- **Task #138** - Ajouter attributs [Authorize] aux endpoints protégés *(4h)*
**Estimation :** 10h

#### **US #131 - Page de connexion Blazor responsive**
- **Task #139** - Créer composant Login.razor avec validation *(8h)*
- **Task #140** - Intégrer appels API dans page Login *(6h)*
**Estimation :** 14h

#### **US #132 - Page d'inscription Blazor avec validation**
- **Task #142** - Créer page Register.razor avec validation avancée *(10h)*
**Estimation :** 10h

#### **US #133 - Service d'authentification Blazor avec gestion d'état**
- **Task #141** - Créer AuthenticationService centralisé *(12h)*
**Estimation :** 12h

#### **US #134 - Tests d'intégration authentification API et Blazor**
- **Task #143** - Développer tests unitaires API authentification *(15h)*
**Estimation :** 15h

#### **US #145 - Service de gestion des rôles utilisateur**
- Logique business rôles
**Estimation :** 12h

#### **US #150 - Middleware d'autorisation basé sur les rôles**
- **Task #256** - Middleware d'autorisation avec attributs personnalisés *(10h)*
- **Task #154** - Implémenter attributs d'autorisation personnalisés *(8h)*
**Estimation :** 18h

#### **US #151 - Gestion des sessions actives et refresh token**
- **Task #257** - SessionService avec refresh tokens et auto-logout *(12h)*
- **Task #164** - Implémenter service de gestion des sessions utilisateur *(10h)*
**Estimation :** 22h

#### **US #152 - Workflow de demande de reset mot de passe**
- **Task #155** - Service d'envoi d'email et templates de reset *(8h)*
**Estimation :** 8h

#### **US #153 - Confirmation et validation du nouveau mot de passe**
- **Task #165** - Page ResetPassword.razor et endpoint confirmation *(10h)*
**Estimation :** 10h

### **📊 User Stories UX/Design (Priorité MOYENNE)**

#### **US #229 - Interface authentification Blazor moderne et sécurisée**
- **Task #234** - Architecture Blazor avec authentication et layout responsive *(15h)*
**Estimation :** 15h

#### **US #230 - Interface gestion campagnes avec éditeur chapitres intégré**
- **Task #236** - Interfaces campagnes avec éditeur Markdown avancé *(18h)*
**Estimation :** 18h

#### **US #231 - Interface création personnages D&D avec formulaires interactifs**
- **Task #237** - Interfaces personnages D&D et combat temps réel *(20h)*
**Estimation :** 20h

#### **US #232 - Interface combat temps réel avec SignalR et interactions**
- Interface combat immersive
**Estimation :** 25h

#### **US #233 - Design system complet avec composants Blazor réutilisables**
- **Task #235** - Design system et composants UI réutilisables *(20h)*
**Estimation :** 20h

### **📊 User Stories Avancées (Priorité FAIBLE)**

#### **US #175 - Système de validation compatibilité GameType**
- Validation inter-GameType
**Estimation :** 12h

#### **US #176 - Duplication et conversion inter-GameType**
- Conversion personnages
**Estimation :** 15h

#### **US #238-241 - User Stories récentes**
- API Login sécurisée
- Page Login moderne
- Marketplace campagnes
- Architecture Combat
**Estimation :** 60h

---

## 📋 **RÉSUMÉ EXÉCUTIF**

### **🎯 Priorisation Recommandée**

#### **PHASE 1 - FONDATIONS (Sprints 1-6) :** 
- **Sécurité et authentification** *(Sprint 0 - parallèle)*
- **Entités et base de données** *(Sprints 1-2)*
- **Système de sorts complet** *(Sprints 3-6)*

#### **PHASE 2 - FONCTIONNALITÉS CŒUR (Sprints 7-14) :**
- **Équipements et échanges** *(Sprints 7-10)*
- **Campagnes et personnages** *(Sprints 11-14)*

#### **PHASE 3 - EXPÉRIENCE AVANCÉE (Sprints 15-20) :**
- **Combat temps réel** *(Sprints 15-16)*
- **Sessions collaboratives** *(Sprints 17-20)*

#### **PHASE 4 - GAMIFICATION (Sprints 21+) :**
- **Statistiques et succès**
- **Analytics ML.NET**

### **⚡ Métriques Finales**

- **Total estimé :** ~1200 heures de développement
- **Équipe de 2 développeurs :** 9 mois
- **Équipe de 3 développeurs :** 6 mois  
- **Livraisons incrémentales :** Toutes les 2 semaines
- **MVP fonctionnel :** Sprint 6 (12 semaines)
- **Produit complet :** Sprint 20 (40 semaines)

### **🚀 Jalons Critiques**

- **Sprint 2 :** Infrastructure DB complète
- **Sprint 6 :** Sorts et personnages jouables
- **Sprint 10 :** Échanges et économie
- **Sprint 14 :** Campagnes complètes
- **Sprint 16 :** Combat temps réel
- **Sprint 20 :** Sessions collaboratives

---

**✅ Cette planification détaillée garantit une livraison progressive de valeur avec validation continue à chaque sprint !**