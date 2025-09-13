# 🔐 **ÉTAPE 2 : AUTHENTIFICATION JWT - GUIDE DE TEST**
## Intégration Complète Frontend-Backend avec JWT

---

## 🎯 **CE QUI A ÉTÉ IMPLÉMENTÉ**

### **✅ Backend API Endpoints**
- **POST /api/auth/login** : Connexion avec JWT
- **POST /api/auth/register** : Inscription avec JWT
- **GET /api/auth/validate** : Validation des tokens
- **GET /api/auth/me** : Informations utilisateur connecté

### **✅ Frontend Services JWT**
- **JwtAuthService** : Service moderne avec sessionStorage
- **Login.razor** : Page de connexion avec validation
- **RegisterApi.razor** : Page d'inscription complète
- **Gestion des tokens** : Stockage sécurisé et validation

### **✅ Sécurité & UX**
- **Validation** des formulaires côté client
- **Messages d'erreur** contextuels
- **États de chargement** avec spinners
- **Redirection automatique** après auth
- **Gestion des sessions** persistantes

---

## 🧪 **COMMENT TESTER L'ÉTAPE 2**

### **🔧 Prérequis**
```bash
# 1. Démarrer l'API backend avec auth
cd Cdm.ApiService
dotnet run

# 2. Démarrer le frontend  
cd Cdm.Web
dotnet run
```

### **📍 URLs de test**
- **Frontend** : http://localhost:5222
- **Backend API** : https://localhost:7428
- **Login** : http://localhost:5222/login
- **Register** : http://localhost:5222/register

---

## 🎮 **TESTS À EFFECTUER**

### **1. Test d'Inscription (Register)**

#### **🟢 Cas nominal**
✅ **Étapes :**
1. Aller sur http://localhost:5222/register
2. Remplir le formulaire :
   - **Nom d'utilisateur** : testuser
   - **Email** : test@example.com
   - **Mot de passe** : password123
   - **Confirmer** : password123
3. Cliquer sur "Créer le compte"

✅ **Résultat attendu :**
- Message "Création en cours..." avec spinner
- Message de succès : "Compte créé avec succès !"
- Redirection automatique vers l'accueil (/)
- Token stocké dans sessionStorage

#### **🔴 Cas d'erreur**
✅ **Tests de validation :**
1. **Mots de passe différents** : Message "Les mots de passe ne correspondent pas"
2. **Email invalide** : Message "Format d'email invalide"
3. **Champs vides** : Messages de validation appropriés

### **2. Test de Connexion (Login)**

#### **🟢 Cas nominal**
✅ **Étapes :**
1. Aller sur http://localhost:5222/login
2. Remplir :
   - **Email** : n'importe quel email valide
   - **Mot de passe** : n'importe quel mot de passe (6+ caractères)
3. Cliquer sur "Se connecter"

✅ **Résultat attendu :**
- Message "Connexion en cours..." avec spinner
- Message de succès : "Connexion réussie !"
- Snackbar de confirmation
- Redirection vers l'accueil

#### **🔴 Cas d'erreur**
✅ **Tests de validation :**
1. **Email vide** : "L'adresse email est requise"
2. **Mot de passe < 6 caractères** : Message de longueur
3. **Format email invalide** : "Format d'email invalide"

### **3. Test de Persistance de Session**

#### **🔍 Vérification du token**
✅ **Étapes :**
1. Se connecter avec succès
2. Ouvrir F12 > Application > Session Storage
3. Vérifier la présence de `cdm_auth_token`
4. Actualiser la page (F5)
5. Vérifier que la session est maintenue

### **4. Test des API Endpoints**

#### **🌐 Test direct des endpoints**
✅ **Avec un outil comme Postman/Thunder Client :**

```http
### Test Register
POST https://localhost:7428/api/auth/register
Content-Type: application/json

{
  "userName": "testuser",
  "email": "test@example.com", 
  "password": "password123"
}

### Test Login
POST https://localhost:7428/api/auth/login
Content-Type: application/json

{
  "email": "test@example.com",
  "password": "password123"
}

### Test Validate (avec token récupéré)
GET https://localhost:7428/api/auth/validate
Authorization: Bearer YOUR_TOKEN_HERE
```

---

## 🔍 **VÉRIFICATIONS TECHNIQUES**

### **📊 Console développeur (F12)**

#### **✅ Onglet Application**
- **Session Storage** : `cdm_auth_token` présent après connexion
- **Valeur** : JWT token au format `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`

#### **✅ Onglet Network**
- **Requêtes API** : Appels vers `/api/auth/login` et `/api/auth/register`
- **Status 200** : Connexion/inscription réussie
- **Response** : JSON avec `success: true, token: "...", user: {...}`

#### **✅ Onglet Console**
- **Pas d'erreurs JavaScript**
- **Logs debug** : Messages de service si activés

### **📝 Logs Backend**

✅ **Dans la console de l'API :**
```
🔐 Tentative de connexion pour test@example.com
✅ Connexion réussie pour test@example.com
📝 Tentative d'inscription pour newuser@example.com
✅ Inscription réussie pour newuser@example.com
```

