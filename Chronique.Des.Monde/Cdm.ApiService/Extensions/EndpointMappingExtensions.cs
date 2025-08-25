using Cdm.ApiService.Endpoints;

namespace Cdm.ApiService.Extensions;

public static class EndpointMappingExtensions
{
    public static void MapAllEndpoints(this WebApplication app)
    {
        // Existing endpoints
        app.MapWeatherEndpoints();
        app.MapCharacterEndpoints();
        app.MapContentBlockEndpoints();
        app.MapChapterEndpoints();
        app.MapNpcEndpoints();
        app.MapInvitationEndpoints();
    }
}