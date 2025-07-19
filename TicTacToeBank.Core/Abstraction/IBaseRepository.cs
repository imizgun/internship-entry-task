namespace TicTacToeBank.Core.Abstraction;

public interface IBaseRepository<T> where T : class, IIdentifiable {
	Task<T?> GetByIdAsync(Guid id);
}