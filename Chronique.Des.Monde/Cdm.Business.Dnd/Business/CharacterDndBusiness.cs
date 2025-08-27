namespace Cdm.Business.Dnd.Business;

using Cmd.Abstraction;
using Cmd.Abstraction.ModelsRequest;
using Cmd.Abstraction.ModelsView;
using Cdm.Business.Dnd.ModelsView;
using Cdm.Data.Dnd;
using Cdm.Data.Dnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class CharacterDndBusiness : ICharacterBusiness
{
    private readonly DndDbContext context;
    private readonly ILogger<CharacterDndBusiness> logger;

    public CharacterDndBusiness(DndDbContext context, ILogger<CharacterDndBusiness> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    public async Task<IEnumerable<ICharacterView>> GetAllCharactersByUserId(int userId)
    {
        this.logger.LogInformation("Getting all D&D characters for user {UserId}", userId);

        var characters = await this.context.CharactersDnd
            .Where(c => c.UserId == userId && !c.IsNpc)
            .ToListAsync();

        return characters.Select(this.MapToView).ToList();
    }

    public async Task<ICharacterView> GetCharacterById(int characterId)
    {
        this.logger.LogInformation("Getting D&D character {CharacterId}", characterId);

        var character = await this.context.CharactersDnd
            .FirstOrDefaultAsync(c => c.Id == characterId && !c.IsNpc);

        if (character == null)
        {
            throw new InvalidOperationException($"Character with ID {characterId} not found");
        }

        return this.MapToView(character);
    }

    public async Task<ICharacterView> CreateCharacter(CharacterRequest character, int userId)
    {
        this.logger.LogInformation("Creating D&D character {Name} for user {UserId}", character.Name, userId);

        var dndCharacter = new CharacterDnd
        {
            Name = character.Name ?? "Unknown",
            Background = character.Background,
            Picture = character.Picture ?? "",
            Life = character.Life,
            Leveling = character.Leveling,
            UserId = userId,
            IsNpc = false,
            CreatedAt = DateTime.UtcNow
        };

        // Extraire les compétences D&D depuis le dictionnaire
        if (character.Competences.TryGetValue("Class", out var classValue))
            dndCharacter.Class = classValue.ToString() ?? "";

        if (character.Competences.TryGetValue("ClassArmor", out var armorValue) && int.TryParse(armorValue.ToString(), out var armor))
            dndCharacter.ClassArmor = armor;

        if (character.Competences.TryGetValue("Strong", out var strongValue) && int.TryParse(strongValue.ToString(), out var strong))
            dndCharacter.Strong = strong;

        if (character.Competences.TryGetValue("Dexterity", out var dexValue) && int.TryParse(dexValue.ToString(), out var dex))
            dndCharacter.Dexterity = dex;

        if (character.Competences.TryGetValue("Constitution", out var conValue) && int.TryParse(conValue.ToString(), out var con))
            dndCharacter.Constitution = con;

        if (character.Competences.TryGetValue("Intelligence", out var intValue) && int.TryParse(intValue.ToString(), out var intel))
            dndCharacter.Intelligence = intel;

        if (character.Competences.TryGetValue("Wisdoms", out var wisValue) && int.TryParse(wisValue.ToString(), out var wis))
            dndCharacter.Wisdoms = wis;

        if (character.Competences.TryGetValue("Charism", out var chaValue) && int.TryParse(chaValue.ToString(), out var cha))
            dndCharacter.Charism = cha;

        this.context.CharactersDnd.Add(dndCharacter);
        await this.context.SaveChangesAsync();

        this.logger.LogInformation("D&D character {Name} created with ID {Id}", dndCharacter.Name, dndCharacter.Id);

        return this.MapToView(dndCharacter);
    }

    public async Task<ICharacterView> UpdateCharacter(CharacterRequest character, int characterId)
    {
        this.logger.LogInformation("Updating D&D character {CharacterId}", characterId);

        var existingCharacter = await this.context.CharactersDnd
            .FirstOrDefaultAsync(c => c.Id == characterId && !c.IsNpc);

        if (existingCharacter == null)
        {
            throw new InvalidOperationException($"Character with ID {characterId} not found");
        }

        // Mettre à jour les propriétés de base
        existingCharacter.Name = character.Name ?? existingCharacter.Name;
        existingCharacter.Background = character.Background ?? existingCharacter.Background;
        existingCharacter.Picture = character.Picture ?? existingCharacter.Picture;
        existingCharacter.Life = character.Life;
        existingCharacter.Leveling = character.Leveling;

        // Mettre à jour les compétences D&D
        if (character.Competences.TryGetValue("Class", out var classValue))
            existingCharacter.Class = classValue.ToString() ?? existingCharacter.Class;

        if (character.Competences.TryGetValue("ClassArmor", out var armorValue) && int.TryParse(armorValue.ToString(), out var armor))
            existingCharacter.ClassArmor = armor;

        if (character.Competences.TryGetValue("Strong", out var strongValue) && int.TryParse(strongValue.ToString(), out var strong))
            existingCharacter.Strong = strong;

        if (character.Competences.TryGetValue("Dexterity", out var dexValue) && int.TryParse(dexValue.ToString(), out var dex))
            existingCharacter.Dexterity = dex;

        if (character.Competences.TryGetValue("Constitution", out var conValue) && int.TryParse(conValue.ToString(), out var con))
            existingCharacter.Constitution = con;

        if (character.Competences.TryGetValue("Intelligence", out var intValue) && int.TryParse(intValue.ToString(), out var intel))
            existingCharacter.Intelligence = intel;

        if (character.Competences.TryGetValue("Wisdoms", out var wisValue) && int.TryParse(wisValue.ToString(), out var wis))
            existingCharacter.Wisdoms = wis;

        if (character.Competences.TryGetValue("Charism", out var chaValue) && int.TryParse(chaValue.ToString(), out var cha))
            existingCharacter.Charism = cha;

        await this.context.SaveChangesAsync();

        this.logger.LogInformation("D&D character {CharacterId} updated successfully", characterId);

        return this.MapToView(existingCharacter);
    }

    public async Task DeleteCharacter(int characterId)
    {
        this.logger.LogInformation("Deleting D&D character {CharacterId}", characterId);

        var character = await this.context.CharactersDnd
            .FirstOrDefaultAsync(c => c.Id == characterId && !c.IsNpc);

        if (character == null)
        {
            throw new InvalidOperationException($"Character with ID {characterId} not found");
        }

        this.context.CharactersDnd.Remove(character);
        await this.context.SaveChangesAsync();

        this.logger.LogInformation("D&D character {CharacterId} deleted successfully", characterId);
    }

    private CharacterDndView MapToView(CharacterDnd character)
    {
        return new CharacterDndView
        {
            Id = character.Id,
            Name = character.Name,
            Background = character.Background,
            Picture = character.Picture,
            Life = character.Life,
            Leveling = character.Leveling,
            Class = character.Class,
            ClassArmor = character.ClassArmor,
            Strong = character.Strong,
            AdditionalStrong = character.AdditionalStrong,
            Dexterity = character.Dexterity,
            AdditionalDexterity = character.AdditionalDexterity,
            Constitution = character.Constitution,
            AdditionalConstitution = character.AdditionalConstitution,
            Intelligence = character.Intelligence,
            AdditionalIntelligence = character.AdditionalIntelligence,
            Wisdoms = character.Wisdoms,
            AdditionalWisdoms = character.AdditionalWisdoms,
            Charism = character.Charism,
            AdditionalCharism = character.AdditionalCharism
        };
    }
}