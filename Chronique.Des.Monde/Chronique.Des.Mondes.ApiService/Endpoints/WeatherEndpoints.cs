using Chronique.Des.Mondes.SampleWeather;
using Chronique.Des.Mondes.SampleWeather.Business;
using Microsoft.AspNetCore.Authorization;

public static class WeatherEndpoints
{
    public static void MapWeatherEndpoints(this WebApplication app)
    {
        app.MapGet("/weather", [Authorize] () =>
        {
            var service = new WeatherService();
            return service.GetWeather();
        });
    }
}