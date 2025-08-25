# Systèmes de Sorts et Équipements - Chronique des Mondes

Ce document détaille les spécifications complètes pour les systèmes de sorts et d'équipements, incluant la gestion des modificateurs de lancer spécifiques D&D et les mécaniques d'échange entre joueurs.

## 🪄 Système de Sorts

### Architecture Générale

#### Gestion Multi-Système
- **Vue globale** : Liste de tous les sorts avec filtre par système de jeu (par défaut : générique)
- **Compatibilité** : Un personnage D&D ne peut pas apprendre un sort Skyrim
- **Unicité** : Un personnage ne peut avoir qu'une seule instance de chaque sort
- **Échanges** : **❌ AUCUN échange possible** entre utilisateurs

#### **Types de Sorts - Architecture en 2 Niveaux**

##### **1. Sorts Officiels (Base de Données) 🌟**
- **Source** : Injection SQL par l'administrateur
- **Visibilité** : **Publics pour TOUS les utilisateurs**
- **Maintenance** : Gérés par l'équipe de développement
- **Système par système** :
  - **D&D** : Sorts officiels avec toutes les propriétés (dégâts, DD, coûts)
  - **Skyrim** : Ajoutés via script d'injection
  - **Génériques** : Sorts de base non spécialisés

##### **2. Sorts Privés Utilisateurs 👤**
- **Source** : Créés par les utilisateurs individuels
- **Visibilité** : **Privés uniquement à leur créateur**
- **Partage** : **IMPOSSIBLE** - pas de système de marketplace
- **Usage** : Pour campagnes personnalisées et besoins spécifiques

#### **Workflow de Création Utilisateur**

##### **Sorts Génériques (Campagnes Génériques)**
- **Champs obligatoires** : Titre, description, image (optionnelle)
- **Usage** : Le MJ/joueur gère manuellement avec ses règles JDR
- **Aucune assistance de combat automatique**

##### **Sorts avec Tags Spécialisés (ex: D&D)**
- **Tag D&D** ajoute automatiquement :
  - Dégâts (ex: `2d8`)
  - Jet d'attaque requis (ex: `1d20 + modificateur Intelligence`)
  - Coût en emplacements de sort (ex: `1 emplacement niveau 2`)
  - École de magie, temps d'incantation, portée, durée, composants
- **Assistance de combat** : Calculs automatiques dans le système

## ⚔️ Système d'Équipements

### Architecture Générale

#### **Logique Bi-Niveaux Étendue avec Échange**

##### **1. Équipements Officiels (Base de Données) 🌟**
- **Source** : Injection SQL administrative
- **Visibilité** : **Publics pour tous**
- **Maintenance** : Scripts d'injection par système de jeu

##### **2. Équipements Privés Utilisateurs 👤**
- **Source** : Créés par les utilisateurs
- **Visibilité** : **Privés uniquement**
- **Multi-instances** : Un personnage peut avoir plusieurs exemplaires

#### **Système d'Échange d'Équipements** 🔄

##### **MJ → Joueur (Proposition d'Équipement)**
- **Le MJ peut proposer** ses équipements personnalisés aux joueurs de sa campagne
- **Source MJ** : Équipements officiels OU équipements privés créés par le MJ
- **Mécanisme** : Le joueur accepte/refuse la proposition
- **Résultat si accepté** :
  - ✅ L'équipement est **ajouté** à l'inventaire du joueur
  - ✅ L'équipement **reste disponible** chez le MJ (pas de suppression)
  - ✅ Le MJ peut re-proposer le même équipement à d'autres joueurs

##### **Joueur → Joueur (Échange Direct)**
- **Contexte** : Uniquement entre joueurs de la **même campagne**
- **Mécanisme** : Échange direct d'objets de l'inventaire
- **Résultat** :
  - ❌ Le joueur donneur **perd** l'objet de son inventaire
  - ✅ Le joueur receveur **gagne** l'objet dans son inventaire
  - 🔄 Transfert de propriété complet

#### Types d'Équipements Utilisateur

##### **Équipements Génériques**
- **Champs obligatoires** : Titre, description, image, tags de recherche
- **Gestion manuelle** par le MJ selon ses règles

##### **Équipements avec Bonus Spécialisés (ex: D&D)**
- **Bonus de toucher** : Ajout automatique à la dextérité/force
- **Formule de dégâts** : (ex: `1d8 + modificateur Force`)
- **Calculs automatiques** : CA, bonus d'attaque, etc.

## 🎯 Modificateurs de Lancer D&D (Priorité Haute)

### Classes et leurs Modificateurs Principaux

```csharp
public enum SpellcastingAbility
{
    Intelligence,  // Magicien, Occultiste
    Wisdom,        // Clerc, Druide, Rôdeur
    Charisma       // Barde, Ensorceleur, Paladin
}

public static class DndSpellcastingRules
{
    public static SpellcastingAbility GetSpellcastingAbility(string characterClass)
    {
        return characterClass switch
        {
            "Magicien" => SpellcastingAbility.Intelligence,
            "Occultiste" => SpellcastingAbility.Intelligence,
            "Clerc" => SpellcastingAbility.Wisdom,
            "Druide" => SpellcastingAbility.Wisdom,
            "Rôdeur" => SpellcastingAbility.Wisdom,
            "Barde" => SpellcastingAbility.Charisma,
            "Ensorceleur" => SpellcastingAbility.Charisma,
            "Paladin" => SpellcastingAbility.Charisma,
            _ => throw new ArgumentException($"Classe {characterClass} n'est pas un lanceur de sorts")
        };
    }
    
    public static int CalculateSpellAttackBonus(int abilityScore, int proficiencyBonus)
    {
        var abilityModifier = (abilityScore - 10) / 2;
        return abilityModifier + proficiencyBonus;
    }
    
    public static int CalculateSpellSaveDC(int abilityScore, int proficiencyBonus)
    {
        return 8 + CalculateSpellAttackBonus(abilityScore, proficiencyBonus);
    }
}
```

