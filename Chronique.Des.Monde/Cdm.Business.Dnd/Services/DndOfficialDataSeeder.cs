using Cdm.Data.Dnd;
using Cdm.Data.Dnd.Models;
using Cdm.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Cdm.Business.Dnd.Services;

/// <summary>
/// Service d'injection des données officielles D&D 5e dans la base de données
/// Injecte les sorts, équipements et autres éléments du System Reference Document
/// </summary>
public class DndOfficialDataSeeder
{
    private readonly DndDbContext context;
    private readonly ILogger<DndOfficialDataSeeder> logger;

    public DndOfficialDataSeeder(DndDbContext context, ILogger<DndOfficialDataSeeder> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    /// <summary>
    /// Injecte toutes les données officielles D&D si elles n'existent pas déjà
    /// </summary>
    public async Task SeedAllOfficialDataAsync()
    {
        this.logger.LogInformation("Starting D&D 5e official data seeding...");

        await SeedOfficialSpellsAsync();
        // TODO: Ajouter l'injection d'équipements
        // await SeedOfficialEquipmentAsync();

        this.logger.LogInformation("D&D 5e official data seeding completed");
    }

    /// <summary>
    /// Injecte les sorts officiels D&D 5e du System Reference Document
    /// </summary>
    public async Task SeedOfficialSpellsAsync()
    {
        this.logger.LogInformation("Seeding official D&D 5e spells...");

        // Vérifier si des sorts officiels existent déjà
        var existingOfficialSpells = await this.context.SpellsDnd
            .Where(s => s.CreatedByUserId == 0) // 0 = Officiel
            .CountAsync();

        if (existingOfficialSpells > 0)
        {
            this.logger.LogInformation("Official spells already exist ({Count} spells), skipping seeding", existingOfficialSpells);
            return;
        }

        var officialSpells = CreateOfficialSpellsList();

        await this.context.SpellsDnd.AddRangeAsync(officialSpells);
        await this.context.SaveChangesAsync();

        this.logger.LogInformation("Successfully seeded {Count} official D&D 5e spells", officialSpells.Count);
    }

    /// <summary>
    /// Crée la liste des sorts officiels D&D 5e selon le System Reference Document
    /// Version simplifiée pour éviter les erreurs de compilation
    /// </summary>
    private static List<SpellDnd> CreateOfficialSpellsList()
    {
        var now = DateTime.UtcNow;
        var spells = new List<SpellDnd>();

        // ===== SORTS OFFICIELS D&D 5E - VERSION SIMPLIFIÉE =====

        spells.Add(new SpellDnd
        {
            Name = "Projectile magique",
            Description = "Vous créez trois dards scintillants d'énergie magique. Chaque dard touche automatiquement une créature de votre choix que vous pouvez voir dans la portée. Un dard inflige 1d4 + 1 dégâts de force à sa cible.",
            School = "Évocation",
            Level = 1,
            CastingTime = "1 action",
            Range = "36 mètres",
            Duration = "Instantané",
            Components = "V, S",
            Damage = "3d4+3",
            GameType = GameType.DnD,
            CreatedByUserId = 0, // Officiel
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "évocation,force,projectile,automatique"
        });

        spells.Add(new SpellDnd
        {
            Name = "Lumière",
            Description = "Vous touchez un objet qui ne fait pas plus de 3 mètres dans n'importe quelle dimension. Jusqu'à la fin du sort, l'objet émet une lumière vive dans un rayon de 6 mètres et une lumière faible sur 6 mètres supplémentaires.",
            School = "Évocation",
            Level = 0, // Cantrip
            CastingTime = "1 action",
            Range = "Contact",
            Duration = "1 heure",
            Components = "V, M",
            MaterialComponent = "une luciole ou de la mousse phosphorescente",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "évocation,lumière,cantrip,utilitaire"
        });

        spells.Add(new SpellDnd
        {
            Name = "Boule de feu",
            Description = "Un rayon brillant jaillit de votre doigt tendu vers un point de votre choix dans la portée et explose dans un rugissement de flammes. Chaque créature dans une sphère de 6 mètres de rayon centrée sur ce point doit faire un jet de sauvegarde de Dextérité.",
            School = "Évocation",
            Level = 3,
            CastingTime = "1 action",
            Range = "45 mètres",
            Duration = "Instantané",
            Components = "V, S, M",
            MaterialComponent = "une petite boule de guano de chauve-souris et du soufre",
            Damage = "8d6",
            SavingThrow = "Dextérité",
            HigherLevelDamage = 1,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "évocation,feu,zone,dégâts,classique"
        });

        spells.Add(new SpellDnd
        {
            Name = "Soins",
            Description = "Une créature que vous touchez récupère un nombre de points de vie égal à 1d8 + votre modificateur de caractéristique d'incantation. Ce sort n'a aucun effet sur les morts-vivants ou les artificiels.",
            School = "Évocation",
            Level = 1,
            CastingTime = "1 action",
            Range = "Contact",
            Duration = "Instantané",
            Components = "V, S",
            Damage = "1d8+MOD",
            HigherLevelDamage = 1,
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "évocation,soin,toucher,instantané"
        });

        spells.Add(new SpellDnd
        {
            Name = "Bouclier",
            Description = "Une barrière invisible de force magique apparaît et vous protège. Jusqu'au début de votre prochain tour, vous avez un bonus de +5 à la CA, y compris contre l'attaque déclenchante.",
            School = "Abjuration",
            Level = 1,
            CastingTime = "1 réaction",
            Range = "Personnelle",
            Duration = "1 tour",
            Components = "V, S",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "abjuration,protection,réaction,CA"
        });

        return spells;
    }
}

/// <summary>
/// Extensions pour simplifier l'injection de données
/// </summary>
public static class DndDataSeederExtensions
{
    /// <summary>
    /// Ajoute le service d'injection de données D&D à la DI
    /// </summary>
    public static IServiceCollection AddDndDataSeeder(this IServiceCollection services)
    {
        services.AddScoped<DndOfficialDataSeeder>();
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