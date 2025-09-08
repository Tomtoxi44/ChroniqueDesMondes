using Microsoft.AspNetCore.Mvc;
using Cdm.Data.Dnd.Models.Extensions;
using Cdm.Data.Dnd.Models;
using Cdm.Common.DnD;
using System.Security.Claims;

namespace Cdm.ApiService.Endpoints;

/// <summary>
/// Endpoints pour les calculs automatiques D&D (modificateurs, bonus d'attaque, DD de sauvegarde)
/// </summary>
public static class DndCalculatorEndpoints
{
    public static void MapDndCalculatorEndpoints(this WebApplication app)
    {
        var dndGroup = app.MapGroup("/api/dnd").RequireAuthorization();

        // GET /api/dnd/character/{id}/spellcasting-stats - Statistiques de lancement de sorts
        dndGroup.MapGet("/character/{characterId:int}/spellcasting-stats", async (
            int characterId,
            IServiceProvider serviceProvider,
            ClaimsPrincipal user) =>
        {
            try
            {
                // TODO: Récupérer le personnage depuis la base de données
                // Pour l'instant, exemple avec un personnage fictif
                var character = new CharacterDnd
                {
                    Id = characterId,
                    Name = "Gandalf le Gris",
                    Class = "magicien",
                    Leveling = 5,
                    Intelligence = 18,
                    AdditionalIntelligence = 0,
                    Wisdoms = 14,
                    AdditionalWisdoms = 0,
                    Charism = 12,
                    AdditionalCharism = 0
                };

                if (!character.CanCastSpells())
                {
                    return Results.BadRequest(new { error = $"La classe '{character.Class}' ne peut pas lancer de sorts" });
                }

                var stats = character.GetSpellcastingStats();
                
                return Results.Ok(new
                {
                    character = new
                    {
                        character.Id,
                        character.Name,
                        character.Class,
                        Level = character.Leveling
                    },
                    spellcasting = new
                    {
                        ability = stats.SpellcastingAbility.ToString(),
                        abilityScore = character.GetSpellcastingAbilityScore(),
                        abilityModifier = stats.AbilityModifier,
                        proficiencyBonus = stats.ProficiencyBonus,
                        spellAttackBonus = stats.SpellAttackBonus,
                        spellSaveDC = stats.SpellSaveDC,
                        maxSpellLevel = stats.MaxSpellLevel
                    },
                    calculations = new
                    {
                        formula_attack = $"1d20 + {stats.SpellAttackBonus} (mod {stats.SpellcastingAbility} + maîtrise)",
                        formula_dc = $"8 + {stats.AbilityModifier} + {stats.ProficiencyBonus} = {stats.SpellSaveDC}",
                        example_spell = $"Projectile Magique : Attaque automatique, 1d4+1 dégâts par projectile"
                    }
                });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // GET /api/dnd/character/{id}/all-stats - Toutes les statistiques calculées
        dndGroup.MapGet("/character/{characterId:int}/all-stats", async (
            int characterId,
            IServiceProvider serviceProvider,
            ClaimsPrincipal user) =>
        {
            try
            {
                // Exemple avec un personnage D&D complet
                var character = new CharacterDnd
                {
                    Id = characterId,
                    Name = "Aragorn",
                    Class = "rôdeur",
                    Leveling = 8,
                    Strong = 16,
                    AdditionalStrong = 0,
                    Dexterity = 18,
                    AdditionalDexterity = 0,
                    Constitution = 14,
                    AdditionalConstitution = 0,
                    Intelligence = 12,
                    AdditionalIntelligence = 0,
                    Wisdoms = 16,
                    AdditionalWisdoms = 0,
                    Charism = 10,
                    AdditionalCharism = 0,
                    ClassArmor = 2 // Armure de cuir clouté
                };

                var response = new
                {
                    character = new
                    {
                        character.Id,
                        character.Name,
                        character.Class,
                        Level = character.Leveling
                    },
                    abilities = new
                    {
                        strength = new
                        {
                            total = character.TotalStrength(),
                            modifier = character.StrengthModifier(),
                            display = $"{character.TotalStrength()} ({character.StrengthModifier():+0;-0;+0})"
                        },
                        dexterity = new
                        {
                            total = character.TotalDexterity(),
                            modifier = character.DexterityModifier(),
                            display = $"{character.TotalDexterity()} ({character.DexterityModifier():+0;-0;+0})"
                        },
                        constitution = new
                        {
                            total = character.TotalConstitution(),
                            modifier = character.ConstitutionModifier(),
                            display = $"{character.TotalConstitution()} ({character.ConstitutionModifier():+0;-0;+0})"
                        },
                        intelligence = new
                        {
                            total = character.TotalIntelligence(),
                            modifier = character.IntelligenceModifier(),
                            display = $"{character.TotalIntelligence()} ({character.IntelligenceModifier():+0;-0;+0})"
                        },
                        wisdom = new
                        {
                            total = character.TotalWisdom(),
                            modifier = character.WisdomModifier(),
                            display = $"{character.TotalWisdom()} ({character.WisdomModifier():+0;-0;+0})"
                        },
                        charisma = new
                        {
                            total = character.TotalCharisma(),
                            modifier = character.CharismaModifier(),
                            display = $"{character.TotalCharisma()} ({character.CharismaModifier():+0;-0;+0})"
                        }
                    },
                    combat = new
                    {
                        armorClass = character.CalculateArmorClass(),
                        hitPoints = character.CalculateHitPoints(),
                        proficiencyBonus = character.ProficiencyBonus()
                    },
                    spellcasting = character.CanCastSpells() ? (object)new
                    {
                        canCast = true,
                        stats = character.GetSpellcastingStats(),
                        spellcastingAbilityScore = character.GetSpellcastingAbilityScore()
                    } : new { canCast = false }
                };

                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // POST /api/dnd/calculate/spell-attack - Calculateur de bonus d'attaque de sort
        dndGroup.MapPost("/calculate/spell-attack", ([FromBody] SpellAttackCalculationRequest request) =>
        {
            try
            {
                var spellcastingAbility = DndSpellcastingRules.GetSpellcastingAbility(request.CharacterClass);
                var proficiencyBonus = DndSpellcastingRules.CalculateProficiencyBonus(request.CharacterLevel);
                var abilityModifier = DndSpellcastingRules.CalculateAbilityModifier(request.AbilityScore);
                var spellAttackBonus = DndSpellcastingRules.CalculateSpellAttackBonus(request.AbilityScore, proficiencyBonus);
                var spellSaveDC = DndSpellcastingRules.CalculateSpellSaveDC(request.AbilityScore, proficiencyBonus);

                return Results.Ok(new
                {
                    input = request,
                    results = new
                    {
                        spellcastingAbility = spellcastingAbility.ToString(),
                        abilityModifier,
                        proficiencyBonus,
                        spellAttackBonus,
                        spellSaveDC
                    },
                    examples = new
                    {
                        attackRoll = $"1d20 + {spellAttackBonus}",
                        saveFormula = $"8 + {abilityModifier} + {proficiencyBonus} = {spellSaveDC}",
                        practical = $"Pour toucher CA 15, il faut faire {15 - spellAttackBonus}+ sur le d20"
                    }
                });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });
    }
}

/// <summary>
/// Modèle de requête pour calculer les bonus d'attaque de sort
/// </summary>
public record SpellAttackCalculationRequest
{
    public string CharacterClass { get; init; } = string.Empty;
    public int CharacterLevel { get; init; }
    public int AbilityScore { get; init; }
}