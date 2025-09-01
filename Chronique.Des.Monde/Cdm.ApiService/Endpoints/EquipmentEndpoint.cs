using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Cmd.Abstraction.Equipment;
using Cdm.Business.Dnd.Extensions;
using Cdm.Business.Dnd.Models.Equipment;
using Cdm.Common.Enums;
using System.Security.Claims;

namespace Cdm.ApiService.Endpoints;

/// <summary>
/// Endpoints pour la gestion des équipements - API générique qui redispatch selon le GameType
/// Suit exactement le pattern SpellEndpoint avec injection par clé
/// </summary>
public static class EquipmentEndpoint
{
    public static void MapEquipmentEndpoints(this WebApplication app)
    {
        var equipmentGroup = app.MapGroup("/api/equipment").RequireAuthorization();

        // GET /api/equipment?userId={id}&gameType={type} - Liste des équipements d'un utilisateur
        equipmentGroup.MapGet("/", async (
            int userId, 
            GameType gameType,
            [FromKeyedServices(DndBusinessExtensions.DndKey)] IEquipmentBusiness equipmentBusiness,
            ClaimsPrincipal user) =>
        {
            try
            {
                var requestingUserId = GetUserIdFromClaims(user);
                
                // Validation : un utilisateur ne peut voir que ses propres équipements
                if (requestingUserId != userId)
                {
                    return Results.Forbid();
                }

                var equipment = await equipmentBusiness.GetAllEquipmentsByUserId(userId, gameType);
                return Results.Ok(equipment);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // GET /api/equipment/{id}?userId={userId} - Détails d'un équipement
        equipmentGroup.MapGet("/{id:int}", async (
            int id, 
            int userId,
            [FromKeyedServices(DndBusinessExtensions.DndKey)] IEquipmentBusiness equipmentBusiness,
            ClaimsPrincipal user) =>
        {
            try
            {
                var requestingUserId = GetUserIdFromClaims(user);
                
                if (requestingUserId != userId)
                {
                    return Results.Forbid();
                }

                var equipment = await equipmentBusiness.GetEquipmentById(id, userId);
                return Results.Ok(equipment);
            }
            catch (InvalidOperationException)
            {
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // POST /api/equipment?userId={id} - Création d'un équipement générique
        equipmentGroup.MapPost("/", async (
            int userId, 
            [FromBody] EquipmentRequest request, 
            [FromKeyedServices(DndBusinessExtensions.DndKey)] IEquipmentBusiness equipmentBusiness,
            ClaimsPrincipal user) =>
        {
            try
            {
                var requestingUserId = GetUserIdFromClaims(user);
                
                if (requestingUserId != userId)
                {
                    return Results.Forbid();
                }

                var equipment = await equipmentBusiness.CreateEquipment(request, userId);
                return Results.Created($"/api/equipment/{equipment.Id}", equipment);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // POST /api/equipment/dnd?userId={id} - Création D&D avec propriétés spécialisées
        equipmentGroup.MapPost("/dnd", async (
            int userId, 
            [FromBody] EquipmentDndRequest request, 
            [FromKeyedServices(DndBusinessExtensions.DndKey)] IEquipmentBusiness equipmentBusiness,
            ClaimsPrincipal user) =>
        {
            try
            {
                var requestingUserId = GetUserIdFromClaims(user);
                
                if (requestingUserId != userId)
                {
                    return Results.Forbid();
                }

                // Convertir EquipmentDndRequest en EquipmentRequest générique avec propriétés spécialisées
                var equipmentRequest = new EquipmentRequest
                {
                    Name = request.Name,
                    Description = request.Description,
                    ImageUrl = request.ImageUrl,
                    GameType = GameType.DnD,
                    Tags = request.Tags,
                    Category = request.Category,
                    Weight = request.Weight,
                    Value = request.Value,
                    SpecializedProperties = new Dictionary<string, object>
                    {
                        { "WeaponType", request.WeaponType },
                        { "Damage", request.Damage },
                        { "DamageType", request.DamageType },
                        { "AttackBonus", request.AttackBonus },
                        { "Properties", request.Properties },
                        { "ArmorClass", request.ArmorClass },
                        { "MaxDexBonus", request.MaxDexBonus },
                        { "StealthDisadvantage", request.StealthDisadvantage },
                        { "StrengthRequirement", request.StrengthRequirement },
                        { "IsWeapon", request.IsWeapon },
                        { "IsArmor", request.IsArmor },
                        { "IsShield", request.IsShield },
                        { "IsMagical", request.IsMagical },
                        { "RequiresAttunement", request.RequiresAttunement },
                        { "Rarity", request.Rarity },
                        { "MagicalProperties", request.MagicalProperties ?? "" },
                        { "IsConsumable", request.IsConsumable },
                        { "Effect", request.Effect ?? "" }
                    }
                };

                if (request.Charges.HasValue)
                {
                    equipmentRequest.SpecializedProperties["Charges"] = request.Charges.Value;
                }

                var equipment = await equipmentBusiness.CreateEquipment(equipmentRequest, userId);
                return Results.Created($"/api/equipment/{equipment.Id}", equipment);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // PUT /api/equipment/{id} - Modification d'un équipement
        equipmentGroup.MapPut("/{id:int}", async (
            int id, 
            [FromBody] EquipmentRequest request, 
            [FromKeyedServices(DndBusinessExtensions.DndKey)] IEquipmentBusiness equipmentBusiness,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                var equipment = await equipmentBusiness.UpdateEquipment(request, id, userId);
                return Results.Ok(equipment);
            }
            catch (InvalidOperationException)
            {
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // DELETE /api/equipment/{id} - Suppression d'un équipement
        equipmentGroup.MapDelete("/{id:int}", async (
            int id, 
            [FromKeyedServices(DndBusinessExtensions.DndKey)] IEquipmentBusiness equipmentBusiness,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                await equipmentBusiness.DeleteEquipment(id, userId);
                return Results.NoContent();
            }
            catch (InvalidOperationException)
            {
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // GET /api/equipment/search?q={text}&userId={id}&gameType={type} - Recherche d'équipements
        equipmentGroup.MapGet("/search", async (
            string q,
            int userId,
            GameType gameType,
            [FromKeyedServices(DndBusinessExtensions.DndKey)] IEquipmentBusiness equipmentBusiness,
            ClaimsPrincipal user) =>
        {
            try
            {
                var requestingUserId = GetUserIdFromClaims(user);
                
                if (requestingUserId != userId)
                {
                    return Results.Forbid();
                }

                var equipment = await equipmentBusiness.SearchEquipments(q, userId, gameType);
                return Results.Ok(equipment);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // GET /api/equipment/category/{category}?userId={id}&gameType={type} - Équipements par catégorie
        equipmentGroup.MapGet("/category/{category}", async (
            string category,
            int userId,
            GameType gameType,
            [FromKeyedServices(DndBusinessExtensions.DndKey)] IEquipmentBusiness equipmentBusiness,
            ClaimsPrincipal user) =>
        {
            try
            {
                var requestingUserId = GetUserIdFromClaims(user);
                
                if (requestingUserId != userId)
                {
                    return Results.Forbid();
                }

                var equipment = await equipmentBusiness.GetEquipmentsByCategory(category, userId, gameType);
                return Results.Ok(equipment);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });
    }

    private static int GetUserIdFromClaims(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        throw new UnauthorizedAccessException("Invalid user token");
    }
}