using Cdm.ApiService.Endpoints;

namespace Cdm.ApiService.Extensions;

public static class EndpointMappingExtensions
{
    public static void MapApplicationEndpoints(this WebApplication app)
    {
        app.MapCharacterEndpoints();
        app.MapInvitationEndpoints();
    }
}