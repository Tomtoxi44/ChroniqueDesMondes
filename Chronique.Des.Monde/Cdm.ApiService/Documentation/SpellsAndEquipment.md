# Systèmes de Sorts et Équipements - Chronique des Mondes

Ce document détaille les spécifications complètes pour les systèmes de sorts et d'équipements, incluant la gestion des modificateurs de lancer spécifiques D&D.

## 🪄 Système de Sorts

### Architecture Générale

#### Gestion Multi-Système
- **Vue globale** : Liste de tous les sorts avec filtre par système de jeu (par défaut : générique)
- **Compatibilité** : Un personnage D&D ne peut pas apprendre un sort Skyrim
- **Unicité** : Un personnage ne peut avoir qu'une seule instance de chaque sort

#### Types de Sorts

##### 1. **Sorts Génériques**
- **Champs obligatoires** :
  - Titre
  - Description
  - Image (optionnelle)
- **Visibilité** : Privés à l'utilisateur créateur
- **Évolutivité** : Possibilité d'ajouter des tags pour spécialiser

##### 2. **Sorts avec Tags Spécialisés**
- **Tag D&D** ajoute :
  - Dégâts (ex: `2d8`)
  - Jet d'attaque requis (ex: `1d20 + modificateur`)
  - Coût en emplacements de sort (ex: `1 emplacement niveau 1`)
  - École de magie
  - Temps d'incantation
  - Portée et durée
  - Composants (V, S, M)

##### 3. **Sorts Communautaires/Officiels**
- **Sorts prédéfinis** en base pour chaque système
- **Visibilité publique** pour tous les joueurs
- **Maintenance** par les administrateurs

## ⚔️ Système d'Équipements

### Architecture Générale

#### Gestion Multi-Instances
- **Accumulation** : Un personnage peut avoir plusieurs exemplaires du même objet
- **Inventaire** : Système de quantités par item

#### Types d'Équipements

##### 1. **Équipements Génériques**
- **Champs obligatoires** :
  - Titre
  - Description
  - Image (optionnelle)
  - Tags pour recherche rapide

##### 2. **Équipements avec Bonus Spécialisés**
- **Bonus de toucher** : Ajout à la dextérité ou autre caractéristique
- **Dégâts** : Formule de dégâts (ex: `1d8 + modificateur Force`)
- **Propriétés spéciales** : Effets magiques, résistances, etc.

##### 3. **Équipements D&D Spécialisés**
- **Stats complètes** : CA, bonus, malus, pré-requis
- **Intégration combat** : Calculs automatiques des modificateurs

## 🎯 Modificateurs de Lancer D&D (Priorité Haute)

### Système de Classe et Caractéristique de Lancer

#### Classes et leurs Modificateurs Principaux

##### **Classes de Lanceurs de Sorts**
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

#### Exemples Concrets
- **Mage niveau 5** (Intelligence 18, bonus maîtrise +3)
  - Modificateur Intelligence : +4
  - Bonus d'attaque de sort : +7 (4 + 3)
  - DD de sauvegarde : 15 (8 + 4 + 3)

- **Paladin niveau 3** (Charisme 14, bonus maîtrise +2)
  - Modificateur Charisme : +2
  - Bonus d'attaque de sort : +4 (2 + 2)
  - DD de sauvegarde : 12 (8 + 2 + 2)

## 📡 Nouveaux Endpoints API

### Gestion des Sorts

#### Endpoints Sorts Globaux
| Méthode | Endpoint | Description | Headers Requis |
|---------|----------|-------------|----------------|
| `GET` | `/spells?gameType={type}&userId={id}` | Liste des sorts disponibles | `Authorization: Bearer {token}` |
| `GET` | `/spell/{id}` | Détails d'un sort | `Authorization: Bearer {token}` |
| `POST` | `/spell?userId={id}` | Création de sort personnalisé | `Authorization`, `X-GameType: {type}` |
| `PUT` | `/spell/{id}` | Modification de sort (si propriétaire) | `Authorization: Bearer {token}` |
| `DELETE` | `/spell/{id}` | Suppression de sort (si propriétaire) | `Authorization: Bearer {token}` |

#### Endpoints Sorts de Personnage
| Méthode | Endpoint | Description | Headers Requis |
|---------|----------|-------------|----------------|
| `GET` | `/character/{id}/spells` | Sorts connus du personnage | `Authorization: Bearer {token}` |
| `POST` | `/character/{id}/spells/{spellId}` | Apprendre un sort | `Authorization: Bearer {token}` |
| `DELETE` | `/character/{id}/spells/{spellId}` | Oublier un sort | `Authorization: Bearer {token}` |

### Gestion des Équipements

#### Endpoints Équipements Globaux
| Méthode | Endpoint | Description | Headers Requis |
|---------|----------|-------------|----------------|
| `GET` | `/equipment?gameType={type}&userId={id}` | Liste des équipements disponibles | `Authorization: Bearer {token}` |
| `GET` | `/equipment/{id}` | Détails d'un équipement | `Authorization: Bearer {token}` |
| `POST` | `/equipment?userId={id}` | Création d'équipement personnalisé | `Authorization`, `X-GameType: {type}` |
| `PUT` | `/equipment/{id}` | Modification d'équipement | `Authorization: Bearer {token}` |
| `DELETE` | `/equipment/{id}` | Suppression d'équipement | `Authorization: Bearer {token}` |

#### Endpoints Inventaire de Personnage
| Méthode | Endpoint | Description | Headers Requis |
|---------|----------|-------------|----------------|
| `GET` | `/character/{id}/inventory` | Inventaire du personnage | `Authorization: Bearer {token}` |
| `POST` | `/character/{id}/inventory/{equipmentId}` | Ajouter équipement (quantité) | `Authorization: Bearer {token}` |
| `PUT` | `/character/{id}/inventory/{equipmentId}` | Modifier quantité équipement | `Authorization: Bearer {token}` |
| `DELETE` | `/character/{id}/inventory/{equipmentId}` | Retirer équipement | `Authorization: Bearer {token}` |

