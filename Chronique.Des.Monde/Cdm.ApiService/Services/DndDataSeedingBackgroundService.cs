using Cdm.Business.Dnd.Services;

namespace Cdm.ApiService.Services;

/// <summary>
/// Service d'arrière-plan qui injecte automatiquement les données officielles D&D au démarrage
/// Utilise BackgroundService pour s'exécuter une seule fois au lancement de l'application
/// </summary>
public class DndDataSeedingBackgroundService : BackgroundService
{
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<DndDataSeedingBackgroundService> logger;

    public DndDataSeedingBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<DndDataSeedingBackgroundService> logger)
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            this.logger.LogInformation("🚀 Starting automatic D&D official data seeding...");

            // Attendre un peu que l'application soit complètement démarrée
            await Task.Delay(2000, stoppingToken);

            // Injecter les données officielles
            await this.serviceProvider.SeedDndOfficialDataAsync();

            this.logger.LogInformation("✅ Automatic D&D official data seeding completed successfully");
        }
        catch (OperationCanceledException)
        {
            this.logger.LogWarning("⚠️ D&D data seeding was cancelled during application shutdown");
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "❌ Error during automatic D&D data seeding");
            // Ne pas faire planter l'application si l'injection échoue
        }
    }
}

/// <summary>
/// Extensions pour enregistrer le service d'injection automatique
/// </summary>
public static class DndDataSeedingServiceExtensions
{
    /// <summary>
    /// Ajoute le service d'injection automatique des données D&D au démarrage
    /// </summary>
    public static IServiceCollection AddDndAutoDataSeeding(this IServiceCollection services)
    {
        services.AddHostedService<DndDataSeedingBackgroundService>();
        return services;
    }
}