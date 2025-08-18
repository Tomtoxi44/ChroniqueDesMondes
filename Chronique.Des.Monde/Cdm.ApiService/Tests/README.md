# Guide des Tests API Character

## Structure des Dossiers

```
Cdm.ApiService/Tests/
??? character-config.http          # Configuration commune (variables, URLs, tokens)
??? Generic/
?   ??? character-generic-crud.http # Tests CRUD génériques
??? Dnd/
?   ??? character-dnd-specific.http # Tests spécifiques D&D
??? Security/
?   ??? character-auth-tests.http   # Tests d'authentification/autorisation
??? Scenarios/
    ??? character-e2e-scenarios.http # Tests end-to-end et scénarios complexes
```

## Types de Tests Disponibles

### 1. Tests Génériques CRUD (`Generic/character-generic-crud.http`)
- ? GET - Récupération de personnages (tous + par ID)
- ? POST - Création avec API générique
- ? PUT - Mise à jour avec API générique
- ? DELETE - Suppression
- ? Tests d'erreurs (ressources inexistantes, données invalides)

### 2. Tests Spécifiques D&D (`Dnd/character-dnd-specific.http`)
- ? Création de différentes classes D&D (Guerrier, Mage, Rôdeur, Voleur, Barbare, Clerc)
- ? Mise à jour avec format D&D spécifique
- ? Tests de validation des stats D&D (min/max)
- ? Tests avec classes populaires D&D

### 3. Tests de Sécurité (`Security/character-auth-tests.http`)
- ? Tests sans authentification (401 attendu)
- ? Tests avec tokens invalides/expirés
- ? Tests sans header X-GameType
- ? Tests avec GameType non supporté
- ? Tests d'accès non autorisé (autres utilisateurs)

### 4. Scénarios End-to-End (`Scenarios/character-e2e-scenarios.http`)
- ? Workflow complet générique (Créer ? Lire ? Modifier ? Supprimer)
- ? Workflow complet D&D
- ? Gestion d'équipe complète (4 personnages)
- ? Tests de performances (création multiple)
- ? Tests de robustesse (données limites)

## Configuration Requise

### Variables à personnaliser dans `character-config.http` :
```http
@baseUrl = https://localhost:7428        # URL de votre API
@userId = 1                              # ID utilisateur valide
@gameType = dnd                          # Type de jeu
@characterId = 1                         # ID personnage existant pour tests
@authToken = Bearer your-jwt-token-here  # Token JWT valide
```

## Comment Utiliser

### 1. **Prérequis :**
- Démarrer l'API (`Cdm.ApiService`)
- Obtenir un token JWT valide
- Mettre à jour les variables dans `character-config.http`

### 2. **Ordre de tests recommandé :**
1. `character-config.http` - Vérifier la configuration
2. `Security/character-auth-tests.http` - Valider la sécurité
3. `Generic/character-generic-crud.http` - Tester l'API générique
4. `Dnd/character-dnd-specific.http` - Tester les spécificités D&D
5. `Scenarios/character-e2e-scenarios.http` - Scénarios complexes

### 3. **Dans VS Code :**
- Installer l'extension "REST Client"
- Ouvrir les fichiers `.http`
- Cliquer sur "Send Request" au-dessus de chaque requête

## Endpoints Testés

### API Générique :
- `GET /character?userId={id}` - Liste des personnages
- `GET /character/{id}` - Détails d'un personnage
- `POST /character?userId={id}` - Création générique
- `PUT /character/{id}` - Modification générique
- `DELETE /character/{id}` - Suppression

### API D&D Spécifique :
- `POST /character/dnd?userId={id}` - Création D&D
- `PUT /character/dnd/{id}` - Modification D&D

## Codes de Réponse Attendus

| Endpoint | Succès | Erreur |
|----------|--------|--------|
| GET /character | 200 OK | 401 Unauthorized, 400 BadRequest |
| GET /character/{id} | 200 OK | 404 NotFound, 401 Unauthorized |
| POST /character | 201 Created | 400 BadRequest, 401 Unauthorized |
| PUT /character/{id} | 200 OK | 404 NotFound, 400 BadRequest |
| DELETE /character/{id} | 204 NoContent | 404 NotFound, 401 Unauthorized |

## Maintenance

Pour ajouter de nouveaux tests :
1. **Tests génériques** ? `Generic/character-generic-crud.http`
2. **Tests spécifiques jeu** ? Créer un dossier `{GameType}/`
3. **Tests sécurité** ? `Security/character-auth-tests.http`
4. **Nouveaux scénarios** ? `Scenarios/character-e2e-scenarios.http`

## Notes Importantes

- ?? **Authentification requise** : Tous les endpoints nécessitent un token JWT
- ?? **Header X-GameType** : Obligatoire pour le routing vers le bon service
- ?? **IDs de test** : Ajustez `@characterId` selon vos données de test
- ?? **Nettoyage** : Les tests de scénarios peuvent créer beaucoup de données