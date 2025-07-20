using TicTacToeBank.Core.Domain;

namespace TicTacToeBank.Core.Abstraction;

public interface IGameRepository : IBaseRepository<Game> {
	Task<Guid> CreateGameAsync(Game game);
}