using Microsoft.EntityFrameworkCore;
using TicTacToeBank.Core.Abstraction;
using TicTacToeBank.Core.Domain;
using TicTacToeBank.DatabaseAccess.Abstractions;

namespace TicTacToeBank.DatabaseAccess.Repositories;

public class GameCellRepository : BaseRepository<GameCell>, IGameCellRepository {
	public GameCellRepository(TicTacToeDbContext dbContext) : base(dbContext) { }

	public override async Task<GameCell?> GetByIdAsync(Guid id) {
		return await TypeSet
			.Include(x => x.Game)
			.FirstOrDefaultAsync(x => x.Id == id);
	}

	public override async Task<bool> UpdateAsync(GameCell entity) {
		var res = await TypeSet.Where(x => x.Id == entity.Id)
			.ExecuteUpdateAsync(p => 
				p.SetProperty(x => x.Status, entity.Status));
		
		return res > 0;
	}
}