### Exemples Concrets
- **Mage niveau 5** (Intelligence 18, bonus maîtrise +3)
  - Modificateur Intelligence : +4
  - Bonus d'attaque de sort : +7 (4 + 3)
  - DD de sauvegarde : 15 (8 + 4 + 3)

- **Paladin niveau 3** (Charisme 14, bonus maîtrise +2)
  - Modificateur Charisme : +2
  - Bonus d'attaque de sort : +4 (2 + 2)
  - DD de sauvegarde : 12 (8 + 2 + 2)

## 📡 Endpoints API

### Gestion des Sorts

| Méthode | Endpoint | Description |
|---------|----------|-------------|
| `GET` | `/spells?gameType={type}&userId={id}` | Sorts officiels + privés utilisateur |
| `GET` | `/spells/official?gameType={type}` | **Sorts officiels uniquement** |
| `GET` | `/spells/user?gameType={type}&userId={id}` | **Sorts privés utilisateur uniquement** |
| `POST` | `/spell?userId={id}` | **Création de sort privé utilisateur** |
| `GET` | `/character/{id}/spells` | Sorts connus du personnage |
| `POST` | `/character/{id}/spells/{spellId}` | Apprendre un sort |

### Gestion des Équipements

| Méthode | Endpoint | Description |
|---------|----------|-------------|
| `GET` | `/equipment?gameType={type}&userId={id}` | Équipements officiels + privés utilisateur |
| `POST` | `/equipment?userId={id}` | **Création d'équipement privé utilisateur** |
| `GET` | `/character/{id}/inventory` | Inventaire du personnage |
| `POST` | `/character/{id}/inventory/{equipmentId}` | Ajouter équipement |

### Échanges d'Équipements

| Méthode | Endpoint | Description |
|---------|----------|-------------|
| `POST` | `/campaign/{campaignId}/equipment/offer` | **MJ propose équipement à joueur** |
| `PUT` | `/campaign/{campaignId}/equipment/offer/{offerId}` | **Joueur accepte/refuse proposition** |
| `POST` | `/campaign/{campaignId}/equipment/trade` | **Échange direct joueur→joueur** |
| `GET` | `/campaign/{campaignId}/equipment/offers?playerId={id}` | **Liste des propositions en attente** |

## 📊 Modèles de Données

### Structure Spell avec Source
```csharp
public class Spell
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? ImageUrl { get; set; }
    public string GameType { get; set; }
    public int CreatedByUserId { get; set; } // 0 = Administrateur
    public bool IsPublic { get; set; } // true pour officiels, false pour privés
    public List<string> Tags { get; set; }
    public SpellDndProperties? DndProperties { get; set; }
    
    // Propriété calculée
    public SpellSource Source => CreatedByUserId == 0 ? SpellSource.Official : SpellSource.Private;
}

public enum SpellSource
{
    Official,  // Créé par admin (injection SQL)
    Private    // Créé par utilisateur
}
```

### Modèles pour Échanges
```csharp
public class EquipmentOffer
{
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public int GameMasterId { get; set; }
    public int TargetPlayerId { get; set; }
    public int EquipmentId { get; set; }
    public int Quantity { get; set; }
    public string? Message { get; set; }
    public OfferStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class EquipmentTrade
{
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public int FromPlayerId { get; set; }
    public int ToPlayerId { get; set; }
    public int EquipmentId { get; set; }
    public int Quantity { get; set; }
    public TradeStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

## 🔧 Services Métier

### Interfaces de Services
```csharp
public interface ISpellService
{
    Task<List<Spell>> GetOfficialSpellsAsync(string gameType);
    Task<List<Spell>> GetUserPrivateSpellsAsync(int userId, string gameType);
    Task<bool> CanUserModifySpellAsync(int userId, int spellId);
}

public interface IEquipmentExchangeService
{
    Task<EquipmentOffer> CreateOfferAsync(int campaignId, int gmId, int playerId, int equipmentId, int quantity, string? message);
    Task<EquipmentTrade> ProposeTradeAsync(int campaignId, int fromPlayerId, int toPlayerId, int equipmentId, int quantity, string? message);
    Task<bool> ArePlayersInSameCampaignAsync(int campaignId, int playerId1, int playerId2);
}
```

## 🔒 Validation des Règles Métier

### **Sorts**
- **Sorts officiels** : Non modifiables par les utilisateurs
- **Sorts privés** : Modifiables uniquement par leur créateur
- **Compatibilité système** : Validation stricte gameType personnage/sort
- **Unicité** : Un personnage ne peut avoir qu'une fois chaque sort
- **❌ Aucun échange possible** entre utilisateurs

### **Équipements et Échanges** 🔄
- **Propositions MJ** : Seul le MJ peut proposer des équipements aux joueurs de sa campagne
- **Acceptation équipement** : L'équipement est copié (pas déplacé) depuis le MJ vers le joueur
- **Échanges joueurs** : Uniquement entre joueurs de la même campagne
- **Transfert propriété** : L'équipement est déplacé (pas copié) entre joueurs
- **Validation quantité** : Vérification que le joueur possède la quantité à échanger

---

*Retour au [README principal](./README.md) | Voir aussi : [Schéma de Base de Données](./DatabaseSchema.md)*