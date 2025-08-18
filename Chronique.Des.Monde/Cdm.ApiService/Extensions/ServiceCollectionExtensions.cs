namespace Chronique.Des.Mondes.ApiService.Extensions;

using Cmd.Business.Character.Extensions;
using Cdm.Business.Common.Business.Users;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddCommonBusiness();
        services.AddDndBusiness();
        services.AddSkyrimBusiness();
        
        return services;
    }

    public static IServiceCollection AddCommonBusiness(this IServiceCollection services)
    {
        services.AddTransient<UserBusiness>();
        services.AddScoped<PasswordService>();
        services.AddScoped<JwtService>();

        return services;
    }

    public static IServiceCollection AddSkyrimBusiness(this IServiceCollection services)
    {
        // services.AddKeyedTransient<IPlayerCharacterBusiness, PlayerCharacterBusinessSky>("Skyrim");

        return services;
    }
}