namespace Cdm.Migrations;

using Chronique.Des.Mondes.Data.Models;
using Data.Dnd.Models;
using Microsoft.EntityFrameworkCore;

public class MigrationContext : DbContext
{
    public MigrationContext(DbContextOptions options) : base(options)
    {
    }

    protected MigrationContext()
    {
    }
    public DbSet<CharacterDnd> CharacterDnd { get; set; }

    public DbSet<User> Users { get; set; }
}
