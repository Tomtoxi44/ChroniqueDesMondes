# 🛠️ Guide de Développement

Ce guide complet vous permet de démarrer rapidement le développement sur **Chronique des Mondes**, avec tous les outils, standards et bonnes pratiques pour maintenir une qualité de code exceptionnelle.

---

## 🚀 **Setup Environnement de Développement**

### 📋 **Prérequis Obligatoires**

#### **🔧 Outils de Base**
```
✅ Visual Studio 2022 (17.8+)
├── Workloads requis :
│   ├── 🌐 ASP.NET et développement web
│   ├── 🎯 Développement .NET multi-plateforme
│   └── ☁️ Développement Azure
├── Extensions recommandées :
│   ├── 🧪 Test Explorer
│   ├── 📊 Code Coverage
│   └── 🔍 SonarLint

✅ .NET 9 SDK (9.0.100+)
├── 🔍 Vérification : dotnet --version
└── 📦 NuGet Package Manager mis à jour

✅ SQL Server (LocalDB suffisant)
├── 🔍 SQL Server Management Studio (SSMS)
└── 📊 Azure Data Studio (optionnel)

✅ Git for Windows
├── 🔧 Configuration initiale requise
└── 🔑 Authentification GitHub
```

#### **🔧 Configuration Git Initiale**
```bash
# Configuration utilisateur
git config --global user.name "Votre Nom"
git config --global user.email "votre.email@example.com"

# Configuration éditeur par défaut
git config --global core.editor "code --wait"

# Configuration fin de ligne (Windows)
git config --global core.autocrlf true

# Vérification configuration
git config --list
```

### 🏗️ **Installation et Configuration**

#### **📂 Étape 1 : Clone du Repository**
```bash
# Clone du projet principal
git clone https://github.com/Tomtoxi44/ChroniqueDesMondes.git
cd ChroniqueDesMondes/Chronique.Des.Monde

# Vérification de la structure
dir
# Sortie attendue :
# - Cdm.ApiService/
# - Cdm.Web/
# - Cdm.Data.Common/
# - Cdm.Business.Common/
# - etc.
```

#### **🔄 Étape 2 : Restauration et Build**
```bash
# Restauration des packages NuGet
dotnet restore

# Build complet de la solution
dotnet build --configuration Debug

# Exécution des tests pour valider l'environnement
dotnet test --logger "console;verbosity=minimal"

# Si tout fonctionne, vous devriez voir :
# ✅ Build succeeded
# ✅ Tests passed
```

#### **🗄️ Étape 3 : Configuration Base de Données**
```bash
# Navigation vers le projet de migrations
cd Cdm.Migrations

# Vérification de la connexion LocalDB
sqlcmd -S "(localdb)\mssqllocaldb" -Q "SELECT @@VERSION"

# Application des migrations
dotnet ef database update --startup-project ../Cdm.ApiService

# Vérification que la base est créée
dotnet ef database list --startup-project ../Cdm.ApiService
```

#### **⚙️ Étape 4 : Configuration appsettings.Development.json**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ChroniqueDesMondes_Dev;Trusted_Connection=true;MultipleActiveResultSets=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore": "Information",
      "Microsoft.AspNetCore.SignalR": "Debug"
    }
  },
  "Jwt": {
    "Secret": "VotreCléSecrèteTrèsLonguePourLeJWT-DéveloppementUniquement",
    "Issuer": "ChroniqueDesMondes",
    "Audience": "ChroniqueDesMondes-Users",
    "ExpirationHours": 24
  },
  "AllowedHosts": "*",
  "Environment": "Development"
}
```

---

## 📊 **Standards de Code et Conventions**

### 🎯 **Conventions de Nommage .NET**

#### **📝 Classes et Interfaces**
```csharp
// ✅ CORRECT
public class CharacterService : ICharacterService { }
public interface ISpellRepository { }
public class DndSpellcastingCalculator { }
public enum GameType { Dnd, Generic, Pathfinder }

