namespace Cdm.Data.Dnd;

using Dnd.Models;
using Microsoft.EntityFrameworkCore;

public class DndDbContext : DbContext
{
    public DndDbContext(DbContextOptions<DndDbContext> options) : base(options)
    {
    }
}