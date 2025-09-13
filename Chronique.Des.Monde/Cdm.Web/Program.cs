using Cdm.Web.Components;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Services de base
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// MudBlazor pour l'UI moderne
builder.Services.AddMudServices();

// Services de base
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Pipeline simple
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

Console.WriteLine("🌃 Chronique des Mondes - Mode Sombre Moderne");
Console.WriteLine("📍 http://localhost:5222");

app.Run();
