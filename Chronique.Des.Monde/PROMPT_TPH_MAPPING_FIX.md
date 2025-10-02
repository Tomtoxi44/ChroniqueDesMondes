# 🤖 PROMPT GITHUB COPILOT - Correction Mapping TPH Entity Framework

## CONTEXTE TECHNIQUE
**Problème :** Entity Framework utilise les noms de tables dérivées (`SpellsDnd`) au lieu des tables parentes (`ASpell`) malgré la configuration TPH (Table Per Hierarchy).

**Architecture actuelle :**
- Classes parentes : `ACharacter`, `ASpell`, `AEquipment` (tables DB existantes)  
- Classes dérivées : `CharacterDnd`, `SpellDnd`, `EquipmentDnd` (types C# seulement)
- Pattern TPH avec discriminateur `GameType = "Dnd"`

**Erreur SqlException :** `Nom d'objet 'SpellsDnd' non valide`

## INSTRUCTIONS POUR COPILOT

### 🎯 OBJECTIF
Corriger la configuration TPH dans `DndDbContext.cs` pour que Entity Framework utilise les tables parentes (`ASpell`, `ACharacter`, `AEquipment`) lors des requêtes sur les types dérivés (`SpellDnd`, `CharacterDnd`, `EquipmentDnd`).

### 📋 ACTIONS REQUISES

1. **Analyser le fichier `Cdm.Data.Dnd/DndDbContext.cs`**
   - Localiser la méthode `ConfigureExistingTables`
   - Identifier la configuration TPH actuelle

2. **Implémenter la configuration TPH correcte :**
   ```csharp
   // REMPLACER la méthode ConfigureExistingTables par :
   private void ConfigureExistingTables(ModelBuilder modelBuilder)
   {
       // Configuration TPH avec discriminateur explicite
       modelBuilder.Entity<ACharacter>()
           .HasDiscriminator<string>("GameType")
           .HasValue<CharacterDnd>("Dnd");
       
       modelBuilder.Entity<ASpell>()
           .HasDiscriminator<string>("GameType") 
           .HasValue<SpellDnd>("Dnd");
       
       modelBuilder.Entity<AEquipment>()
           .HasDiscriminator<string>("GameType")
           .HasValue<EquipmentDnd>("Dnd");
   }
   ```

3. **Vérifier les DbSet dans DndDbContext :**
   - S'assurer qu'ils utilisent les types de base : `DbSet<ASpell>`, `DbSet<ACharacter>`, `DbSet<AEquipment>`
   - Supprimer tout DbSet de types dérivés : ~~`DbSet<SpellDnd>`~~, ~~`DbSet<CharacterDnd>`~~

4. **Nettoyer et tester :**
   - Exécuter : `dotnet clean && dotnet build`
   - Tester avec : `dotnet run --project Cdm.AppHost`
   - Vérifier que le seeding fonctionne sans erreur de table

### 🔍 POINTS DE VALIDATION
- ✅ EF doit utiliser `ASpell` au lieu de `SpellsDnd`
- ✅ Le service de seeding doit démarrer sans SqlException
- ✅ Les requêtes LINQ sur `SpellDnd` doivent fonctionner
- ✅ Le discriminateur `GameType = "Dnd"` doit filtrer correctement

### 📁 FICHIERS À MODIFIER
- **Principal :** `Cdm.Data.Dnd/DndDbContext.cs`
- **Possibles :** Vérifier les configurations dans `Cdm.Data.Common/Models/Configuration/`

### 🚨 CONTRAINTES
- **NE PAS** modifier les tables de base de données existantes
- **NE PAS** changer les noms des classes (`SpellDnd`, `CharacterDnd`, `EquipmentDnd`)
- **CONSERVER** l'héritage TPH : `SpellDnd : ASpell`
- **MAINTENIR** la compatibilité avec le système de seeding existant

### 🧪 VALIDATION FINALE
Le prompt est réussi quand cette commande s'exécute sans erreur :
```bash
dotnet run --project Cdm.AppHost
# Doit afficher : "🚀 Starting automatic D&D official data seeding..."
# Sans erreur : "Nom d'objet 'SpellsDnd' non valide"
```

---
**Instructions Copilot :** Analyse le code existant, identifie le problème de mapping TPH et applique la configuration correcte pour résoudre l'erreur SqlException. Concentre-toi sur la méthode `ConfigureExistingTables` dans `DndDbContext.cs`.