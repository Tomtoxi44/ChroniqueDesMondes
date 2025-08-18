namespace Chronique.Des.Mondes.Data;

using Chronique.Des.Mondes.Data.Models;
using Chronique.Des.Mondes.Data.Models.Configuration;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}