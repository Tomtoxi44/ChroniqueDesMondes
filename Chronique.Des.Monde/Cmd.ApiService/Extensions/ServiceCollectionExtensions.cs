namespace Cmd.ApiService.Extensions;

using Abstraction;

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

    public static IServiceCollection AddDndBusiness(this IServiceCollection services)
    {
        // services.AddKeyedTransient<IPlayerCharacterBusiness, PlayerCharacterBusinessDnd>("Dnd");

        return services;
    }

    public static IServiceCollection AddSkyrimBusiness(this IServiceCollection services)
    {
        // services.AddKeyedTransient<IPlayerCharacterBusiness, PlayerCharacterBusinessSky>("Skyrim");

        return services;
    }
}