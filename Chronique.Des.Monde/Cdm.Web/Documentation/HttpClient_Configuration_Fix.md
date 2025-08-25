# Configuration HttpClient - Correction de l'erreur BaseAddress

## 🐛 Problème Résolu

**Erreur**: `System.InvalidOperationException: An invalid request URI was provided. Either the request URI must be an absolute URI or BaseAddress must be set.`

**Cause**: L'URL Aspire `"https+http://apiservice"` n'était pas correctement résolue dans l'environnement de développement.

## ✅ Solution Implémentée

### 1. Configuration Hybride des HttpClients

```csharp
// Configuration avec fallback intelligent
builder.Services.AddHttpClient<IApiService, ApiService>(client =>
{
    var apiServiceUrl = builder.Configuration.GetConnectionString("apiservice") 
                       ?? builder.Configuration["Services:ApiService:Url"]
                       ?? (builder.Environment.IsDevelopment() 
                           ? "https://localhost:7428"  // Dev URL
                           : "https+http://apiservice"); // Aspire URL
    
    client.BaseAddress = new Uri(apiServiceUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});
```

### 2. Configuration dans appsettings.json

```json
{
  "Services": {
    "ApiService": {
      "Url": "https://localhost:7428"
    }
  },
  "Logging": {
    "LogLevel": {
      "Cdm.Web.Services": "Debug"
    }
  }
}
```

### 3. Logging Amélioré

- **Logging de l'URL de base** au démarrage
- **Logging des requêtes** avec URL cible
- **Logging des réponses** avec codes de statut
- **Gestion d'erreurs** détaillée

## 🔧 Hiérarchie de Configuration

La configuration suit cette priorité :

1. **ConnectionString "apiservice"** (Aspire)
2. **Services:ApiService:Url** (appsettings.json)
3. **Fallback développement** (`https://localhost:7428`)
4. **Fallback production** (`https+http://apiservice`)

## 🌍 URLs par Environnement

### Développement Local
- **API Service**: `https://localhost:7428`
- **Web Frontend**: `https://localhost:7153`

### Production/Aspire
- **API Service**: Résolu par Aspire
- **Web Frontend**: Résolu par Aspire

## 🔍 Diagnostic

### Vérifier les logs au démarrage :
```
ApiService initialized with BaseAddress: https://localhost:7428
```

### Vérifier les logs de requête :
```
Tentative de connexion pour user@email.com vers https://localhost:7428/login
Réponse de l'API login: Status=OK, Content Length=123
```

## 🛠️ Configuration Alternative

Si vous préférez une configuration plus simple pour le développement :

### Option 1 : URL fixe en développement
```csharp
builder.Services.AddHttpClient<IApiService, ApiService>(client =>
{
    if (builder.Environment.IsDevelopment())
    {
        client.BaseAddress = new Uri("https://localhost:7428");
    }
    else
    {
        // Configuration Aspire pour production
        client.BaseAddress = new Uri("https+http://apiservice");
    }
});
```

### Option 2 : Variables d'environnement
```bash
# Dans le fichier .env ou variables d'environnement
API_SERVICE_URL=https://localhost:7428
```

```csharp
var apiUrl = Environment.GetEnvironmentVariable("API_SERVICE_URL") 
            ?? "https://localhost:7428";
client.BaseAddress = new Uri(apiUrl);
```

## 🚀 Services Configurés

### HttpClients Configurés
- **IApiService** - Authentification (login/register)
- **ICharacterService** - Gestion des personnages

### Configuration Commune
- **Accept**: `application/json`
- **Timeout**: 30 secondes
- **BaseAddress**: Résolue par hiérarchie de configuration

## 🔐 Sécurité

### Headers de Sécurité
- **Accept**: Limite aux réponses JSON
- **Authorization**: Bearer token ajouté automatiquement
- **X-GameType**: Header personnalisé pour le contexte

### Timeout
- **30 secondes** pour éviter les blocages
- **Gestion des timeouts** dans les try-catch

## 📊 Monitoring

### Logs à Surveiller
- **Initialization**: BaseAddress configurée
- **Requests**: URL cible et méthode
- **Responses**: Status codes et taille de contenu
- **Errors**: Exceptions avec contexte complet

### Métriques Important
- **Temps de réponse** des API calls
- **Taux de succès** par endpoint
- **Erreurs de connection** fréquentes

---

*Cette configuration assure une compatibilité entre développement local et déploiement Aspire* 🎯