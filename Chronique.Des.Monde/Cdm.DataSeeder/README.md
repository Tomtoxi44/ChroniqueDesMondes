# Cdm.DataSeeder - Initialisation des Données

## Vue d'ensemble

Cette application console initialise la base de données avec des données de test pour **Chronique des Mondes**.

## Ce qui est créé

### ?? Utilisateur Root
- **Email**: `root@test.com`
- **Mot de passe**: `root`
- **Nom d'utilisateur**: `root`

### ?? Personnages D&D (6 personnages)

1. **Gorthak le Protecteur** (Guerrier Tank)
   - Niveau 5, 68 PV
   - Force: 18, Constitution: 16
   - Classe d'armure: 18

2. **Lyralei l'Archimage** (Mage DPS)
   - Niveau 5, 35 PV
   - Intelligence: 18, Sagesse: 15
   - Classe d'armure: 12

3. **Sœur Luminara** (Clerc Support)
   - Niveau 4, 42 PV
   - Sagesse: 18, Charisme: 16
   - Classe d'armure: 16

4. **Aranis Chassevent** (Rôdeur DPS)
   - Niveau 4, 38 PV
   - Dextérité: 18, Sagesse: 16
   - Classe d'armure: 14

5. **Slinky Ombrelame** (Voleur Utilitaire)
   - Niveau 5, 40 PV
   - Dextérité: 18, Charisme: 15
   - Classe d'armure: 13

6. **Grunk le Furieux** (Barbare Berserker)
   - Niveau 6, 78 PV
   - Force: 20, Constitution: 18
   - Classe d'armure: 15

## Utilisation

### Prérequis
- .NET 9.0
- SQL Server LocalDB

### Exécution rapide
```bash
cd Cdm.DataSeeder
./run-seeding.bat    # Windows
```

### Exécution manuelle
```bash
cd Cdm.DataSeeder
dotnet build
dotnet run
```

### Configuration
Les chaînes de connexion sont configurées dans `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ChroniqueDesMondesDb;Trusted_Connection=true;MultipleActiveResultSets=true",
    "DndConnection": "Server=(localdb)\\mssqllocaldb;Database=ChroniqueDesMondesDndDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

## Fonctionnalités

### ?? Sécurité
- Les mots de passe sont hachés avec `PasswordHasher<T>`
- Vérification des doublons avant création

### ??? Base de données
- Création automatique des bases de données si elles n'existent pas
- Support pour deux contextes : `AppDbContext` (Users) et `DndDbContext` (Characters)

### ?? Logging
- Logs détaillés de chaque étape du seeding
- Codes d'erreur appropriés en cas d'échec

### ?? Idempotence
- Le seeding peut être exécuté plusieurs fois sans créer de doublons
- Détection des données existantes

## Sortie Exemple

```
?? Chronique des Mondes - Data Seeder
=====================================
?? Démarrage du seeding...
??? Création et migration des bases de données...
? Base de données commune créée
? Base de données D&D créée
?? Création de l'utilisateur root...
? Utilisateur root créé (ID: 1)
?? Création des personnages de démonstration...
? Personnage créé: Gorthak le Protecteur (ID: 1)
? Personnage créé: Lyralei l'Archimage (ID: 2)
? Personnage créé: Sœur Luminara (ID: 3)
? Personnage créé: Aranis Chassevent (ID: 4)
? Personnage créé: Slinky Ombrelame (ID: 5)
? Personnage créé: Grunk le Furieux (ID: 6)
?? 6 personnages de démonstration créés!
? Seeding terminé avec succès!

?? Informations de connexion:
   Email: root@test.com
   Mot de passe: root

?? Personnages créés:
   - Gorthak le Protecteur (Guerrier Tank)
   - Lyralei l'Archimage (Mage DPS)
   - Sœur Luminara (Clerc Support)
   - Aranis Chassevent (Rôdeur DPS)
   - Slinky Ombrelame (Voleur Utilitaire)
   - Grunk le Furieux (Barbare Berserker)

?? Vous pouvez maintenant tester votre API!
```

## Après le Seeding

Une fois le seeding terminé, vous pouvez :

1. **Démarrer votre API** (`Cdm.ApiService`)
2. **Vous connecter** avec `root@test.com` / `root`
3. **Obtenir un token JWT** via `/login`
4. **Tester les endpoints** avec les ID de personnages créés
5. **Utiliser les tests HTTP** dans `Cdm.ApiService/Tests/`

## Dépendances

- `Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.Extensions.Hosting`
- `Microsoft.AspNetCore.Identity` (pour le hachage des mots de passe)
- Projets internes : `Cdm.Business.Common`, `Cdm.Data.Common`, `Cdm.Data.Dnd`, `Cdm.Business.Dnd`