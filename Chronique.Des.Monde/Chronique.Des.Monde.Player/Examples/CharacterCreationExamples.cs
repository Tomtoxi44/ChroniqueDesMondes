namespace Cmd.Business.Character.Examples;

using Cmd.Abstraction;
using Cmd.Abstraction.ModelsRequest;
using Cmd.Business.Character.ModelsRequest;
using System.Threading.Tasks;

/// <summary>
/// Exemples d'utilisation de l'API générique pour gérer des personnages D&D
/// </summary>
public class CharacterCreationExamples
{
    private readonly ICharacterBusiness _characterBusiness;

    public CharacterCreationExamples(ICharacterBusiness characterBusiness)
    {
        _characterBusiness = characterBusiness;
    }

    /// <summary>
    /// Exemple 1: Création d'un personnage D&D en utilisant l'approche générique
    /// </summary>
    public async Task CreateWarriorExample(int userId)
    {
        var characterRequest = CharacterRequestFactory.CreateDndCharacterRequest(
            name: "Gorthak le Brave",
            leveling: 3,
            life: 45,
            picture: "warrior.jpg",
            background: "Soldat",
            characterClass: "Guerrier",
            classArmor: 18,
            strong: 16,
            dexterity: 12,
            constitution: 15,
            intelligence: 10,
            wisdoms: 13,
            charism: 8
        );

        var createdCharacter = await _characterBusiness.CreateCharacter(characterRequest, userId);
        // Le personnage est maintenant créé avec tous les modificateurs calculés automatiquement
    }

    /// <summary>
    /// Exemple 2: Création d'un mage en utilisant des dictionnaires personnalisés
    /// </summary>
    public async Task CreateMageExample(int userId)
    {
        var characterRequest = new CharacterRequest
        {
            Name = "Lyralei la Sage",
            Leveling = 5,
            Life = 35,
            Picture = "mage.jpg",
            Background = "Ermite",
            Competences = new Dictionary<string, object>
            {
                { "Class", "Magicien" },
                { "ClassArmor", 12 },
                { "Strong", 8 },
                { "Dexterity", 14 },
                { "Constitution", 12 },
                { "Intelligence", 18 },
                { "Wisdoms", 15 },
                { "Charism", 13 }
            },
            Stats = new Dictionary<string, object>()
        };

        var createdCharacter = await _characterBusiness.CreateCharacter(characterRequest, userId);
        
        // Accès aux compétences via l'interface générique
        var intelligence = createdCharacter.Competences["Intelligence"];
        var additionalIntelligence = createdCharacter.Stats["AdditionalIntelligence"];
    }

    /// <summary>
    /// Exemple 3: Récupération de tous les personnages d'un utilisateur
    /// </summary>
    public async Task GetAllCharactersExample(int userId)
    {
        var characters = await _characterBusiness.GetAllCharactersByUserId(userId);
        
        foreach (var character in characters)
        {
            Console.WriteLine($"Personnage: {character.Name}, Niveau: {character.Leveling}");
        }
    }

    /// <summary>
    /// Exemple 4: Récupération d'un personnage spécifique
    /// </summary>
    public async Task GetCharacterByIdExample(int characterId)
    {
        var character = await _characterBusiness.GetCharacterById(characterId);
        Console.WriteLine($"Personnage trouvé: {character.Name}");
        
        // Accès aux compétences
        var strongValue = character.Competences["Strong"];
        var additionalStrong = character.Stats["AdditionalStrong"];
    }

    /// <summary>
    /// Exemple 5: Mise à jour d'un personnage
    /// </summary>
    public async Task UpdateCharacterExample(int characterId)
    {
        var updateRequest = new CharacterRequest
        {
            Name = "Gorthak le Vétéran",
            Leveling = 4,
            Life = 55,
            Picture = "warrior_veteran.jpg",
            Background = "Commandant",
            Competences = new Dictionary<string, object>
            {
                { "Class", "Paladin" },
                { "ClassArmor", 20 },
                { "Strong", 18 },
                { "Dexterity", 12 },
                { "Constitution", 16 },
                { "Intelligence", 11 },
                { "Wisdoms", 14 },
                { "Charism", 15 }
            },
            Stats = new Dictionary<string, object>()
        };

        var updatedCharacter = await _characterBusiness.UpdateCharacter(updateRequest, characterId);
        Console.WriteLine($"Personnage mis à jour: {updatedCharacter.Name}");
    }

    /// <summary>
    /// Exemple 6: Suppression d'un personnage
    /// </summary>
    public async Task DeleteCharacterExample(int characterId)
    {
        await _characterBusiness.DeleteCharacter(characterId);
        Console.WriteLine($"Personnage avec l'ID {characterId} supprimé.");
    }

    /// <summary>
    /// Exemple 7: Workflow complet CRUD
    /// </summary>
    public async Task CompleteWorkflowExample(int userId)
    {
        // 1. Créer un personnage
        var createRequest = CharacterRequestFactory.CreateDndCharacterRequest(
            "Test Character", 1, 20, "test.jpg", "Aventurier", "Rôdeur",
            14, 12, 16, 14, 11, 15, 10);
        
        var createdCharacter = await _characterBusiness.CreateCharacter(createRequest, userId);
        Console.WriteLine($"Créé: {createdCharacter.Name} (ID: {createdCharacter.Id})");

        // 2. Récupérer le personnage
        var retrievedCharacter = await _characterBusiness.GetCharacterById(createdCharacter.Id);
        Console.WriteLine($"Récupéré: {retrievedCharacter.Name}");

        // 3. Mettre à jour le personnage
        var updateRequest = new CharacterRequest
        {
            Name = "Test Character Modifié",
            Leveling = 2,
            Life = 25,
            Picture = "test_updated.jpg",
            Background = "Vétéran",
            Competences = new Dictionary<string, object>
            {
                { "Class", "Rôdeur Expérimenté" },
                { "ClassArmor", 15 },
                { "Strong", 13 },
                { "Dexterity", 17 },
                { "Constitution", 15 },
                { "Intelligence", 12 },
                { "Wisdoms", 16 },
                { "Charism", 11 }
            },
            Stats = new Dictionary<string, object>()
        };

        var updatedCharacter = await _characterBusiness.UpdateCharacter(updateRequest, createdCharacter.Id);
        Console.WriteLine($"Mis à jour: {updatedCharacter.Name}");

        // 4. Supprimer le personnage
        await _characterBusiness.DeleteCharacter(createdCharacter.Id);
        Console.WriteLine("Personnage supprimé");
    }
}