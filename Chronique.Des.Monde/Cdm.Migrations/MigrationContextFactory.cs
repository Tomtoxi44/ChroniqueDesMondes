using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Cdm.Migrations;

public class MigrationContextFactory : IDesignTimeDbContextFactory<MigrationContext>
{
    public MigrationContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MigrationContext>();
        
        // Configuration temporaire pour les migrations
        // En production, ceci sera configuré via appsettings.json ou variables d'environnement
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ChroniqueDesMondes;Trusted_Connection=true;MultipleActiveResultSets=true");

        return new MigrationContext(optionsBuilder.Options);
    }
}