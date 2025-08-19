using Cdm.Business.Common.Business.User;
using Cdm.Data.Common;
using Cdm.Data.Dnd;
using Cmd.Abstraction;
using Cmd.Business.Character.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Cdm.DataSeeder.Services;

namespace Cdm.DataSeeder;

class Program
{
    static async Task<int> Main(string[] args)
    {
        // ?? Application de Seeding pour Chronique des Mondes
        Console.WriteLine("?? Chronique des Mondes - Data Seeder");
        Console.WriteLine("=====================================");

        var builder = Host.CreateApplicationBuilder(args);

        // Configuration
        builder.Configuration.AddJsonFile("appsettings.json", optional: false);

        // Logging
        builder.Services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });

        // Entity Framework - Base de donn�es commune (Users)
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Entity Framework - Base de donn�es D&D (Characters)
        builder.Services.AddDbContext<DndDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DndConnection")));

        // Services m�tier
        builder.Services.AddScoped<UserBusiness>();
        builder.Services.AddScoped<PasswordService>();
        builder.Services.AddScoped<CharacterDndBusiness>();

        // Service keyed pour l'interface g�n�rique
        builder.Services.AddKeyedScoped<ICharacterBusiness, CharacterDndBusiness>("dnd");

        // Service de seeding
        builder.Services.AddHostedService<DataSeedingService>();

        // Configuration pour arr�ter l'application apr�s le seeding
        builder.Services.Configure<HostOptions>(options =>
        {
            options.ShutdownTimeout = TimeSpan.FromSeconds(30);
        });

        var host = builder.Build();

        try
        {
            Console.WriteLine("?? D�marrage du seeding...");
            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"? Erreur fatale: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            return 1;
        }

        Console.WriteLine("? Seeding termin� avec succ�s!");
        Console.WriteLine();
        Console.WriteLine("?? Informations de connexion:");
        Console.WriteLine("   Email: root@test.com");
        Console.WriteLine("   Mot de passe: root");
        Console.WriteLine();
        Console.WriteLine("?? Personnages cr��s:");
        Console.WriteLine("   - Gorthak le Protecteur (Guerrier Tank)");
        Console.WriteLine("   - Lyralei l'Archimage (Mage DPS)");
        Console.WriteLine("   - S�ur Luminara (Clerc Support)");
        Console.WriteLine("   - Aranis Chassevent (R�deur DPS)");
        Console.WriteLine("   - Slinky Ombrelame (Voleur Utilitaire)");
        Console.WriteLine("   - Grunk le Furieux (Barbare Berserker)");
        Console.WriteLine();
        Console.WriteLine("?? Vous pouvez maintenant tester votre API!");

        return 0;
    }
}