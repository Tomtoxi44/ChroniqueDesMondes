using Cdm.ServiceDefaults;
using Cdm.Web.Components;
using Cdm.Web.Services.Authentication;
using Cdm.Web.Services.Api;
using Cdm.Web.Services.Character;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container AVANT Aspire pour éviter les conflits
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Ajouter les contrôleurs pour l'authentification
builder.Services.AddControllers();

builder.Services.AddOutputCache();

// Add authentication services
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

// Configuration URL AVANT les HttpClients
var apiBaseUrl = "https://localhost:7428"; // URL fixe pour développement

Console.WriteLine($"🔧 Configuration API Base URL: {apiBaseUrl}");

// Configure HttpClient for API calls - CONFIGURATION FORCÉE
builder.Services.AddHttpClient<IApiService, ApiService>("ApiServiceClient", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);

    Console.WriteLine($"✅ HttpClient configuré pour ApiService avec BaseAddress: {client.BaseAddress}");
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    return new HttpClientHandler()
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    };
});

// Configure HttpClient for Character Service
builder.Services.AddHttpClient<ICharacterService, CharacterService>("CharacterServiceClient", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);

    Console.WriteLine($"✅ HttpClient configuré pour CharacterService avec BaseAddress: {client.BaseAddress}");
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    return new HttpClientHandler()
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    };
});

// Add custom services with new namespaces (Theme service supprimé)
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddHttpContextAccessor();

// Add service defaults & Aspire client integrations APRÈS notre configuration
builder.AddServiceDefaults();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.UseOutputCache();

// Mapper les contrôleurs
app.MapControllers();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
