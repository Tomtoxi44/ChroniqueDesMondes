using Cdm.Business.Common.Business.Campaigns;
using Cdm.Business.Common.Business.Spells;
using Cdm.Business.Common.Business.Characters;
using Cdm.Business.Common.Business.Equipment;
using Cdm.Business.Common.Interfaces.Combat;
using Cdm.Business.Common.Services.Combat;
using Cdm.Business.Dnd.Services.Combat;
using Cdm.Data;
using Microsoft.EntityFrameworkCore;
using Cdm.Data.Dnd;
using Cdm.Common.Services;
using Cdm.Common;
using Cdm.Business.Common.Business.Users;
using Cdm.Business.Dnd.Extensions;
using Cmd.Abstraction.Characters;
using Cmd.Abstraction.Equipment;

namespace Cdm.ApiService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        // Services métier communs
        services.AddScoped<CampaignBusiness>();
        services.AddScoped<ChapterBusiness>();
        services.AddScoped<ContentBlockBusiness>();
        services.AddScoped<NpcBusiness>();
        services.AddScoped<UserBusiness>();
        services.AddScoped<InvitationService>();
        
        // Services de gestion des sorts
        services.AddScoped<ISpellService, SpellService>();
        services.AddScoped<ICharacterSpellService, CharacterSpellService>();
        services.AddScoped<IEquipmentExchangeService, EquipmentExchangeService>();
        
        // Services de combat ⚔️
        services.AddCombatServices();
        
        // Services métier spécialisés D&D
        services.AddDndBusinessServices();

        // Services communs
        services.AddScoped<IEmailService, AzureEmailService>();
        services.AddScoped<JwtService>();
        services.AddScoped<PasswordService>();

        return services;
    }

    /// <summary>
    /// Ajoute tous les services de combat (moteur, dés, initiative, calculateurs)
    /// </summary>
    public static IServiceCollection AddCombatServices(this IServiceCollection services)
    {
        // Services de combat principaux
        services.AddScoped<ICombatEngine, CombatEngine>();
        services.AddScoped<IDiceRoller, DiceRoller>();
        services.AddScoped<IInitiativeManager, InitiativeManager>();
        
        // Calculateur spécialisé D&D (optionnel selon le contexte)
        services.AddScoped<IDndCombatCalculator, DndCombatCalculator>();

        return services;
    }

    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Base de données principale
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Base de données D&D
        services.AddDbContext<DndDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}