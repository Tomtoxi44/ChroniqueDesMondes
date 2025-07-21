namespace Chronique.Des.Mondes.Player.Business;

using Chronique.Des.Mondes.Abstraction;
using Chronique.Des.Mondes.Common;
using Chronique.Des.Mondes.Data;
using Chronique.Des.Mondes.Data.Models;
using Chronique.Des.Mondes.Player.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



public class PlayerCharacterBusiness : IPlayerCharacterBusiness
{
    private readonly AppDbContext _dbContext;

    public PlayerCharacterBusiness(AppDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public List<GetAllPlayerCharactersView>  GetAllPlayerCharacter(int userId)
    {
        var playerCharacters = this._dbContext.PlayerCharacter.Where(playerCharacter => playerCharacter.UserId == userId)
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

    public PlayerCharactersView GetPlayerCharacterByPlayerId(int playerCharacterId)
    {
        var character = this._dbContext.PlayerCharacter.FirstOrDefault(playerCharacter => playerCharacter.Id == playerCharacterId);

        if(character is null)
            throw new BusinessException($"The playerCharacterId is not found{playerCharacterId}" );

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
            throw new BusinessException("The PlayerCharacterRequest is null.");

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

        this._dbContext.PlayerCharacter.Add(addPlayerCharacter);
        await this._dbContext.SaveChangesAsync();
    }

    public async Task UpdatePlayerCharacterAsync(PlayerCharacterRequest playerCharacter, int playerCharacterId)
    {
        if(playerCharacter is null)
            throw new BusinessException("The PlayerCharacterRequest is null.");

        var matchingCharacter = this._dbContext.PlayerCharacter.FirstOrDefault(playerCharacter => playerCharacter.Id == playerCharacterId);

        if(matchingCharacter is null)
            throw new BusinessException($"playerCharacterId is not found{playerCharacterId}");


        matchingCharacter.Name = playerCharacter.Name;
        matchingCharacter.Leveling = playerCharacter.Leveling;
        matchingCharacter.Picture = playerCharacter.Picture;
        matchingCharacter.Background = playerCharacter.Background;
        matchingCharacter.Class = playerCharacter.Class;
        matchingCharacter.ClassArmor = playerCharacter.ClassArmor;
        matchingCharacter.Life = playerCharacter.Life;
        matchingCharacter.Strong = playerCharacter.Strong;
        matchingCharacter.Dexterity = playerCharacter.Dexterity;
        matchingCharacter.Constitution = playerCharacter.Constitution;
        matchingCharacter.Intelligence = playerCharacter.Intelligence;
        matchingCharacter.Wisdoms = playerCharacter.Wisdoms;
        matchingCharacter.Charism = playerCharacter.Charism;
        matchingCharacter.AdditionalStrong = this.SetAdditionalStats(playerCharacter.Strong);
        matchingCharacter.AdditionalDexterity = this.SetAdditionalStats(playerCharacter.Dexterity);
        matchingCharacter.AdditionalConstitution = this.SetAdditionalStats(playerCharacter.Constitution);
        matchingCharacter.AdditionalIntelligence = this.SetAdditionalStats(playerCharacter.Intelligence);
        matchingCharacter.AdditionalWisdoms = this.SetAdditionalStats(playerCharacter.Wisdoms);
        matchingCharacter.AdditionalCharism = this.SetAdditionalStats(playerCharacter.Charism);

        await this._dbContext.PlayerCharacter.AddAsync(matchingCharacter);
        await this._dbContext.SaveChangesAsync();
    }

    public async Task DeletedPlayerCharacterAsync(int playerCharacterId)
    {
        var matchingCharacter = this._dbContext.PlayerCharacter.FirstOrDefault(playerCharacter => playerCharacter.Id == playerCharacterId);

        if (matchingCharacter is null)
            throw new BusinessException($"playerCharacterId is not found{playerCharacterId}");

        this._dbContext.PlayerCharacter.Remove(matchingCharacter);
        await this._dbContext.SaveChangesAsync();
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
