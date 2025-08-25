# 🏗️ Architecture Technique .NET 9

Cette page détaille l'architecture technique complète de **Chronique des Mondes** basée sur .NET 9, Blazor Server, et Azure Cloud.

---

## 🎯 **Vue d'Ensemble Architecture**

### **📊 Stack Technique Moderne**
```
┌─────────────────────────────────────────────────────────────┐
│                   ARCHITECTURE .NET 9                      │
├─────────────────────────────────────────────────────────────┤
│ 🎨 FRONTEND                                                │
│ ├── Blazor Server (.NET 9)      : UI Interactive C#        │
│ ├── SignalR                     : Temps réel WebSocket     │
│ ├── Bootstrap 5                 : CSS Framework responsive │
│ └── Progressive Web App         : Offline + notifications  │
├─────────────────────────────────────────────────────────────┤
│ 🔧 BACKEND                                                 │
│ ├── ASP.NET Core API (.NET 9)   : REST endpoints          │
│ ├── Entity Framework Core       : ORM + Migrations        │
│ ├── FluentValidation            : Validation métier       │
│ └── JWT Authentication          : Sécurité stateless      │
├─────────────────────────────────────────────────────────────┤
│ 🗄️ DATA                                                    │
│ ├── SQL Server                  : Base relationnelle      │
│ ├── Redis Cache                 : Cache distribué         │
│ ├── Azure Blob Storage          : Images/fichiers         │
│ └── Application Insights        : Logs + monitoring       │
├─────────────────────────────────────────────────────────────┤
│ ☁️ INFRASTRUCTURE                                          │
│ ├── Azure App Service           : Hosting applications    │
│ ├── Docker Containers           : Déploiement isolé       │
│ ├── GitHub Actions              : CI/CD pipeline          │
│ └── Azure Key Vault             : Secrets management      │
└─────────────────────────────────────────────────────────────┘
```

---

## 🏗️ **Architecture Applicative**

### **📁 Structure des Projets**
```
Chronique.Des.Monde.sln
├── 🎨 Cdm.Web                    # Interface Blazor Server
├── 🔧 Cdm.ApiService             # API REST + SignalR Hubs
├── 💼 Cdm.Business.Common        # Services métier génériques
├── 🎲 Cdm.Business.Dnd           # Services spécialisés D&D
├── 🗄️ Cdm.Data.Common            # Entités et DbContext génériques
├── 🐉 Cdm.Data.Dnd               # Entités spécialisées D&D
├── 🔄 Cdm.Migrations             # Migrations Entity Framework
├── 🧪 Cdm.Tests                  # Tests automatisés
├── ⚙️ Cdm.ServiceDefaults        # Configuration partagée
└── 🚀 Cdm.AppHost                # Orchestration .NET Aspire
```

### **🔄 Flux de Données Architecture**
```
🎨 Blazor Components
    ↕️ SignalR (temps réel)
    ↕️ HTTP Client (API calls)
🔧 ASP.NET Core API
    ↕️ Dependency Injection
💼 Business Services
    ↕️ Repository Pattern
🗄️ Entity Framework Core
    ↕️ Migrations & LINQ
💾 SQL Server Database
```

---

## 🎨 **Frontend Blazor Architecture**

### **🧩 Structure Composants Blazor**
```
Cdm.Web/
├── 📁 Components/
│   ├── 📁 Characters/            # Gestion personnages
│   │   ├── CreateCharacter.razor
│   │   ├── CharacterCard.razor
│   │   └── CharacterWizard.razor
│   ├── 📁 Spells/               # Système de sorts
│   │   ├── SpellGrid.razor
│   │   ├── SpellModal.razor
│   │   └── Grimoire.razor
│   ├── 📁 Equipment/            # Équipements
│   │   ├── InventoryPanel.razor
│   │   └── TradeModal.razor
│   ├── 📁 Campaigns/            # Campagnes
│   │   ├── CampaignDashboard.razor
│   │   └── ChapterEditor.razor
│   ├── 📁 Combat/               # Combat temps réel
│   │   ├── CombatInterface.razor
│   │   └── InitiativeTracker.razor
│   └── 📁 Shared/               # Composants partagés
│       ├── MainLayout.razor
│       ├── NavMenu.razor
│       └── LoadingSpinner.razor
├── 📁 Pages/                    # Pages principales
│   ├── Dashboard.razor
│   ├── Characters.razor
│   ├── Spells.razor
│   └── Session.razor
├── 📁 Services/                 # Services côté client
│   ├── AuthenticationService.cs
│   ├── SignalRService.cs
│   └── ApiService.cs
└── 📁 wwwroot/                  # Assets statiques
    ├── css/app.css
    ├── js/app.js
    └── images/
```

