using Microsoft.EntityFrameworkCore;
using TicTacToeBank.Core.Abstraction;
using TicTacToeBank.Core.Domain;
using TicTacToeBank.DatabaseAccess.Abstractions;

namespace TicTacToeBank.DatabaseAccess.Repositories;

public class GameCellRepository : BaseRepository<GameCell>, IGameCellRepository {
	public GameCellRepository(TicTacToeDbContext dbContext) : base(dbContext) { }

	public async Task<Guid> CreateAsync(GameCell gameCell) {
		await DbContext.AddAsync(gameCell);

		var res = await DbContext.SaveChangesAsync();
		return res > 0 ? gameCell.Id : Guid.Empty;
	}

	public override async Task<GameCell?> GetByIdAsync(Guid id) {
		return await TypeSet
			.Include(x => x.Game)
			.FirstOrDefaultAsync(x => x.Id == id);
	}
}