## 🎮 Interfaces Utilisateur

### 1. **Page Sorts Globale** (`/spells`)

#### Fonctionnalités
- **Filtre par système** : D&D, Skyrim, Générique
- **Filtre par école** (D&D) : Évocation, Enchantement, etc.
- **Recherche textuelle** par nom et description
- **Tri** : par niveau, alphabétique, popularité

#### Actions par Sort
- **Voir détails** : Modal avec informations complètes
- **Ajouter au personnage** : Si compatible et pas déjà appris
- **Créer similaire** : Dupliquer et modifier un sort existant

### 2. **Interface Personnage - Onglet Sorts**

#### Vue Dédiée
- **Sorts connus** : Liste des sorts appris par le personnage
- **Sorts disponibles** : Filtrage automatique selon le gameType du personnage
- **Modificateurs** : Affichage du bonus d'attaque et DD calculés
- **Emplacements** : Gestion des emplacements de sorts par niveau (D&D)

#### Calculs Automatiques
```typescript
interface CharacterSpellcasting {
  spellcastingAbility: 'intelligence' | 'wisdom' | 'charisma';
  spellAttackBonus: number;
  spellSaveDC: number;
  spellSlots: { [level: number]: { max: number, used: number } };
  knownSpells: Spell[];
}
```

### 3. **Interface Personnage - Onglet Équipements**

#### Gestion d'Inventaire
- **Équipements portés** : Avec calculs automatiques de CA, bonus, etc.
- **Inventaire** : Objets possédés avec quantités
- **Équipements disponibles** : Filtrés selon le gameType

#### Actions Rapides
- **Équiper/Déséquiper** : Calculs automatiques des bonus
- **Ajouter à l'inventaire** : Avec gestion des quantités
- **Créer équipement personnalisé** : Selon les besoins de la campagne

## 📊 Modèles de Données

### Structure Spell
```csharp
public class Spell
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? ImageUrl { get; set; }
    public string GameType { get; set; }
    public int CreatedByUserId { get; set; }
    public bool IsPublic { get; set; }
    public List<string> Tags { get; set; }
    
    // Propriétés D&D spécialisées
    public SpellDndProperties? DndProperties { get; set; }
}

public class SpellDndProperties
{
    public int Level { get; set; }
    public string School { get; set; }
    public string CastingTime { get; set; }
    public string Range { get; set; }
    public string Duration { get; set; }
    public List<string> Components { get; set; }
    public string? DamageFormula { get; set; }
    public bool RequiresAttackRoll { get; set; }
    public bool RequiresSavingThrow { get; set; }
    public string? SavingThrowAbility { get; set; }
}
```

### Structure Equipment
```csharp
public class Equipment
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? ImageUrl { get; set; }
    public string GameType { get; set; }
    public int CreatedByUserId { get; set; }
    public bool IsPublic { get; set; }
    public List<string> Tags { get; set; }
    
    // Propriétés génériques
    public EquipmentGenericProperties? GenericProperties { get; set; }
    
    // Propriétés D&D spécialisées
    public EquipmentDndProperties? DndProperties { get; set; }
}

public class EquipmentGenericProperties
{
    public decimal Weight { get; set; }
    public int Value { get; set; }
    public string? AttackBonusAbility { get; set; }
    public string? DamageFormula { get; set; }
}

public class EquipmentDndProperties
{
    public string EquipmentType { get; set; } // Weapon, Armor, Shield, etc.
    public string? WeaponCategory { get; set; } // Simple, Martial
    public string? ArmorCategory { get; set; } // Light, Medium, Heavy
    public int? ArmorClassBase { get; set; }
    public int? ArmorClassDexBonus { get; set; }
    public string? DamageType { get; set; }
    public List<string> Properties { get; set; }
    public string Rarity { get; set; }
    public bool RequiresAttunement { get; set; }
}
```

### Structure CharacterSpell et CharacterEquipment
```csharp
public class CharacterSpell
{
    public int CharacterId { get; set; }
    public int SpellId { get; set; }
    public DateTime LearnedDate { get; set; }
    public bool IsPrepared { get; set; } // Pour D&D
}

public class CharacterEquipment
{
    public int CharacterId { get; set; }
    public int EquipmentId { get; set; }
    public int Quantity { get; set; }
    public bool IsEquipped { get; set; }
    public Dictionary<string, object> CustomProperties { get; set; }
}
```

## 🔧 Intégration Business Layer

### Service de Calcul des Modificateurs
```csharp
public interface ISpellcastingService
{
    SpellcastingAbility GetSpellcastingAbility(string characterClass);
    int CalculateSpellAttackBonus(Character character);
    int CalculateSpellSaveDC(Character character);
    int GetProficiencyBonus(int characterLevel);
    bool CanLearnSpell(Character character, Spell spell);
}

public interface IEquipmentService
{
    int CalculateArmorClass(Character character);
    Dictionary<string, int> CalculateAbilityModifiers(Character character);
    bool CanEquipItem(Character character, Equipment equipment);
    EquipmentSlot GetEquipmentSlot(Equipment equipment);
}
```

### Validation des Règles Métier
- **Unicité des sorts** : Vérification qu'un personnage n'apprend pas deux fois le même sort
- **Compatibilité système** : Un personnage D&D ne peut pas avoir d'objets Skyrim
- **Pré-requis d'équipement** : Vérification Force/Dex pour certains objets
- **Emplacements de sorts** : Gestion des limitations par niveau et repos

---

*Retour au [README principal](./README.md)*