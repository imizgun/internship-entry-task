using TicTacToeBank.Core.Abstraction;
using TicTacToeBank.Core.Domain.Enums;
using TicTacToeBank.Core.Domain.Exceptions;

namespace TicTacToeBank.Core.Domain;

public class Game : IIdentifiable {
	public Guid Id { get; set; }
	public GameStatus Status { get; set; }
	public int Size { get; set; }
	public Guid XPlayerId { get; set; }
	public Guid OPlayerId { get; set; }
	public int MovesCount { get; set; } = 0;
	public DateTime CreatedAt { get; set; }
	public List<GameCell> Cells { get; set; } = new();

	public Game() { }
	
	private Game(Guid id, GameStatus status, int size, Guid xPlayerId, Guid oPlayerId, int movesCount, DateTime createdAt) {
		Id = id;
		Status = status;
		Size = size;
		XPlayerId = xPlayerId;
		OPlayerId = oPlayerId;
		MovesCount = movesCount;
		CreatedAt = createdAt;

		for (var i = 0; i < size; i++) {
			for (var j = 0; j < size; j++) {
				Cells.Add(GameCell.CreateNew(id, i, j));
			}
		}
	}
	
	public static Game CreateNew(int size, Guid xPlayerId, Guid oPlayerId) {
		if (size < 3) 
			throw new InvalidGameSizeException("Game size must be at least 3x3.");
		
		if (xPlayerId == oPlayerId)
			throw new ArgumentException("Players must have different IDs.");
		
		if (xPlayerId == Guid.Empty || oPlayerId == Guid.Empty)
			throw new ArgumentException("Player IDs cannot be empty.");
		
		return new Game(
			Guid.NewGuid(), 
			GameStatus.Pending, 
			size,
			xPlayerId,
			oPlayerId,
			0,
			DateTime.UtcNow
			);
	}
}