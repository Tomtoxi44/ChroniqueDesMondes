using Cdm.ApiService.Endpoints;

namespace Cdm.ApiService.Extensions;

/// <summary>
/// Extensions pour l'enregistrement de tous les endpoints de l'API
/// </summary>
public static class EndpointMappingExtensions
{
    public static WebApplication MapAllEndpoints(this WebApplication app)
    {
        // Endpoints principaux
        app.MapSpellEndpoints();
        app.MapCharacterSpellEndpoints();
        app.MapEquipmentExchangeEndpoints();
        
        // Endpoints de calculs D&D
        app.MapDndCalculatorEndpoints();
        
        // Endpoints d'administration D&D (NOUVEAU)
        app.MapDndAdminEndpoints();

        return app;
    }
}