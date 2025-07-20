namespace Chronique.Des.Monde.Player.Business;

using Chronique.Des.Monde.Common;
using Chronique.Des.Monde.Player.Models;
using Chronique.Des.Mondes.Data;
using Chronique.Des.Mondes.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



public class PlayerCharacterBusiness
{
    private readonly AppDbContext _dbContext;

    public PlayerCharacterBusiness(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<GetAllPlayerCharactersView>  GetAllPlayerCharacterAsync(int userId)
    {
        var playerCharacters = _dbContext.PlayerCharacter.Where(playerCharacter => playerCharacter.UserId == userId)
                .Select(playerCharacter => new GetAllPlayerCharactersView()
                {
                    Id = playerCharacter.Id,
                    Class = playerCharacter.Class,
                    Leveling = playerCharacter.Leveling,
                    Life = playerCharacter.Life,
                    Name = playerCharacter.Name,
                    Picture = playerCharacter.Picture,
                    UserId = playerCharacter.UserId
                }).ToList();

        if(playerCharacters.Count > 0)
            return new();
        

        return playerCharacters;
    }

    public PlayerCharactersView GetPlayerCharacterByPlayerIdAsync(int userId,int playerCharacterId)
    {
        var character = _dbContext.PlayerCharacter.FirstOrDefault(playerCharacter => playerCharacter.UserId == userId || playerCharacter.Id == playerCharacterId);

        if (character is null)
        {
            throw new BusinessException($"playerCharacterId is null{playerCharacterId}" );
        }

        var result = new PlayerCharactersView()
        {
            Id = character.Id,
            Class = character.Class,
            Leveling = character.Leveling,
            Life = character.Life,
            Name = character.Name,
            Picture = character.Picture,
            Background = character.Background,
            ClassArmor = character.ClassArmor,
            Strong = character.Strong,
            Dexterity = character.Dexterity,
            Constitution = character.Constitution,
            Intelligence = character.Intelligence,
            Wisdoms = character.Wisdoms,
            Charism = character.Charism,
            AdditionalStrong = character.AdditionalStrong,
            AdditionalDexterity = character.AdditionalDexterity,
            AdditionalCharism = character.AdditionalCharism,
            AdditionalConstitution = character.AdditionalConstitution,
            AdditionalIntelligence = character.AdditionalIntelligence,
            AdditionalWisdoms = character.AdditionalWisdoms,
        };

        return result;
    }

    public async Task CreatePlayerCharacterRequestAsync(PlayerCharacterRequest playerCharacter, int userId)
    {
        if (playerCharacter is null)
        {
            throw new BusinessException("Le PlayerCharacterRequest est null.");
        }

        var addPlayerCharacter = new PlayerCharacter()
        {
            UserId = userId,
            Name = playerCharacter.Name,
            Leveling = playerCharacter.Leveling,
            Picture = playerCharacter.Picture,
            Background = playerCharacter.Background,
            Class = playerCharacter.Class,
            ClassArmor = playerCharacter.ClassArmor,
            Life = playerCharacter.Life,
            Strong = playerCharacter.Strong,
            Dexterity = playerCharacter.Dexterity,
            Constitution = playerCharacter.Constitution,
            Intelligence = playerCharacter.Intelligence,
            Wisdoms = playerCharacter.Wisdoms,
            Charism = playerCharacter.Charism,
            AdditionalStrong = this.SetAdditionalStats(playerCharacter.Strong),
            AdditionalDexterity = this.SetAdditionalStats(playerCharacter.Dexterity),
            AdditionalConstitution = this.SetAdditionalStats(playerCharacter.Constitution),
            AdditionalIntelligence = this.SetAdditionalStats(playerCharacter.Intelligence),
            AdditionalWisdoms = this.SetAdditionalStats(playerCharacter.Wisdoms),
            AdditionalCharism = this.SetAdditionalStats(playerCharacter.Charism),
        };

        _dbContext.PlayerCharacter.Add(addPlayerCharacter);
        await _dbContext.SaveChangesAsync();
    }

    private int SetAdditionalStats(int stats)
    {
        switch (stats)
        {
            case 1:
                return -5;
            case <= 3:
                return -4;
            case <= 5:
                return -3;
            case <= 7:
                return -2;
            case <= 9:
                return -1;
            case <= 11:
                return 0;
            case <= 13:
                return 1;
            case <= 15:
                return 2;
            case <= 17:
                return 3;
            case <= 19:
                return 4;
        }
        return 5;
    }
}
