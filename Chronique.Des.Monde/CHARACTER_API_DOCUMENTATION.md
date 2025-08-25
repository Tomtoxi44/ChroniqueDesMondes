# API Character - Documentation

## Vue d'ensemble

Cette API fournit un système CRUD complet et générique pour la gestion des personnages de jeux de rôle, avec une implémentation spécifique pour D&D et une architecture extensible pour d'autres systèmes de jeu.

## Architecture

### Interface générique (`ICharacterBusiness`)
```csharp
Task<IEnumerable<ICharacterView>> GetAllCharactersByUserId(int userId);
Task<ICharacterView> GetCharacterById(int characterId);
Task<ICharacterView> CreateCharacter(CharacterRequest character, int userId);
Task<ICharacterView> UpdateCharacter(CharacterRequest character, int characterId);
Task DeleteCharacter(int characterId);
```

### Modèles génériques
- `CharacterRequest` : Modèle d'entrée générique avec dictionnaires `Competences` et `Stats`
- `ICharacterView` : Interface de sortie générique
- `CharacterRequestFactory` : Factory pour créer facilement des requests D&D

## Endpoints disponibles

### API CRUD Générique
- `GET /character?userId={id}` - Liste des personnages d'un utilisateur
- `GET /character/{id}` - Détails d'un personnage
- `POST /character?userId={id}` - Création générique
- `PUT /character/{id}` - Modification générique
- `DELETE /character/{id}` - Suppression

### API D&D Spécifique (compatibilité)
- `POST /character/dnd?userId={id}` - Création D&D spécifique
- `PUT /character/dnd/{id}` - Modification D&D spécifique

## Headers requis
- `Authorization: Bearer {token}` - Authentification JWT
- `X-GameType: dnd` - Type de jeu (pour le service keyed)

## Exemples d'utilisation

### Création générique
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

### Création D&D spécifique
```json
POST /character/dnd?userId=1
{
  "name": "Gorthak le Brave",
  "class": "Guerrier",
  "strong": 16,
  "intelligence": 10
}
```

## Fonctionnalités

? **CRUD complet** - Create, Read, Update, Delete
? **API générique** - Extensible à d'autres systèmes de jeu
? **Compatibilité D&D** - Endpoints spécifiques pour rétrocompatibilité
? **Calcul automatique** - Modificateurs D&D calculés automatiquement
? **Gestion d'erreurs** - Exceptions métier appropriées
? **Tests HTTP** - Fichier .http complet pour tous les endpoints
? **Documentation** - Exemples d'utilisation complets

## Code nettoyé

? Supprimé : Méthodes obsolètes (`CreateCharacterByUserId`, etc.)
? Supprimé : Code dupliqué dans les endpoints
? Supprimé : Méthodes spécifiques D&D redondantes
? Ajouté : Architecture générique extensible
? Ajouté : Endpoints RESTful standards
? Ajouté : Factory pattern pour faciliter l'utilisation