namespace Chronique.Des.Mondes.SampleWeather;

public class WeatherService
{
    public object GetWeather()
    {
        // Logique métier simulée
        return new[]
        {
            new { Date = DateTime.Now, TemperatureC = 25, Summary = "Ensoleillé" }
        };
    }
}
