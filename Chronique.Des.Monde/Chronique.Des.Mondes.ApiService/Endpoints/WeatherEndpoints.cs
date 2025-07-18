using Chronique.Des.Mondes.SampleWeather;

public static class WeatherEndpoints
{
    public static void MapWeatherEndpoints(this WebApplication app)
    {
        app.MapGet("/weather", () =>
        {
            var service = new WeatherService();
            return service.GetWeather();
        });
    }
}