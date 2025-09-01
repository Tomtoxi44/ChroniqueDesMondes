using Cdm.Business.Common.Business.Campaigns;
using Cdm.Business.Common.Business.Spells;
using Cdm.Data;
using Microsoft.EntityFrameworkCore;
using Cdm.Data.Dnd;
using Cdm.Common.Services;
using Cdm.Common;
using Cdm.Business.Common.Business.Users;
using Cdm.Business.Dnd.Extensions;

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
        
        // Services métier spécialisés D&D
        services.AddDndBusiness();

        // Services communs
        services.AddScoped<IEmailService, AzureEmailService>();
        services.AddScoped<JwtService>();
        services.AddScoped<PasswordService>();

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