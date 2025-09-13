using Cdm.Web.Components;
using Cdm.Web.Services.Authentication;
using Cdm.Web.Services.Api;
using Cdm.Web.Services.Characters;
using Cdm.Web.Services.Combat;
using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Services de base
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// MudBlazor pour l'UI moderne
builder.Services.AddMudServices();

// Services HTTP et authentification
builder.Services.AddHttpContextAccessor();

// Configuration HttpClient pour l'API
var apiBaseUrl = builder.Configuration.GetValue<string>("ApiSettings:BaseUrl") ?? "https://localhost:7428";

// HttpClient pour les services API avec authentification
builder.Services.AddHttpClient<ICharacterApiService, CharacterApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<ICombatApiService, CombatApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// HttpClient pour le service d'authentification JWT
builder.Services.AddHttpClient<IJwtAuthService, JwtAuthService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Services d'authentification (garder l'ancien pour compatibilité)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/access-denied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.Name = "CdmAuth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

builder.Services.AddAuthorization();

// Services applicatifs
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>(); // Ancien service (compatibilité)
builder.Services.AddScoped<IApiService, ApiService>(); // Ancien service (compatibilité)

var app = builder.Build();

// Pipeline de requêtes
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

Console.WriteLine($"🌃 Chronique des Mondes - API Integration + JWT Auth");
Console.WriteLine($"📍 Frontend: http://localhost:5222");
Console.WriteLine($"🔌 API Backend: {apiBaseUrl}");
Console.WriteLine($"🔐 JWT Authentication: Enabled");

app.Run();
