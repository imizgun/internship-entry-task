using TicTacToeBank.Core.Domain;

namespace TicTacToeBank.Core.Abstraction;

public interface IGameCellRepository : IBaseRepository<GameCell> {
	Task<Guid> CreateAsync(GameCell gameCell);
}