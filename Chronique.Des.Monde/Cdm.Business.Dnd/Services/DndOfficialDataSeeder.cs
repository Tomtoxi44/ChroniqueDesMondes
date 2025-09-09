using Cdm.Data.Dnd;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Cdm.Business.Dnd.Services;

/// <summary>
/// Service d'injection principal des données officielles D&D 5e dans la base de données
/// Coordonne l'injection des sorts, équipements et autres éléments du System Reference Document
/// Délègue le travail aux seeders spécialisés
/// </summary>
public class DndOfficialDataSeeder
{
    private readonly DndDbContext context;
    private readonly ILogger<DndOfficialDataSeeder> logger;
    private readonly DndSpellSeeder spellSeeder;
    private readonly DndEquipmentSeeder equipmentSeeder;

    public DndOfficialDataSeeder(
        DndDbContext context,
        ILogger<DndOfficialDataSeeder> logger,
        DndSpellSeeder spellSeeder,
        DndEquipmentSeeder equipmentSeeder)
    {
        this.context = context;
        this.logger = logger;
        this.spellSeeder = spellSeeder;
        this.equipmentSeeder = equipmentSeeder;
    }

    /// <summary>
    /// Injecte toutes les données officielles D&D si elles n'existent pas déjà
    /// </summary>
    public async Task SeedAllOfficialDataAsync()
    {
        this.logger.LogInformation("🚀 Starting D&D 5e official data seeding...");

        await this.spellSeeder.SeedOfficialSpellsAsync();
        await this.equipmentSeeder.SeedOfficialEquipmentAsync();

        this.logger.LogInformation("✅ D&D 5e official data seeding completed");
    }

    /// <summary>
    /// Injecte les sorts officiels D&D 5e du System Reference Document
    /// </summary>
    public async Task SeedOfficialSpellsAsync()
    {
        await this.spellSeeder.SeedOfficialSpellsAsync();
    }

    /// <summary>
    /// Injecte les équipements officiels D&D 5e du System Reference Document
    /// </summary>
    public async Task SeedOfficialEquipmentAsync()
    {
        await this.equipmentSeeder.SeedOfficialEquipmentAsync();
    }
}

/// <summary>
/// Extensions pour simplifier l'injection de données
/// </summary>
public static class DndDataSeederExtensions
{
    /// <summary>
    /// Ajoute les services d'injection de données D&D à la DI
    /// </summary>
    public static IServiceCollection AddDndDataSeeder(this IServiceCollection services)
    {
        services.AddScoped<DndOfficialDataSeeder>();
        services.AddScoped<DndSpellSeeder>();
        services.AddScoped<DndEquipmentSeeder>();
        return services;
    }

    /// <summary>
    /// Injecte les données officielles D&D au démarrage de l'application
    /// </summary>
    public static async Task SeedDndOfficialDataAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DndOfficialDataSeeder>();
        await seeder.SeedAllOfficialDataAsync();
    }
}