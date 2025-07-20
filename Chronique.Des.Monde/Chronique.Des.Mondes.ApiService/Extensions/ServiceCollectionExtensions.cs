using Microsoft.Extensions.DependencyInjection;

namespace Chronique.Des.Mondes.ApiService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Enregistrez vos services ici
            services.AddScoped<UserService>();
            services.AddScoped<PasswordService>();
            services.AddScoped<JwtService>();

            return services;
        }
    }
}