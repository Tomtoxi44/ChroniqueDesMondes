using Chronique.Des.Monde.Player.Business;
using Microsoft.Extensions.DependencyInjection;

namespace Chronique.Des.Mondes.ApiService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<UserBusiness>();
            services.AddScoped<PlayerCharacterBusiness>();
            services.AddScoped<PasswordService>();
            services.AddScoped<JwtService>();

            return services;
        }
    }
}