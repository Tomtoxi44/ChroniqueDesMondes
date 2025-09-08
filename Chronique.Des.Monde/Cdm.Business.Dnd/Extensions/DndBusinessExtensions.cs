namespace Cdm.Business.Dnd.Extensions;

using Cmd.Abstraction;
using Cmd.Abstraction.Spells;
using Cmd.Abstraction.Equipment;
using Cdm.Business.Dnd.Business;
using Microsoft.Extensions.DependencyInjection;
using Cdm.Business.Dnd.Services;
using Cmd.Abstraction.Characters;

public static class DndBusinessExtensions
{
    public const string DndKey = "Dnd";
    
    public static IServiceCollection AddDndBusiness(this IServiceCollection services)
    {
        // Services de personnages D&D
        services.AddKeyedTransient<ICharacterBusiness, CharacterDndBusiness>(DndKey);

        // Services de sorts D&D
        services.AddKeyedTransient<ISpellBusiness, SpellDndBusiness>(DndKey);

        // Services d'équipements D&D
        services.AddKeyedTransient<IEquipmentBusiness, EquipmentDndBusiness>(DndKey);

        // Service d'injection de données officielles
        services.AddDndDataSeeder();

        return services;
    }
}