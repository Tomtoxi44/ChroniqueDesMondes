using Cdm.ServiceDefaults;
using Cdm.Web.Components;
using Cdm.Web.Services.Authentication;
using Cdm.Web.Services.Api;
using Cdm.Web.Services.Character;
using Cdm.Web.Services.Theme;
using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container AVANT Aspire pour éviter les conflits
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 🎨 Add MudBlazor services
builder.Services.AddMudServices();

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

// 🎨 Theme service for D&D
builder.Services.AddScoped<IThemeService, ThemeService>();

// 🔧 TEMPORARY: Comment out API services for demo
// Configuration URL AVANT les HttpClients
// var apiBaseUrl = "https://localhost:7428"; // URL fixe pour développement

Console.WriteLine($"🎨 Mode Design System - API désactivée temporairement");

// 🔧 TEMPORARY: Mock services instead of real API
// builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
// builder.Services.AddScoped<IApiService, ApiService>();
// builder.Services.AddScoped<ICharacterService, CharacterService>();
// builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddHttpContextAccessor();

// 📡 Add SignalR (préparation Phase 2)
builder.Services.AddSignalR();

// Add service defaults & Aspire client integrations APRÈS notre configuration
// builder.AddServiceDefaults(); // Temporarily disabled

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

// 🔧 TEMPORARY: Comment out auth for demo
// app.UseAuthentication();
// app.UseAuthorization();

app.UseAntiforgery();

app.UseOutputCache();

// Mapper les contrôleurs
app.MapControllers();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// app.MapDefaultEndpoints(); // Temporarily disabled

Console.WriteLine($"🚀 Application démarrée en mode Design System");
Console.WriteLine($"📍 Accédez à : http://localhost:5222/design-system");

app.Run();