// ❌ INCORRECT
public class characterservice { }
public interface spellRepository { }
public class dnd_calculator { }
public enum gameType { }
```

#### **🔧 Méthodes et Propriétés**
```csharp
// ✅ CORRECT
public async Task<Character> CreateCharacterAsync(CreateCharacterDto dto)
public string GameType { get; set; }
public int TotalStrength => BaseStrength + BonusStrength;
private readonly ILogger<CharacterService> _logger;

// ❌ INCORRECT
public async Task<Character> createcharacter(CreateCharacterDto dto)
public string gameType { get; set; }
public int total_strength => base_strength + bonus_strength;
private readonly ILogger<CharacterService> logger;
```

#### **📄 Fichiers et Dossiers**
```
✅ CORRECT Structure :
├── Services/
│   ├── CharacterService.cs
│   ├── SpellService.cs
│   └── CampaignService.cs
├── DTOs/
│   ├── CreateCharacterDto.cs
│   └── CharacterResponseDto.cs
└── Validators/
    ├── CreateCharacterValidator.cs
    └── UpdateSpellValidator.cs

❌ INCORRECT :
├── services/
├── character-service.cs
└── createCharacterDTO.cs
```

### 🧱 **Patterns Architecturaux Recommandés**

#### **🔄 Repository Pattern avec Unit of Work**
```csharp
// Interface générique réutilisable
public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(int id);
    Task<List<TEntity>> GetAllAsync();
    Task<TEntity> AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(int id);
    IQueryable<TEntity> Query();
}

// Implémentation générique
public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;
    
    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }
    
    public virtual async Task<TEntity?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }
    
    public virtual async Task<List<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }
    
    public virtual IQueryable<TEntity> Query()
    {
        return _dbSet.AsQueryable();
    }
}

// Repository spécialisé
public interface ICharacterRepository : IRepository<Character>
{
    Task<List<Character>> GetUserCharactersAsync(int userId, string? gameType = null);
    Task<List<CharacterDnd>> GetDndCharactersWithCalculationsAsync(int userId);
}

public class CharacterRepository : Repository<Character>, ICharacterRepository
{
    public CharacterRepository(AppDbContext context) : base(context) { }
    
    public async Task<List<Character>> GetUserCharactersAsync(int userId, string? gameType = null)
    {
        var query = Query().Where(c => c.UserId == userId);
        
        if (!string.IsNullOrEmpty(gameType))
        {
            query = query.Where(c => c.GameType == gameType);
        }
        
        return await query
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
}
```

#### **🏭 Factory Pattern pour Multi-GameType**
```csharp
public interface ICharacterFactory
{
    Task<Character> CreateAsync(CreateCharacterDto dto, string gameType);
    Character CreateDefault(string gameType);
}

public class CharacterFactory : ICharacterFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CharacterFactory> _logger;
    
    public CharacterFactory(IServiceProvider serviceProvider, ILogger<CharacterFactory> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    public async Task<Character> CreateAsync(CreateCharacterDto dto, string gameType)
    {
        _logger.LogInformation("Creating character of type {GameType} for user {UserId}", 
            gameType, dto.UserId);
            
        return gameType.ToLowerInvariant() switch
        {
            "dnd" => await CreateDndCharacterAsync(dto),
            "generic" => CreateGenericCharacter(dto),
            "pathfinder" => await CreatePathfinderCharacterAsync(dto),
            _ => throw new NotSupportedException($"GameType '{gameType}' is not supported")
        };
    }
    
    private async Task<CharacterDnd> CreateDndCharacterAsync(CreateCharacterDto dto)
    {
        var dndService = _serviceProvider.GetRequiredService<IDndCharacterService>();
        return await dndService.CreateAsync(dto);
    }
    
    private Character CreateGenericCharacter(CreateCharacterDto dto)
    {
        return new Character
        {
            Name = dto.Name,
            GameType = "generic",
            UserId = dto.UserId,
            CreatedAt = DateTime.UtcNow
        };
    }
}
```

#### **📊 Service Result Pattern**
```csharp
// Classe de résultat standardisée
public class ServiceResult<T>
{
    public bool Success { get; private set; }
    public T? Data { get; private set; }
    public string? ErrorMessage { get; private set; }
    public List<string> Errors { get; private set; } = new();
    public Exception? Exception { get; private set; }
    
