namespace Cdm.ApiService.Extensions;

using Cdm.Business.Common.Business.Users;
using Cdm.Business.Common.Business.Campaigns;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddCommonBusiness();
        
        return services;
    }

    public static IServiceCollection AddCommonBusiness(this IServiceCollection services)
    {
        services.AddTransient<UserBusiness>();
        services.AddTransient<CampaignBusiness>();
        services.AddTransient<ChapterBusiness>();
        services.AddTransient<ContentBlockBusiness>();
        services.AddTransient<NpcBusiness>();
        services.AddScoped<Cdm.Common.PasswordService>();
        services.AddScoped<Cdm.Common.JwtService>();

        return services;
    }
}