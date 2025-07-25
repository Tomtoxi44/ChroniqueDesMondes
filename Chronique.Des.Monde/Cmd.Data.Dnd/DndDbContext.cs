namespace Cmd.Data.Dnd;

using Cmd.Data.Dnd.Models;
using Microsoft.EntityFrameworkCore;

public class DndDbContext : DbContext
{
    public DndDbContext(DbContextOptions<DndDbContext> options) : base(options)
    {
    }
}