    private ServiceResult() { }
    
    public static ServiceResult<T> Success(T data)
    {
        return new ServiceResult<T>
        {
            Success = true,
            Data = data
        };
    }
    
    public static ServiceResult<T> Failure(string errorMessage)
    {
        return new ServiceResult<T>
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }
    
    public static ServiceResult<T> Failure(List<string> errors)
    {
        return new ServiceResult<T>
        {
            Success = false,
            Errors = errors
        };
    }
    
    public static ServiceResult<T> Failure(Exception exception)
    {
        return new ServiceResult<T>
        {
            Success = false,
            ErrorMessage = exception.Message,
            Exception = exception
        };
    }
}

// Utilisation dans les services
public class CharacterService : ICharacterService
{
    public async Task<ServiceResult<Character>> CreateCharacterAsync(CreateCharacterDto dto)
    {
        try
        {
            // Validation métier
            var validationResult = await ValidateCharacterCreationAsync(dto);
            if (!validationResult.IsValid)
            {
                return ServiceResult<Character>.Failure(
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }
            
            // Création du personnage
            var character = await _characterFactory.CreateAsync(dto, dto.GameType);
            await _unitOfWork.Characters.AddAsync(character);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Character {CharacterName} created successfully for user {UserId}", 
                character.Name, dto.UserId);
                
            return ServiceResult<Character>.Success(character);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error creating character for user {UserId}", dto.UserId);
            return ServiceResult<Character>.Failure("Erreur lors de la sauvegarde en base de données");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating character for user {UserId}", dto.UserId);
            return ServiceResult<Character>.Failure(ex);
        }
    }
}
```

---

## 🧪 **Standards de Tests**

### 📊 **Stratégie de Tests par Couche**

#### **🔬 Tests Unitaires (Couche Business)**
```csharp
[TestClass]
public class CharacterServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICharacterFactory> _mockCharacterFactory;
    private readonly Mock<ILogger<CharacterService>> _mockLogger;
    private readonly CharacterService _characterService;
    
    public CharacterServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCharacterFactory = new Mock<ICharacterFactory>();
        _mockLogger = new Mock<ILogger<CharacterService>>();
        
        _characterService = new CharacterService(
            _mockUnitOfWork.Object,
            _mockCharacterFactory.Object,
            _mockLogger.Object);
    }
    
    [TestMethod]
    public async Task CreateCharacterAsync_WithValidDndCharacter_ReturnsSuccessResult()
    {
        // Arrange
        var createDto = new CreateCharacterDto
        {
            Name = "Gandalf le Gris",
            Class = "Magicien",
            GameType = "dnd",
            UserId = 1,
            Strong = 12,
            Intelligence = 18
        };
        
        var expectedCharacter = new CharacterDnd
        {
            Id = 1,
            Name = "Gandalf le Gris",
            Class = "Magicien",
            GameType = "dnd",
            UserId = 1
        };
        
        _mockCharacterFactory
            .Setup(x => x.CreateAsync(It.IsAny<CreateCharacterDto>(), "dnd"))
            .ReturnsAsync(expectedCharacter);
            
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        // Act
        var result = await _characterService.CreateCharacterAsync(createDto);
        
        // Assert
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual("Gandalf le Gris", result.Data.Name);
        
        // Vérifier les appels aux mocks
        _mockCharacterFactory.Verify(x => x.CreateAsync(createDto, "dnd"), Times.Once);
        _mockUnitOfWork.Verify(x => x.Characters.AddAsync(expectedCharacter), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [TestMethod]
    [DataRow("")]
    [DataRow(null)]
    [DataRow("   ")]
    public async Task CreateCharacterAsync_WithInvalidName_ReturnsFailure(string invalidName)
    {
        // Arrange
        var createDto = new CreateCharacterDto
        {
            Name = invalidName,
            GameType = "dnd",
            UserId = 1
        };
        
        // Act
        var result = await _characterService.CreateCharacterAsync(createDto);
        
        // Assert
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.ErrorMessage);
        Assert.IsTrue(result.ErrorMessage.Contains("nom"));
        
        // Vérifier qu'aucun appel n'a été fait
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
```

#### **🔗 Tests d'Intégration (API)**
```csharp
[TestClass]
public class CharacterEndpointsIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    
    public CharacterEndpointsIntegrationTests()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remplacer la base par une base en mémoire
                    services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb");
                    });
                });
            });
            
        _client = _factory.CreateClient();
    }
    
    [TestMethod]
    public async Task GetCharacters_WithValidUserId_ReturnsCharacters()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);
        
        // Act
        var response = await _client.GetAsync("/api/characters?userId=1");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var characters = JsonSerializer.Deserialize<List<CharacterDto>>(content);
        
        Assert.IsNotNull(characters);
        // Autres assertions selon les données attendues
    }
    
    [TestMethod]
    public async Task CreateCharacter_WithValidData_ReturnsCreated()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);
        
        var createRequest = new CreateCharacterDto
        {
            Name = "Aragorn",
            Class = "Rôdeur",
            GameType = "dnd",
            UserId = 1
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/characters", createRequest);
        
        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        var locationHeader = response.Headers.Location?.ToString();
        Assert.IsTrue(locationHeader?.Contains("/api/characters/"));
    }
}
```

#### **🎭 Tests End-to-End (Blazor avec Playwright)**
```csharp
[TestClass]
public class CharacterCreationE2ETests
{
    private IPage _page;
    private IBrowser _browser;
    
