namespace Cdm.Business.Dnd.Extensions;

using Cmd.Abstraction;
using Cdm.Business.Dnd.Business;
using Microsoft.Extensions.DependencyInjection;

public static class DndBusinessExtensions
{
    public const string DndKey = "Dnd";
    
    public static IServiceCollection AddDndBusiness(this IServiceCollection services)
    {
        services.AddKeyedTransient<ICharacterBusiness, CharacterDndBusiness>(DndKey);

        return services;
    }
}