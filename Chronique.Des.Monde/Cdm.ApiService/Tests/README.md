# Guide des Tests API Character

## Structure des Dossiers

```
Cdm.ApiService/Tests/
??? character-config.http          # Configuration commune (variables, URLs, tokens)
??? Generic/
?   ??? character-generic-crud.http # Tests CRUD g�n�riques
??? Dnd/
?   ??? character-dnd-specific.http # Tests sp�cifiques D&D
??? Security/
?   ??? character-auth-tests.http   # Tests d'authentification/autorisation
??? Scenarios/
    ??? character-e2e-scenarios.http # Tests end-to-end et sc�narios complexes
```

## Types de Tests Disponibles

### 1. Tests G�n�riques CRUD (`Generic/character-generic-crud.http`)
- ? GET - R�cup�ration de personnages (tous + par ID)
- ? POST - Cr�ation avec API g�n�rique
- ? PUT - Mise � jour avec API g�n�rique
- ? DELETE - Suppression
- ? Tests d'erreurs (ressources inexistantes, donn�es invalides)

### 2. Tests Sp�cifiques D&D (`Dnd/character-dnd-specific.http`)
- ? Cr�ation de diff�rentes classes D&D (Guerrier, Mage, R�deur, Voleur, Barbare, Clerc)
- ? Mise � jour avec format D&D sp�cifique
- ? Tests de validation des stats D&D (min/max)
- ? Tests avec classes populaires D&D

### 3. Tests de S�curit� (`Security/character-auth-tests.http`)
- ? Tests sans authentification (401 attendu)
- ? Tests avec tokens invalides/expir�s
- ? Tests sans header X-GameType
- ? Tests avec GameType non support�
- ? Tests d'acc�s non autoris� (autres utilisateurs)

### 4. Sc�narios End-to-End (`Scenarios/character-e2e-scenarios.http`)
- ? Workflow complet g�n�rique (Cr�er ? Lire ? Modifier ? Supprimer)
- ? Workflow complet D&D
- ? Gestion d'�quipe compl�te (4 personnages)
- ? Tests de performances (cr�ation multiple)
- ? Tests de robustesse (donn�es limites)

## Configuration Requise

### Variables � personnaliser dans `character-config.http` :
```http
@baseUrl = https://localhost:7428        # URL de votre API
@userId = 1                              # ID utilisateur valide
@gameType = dnd                          # Type de jeu
@characterId = 1                         # ID personnage existant pour tests
@authToken = Bearer your-jwt-token-here  # Token JWT valide
```

## Comment Utiliser

### 1. **Pr�requis :**
- D�marrer l'API (`Cdm.ApiService`)
- Obtenir un token JWT valide
- Mettre � jour les variables dans `character-config.http`

### 2. **Ordre de tests recommand� :**
1. `character-config.http` - V�rifier la configuration
2. `Security/character-auth-tests.http` - Valider la s�curit�
3. `Generic/character-generic-crud.http` - Tester l'API g�n�rique
4. `Dnd/character-dnd-specific.http` - Tester les sp�cificit�s D&D
5. `Scenarios/character-e2e-scenarios.http` - Sc�narios complexes

### 3. **Dans VS Code :**
- Installer l'extension "REST Client"
- Ouvrir les fichiers `.http`
- Cliquer sur "Send Request" au-dessus de chaque requ�te

## Endpoints Test�s

### API G�n�rique :
- `GET /character?userId={id}` - Liste des personnages
- `GET /character/{id}` - D�tails d'un personnage
- `POST /character?userId={id}` - Cr�ation g�n�rique
- `PUT /character/{id}` - Modification g�n�rique
- `DELETE /character/{id}` - Suppression

### API D&D Sp�cifique :
- `POST /character/dnd?userId={id}` - Cr�ation D&D
- `PUT /character/dnd/{id}` - Modification D&D

## Codes de R�ponse Attendus

| Endpoint | Succ�s | Erreur |
|----------|--------|--------|
| GET /character | 200 OK | 401 Unauthorized, 400 BadRequest |
| GET /character/{id} | 200 OK | 404 NotFound, 401 Unauthorized |
| POST /character | 201 Created | 400 BadRequest, 401 Unauthorized |
| PUT /character/{id} | 200 OK | 404 NotFound, 400 BadRequest |
| DELETE /character/{id} | 204 NoContent | 404 NotFound, 401 Unauthorized |

## Maintenance

Pour ajouter de nouveaux tests :
1. **Tests g�n�riques** ? `Generic/character-generic-crud.http`
2. **Tests sp�cifiques jeu** ? Cr�er un dossier `{GameType}/`
3. **Tests s�curit�** ? `Security/character-auth-tests.http`
4. **Nouveaux sc�narios** ? `Scenarios/character-e2e-scenarios.http`

## Notes Importantes

- ?? **Authentification requise** : Tous les endpoints n�cessitent un token JWT
- ?? **Header X-GameType** : Obligatoire pour le routing vers le bon service
- ?? **IDs de test** : Ajustez `@characterId` selon vos donn�es de test
- ?? **Nettoyage** : Les tests de sc�narios peuvent cr�er beaucoup de donn�es