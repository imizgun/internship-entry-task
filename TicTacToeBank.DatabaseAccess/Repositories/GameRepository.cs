using Microsoft.EntityFrameworkCore;
using TicTacToeBank.Core.Abstraction;
using TicTacToeBank.Core.Domain;
using TicTacToeBank.DatabaseAccess.Abstractions;

namespace TicTacToeBank.DatabaseAccess.Repositories;

public class GameRepository : BaseRepository<Game>, IGameRepository {
	public GameRepository(TicTacToeDbContext dbContext) : base(dbContext) { }
	
	public override async Task<Game?> GetByIdAsync(Guid id) {
		return await TypeSet.Include(x => x.Cells)
			.FirstOrDefaultAsync(x => x.Id == id);
	}
}