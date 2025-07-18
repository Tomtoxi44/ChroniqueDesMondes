namespace Chronique.Des.Mondes.SampleWeather;

using Chronique.Des.Mondes.SampleWeather.Abstraction;
using Chronique.Des.Mondes.SampleWeather.Module;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class BusinessExtension
{
    /// <summary>
    ///     Registers must needed service for global data access.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>
    ///     Returns the service collection.
    /// </returns>
    public static IServiceCollection AddSampleServices(this IServiceCollection services)
    {
        services.TryAddTransient<ISampleBusiness, SampleWeatherBusiness>();
        return services;
    }
}
