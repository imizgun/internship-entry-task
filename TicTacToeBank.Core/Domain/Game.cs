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
	public int MovesCount { get; set; }
	public int InRowWinCount { get; set; }
	public DateTime CreatedAt { get; set; }
	public List<GameCell> Cells { get; set; } = new();
	private Random _random = new();

	public Game() { }
	
	private Game(Guid id, GameStatus status, int size, Guid xPlayerId, Guid oPlayerId, int movesCount, int inRowWinCount, DateTime createdAt) {
		Id = id;
		Status = status;
		Size = size;
		XPlayerId = xPlayerId;
		OPlayerId = oPlayerId;
		MovesCount = movesCount;
		CreatedAt = createdAt;
		InRowWinCount = inRowWinCount;

		for (var i = 0; i < size; i++) {
			for (var j = 0; j < size; j++) {
				Cells.Add(GameCell.CreateNew(id, i, j));
			}
		}
	}
	
	public static Game CreateNew(int size, Guid xPlayerId, Guid oPlayerId, int inRowWinCount = 3) {
		if (size < 3) 
			throw new InvalidGameSizeException("Game size must be at least 3x3.");
		
		if (xPlayerId == oPlayerId)
			throw new ArgumentException("Players must have different IDs.");
		
		if (xPlayerId == Guid.Empty || oPlayerId == Guid.Empty)
			throw new ArgumentException("Player IDs cannot be empty.");
		
		if (inRowWinCount < 3)
			throw new ArgumentException("In-row win count must be at least 3.");
		
		return new Game(
			Guid.NewGuid(), 
			GameStatus.Pending, 
			size,
			xPlayerId,
			oPlayerId,
			0,
			Math.Min(5, inRowWinCount),
			DateTime.UtcNow
			);
	}
	
	public (GameCell affectedCell, GameStatus newGameStatus, string ETag) MakeMove(Guid playerId, int row, int column) {
		if (Status != GameStatus.Pending)
			throw new InvalidOperationException("Cannot make a move in a finished game.");
		
		if (MovesCount >= Size * Size)
			throw new InvalidOperationException("No more moves can be made in this game.");
		
		var cell = Cells.FirstOrDefault(c => c.Row == row && c.Column == column);
		
		if (cell == null)
			throw new ArgumentException("Invalid cell coordinates.", nameof(row));
		
		if (playerId != XPlayerId && playerId != OPlayerId)
			throw new ArgumentException("Invalid player ID.", nameof(playerId));
		
		if (cell.Status != CellStatus.Empty)
		{
			var expectedStatus = playerId == XPlayerId ? CellStatus.X : CellStatus.O;

			if (cell.Status == expectedStatus)
				return (cell, Status, ComputeETag(this));
			throw new InvalidOperationException("Cell is already occupied by the opponent.");
		}
		
		if (MovesCount % 2 == 1 && playerId == XPlayerId || MovesCount % 2 == 0 && playerId == OPlayerId)
			throw new InvalidOperationException("It's not your turn to play.");
		
		cell.Status = playerId == XPlayerId ? CellStatus.X : CellStatus.O;
		MovesCount++;
		if (OpponentSignProbability(cell.Status) && MovesCount % 3 == 0)
			cell.Status = cell.Status == CellStatus.X ? CellStatus.O : CellStatus.X;
		
		if (CheckWinCondition(playerId))
			Status = playerId == XPlayerId ? GameStatus.WonX : GameStatus.WonO;
		else if (MovesCount == Size * Size) 
			Status = GameStatus.Draw;

		return (cell, Status, ComputeETag(this));
	}
	
	private bool CheckWinCondition(Guid playerId) {
		var playerStatus = playerId == XPlayerId ? CellStatus.X : CellStatus.O;

		var rowWin = CheckRowWin(playerId, playerStatus);

		var columnWin = CheckColumnWin(playerId, playerStatus);

		var diagonalWin = CheckDiagonalWin(playerId, playerStatus);

		return rowWin || columnWin || diagonalWin;
	}
	
	private bool CheckRowWin(Guid playerId, CellStatus playerStatus) {
		var inRowCount = 0;
		for (var i = 0; i < Size; i++) {
			for (var j = 0; j < Size; j++) {
				var currentCell = Cells.FirstOrDefault(c => c.Row == i && c.Column == j);
				if (currentCell?.Status == playerStatus) {
					inRowCount++;
					if (inRowCount >= InRowWinCount) 
						return true;
				} 
				else
					inRowCount = 0;
			}
		}
		return false;
	}
	
	private bool CheckColumnWin(Guid playerId, CellStatus playerStatus) {
		var inRowCount = 0;
		for (var j = 0; j < Size; j++) {
			for (var i = 0; i < Size; i++) {
				var currentCell = Cells.FirstOrDefault(c => c.Row == i && c.Column == j);
				if (currentCell?.Status == playerStatus) {
					inRowCount++;
					if (inRowCount >= InRowWinCount) 
						return true;
				} 
				else
					inRowCount = 0;
			}
		}
		return false;
	}
	
	private bool CheckDiagonalWin(Guid playerId, CellStatus playerStatus) {
		var inRowCount = 0;
		for (int d = -(Size - 1); d <= Size - 1; d++) {
			inRowCount = 0;
			for (int row = 0; row < Size; row++) {
				int col = row - d;
				if (col >= 0 && col < Size) {
					var cell = Cells.FirstOrDefault(c => c.Row == row && c.Column == col);
					if (cell != null && cell.Status == playerStatus) {
						inRowCount++;
						if (inRowCount >= InRowWinCount) return true;
					} else {
						inRowCount = 0;
					}
				}
			}
		}



		for (int d = 0; d <= 2 * (Size - 1); d++) {
			inRowCount = 0;
			for (int row = 0; row < Size; row++) {
				int col = d - row;
				if (col >= 0 && col < Size) {
					var cell = Cells.FirstOrDefault(c => c.Row == row && c.Column == col);
					if (cell != null && cell.Status == playerStatus) {
						inRowCount++;
						if (inRowCount >= InRowWinCount) return true;
					} else {
						inRowCount = 0;
					}
				}
			}
		}

		
		return false;
	}
	
	// Симуляция 10%-ой вероятности того, что противник поставит свой знак 
	private bool OpponentSignProbability(CellStatus status) {
		var rand = _random.Next(0, 10);
		return rand == 0;
	}
	
	private string ComputeETag(Game game)
	{
		return $"\"{game.Id}-{MovesCount}\"";
	}

}