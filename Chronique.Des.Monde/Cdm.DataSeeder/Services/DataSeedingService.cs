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
/// Service de seeding pour initialiser la base de données avec des données de test
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
        _logger.LogInformation("?? Démarrage du seeding des données...");

        try
        {
            using var scope = _serviceProvider.CreateScope();
            
            // 1. Créer et migrer les bases de données
            await CreateAndMigrateDatabases(scope);
            
            // 2. Créer l'utilisateur root
            var rootUserId = await CreateRootUser(scope);
            
            // 3. Créer des personnages de démonstration
            await CreateDemoCharacters(scope, rootUserId);
            
            _logger.LogInformation("? Seeding terminé avec succès!");
            _logger.LogInformation("?? Utilisateur root créé : Email=root@test.com, Password=root");
            _logger.LogInformation("?? Personnages créés pour l'utilisateur root");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "? Erreur lors du seeding des données");
            throw;
        }
        
        // Arrêter l'application après le seeding
        Environment.Exit(0);
    }

    private async Task CreateAndMigrateDatabases(IServiceScope scope)
    {
        _logger.LogInformation("??? Création et migration des bases de données...");

        // Base de données commune (Users)
        var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await appDbContext.Database.EnsureCreatedAsync();
        _logger.LogInformation("? Base de données commune créée");

        // Base de données D&D (Characters)
        var dndDbContext = scope.ServiceProvider.GetRequiredService<DndDbContext>();
        await dndDbContext.Database.EnsureCreatedAsync();
        _logger.LogInformation("? Base de données D&D créée");
    }

    private async Task<int> CreateRootUser(IServiceScope scope)
    {
        _logger.LogInformation("?? Création de l'utilisateur root...");

        var userBusiness = scope.ServiceProvider.GetRequiredService<UserBusiness>();
        var passwordService = scope.ServiceProvider.GetRequiredService<PasswordService>();

        // Vérifier si l'utilisateur root existe déjà
        var existingUser = await userBusiness.GetUserByEmailAsync("root@test.com");
        if (existingUser != null)
        {
            _logger.LogWarning("?? L'utilisateur root existe déjà (ID: {UserId})", existingUser.Id);
            return existingUser.Id;
        }

        // Créer l'utilisateur root
        var rootUserRequest = new UserRequest
        {
            UserName = "root",
            UserEmail = "root@test.com",
            Password = passwordService.HashPassword("root")
        };

        await userBusiness.RegisterUserAsync(rootUserRequest);
        
        var createdUser = await userBusiness.GetUserByEmailAsync("root@test.com");
        _logger.LogInformation("? Utilisateur root créé (ID: {UserId})", createdUser!.Id);
        
        return createdUser.Id;
    }

    private async Task CreateDemoCharacters(IServiceScope scope, int rootUserId)
    {
        _logger.LogInformation("?? Création des personnages de démonstration...");

        var characterBusiness = scope.ServiceProvider.GetRequiredService<CharacterDndBusiness>();

        // Vérifier s'il y a déjà des personnages pour cet utilisateur
        var existingCharacters = await characterBusiness.GetAllCharactersByUserId(rootUserId);
        if (existingCharacters.Any())
        {
            _logger.LogWarning("?? Des personnages existent déjà pour l'utilisateur root ({Count} personnages)", existingCharacters.Count());
            return;
        }

        // Créer une équipe de démonstration variée
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
                background: "Érudit",
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
                name: "Sœur Luminara",
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

            // Rôdeur DPS à distance
            CharacterRequestFactory.CreateDndCharacterRequest(
                name: "Aranis Chassevent",
                leveling: 4,
                life: 38,
                picture: "ranger_bow.jpg",
                background: "Chasseur",
                characterClass: "Rôdeur",
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
                _logger.LogInformation("? Personnage créé: {CharacterName} (ID: {CharacterId})", 
                    createdCharacter.Name, createdCharacter.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Erreur lors de la création du personnage {CharacterName}", characterRequest.Name);
            }
        }

        _logger.LogInformation("?? {Count} personnages de démonstration créés!", demoCharacters.Length);
    }
}