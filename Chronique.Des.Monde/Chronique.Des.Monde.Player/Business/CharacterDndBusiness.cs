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
using Cmd.Business.Character.ModelsRequest;

public class CharacterDndBusiness : ICharacterBusiness
{
    private readonly DndDbContext _dbContext;

    public CharacterDndBusiness(DndDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public List<GetAllCharacterDndRequestView>  GetAllCharacterDnd(int userId)
    {
        var characterDnds = this._dbContext.Set<CharacterDnd>().Where(characterDnd => characterDnd.UserId == userId)
                .Select(characterDnd => new GetAllCharacterDndRequestView()
                {
                    Id = characterDnd.Id,
                    Class = characterDnd.Class,
                    Leveling = characterDnd.Leveling,
                    Life = characterDnd.Life,
                    Name = characterDnd.Name,
                    Picture = characterDnd.Picture,
                    UserId = characterDnd.UserId
                }).ToList();

        if(characterDnds.Count > 0)
            return new();
        

        return characterDnds;
    }

    public CharacterDndView GetCharacterDndByPlayerId(int characterDndId)
    {
        var character = this._dbContext.Set<CharacterDnd>().FirstOrDefault(characterDnd => characterDnd.Id == characterDndId);

        if(character is null)
            throw new BusinessException($"The characterDndId is not found{characterDndId}" );

        var result = new CharacterDndView()
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

    public async Task CreateCharacterDndRequestAsync(CharacterDndRequest characterDndDnd, int userId)
    {
        if (characterDndDnd is null)
            throw new BusinessException("The characterDndRequest is null.");

        var addCharacterDnd = new CharacterDnd()
        {
            UserId = userId,
            Name = characterDndDnd.Name,
            Leveling = characterDndDnd.Leveling,
            Picture = characterDndDnd.Picture,
            Background = characterDndDnd.Background,
            Class = characterDndDnd.Class,
            ClassArmor = characterDndDnd.ClassArmor,
            Life = characterDndDnd.Life,
            Strong = characterDndDnd.Strong,
            Dexterity = characterDndDnd.Dexterity,
            Constitution = characterDndDnd.Constitution,
            Intelligence = characterDndDnd.Intelligence,
            Wisdoms = characterDndDnd.Wisdoms,
            Charism = characterDndDnd.Charism,
            AdditionalStrong = this.SetAdditionalStats(characterDndDnd.Strong),
            AdditionalDexterity = this.SetAdditionalStats(characterDndDnd.Dexterity),
            AdditionalConstitution = this.SetAdditionalStats(characterDndDnd.Constitution),
            AdditionalIntelligence = this.SetAdditionalStats(characterDndDnd.Intelligence),
            AdditionalWisdoms = this.SetAdditionalStats(characterDndDnd.Wisdoms),
            AdditionalCharism = this.SetAdditionalStats(characterDndDnd.Charism),
        };

        this._dbContext.Set<CharacterDnd>().Add(addCharacterDnd);
        await this._dbContext.SaveChangesAsync();
    }

    public async Task UpdateCharacterDndAsync(CharacterDndRequest characterDndDnd, int characterDndId)
    {
        if(characterDndDnd is null)
            throw new BusinessException("The characterDndRequest is null.");

        var matchingCharacter = this._dbContext.Set<CharacterDnd>().FirstOrDefault(cd=> cd.Id == characterDndId);

        if(matchingCharacter is null)
            throw new BusinessException($"characterDndId is not found{characterDndId}");


        matchingCharacter.Name = characterDndDnd.Name;
        matchingCharacter.Leveling = characterDndDnd.Leveling;
        matchingCharacter.Picture = characterDndDnd.Picture;
        matchingCharacter.Background = characterDndDnd.Background;
        matchingCharacter.Class = characterDndDnd.Class;
        matchingCharacter.ClassArmor = characterDndDnd.ClassArmor;
        matchingCharacter.Life = characterDndDnd.Life;
        matchingCharacter.Strong = characterDndDnd.Strong;
        matchingCharacter.Dexterity = characterDndDnd.Dexterity;
        matchingCharacter.Constitution = characterDndDnd.Constitution;
        matchingCharacter.Intelligence = characterDndDnd.Intelligence;
        matchingCharacter.Wisdoms = characterDndDnd.Wisdoms;
        matchingCharacter.Charism = characterDndDnd.Charism;
        matchingCharacter.AdditionalStrong = this.SetAdditionalStats(characterDndDnd.Strong);
        matchingCharacter.AdditionalDexterity = this.SetAdditionalStats(characterDndDnd.Dexterity);
        matchingCharacter.AdditionalConstitution = this.SetAdditionalStats(characterDndDnd.Constitution);
        matchingCharacter.AdditionalIntelligence = this.SetAdditionalStats(characterDndDnd.Intelligence);
        matchingCharacter.AdditionalWisdoms = this.SetAdditionalStats(characterDndDnd.Wisdoms);
        matchingCharacter.AdditionalCharism = this.SetAdditionalStats(characterDndDnd.Charism);

        await this._dbContext.Set<CharacterDnd>().AddAsync(matchingCharacter);
        await this._dbContext.SaveChangesAsync();
    }

    public async Task DeletedCharacterAsync(int characterDndId)
    {
        var matchingCharacter = this._dbContext.Set<CharacterDnd>().FirstOrDefault(character => character.Id == characterDndId);

        if (matchingCharacter is null)
            throw new BusinessException($"CharacterId is not found{characterDndId}");

        this._dbContext.Set<CharacterDnd>().Remove(matchingCharacter);
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

    public ICharacterView GetCharacterByCharacterId(int characterId)
    {
        throw new NotImplementedException();
    }

    public void CreateCharacter(CharacterRequest character)
    {
        var newCharacter = new CharacterDnd
        {
            Life = character.Life,
            Strong = (int)character.Competences.GetValueOrDefault(nameof(CharacterDnd.Strong)),
        };
         
    }
}