    [TestInitialize]
    public async Task Setup()
    {
        var playwright = await Playwright.CreateAsync();
        _browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false // Mettre à true pour les tests CI/CD
        });
        
        var context = await _browser.NewContextAsync();
        _page = await context.NewPageAsync();
        
        // Navigation vers l'application
        await _page.GotoAsync("https://localhost:5001");
    }
    
    [TestMethod]
    public async Task CreateDndCharacter_FullWorkflow_Success()
    {
        // Login
        await _page.ClickAsync("[data-testid='login-button']");
        await _page.FillAsync("[data-testid='email']", "test@example.com");
        await _page.FillAsync("[data-testid='password']", "Test123!");
        await _page.ClickAsync("[data-testid='submit-login']");
        
        // Attendre la redirection
        await _page.WaitForURLAsync("**/dashboard");
        
        // Créer un personnage
        await _page.ClickAsync("[data-testid='create-character']");
        await _page.ClickAsync("[data-testid='gametype-dnd']");
        
        // Remplir le formulaire
        await _page.FillAsync("[data-testid='character-name']", "Legolas");
        await _page.SelectOptionAsync("[data-testid='character-class']", "Rôdeur");
        
        // Allocation des statistiques
        await _page.ClickAsync("[data-testid='dexterity-plus']"); // 15
        await _page.ClickAsync("[data-testid='dexterity-plus']"); // 16
        await _page.ClickAsync("[data-testid='wisdom-plus']");    // 15
        
        // Vérifier les calculs automatiques
        var armorClass = await _page.TextContentAsync("[data-testid='calculated-ac']");
        Assert.AreEqual("13", armorClass); // 10 + 3 (Dex modifier)
        
        // Soumettre
        await _page.ClickAsync("[data-testid='submit-character']");
        
        // Vérifier la création
        await _page.WaitForURLAsync("**/character/*");
        var characterName = await _page.TextContentAsync("[data-testid='character-name']");
        Assert.AreEqual("Legolas", characterName);
    }
    
    [TestCleanup]
    public async Task Cleanup()
    {
        await _browser.CloseAsync();
    }
}
```

---

## 🔧 **Outils de Développement**

### 📊 **Configuration SonarQube**
```xml
<!-- Dans le fichier .csproj principal -->
<PropertyGroup>
  <SonarQubeTestProject>false</SonarQubeTestProject>
  <WarningsAsErrors />
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  <WarningsNotAsErrors>CS1591</WarningsNotAsErrors>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="SonarAnalyzer.CSharp" Version="9.12.0.78982">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>analyzers</IncludeAssets>
  </PackageReference>
</ItemGroup>
```

### 🔍 **Configuration EditorConfig**
```ini
# .editorconfig
root = true