### **⚡ Services Blazor Principaux**
```csharp
// Service d'authentification côté client
public class ClientAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    
    public async Task<bool> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", result.Token);
            return true;
        }
        return false;
    }
}

// Service SignalR pour temps réel
public class SignalRService : IAsyncDisposable
{
    private HubConnection? _hubConnection;
    
    public async Task StartAsync()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("/sessionHub")
            .Build();
            
        await _hubConnection.StartAsync();
    }
    
    public async Task JoinSessionAsync(string sessionId)
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.SendAsync("JoinSession", sessionId);
        }
    }
}
```

---

## 🔧 **Backend API Architecture**

### **🎯 Endpoints Pattern**
```csharp
// Exemple d'endpoint moderne .NET 9
public static class CharacterEndpoints
{
    public static void MapCharacterEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/characters")
            .WithTags("Characters")
            .RequireAuthorization();
            
        group.MapGet("/", GetCharacters)
            .WithName("GetCharacters")
            .WithOpenApi();
            
        group.MapPost("/", CreateCharacter)
            .WithName("CreateCharacter")
            .WithOpenApi();
            
        group.MapPut("/{id:int}", UpdateCharacter)
            .WithName("UpdateCharacter")
            .WithOpenApi();
    }
    
    private static async Task<IResult> GetCharacters(
        [FromQuery] int userId,
        [FromQuery] string? gameType,
        ICharacterService characterService)
    {
        var characters = await characterService.GetUserCharactersAsync(userId, gameType);
        return Results.Ok(characters);
    }
    
    private static async Task<IResult> CreateCharacter(
        CreateCharacterDto dto,
        ICharacterService characterService,
        ClaimsPrincipal user)
    {
        var userId = user.GetUserId();
        var result = await characterService.CreateCharacterAsync(dto, userId);
        
        return result.Success 
            ? Results.Created($"/api/characters/{result.Data.Id}", result.Data)
            : Results.BadRequest(result.ErrorMessage);
    }
}
```

### **🔗 SignalR Hubs pour Temps Réel**
```csharp
[Authorize]
public class SessionHub : Hub
{
    private readonly ISessionService _sessionService;
    
    public async Task JoinSession(string sessionId)
    {
        var userId = Context.User.GetUserId();
        
        // Validation autorisation
        var canJoin = await _sessionService.CanUserJoinSessionAsync(userId, sessionId);
        if (!canJoin) return;
        
        // Rejoindre le groupe SignalR
        await Groups.AddToGroupAsync(Context.ConnectionId, $"session_{sessionId}");
        
        // Notifier les autres participants
        await Clients.Group($"session_{sessionId}")
            .SendAsync("UserJoined", new { UserId = userId, JoinedAt = DateTime.UtcNow });
    }
    
    public async Task UpdateChapterProgress(string sessionId, int chapterId, string action)
    {
        var userId = Context.User.GetUserId();
        var isGM = await _sessionService.IsGameMasterAsync(userId, sessionId);
        
        if (!isGM) return;
        
        var result = await _sessionService.UpdateChapterAsync(sessionId, chapterId, action);
        
        // Diffuser à tous les participants
        await Clients.Group($"session_{sessionId}")
            .SendAsync("ChapterUpdated", result);
    }
}
```

---

## 🗄️ **Architecture de Données**

### **📊 Entity Framework Core Setup**
```csharp
// DbContext principal avec multi-GameType
public class AppDbContext : DbContext
{
    // Entités communes
    public DbSet<User> Users { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<Spell> Spells { get; set; }
    public DbSet<Equipment> Equipment { get; set; }
    
    // Entités D&D spécialisées
    public DbSet<CharacterDnd> CharactersDnd { get; set; }
    public DbSet<NPC> NPCs { get; set; }
    
    // Sessions et combat
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Combat> Combats { get; set; }
    public DbSet<CombatAction> CombatActions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuration des entités
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        
        // Configuration spécialisée multi-GameType
        ConfigureMultiGameTypeEntities(modelBuilder);
    }
    
    private void ConfigureMultiGameTypeEntities(ModelBuilder modelBuilder)
    {
        // Discriminator pour personnages
        modelBuilder.Entity<Character>()
            .HasDiscriminator<string>("GameType")
            .HasValue<CharacterDnd>("dnd")
            .HasValue<Character>("generic");
            
        // JSON pour propriétés flexibles
        modelBuilder.Entity<Spell>()
            .Property(e => e.DndProperties)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<DndSpellProperties>(v, (JsonSerializerOptions)null));
    }
}
```

