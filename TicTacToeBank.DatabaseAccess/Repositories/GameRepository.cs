using Microsoft.EntityFrameworkCore;
using TicTacToeBank.Core.Abstraction;
using TicTacToeBank.Core.Domain;
using TicTacToeBank.DatabaseAccess.Abstractions;

namespace TicTacToeBank.DatabaseAccess.Repositories;

public class GameRepository : BaseRepository<Game>, IGameRepository
{
    public GameRepository(TicTacToeDbContext dbContext) : base(dbContext) { }

    public async Task<Guid> CreateGameAsync(Game game)
    {
        await DbContext.AddAsync(game);

        var res = await DbContext.SaveChangesAsync();
        return res > 0 ? game.Id : Guid.Empty;
    }

    public override async Task<Game?> GetByIdAsync(Guid id)
    {
        var game = await TypeSet
            .Include(x => x.Cells)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (game == null) return null;

        game.Cells = game.Cells
            .OrderBy(c => c.Row)
            .ThenBy(c => c.Column)
            .ToList();

        return game;
    }

    public override async Task<bool> UpdateAsync(Game entity)
    {
        var res = await TypeSet.Where(x => x.Id == entity.Id)
            .ExecuteUpdateAsync(p =>
                p.SetProperty(x => x.Status, entity.Status)
                    .SetProperty(x => x.MovesCount, entity.MovesCount));

        return res > 0;
    }
}