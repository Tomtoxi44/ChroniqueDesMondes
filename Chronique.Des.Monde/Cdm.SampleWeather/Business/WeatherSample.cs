using Chronique.Des.Mondes.SampleWeather.Models;

namespace Chronique.Des.Mondes.SampleWeather.Business;

public class WeatherService
{
    public Weather GetWeather()
    {
        var temp = new Random();
        // Logique métier simulée
        return new Weather()
        {
            Date = DateTime.Now,
            TemperatureC = temp.Next(50),
            Summary = "Ensoleillé"
        };
    }
}