### **🏗️ Repository Pattern Implementation**
```csharp
// Repository générique
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
    
    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }
    
    public virtual void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }
    
    public virtual void Remove(TEntity entity)
    {
        _dbSet.Remove(entity);
    }
}

// Repository spécialisé
public class CharacterRepository : Repository<Character>, ICharacterRepository
{
    public CharacterRepository(AppDbContext context) : base(context) { }
    
    public async Task<List<Character>> GetUserCharactersAsync(int userId, string? gameType = null)
    {
        var query = _dbSet.Where(c => c.UserId == userId);
        
        if (!string.IsNullOrEmpty(gameType))
        {
            query = query.Where(c => c.GameType == gameType);
        }
        
        return await query
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
    
    public async Task<List<CharacterDnd>> GetDndCharactersWithCalculationsAsync(int userId)
    {
        return await _context.CharactersDnd
            .Where(c => c.UserId == userId)
            .Include(c => c.CharacterSpells)
                .ThenInclude(cs => cs.Spell)
            .Include(c => c.CharacterEquipment)
                .ThenInclude(ce => ce.Equipment)
            .ToListAsync();
    }
}
```

---

## ⚙️ **Configuration et Déploiement**

### **🔧 Program.cs Configuration**
```csharp
var builder = WebApplication.CreateBuilder(args);

// Services de base
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Entity Framework
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

// Services métier
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<ISpellService, SpellService>();
builder.Services.AddScoped<ICampaignService, CampaignService>();
builder.Services.AddScoped<ICombatService, CombatService>();

// SignalR
builder.Services.AddSignalR();

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

var app = builder.Build();

// Pipeline de développement
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// API Endpoints
app.MapCharacterEndpoints();
app.MapSpellEndpoints();
app.MapCampaignEndpoints();

// SignalR Hubs
app.MapHub<SessionHub>("/sessionHub");
app.MapHub<CombatHub>("/combatHub");

app.Run();
```

### **🐳 Docker Configuration**
```dockerfile
# Dockerfile pour production
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Cdm.Web/Cdm.Web.csproj", "Cdm.Web/"]
COPY ["Cdm.ApiService/Cdm.ApiService.csproj", "Cdm.ApiService/"]
COPY ["Cdm.Business.Common/Cdm.Business.Common.csproj", "Cdm.Business.Common/"]
COPY ["Cdm.Business.Dnd/Cdm.Business.Dnd.csproj", "Cdm.Business.Dnd/"]
COPY ["Cdm.Data.Common/Cdm.Data.Common.csproj", "Cdm.Data.Common/"]
COPY ["Cdm.Data.Dnd/Cdm.Data.Dnd.csproj", "Cdm.Data.Dnd/"]

RUN dotnet restore "Cdm.Web/Cdm.Web.csproj"
COPY . .
WORKDIR "/src/Cdm.Web"
RUN dotnet build "Cdm.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cdm.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cdm.Web.dll"]
```

---

## 🚀 **CI/CD Pipeline**

### **⚙️ GitHub Actions Workflow**
```yaml
name: Deploy to Azure

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET 9
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '9.0.x'
        
    - name: Restore dependencies
      run: dotnet restore
      
    - name: Build
      run: dotnet build --no-restore --configuration Release
      
    - name: Test
      run: dotnet test --no-build --configuration Release --logger "trx" --collect:"XPlat Code Coverage"
      
    - name: Generate code coverage report
      run: |
        dotnet tool install -g dotnet-reportgenerator-globaltool
        reportgenerator -reports:**/coverage.cobertura.xml -targetdir:./coverage -reporttypes:Html
        
    - name: Upload coverage to Codecov
      uses: codecov/codecov-action@v3
      
  deploy:
    needs: build-and-test
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Login to Azure
      uses: azure/login@v1
      with:
        creds: ${{ secrets.AZURE_CREDENTIALS }}
        
    - name: Build and deploy to Azure Web App
      uses: azure/webapps-deploy@v2
      with:
        app-name: 'chronique-des-mondes'
        slot-name: 'production'
        package: './publish'
```

---

## 📊 **Monitoring et Observabilité**

### **📈 Application Insights Setup**
```csharp
// Configuration monitoring
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});

// Custom telemetry
public class CustomTelemetryService
{
    private readonly TelemetryClient _telemetryClient;
    
    public void TrackUserAction(string userId, string action, Dictionary<string, string> properties)
    {
        _telemetryClient.TrackEvent($"UserAction_{action}", properties);
    }
    
    public void TrackPerformance(string operationName, TimeSpan duration)
    {
        _telemetryClient.TrackDependency("Performance", operationName, DateTime.UtcNow.Subtract(duration), duration, true);
    }
}
```

---

## 🔒 **Sécurité et Authentification**

### **🛡️ JWT Implementation Complète**
```csharp
public class JwtService
{
    private readonly IConfiguration _configuration;
    
    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.UserEmail),
            new Claim("GameMaster", user.IsGameMaster.ToString())
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

Cette architecture .NET 9 moderne assure **performance, scalabilité et maintenabilité** pour une plateforme JDR collaborative de haute qualité ! 🚀