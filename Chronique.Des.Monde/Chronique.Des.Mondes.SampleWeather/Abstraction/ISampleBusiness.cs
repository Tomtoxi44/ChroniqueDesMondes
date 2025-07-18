namespace Chronique.Des.Mondes.SampleWeather.Abstraction;

using Chronique.Des.Mondes.SampleWeather.Module.Model;
using System.Threading.Tasks;

/// <summary>
///     Interface which defines the <see cref="ISampleBusiness" /> methods.
/// </summary>
public interface ISampleBusiness
{
    /// <summary>
    ///     Method used to get the sample.
    /// </summary>
    /// <returns>Return a BusinessResult of a <see cref="SampleWeatherView"/> collection.</returns>
    public Task<SampleWeatherView> GetSampleDataAsync();
}
