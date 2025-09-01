using Cdm.ApiService.Endpoints;

namespace Cdm.ApiService.Extensions;

public static class EndpointMappingExtensions
{
    public static WebApplication MapAllEndpoints(this WebApplication app)
    {
        // Mapping des endpoints existants
        app.MapWeatherEndpoints();
        app.MapCharacterEndpoints();
        app.MapInvitationEndpoints();
        app.MapContentBlockEndpoints();
        app.MapChapterEndpoints();
        app.MapNpcEndpoints();

        // Mapping des endpoints de sorts (API générique avec redispatch)
        app.MapSpellEndpoints();

        // Mapping des endpoints d'équipements (API générique avec redispatch)
        app.MapEquipmentEndpoints();
        app.MapEquipmentExchangeEndpoints();

        return app;
    }
}