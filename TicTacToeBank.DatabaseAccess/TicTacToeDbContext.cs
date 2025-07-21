using Microsoft.EntityFrameworkCore;
using TicTacToeBank.Core.Domain;
using TicTacToeBank.DatabaseAccess.Configurations;

namespace TicTacToeBank.DatabaseAccess;

public class TicTacToeDbContext : DbContext
{
    public TicTacToeDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new GameConfiguration());
        modelBuilder.ApplyConfiguration(new GameCellConfiguration());
    }

    public DbSet<Game> Games { get; set; } = null!;
    public DbSet<GameCell> GameCells { get; set; } = null!;
}