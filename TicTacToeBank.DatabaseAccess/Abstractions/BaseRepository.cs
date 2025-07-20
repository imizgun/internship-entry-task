using Microsoft.EntityFrameworkCore;
using TicTacToeBank.Core.Abstraction;

namespace TicTacToeBank.DatabaseAccess.Abstractions;

public abstract class BaseRepository<T> : IBaseRepository<T> where T : class, IIdentifiable {
	protected readonly DbSet<T> TypeSet;
	protected readonly TicTacToeDbContext DbContext;
	
	public BaseRepository(TicTacToeDbContext dbContext) {
		DbContext = dbContext;
		TypeSet = dbContext.Set<T>();
	}

	public virtual async Task<T?> GetByIdAsync(Guid id) {
		return await TypeSet.FindAsync(id);
	}

	public abstract Task<bool> UpdateAsync(T entity);
}