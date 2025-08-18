# API Character - Documentation

## Vue d'ensemble

Cette API fournit un syst�me CRUD complet et g�n�rique pour la gestion des personnages de jeux de r�le, avec une impl�mentation sp�cifique pour D&D et une architecture extensible pour d'autres syst�mes de jeu.

## Architecture

### Interface g�n�rique (`ICharacterBusiness`)
```csharp
Task<IEnumerable<ICharacterView>> GetAllCharactersByUserId(int userId);
Task<ICharacterView> GetCharacterById(int characterId);
Task<ICharacterView> CreateCharacter(CharacterRequest character, int userId);
Task<ICharacterView> UpdateCharacter(CharacterRequest character, int characterId);
Task DeleteCharacter(int characterId);
```

### Mod�les g�n�riques
- `CharacterRequest` : Mod�le d'entr�e g�n�rique avec dictionnaires `Competences` et `Stats`
- `ICharacterView` : Interface de sortie g�n�rique
- `CharacterRequestFactory` : Factory pour cr�er facilement des requests D&D

## Endpoints disponibles

### API CRUD G�n�rique
- `GET /character?userId={id}` - Liste des personnages d'un utilisateur
- `GET /character/{id}` - D�tails d'un personnage
- `POST /character?userId={id}` - Cr�ation g�n�rique
- `PUT /character/{id}` - Modification g�n�rique
- `DELETE /character/{id}` - Suppression

### API D&D Sp�cifique (compatibilit�)
- `POST /character/dnd?userId={id}` - Cr�ation D&D sp�cifique
- `PUT /character/dnd/{id}` - Modification D&D sp�cifique

## Headers requis
- `Authorization: Bearer {token}` - Authentification JWT
- `X-GameType: dnd` - Type de jeu (pour le service keyed)

## Exemples d'utilisation

### Cr�ation g�n�rique
```json
POST /character?userId=1
{
  "name": "Lyralei la Sage",
  "leveling": 5,
  "life": 35,
  "competences": {
    "Class": "Magicien",
    "Strong": 8,
    "Intelligence": 18
  }
}
```

### Cr�ation D&D sp�cifique
```json
POST /character/dnd?userId=1
{
  "name": "Gorthak le Brave",
  "class": "Guerrier",
  "strong": 16,
  "intelligence": 10
}
```

## Fonctionnalit�s

? **CRUD complet** - Create, Read, Update, Delete
? **API g�n�rique** - Extensible � d'autres syst�mes de jeu
? **Compatibilit� D&D** - Endpoints sp�cifiques pour r�trocompatibilit�
? **Calcul automatique** - Modificateurs D&D calcul�s automatiquement
? **Gestion d'erreurs** - Exceptions m�tier appropri�es
? **Tests HTTP** - Fichier .http complet pour tous les endpoints
? **Documentation** - Exemples d'utilisation complets

## Code nettoy�

? Supprim� : M�thodes obsol�tes (`CreateCharacterByUserId`, etc.)
? Supprim� : Code dupliqu� dans les endpoints
? Supprim� : M�thodes sp�cifiques D&D redondantes
? Ajout� : Architecture g�n�rique extensible
? Ajout� : Endpoints RESTful standards
? Ajout� : Factory pattern pour faciliter l'utilisation