[*]
charset = utf-8
end_of_line = crlf
trim_trailing_whitespace = true
insert_final_newline = true
indent_style = space
indent_size = 4

[*.{cs,vb}]
# Règles de style .NET
dotnet_sort_system_directives_first = true
dotnet_style_qualification_for_field = false:suggestion
dotnet_style_qualification_for_property = false:suggestion

# Règles de style C#
csharp_prefer_braces = true:warning
csharp_style_var_for_built_in_types = false:suggestion
csharp_style_var_when_type_is_apparent = true:suggestion

# Règles de formatage
csharp_new_line_before_open_brace = all
csharp_new_line_before_else = true
csharp_new_line_before_catch = true
csharp_new_line_before_finally = true

[*.{js,ts,json,yml,yaml}]
indent_size = 2

[*.md]
trim_trailing_whitespace = false
```

### 🚀 **Scripts PowerShell Utilitaires**
```powershell
# Scripts/BuildAndTest.ps1
param(
    [Parameter(Mandatory=$false)]
    [string]$Configuration = "Debug",
    
    [Parameter(Mandatory=$false)]
    [switch]$RunTests = $true,
    
    [Parameter(Mandatory=$false)]
    [switch]$GenerateReport = $false
)

Write-Host "🏗️ Building Chronique des Mondes..." -ForegroundColor Green

# Restauration des packages
Write-Host "📦 Restoring packages..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

# Build
Write-Host "🔨 Building solution..." -ForegroundColor Yellow
dotnet build --configuration $Configuration --no-restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

# Tests
if ($RunTests) {
    Write-Host "🧪 Running tests..." -ForegroundColor Yellow
    dotnet test --configuration $Configuration --no-build --logger "console;verbosity=minimal"
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}

# Génération rapport de couverture
if ($GenerateReport) {
    Write-Host "📊 Generating coverage report..." -ForegroundColor Yellow
    dotnet test --configuration $Configuration --no-build --collect:"XPlat Code Coverage"
    
    # Installation de ReportGenerator si nécessaire
    if (!(Get-Command "reportgenerator" -ErrorAction SilentlyContinue)) {
        dotnet tool install --global dotnet-reportgenerator-globaltool
    }
    
    # Génération du rapport HTML
    reportgenerator -reports:**/coverage.cobertura.xml -targetdir:./coverage -reporttypes:Html
    
    Write-Host "📊 Coverage report generated in ./coverage/index.html" -ForegroundColor Green
}

Write-Host "✅ Build completed successfully!" -ForegroundColor Green
```

---

## 🔄 **Workflow Git Recommandé**

### 🌿 **GitFlow Adapté**
```bash
# Démarrer une nouvelle fonctionnalité
git checkout develop
git pull origin develop
git checkout -b feature/US-183-sorts-core

# Développement avec commits atomiques
git add .
git commit -m "feat(spells): implement official spell loading from SRD

- Add SpellSRDLoader service
- Implement JSON parsing for D&D spell properties
- Add validation for spell data integrity
- Include unit tests for spell creation"

# Push régulier de la branche
git push -u origin feature/US-183-sorts-core

# Avant merge : rebase et nettoyage
git checkout develop
git pull origin develop
git checkout feature/US-183-sorts-core
git rebase develop

# Création Pull Request
# Review → Tests → Merge
```

### 📝 **Convention de Commits**
```bash
# Format : type(scope): description
# 
# Types valides :
# feat     : Nouvelle fonctionnalité
# fix      : Correction de bug
# docs     : Documentation
# style    : Formatage (sans impact fonctionnel)
# refactor : Refactoring (sans nouvelle feature ni bug fix)
# test     : Ajout/modification de tests
# chore    : Tâches de maintenance

# Exemples corrects :
git commit -m "feat(characters): add D&D 5e automatic calculations

- Implement armor class calculation based on equipped armor
- Add proficiency bonus calculation by character level  
- Include modifier calculation from ability scores
- Add comprehensive unit tests for all calculations

Closes #US-173"

git commit -m "fix(auth): resolve JWT token expiration handling

