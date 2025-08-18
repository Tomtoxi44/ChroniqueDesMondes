namespace Cmd.Business.Character.Business;

using Cdm.Data.Dnd;
using Cdm.Data.Dnd.Models;
using Abstraction;
using Cdm.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cmd.Abstraction.ModelsRequest;
using Cmd.Abstraction.ModelsView;
using Cmd.Business.Character.ModelsView;
using Microsoft.EntityFrameworkCore;

public class CharacterDndBusiness : ICharacterBusiness
{
    private readonly DndDbContext _dbContext;

    public CharacterDndBusiness(DndDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region Méthodes génériques (interface ICharacterBusiness)

    public async Task<IEnumerable<ICharacterView>> GetAllCharactersByUserId(int userId)
    {
        var characters = await _dbContext.Set<CharacterDnd>()
            .Where(c => c.UserId == userId)
            .ToListAsync();

        return characters.Select(ConvertToCharacterView);
    }

    public async Task<ICharacterView> GetCharacterById(int characterId)
    {
        var character = await _dbContext.Set<CharacterDnd>()
            .FirstOrDefaultAsync(c => c.Id == characterId);

        if (character is null)
            throw new BusinessException($"Character with ID {characterId} not found.");

        return ConvertToCharacterView(character);
    }

    public async Task<ICharacterView> CreateCharacter(CharacterRequest characterRequest, int userId)
    {
        if (characterRequest is null)
            throw new BusinessException("The character request is null.");

        var newCharacterDnd = MapCharacterRequestToEntity(characterRequest, userId);
        CalculateAdditionalStats(newCharacterDnd);

        _dbContext.Set<CharacterDnd>().Add(newCharacterDnd);
        await _dbContext.SaveChangesAsync();

        return ConvertToCharacterView(newCharacterDnd);
    }

    public async Task<ICharacterView> UpdateCharacter(CharacterRequest characterRequest, int characterId)
    {
        if (characterRequest is null)
            throw new BusinessException("The character request is null.");

        var existingCharacter = await _dbContext.Set<CharacterDnd>()
            .FirstOrDefaultAsync(c => c.Id == characterId);

        if (existingCharacter is null)
            throw new BusinessException($"Character with ID {characterId} not found.");

        // Mettre à jour les propriétés depuis le CharacterRequest générique
        existingCharacter.Name = characterRequest.Name ?? existingCharacter.Name;
        existingCharacter.Leveling = characterRequest.Leveling;
        existingCharacter.Picture = characterRequest.Picture ?? existingCharacter.Picture;
        existingCharacter.Background = characterRequest.Background ?? existingCharacter.Background;
        existingCharacter.Life = characterRequest.Life;

        // Mapper les compétences spécifiques D&D
        existingCharacter.Class = GetStringValueFromDictionary(characterRequest.Competences, nameof(CharacterDnd.Class));
        existingCharacter.ClassArmor = GetIntValueFromDictionary(characterRequest.Competences, nameof(CharacterDnd.ClassArmor));
        existingCharacter.Strong = GetIntValueFromDictionary(characterRequest.Competences, nameof(CharacterDnd.Strong));
        existingCharacter.Dexterity = GetIntValueFromDictionary(characterRequest.Competences, nameof(CharacterDnd.Dexterity));
        existingCharacter.Constitution = GetIntValueFromDictionary(characterRequest.Competences, nameof(CharacterDnd.Constitution));
        existingCharacter.Intelligence = GetIntValueFromDictionary(characterRequest.Competences, nameof(CharacterDnd.Intelligence));
        existingCharacter.Wisdoms = GetIntValueFromDictionary(characterRequest.Competences, nameof(CharacterDnd.Wisdoms));
        existingCharacter.Charism = GetIntValueFromDictionary(characterRequest.Competences, nameof(CharacterDnd.Charism));

        CalculateAdditionalStats(existingCharacter);

        await _dbContext.SaveChangesAsync();
        return ConvertToCharacterView(existingCharacter);
    }

    public async Task DeleteCharacter(int characterId)
    {
        var character = await _dbContext.Set<CharacterDnd>()
            .FirstOrDefaultAsync(c => c.Id == characterId);

        if (character is null)
            throw new BusinessException($"Character with ID {characterId} not found.");

        _dbContext.Set<CharacterDnd>().Remove(character);
        await _dbContext.SaveChangesAsync();
    }

    #endregion

    #region Méthodes utilitaires privées

    private CharacterDnd MapCharacterRequestToEntity(CharacterRequest characterRequest, int userId)
    {
        return new CharacterDnd
        {
            UserId = userId,
            Name = characterRequest.Name ?? string.Empty,
            Leveling = characterRequest.Leveling,
            Picture = characterRequest.Picture ?? string.Empty,
            Background = characterRequest.Background,
            Life = characterRequest.Life,
            Class = GetStringValueFromDictionary(characterRequest.Competences, nameof(CharacterDnd.Class)),
            ClassArmor = GetIntValueFromDictionary(characterRequest.Competences, nameof(CharacterDnd.ClassArmor)),
            Strong = GetIntValueFromDictionary(characterRequest.Competences, nameof(CharacterDnd.Strong)),
            Dexterity = GetIntValueFromDictionary(characterRequest.Competences, nameof(CharacterDnd.Dexterity)),
            Constitution = GetIntValueFromDictionary(characterRequest.Competences, nameof(CharacterDnd.Constitution)),
            Intelligence = GetIntValueFromDictionary(characterRequest.Competences, nameof(CharacterDnd.Intelligence)),
            Wisdoms = GetIntValueFromDictionary(characterRequest.Competences, nameof(CharacterDnd.Wisdoms)),
            Charism = GetIntValueFromDictionary(characterRequest.Competences, nameof(CharacterDnd.Charism))
        };
    }

    private void CalculateAdditionalStats(CharacterDnd character)
    {
        character.AdditionalStrong = SetAdditionalStats(character.Strong);
        character.AdditionalDexterity = SetAdditionalStats(character.Dexterity);
        character.AdditionalConstitution = SetAdditionalStats(character.Constitution);
        character.AdditionalIntelligence = SetAdditionalStats(character.Intelligence);
        character.AdditionalWisdoms = SetAdditionalStats(character.Wisdoms);
        character.AdditionalCharism = SetAdditionalStats(character.Charism);
    }

    private CharacterDndView ConvertToCharacterView(CharacterDnd character)
    {
        return new CharacterDndView
        {
            Id = character.Id,
            Name = character.Name,
            Leveling = character.Leveling,
            Picture = character.Picture,
            Background = character.Background,
            Life = character.Life,
            Class = character.Class,
            ClassArmor = character.ClassArmor,
            Strong = character.Strong,
            Dexterity = character.Dexterity,
            Constitution = character.Constitution,
            Intelligence = character.Intelligence,
            Wisdoms = character.Wisdoms,
            Charism = character.Charism,
            AdditionalStrong = character.AdditionalStrong,
            AdditionalDexterity = character.AdditionalDexterity,
            AdditionalConstitution = character.AdditionalConstitution,
            AdditionalIntelligence = character.AdditionalIntelligence,
            AdditionalWisdoms = character.AdditionalWisdoms,
            AdditionalCharism = character.AdditionalCharism
        };
    }

    private int GetIntValueFromDictionary(IReadOnlyDictionary<string, object> dictionary, string key)
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            return value switch
            {
                int intValue => intValue,
                string stringValue when int.TryParse(stringValue, out var parsedValue) => parsedValue,
                _ => 0
            };
        }
        return 0;
    }

    private string GetStringValueFromDictionary(IReadOnlyDictionary<string, object> dictionary, string key)
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            return value?.ToString() ?? string.Empty;
        }
        return string.Empty;
    }

    private int SetAdditionalStats(int stats)
    {
        return stats switch
        {
            1 => -5,
            <= 3 => -4,
            <= 5 => -3,
            <= 7 => -2,
            <= 9 => -1,
            <= 11 => 0,
            <= 13 => 1,
            <= 15 => 2,
            <= 17 => 3,
            <= 19 => 4,
            _ => 5
        };
    }

    #endregion
}
