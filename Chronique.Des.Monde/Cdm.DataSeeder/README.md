# Cdm.DataSeeder - Initialisation des Donn�es

## Vue d'ensemble

Cette application console initialise la base de donn�es avec des donn�es de test pour **Chronique des Mondes**.

## Ce qui est cr��

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

3. **S�ur Luminara** (Clerc Support)
   - Niveau 4, 42 PV
   - Sagesse: 18, Charisme: 16
   - Classe d'armure: 16

4. **Aranis Chassevent** (R�deur DPS)
   - Niveau 4, 38 PV
   - Dext�rit�: 18, Sagesse: 16
   - Classe d'armure: 14

5. **Slinky Ombrelame** (Voleur Utilitaire)
   - Niveau 5, 40 PV
   - Dext�rit�: 18, Charisme: 15
   - Classe d'armure: 13

6. **Grunk le Furieux** (Barbare Berserker)
   - Niveau 6, 78 PV
   - Force: 20, Constitution: 18
   - Classe d'armure: 15

## Utilisation

### Pr�requis
- .NET 9.0
- SQL Server LocalDB

### Ex�cution rapide
```bash
cd Cdm.DataSeeder
./run-seeding.bat    # Windows
```

### Ex�cution manuelle
```bash
cd Cdm.DataSeeder
dotnet build
dotnet run
```

### Configuration
Les cha�nes de connexion sont configur�es dans `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ChroniqueDesMondesDb;Trusted_Connection=true;MultipleActiveResultSets=true",
    "DndConnection": "Server=(localdb)\\mssqllocaldb;Database=ChroniqueDesMondesDndDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

## Fonctionnalit�s

### ?? S�curit�
- Les mots de passe sont hach�s avec `PasswordHasher<T>`
- V�rification des doublons avant cr�ation

### ??? Base de donn�es
- Cr�ation automatique des bases de donn�es si elles n'existent pas
- Support pour deux contextes : `AppDbContext` (Users) et `DndDbContext` (Characters)

### ?? Logging
- Logs d�taill�s de chaque �tape du seeding
- Codes d'erreur appropri�s en cas d'�chec

### ?? Idempotence
- Le seeding peut �tre ex�cut� plusieurs fois sans cr�er de doublons
- D�tection des donn�es existantes

## Sortie Exemple

```
?? Chronique des Mondes - Data Seeder
=====================================
?? D�marrage du seeding...
??? Cr�ation et migration des bases de donn�es...
? Base de donn�es commune cr��e
? Base de donn�es D&D cr��e
?? Cr�ation de l'utilisateur root...
? Utilisateur root cr�� (ID: 1)
?? Cr�ation des personnages de d�monstration...
? Personnage cr��: Gorthak le Protecteur (ID: 1)
? Personnage cr��: Lyralei l'Archimage (ID: 2)
? Personnage cr��: S�ur Luminara (ID: 3)
? Personnage cr��: Aranis Chassevent (ID: 4)
? Personnage cr��: Slinky Ombrelame (ID: 5)
? Personnage cr��: Grunk le Furieux (ID: 6)
?? 6 personnages de d�monstration cr��s!
? Seeding termin� avec succ�s!

?? Informations de connexion:
   Email: root@test.com
   Mot de passe: root

?? Personnages cr��s:
   - Gorthak le Protecteur (Guerrier Tank)
   - Lyralei l'Archimage (Mage DPS)
   - S�ur Luminara (Clerc Support)
   - Aranis Chassevent (R�deur DPS)
   - Slinky Ombrelame (Voleur Utilitaire)
   - Grunk le Furieux (Barbare Berserker)

?? Vous pouvez maintenant tester votre API!
```

## Apr�s le Seeding

Une fois le seeding termin�, vous pouvez :

1. **D�marrer votre API** (`Cdm.ApiService`)
2. **Vous connecter** avec `root@test.com` / `root`
3. **Obtenir un token JWT** via `/login`
4. **Tester les endpoints** avec les ID de personnages cr��s
5. **Utiliser les tests HTTP** dans `Cdm.ApiService/Tests/`

## D�pendances

- `Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.Extensions.Hosting`
- `Microsoft.AspNetCore.Identity` (pour le hachage des mots de passe)
- Projets internes : `Cdm.Business.Common`, `Cdm.Data.Common`, `Cdm.Data.Dnd`, `Cdm.Business.Dnd`