- Fix token refresh mechanism in AuthService
- Update client-side token storage logic
- Add proper error handling for expired tokens

Fixes #Bug-45"

git commit -m "docs(api): add comprehensive endpoint documentation

- Document all character-related endpoints
- Include request/response examples
- Add authentication requirements
- Update OpenAPI specifications"
```

---

## 📊 **Performance et Optimisation**

### ⚡ **Entity Framework Best Practices**
```csharp
// ✅ Utilisation correcte d'Include pour éviter N+1
public async Task<List<CampaignDto>> GetUserCampaignsAsync(int userId)
{
    return await _context.Campaigns
        .Include(c => c.GameMaster)
        .Include(c => c.Players)
            .ThenInclude(p => p.User)
        .Include(c => c.CurrentChapter)
        .Where(c => c.GameMasterId == userId || c.Players.Any(p => p.UserId == userId))
        .Select(c => new CampaignDto
        {
            Id = c.Id,
            Name = c.Name,
            GameMasterName = c.GameMaster.UserName,
            PlayerCount = c.Players.Count(p => p.IsActive),
            CurrentChapterTitle = c.CurrentChapter != null ? c.CurrentChapter.Title : null,
            ProgressPercentage = CalculateProgress(c)
        })
        .ToListAsync();
}

// ✅ Pagination pour éviter les gros datasets
public async Task<PagedResult<SpellDto>> GetSpellsPagedAsync(SpellSearchCriteria criteria)
{
    var query = _context.Spells.AsQueryable();
    
    // Application des filtres
    if (!string.IsNullOrEmpty(criteria.GameType))
        query = query.Where(s => s.GameType == criteria.GameType);
        
    if (!string.IsNullOrEmpty(criteria.SearchTerm))
        query = query.Where(s => EF.Functions.Contains(s.Name, criteria.SearchTerm) || 
                                 EF.Functions.Contains(s.Description, criteria.SearchTerm));
    
    var totalCount = await query.CountAsync();
    
    var spells = await query
        .Skip(criteria.Skip)
        .Take(criteria.Take)
        .Select(s => new SpellDto
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description.Length > 200 ? 
                s.Description.Substring(0, 200) + "..." : s.Description,
            GameType = s.GameType,
            Source = s.CreatedByUserId == 0 ? SpellSource.Official : SpellSource.Private
        })
        .ToListAsync();
    
    return new PagedResult<SpellDto>
    {
        Items = spells,
        TotalCount = totalCount,
        Page = criteria.Page,
        PageSize = criteria.PageSize,
        TotalPages = (int)Math.Ceiling(totalCount / (double)criteria.PageSize)
    };
}
```

### 💾 **Cache Intelligent**
```csharp
public class CachedSpellService : ISpellService
{
    private readonly ISpellService _inner;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachedSpellService> _logger;
    
    public async Task<List<SpellDto>> GetOfficialSpellsAsync(string gameType)
    {
        var cacheKey = $"official_spells_{gameType}";
        
        if (_cache.TryGetValue(cacheKey, out List<SpellDto>? cachedSpells))
        {
            _logger.LogDebug("Cache hit for official spells {GameType}", gameType);
            return cachedSpells!;
        }
        
        var spells = await _inner.GetOfficialSpellsAsync(gameType);
        
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
            Priority = CacheItemPriority.High,
            SlidingExpiration = TimeSpan.FromMinutes(20)
        };
        
        _cache.Set(cacheKey, spells, cacheOptions);
        _logger.LogInformation("Cached {Count} official spells for {GameType}", spells.Count, gameType);
        
        return spells;
    }
    
    public async Task InvalidateUserSpellCacheAsync(int userId, string gameType)
    {
        var cacheKey = $"user_spells_{userId}_{gameType}";
        _cache.Remove(cacheKey);
        _logger.LogDebug("Invalidated spell cache for user {UserId}, game type {GameType}", userId, gameType);
    }
}
```

---

Ce guide de développement complet vous permet de démarrer efficacement sur **Chronique des Mondes** avec tous les standards et bonnes pratiques pour maintenir une qualité exceptionnelle ! 🚀🛠️

**Prêt pour la dernière page ?** 📊