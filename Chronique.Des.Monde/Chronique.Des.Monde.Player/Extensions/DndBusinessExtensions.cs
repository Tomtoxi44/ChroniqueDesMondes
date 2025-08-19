namespace Cmd.Business.Character.Extensions;

using Cmd.Abstraction;
using Cmd.Business.Character.Business;
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
