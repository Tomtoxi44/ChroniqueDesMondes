namespace Chronique.Des.Mondes.Data;

using Microsoft.EntityFrameworkCore;
using Chronique.Des.Mondes.Data.Models;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Users> Users { get; set; }
}