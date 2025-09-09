using Cdm.Business.Dnd.Business;
using Cdm.Business.Dnd.Services;
using Cmd.Abstraction.Spells;
// using Cmd.Abstraction.Characters; // Temporairement commenté
using Microsoft.Extensions.DependencyInjection;

namespace Cdm.Business.Dnd.Extensions;

/// <summary>
/// Extensions pour l'enregistrement des services métier D&D dans la DI
/// </summary>
public static class DndBusinessExtensions
{
    public const string DndKey = "DnD";

    public static IServiceCollection AddDndBusinessServices(this IServiceCollection services)
    {
        // Services métier avec clés pour l'injection par GameType
        services.AddKeyedScoped<ISpellBusiness, SpellDndBusiness>(DndKey);
        // services.AddKeyedScoped<ICharacterBusiness, CharacterDndBusiness>(DndKey); // TODO: Corriger l'interface

        // Service d'injection de données officielles
        services.AddDndDataSeeder();

        return services;
    }
}