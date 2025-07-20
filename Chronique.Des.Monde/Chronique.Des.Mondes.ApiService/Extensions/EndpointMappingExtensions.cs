using Microsoft.AspNetCore.Builder;

public static class EndpointMappingExtensions
{
    public static void MapApplicationEndpoints(this WebApplication app)
    {
        app.MapWeatherEndpoints();
        app.MapUserEndpoints();

    }
}