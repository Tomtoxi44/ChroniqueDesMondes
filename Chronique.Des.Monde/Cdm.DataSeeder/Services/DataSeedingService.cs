using Cdm.Business.Common.Business.User;
using Cdm.Business.Common.Business.User.Models;
using Cdm.Data.Common;
using Cdm.Data.Dnd;
using Cmd.Abstraction;
using Cmd.Business.Character.Business;
using Cmd.Business.Character.ModelsRequest;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Cdm.DataSeeder.Services;

namespace Cdm.DataSeeder.Services;

/// <summary>
/// Service de seeding pour initialiser la base de donn�es avec des donn�es de test
/// </summary>
public class DataSeedingService : BackgroundService
{
    private readonly ILogger<DataSeedingService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public DataSeedingService(ILogger<DataSeedingService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("?? D�marrage du seeding des donn�es...");

        try
        {
            using var scope = _serviceProvider.CreateScope();
            
            // 1. Cr�er et migrer les bases de donn�es
            await CreateAndMigrateDatabases(scope);
            
            // 2. Cr�er l'utilisateur root
            var rootUserId = await CreateRootUser(scope);
            
            // 3. Cr�er des personnages de d�monstration
            await CreateDemoCharacters(scope, rootUserId);
            
            _logger.LogInformation("? Seeding termin� avec succ�s!");
            _logger.LogInformation("?? Utilisateur root cr�� : Email=root@test.com, Password=root");
            _logger.LogInformation("?? Personnages cr��s pour l'utilisateur root");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "? Erreur lors du seeding des donn�es");
            throw;
        }
        
        // Arr�ter l'application apr�s le seeding
        Environment.Exit(0);
    }

    private async Task CreateAndMigrateDatabases(IServiceScope scope)
    {
        _logger.LogInformation("??? Cr�ation et migration des bases de donn�es...");

        // Base de donn�es commune (Users)
        var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await appDbContext.Database.EnsureCreatedAsync();
        _logger.LogInformation("? Base de donn�es commune cr��e");

        // Base de donn�es D&D (Characters)
        var dndDbContext = scope.ServiceProvider.GetRequiredService<DndDbContext>();
        await dndDbContext.Database.EnsureCreatedAsync();
        _logger.LogInformation("? Base de donn�es D&D cr��e");
    }

    private async Task<int> CreateRootUser(IServiceScope scope)
    {
        _logger.LogInformation("?? Cr�ation de l'utilisateur root...");

        var userBusiness = scope.ServiceProvider.GetRequiredService<UserBusiness>();
        var passwordService = scope.ServiceProvider.GetRequiredService<PasswordService>();

        // V�rifier si l'utilisateur root existe d�j�
        var existingUser = await userBusiness.GetUserByEmailAsync("root@test.com");
        if (existingUser != null)
        {
            _logger.LogWarning("?? L'utilisateur root existe d�j� (ID: {UserId})", existingUser.Id);
            return existingUser.Id;
        }

        // Cr�er l'utilisateur root
        var rootUserRequest = new UserRequest
        {
            UserName = "root",
            UserEmail = "root@test.com",
            Password = passwordService.HashPassword("root")
        };

        await userBusiness.RegisterUserAsync(rootUserRequest);
        
        var createdUser = await userBusiness.GetUserByEmailAsync("root@test.com");
        _logger.LogInformation("? Utilisateur root cr�� (ID: {UserId})", createdUser!.Id);
        
        return createdUser.Id;
    }

    private async Task CreateDemoCharacters(IServiceScope scope, int rootUserId)
    {
        _logger.LogInformation("?? Cr�ation des personnages de d�monstration...");

        var characterBusiness = scope.ServiceProvider.GetRequiredService<CharacterDndBusiness>();

        // V�rifier s'il y a d�j� des personnages pour cet utilisateur
        var existingCharacters = await characterBusiness.GetAllCharactersByUserId(rootUserId);
        if (existingCharacters.Any())
        {
            _logger.LogWarning("?? Des personnages existent d�j� pour l'utilisateur root ({Count} personnages)", existingCharacters.Count());
            return;
        }

        // Cr�er une �quipe de d�monstration vari�e
        var demoCharacters = new[]
        {
            // Guerrier Tank
            CharacterRequestFactory.CreateDndCharacterRequest(
                name: "Gorthak le Protecteur",
                leveling: 5,
                life: 68,
                picture: "warrior_tank.jpg",
                background: "Garde Royal",
                characterClass: "Guerrier",
                classArmor: 18,
                strong: 18,
                dexterity: 12,
                constitution: 16,
                intelligence: 10,
                wisdoms: 13,
                charism: 14
            ),

            // Mage DPS
            CharacterRequestFactory.CreateDndCharacterRequest(
                name: "Lyralei l'Archimage",
                leveling: 5,
                life: 35,
                picture: "mage_arcane.jpg",
                background: "�rudit",
                characterClass: "Magicien",
                classArmor: 12,
                strong: 8,
                dexterity: 14,
                constitution: 12,
                intelligence: 18,
                wisdoms: 15,
                charism: 13
            ),

            // Clerc Support
            CharacterRequestFactory.CreateDndCharacterRequest(
                name: "S�ur Luminara",
                leveling: 4,
                life: 42,
                picture: "cleric_light.jpg",
                background: "Acolyte",
                characterClass: "Clerc",
                classArmor: 16,
                strong: 13,
                dexterity: 10,
                constitution: 15,
                intelligence: 12,
                wisdoms: 18,
                charism: 16
            ),

            // R�deur DPS � distance
            CharacterRequestFactory.CreateDndCharacterRequest(
                name: "Aranis Chassevent",
                leveling: 4,
                life: 38,
                picture: "ranger_bow.jpg",
                background: "Chasseur",
                characterClass: "R�deur",
                classArmor: 14,
                strong: 13,
                dexterity: 18,
                constitution: 14,
                intelligence: 12,
                wisdoms: 16,
                charism: 10
            ),

            // Voleur Utilitaire
            CharacterRequestFactory.CreateDndCharacterRequest(
                name: "Slinky Ombrelame",
                leveling: 5,
                life: 40,
                picture: "rogue_stealth.jpg",
                background: "Criminel",
                characterClass: "Voleur",
                classArmor: 13,
                strong: 10,
                dexterity: 18,
                constitution: 13,
                intelligence: 14,
                wisdoms: 12,
                charism: 15
            ),

            // Barbare Berserker
            CharacterRequestFactory.CreateDndCharacterRequest(
                name: "Grunk le Furieux",
                leveling: 6,
                life: 78,
                picture: "barbarian_rage.jpg",
                background: "Tribal",
                characterClass: "Barbare",
                classArmor: 15,
                strong: 20,
                dexterity: 14,
                constitution: 18,
                intelligence: 8,
                wisdoms: 11,
                charism: 9
            )
        };

        foreach (var characterRequest in demoCharacters)
        {
            try
            {
                var createdCharacter = await characterBusiness.CreateCharacter(characterRequest, rootUserId);
                _logger.LogInformation("? Personnage cr��: {CharacterName} (ID: {CharacterId})", 
                    createdCharacter.Name, createdCharacter.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Erreur lors de la cr�ation du personnage {CharacterName}", characterRequest.Name);
            }
        }

        _logger.LogInformation("?? {Count} personnages de d�monstration cr��s!", demoCharacters.Length);
    }
}