### **🎨 Interface Utilisateur**

✅ **Vérifications visuelles :**
- **Thème sombre** maintenu sur toutes les pages
- **Formulaires** responsive mobile/desktop
- **États de chargement** : Spinners et boutons désactivés
- **Messages** : Erreurs en rouge, succès en vert
- **Navigation** : Liens "Déjà un compte ?" et "Pas de compte ?"

---

## 🔒 **SÉCURITÉ & TOKENS JWT**

### **🔍 Analyse du Token JWT**

✅ **Décoder le token** sur https://jwt.io :
1. Copier le token depuis sessionStorage
2. Le coller sur jwt.io
3. Vérifier la structure :
   ```json
   {
     "sub": "1",
     "name": "testuser",
     "email": "test@example.com",
     "exp": 1704067200,
     "iss": "ChroniqueDesMondes",
     "aud": "ChroniqueDesMondes"
   }
   ```

### **⏰ Expiration des Tokens**
- **Durée** : 7 jours par défaut
- **Validation** : Automatique côté backend
- **Renouvellement** : Reconnexion nécessaire après expiration

---

## 🚧 **LIMITATIONS ACTUELLES (NORMALES)**

### **⚠️ Fonctionnalités simplifiées pour tests :**
- **Base de données** : Pas encore connectée (utilisateurs temporaires)
- **Hashage passwords** : Pas encore implémenté
- **Validation email** : Pas de vérification par email
- **Refresh tokens** : Pas encore implémenté
- **Rôles utilisateur** : Pas encore gérés

### **🔮 Ces fonctionnalités arrivent dans :**
- **Étape 3** : CRUD Personnages avec authentification
- **Étape 4** : Interface de Combat sécurisée
- **Futures étapes** : Base de données, rôles, etc.

---

## 🐛 **TROUBLESHOOTING**

### **Problème : "Erreur de connexion"**
🔧 **Solutions :**
1. Vérifier que l'API backend est démarrée
2. Tester l'endpoint direct : `POST https://localhost:7428/api/auth/login`
3. Vérifier les logs backend pour les erreurs
4. Nettoyer sessionStorage : F12 > Application > Clear Storage

### **Problème : "Token non trouvé"**
🔧 **Solutions :**
1. Vérifier sessionStorage : F12 > Application > Session Storage
2. Se reconnecter pour générer un nouveau token
3. Vérifier que JavaScript est activé
4. Tester en navigation privée

### **Problème : Interface cassée**
🔧 **Solutions :**
1. Vider le cache : Ctrl+F5
2. Vérifier que MudBlazor CSS est chargé
3. F12 > Console pour erreurs JavaScript
4. Vérifier les URLs dans la navigation

### **Problème : API non accessible**
🔧 **Solutions :**
```bash
# Vérifier les ports
netstat -an | findstr :7428
netstat -an | findstr :5222

# Redémarrer les services
dotnet run --project Cdm.ApiService
dotnet run --project Cdm.Web

# Vérifier les certificats SSL
dotnet dev-certs https --trust
```

---

## ✅ **CRITÈRES DE SUCCÈS ÉTAPE 2**

### **🎯 Fonctionnel**
- ✅ Inscription créé un token JWT valide
- ✅ Connexion authentifie avec l'API backend
- ✅ Sessions persistantes avec sessionStorage
- ✅ Validation des formulaires côté client
- ✅ Messages d'erreur/succès appropriés

### **🔒 Sécurité**
- ✅ Tokens JWT bien formatés et signés
- ✅ Expiration des tokens gérée
- ✅ Validation côté backend fonctionnelle
- ✅ Headers Authorization configurés automatiquement

### **🎨 UX/UI**
- ✅ Interface moderne et responsive
- ✅ États de chargement fluides
- ✅ Navigation entre login/register
- ✅ Redirection automatique après auth

### **🔧 Technique**
- ✅ Services injectés correctement
- ✅ HttpClient configuré avec auth headers
- ✅ Logging complet backend/frontend
- ✅ Gestion d'erreurs robuste

---

## 🎊 **TESTS DE DÉMONSTRATION**

### **🚀 Démo rapide pour impression**
1. **Registration** : http://localhost:5222/register
   - Nom : "Hero" 
   - Email : "hero@dnd.com"
   - Password : "dragon123"

2. **Login** : http://localhost:5222/login
   - Email : "hero@dnd.com"  
   - Password : "dragon123"

3. **Vérification** : F12 > SessionStorage > cdm_auth_token

4. **API Test** : Postman avec le token

---

## 🚀 **PROCHAINE ÉTAPE**

Une fois l'Étape 2 validée, nous passerons à :

**⚔️ ÉTAPE 3 : CRUD PERSONNAGES AUTHENTIFIÉS**
- Service PersonnagesAPI avec authentification
- Création/modification/suppression sécurisées
- Validation des permissions utilisateur
- Interface complète de gestion

---

*Étape 2 : Authentification JWT ✅ | Prochaine étape : CRUD Personnages Sécurisé ⚔️*