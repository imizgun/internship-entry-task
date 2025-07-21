using TicTacToeBank.Core.Abstraction;
using TicTacToeBank.Core.Domain.Enums;

namespace TicTacToeBank.Core.Domain;

public class GameCell : IIdentifiable
{
    public Guid Id { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }

    public Guid GameId { get; set; }
    public Game Game { get; set; } = null!;

    public CellStatus Status { get; set; }

    public GameCell() { }

    private GameCell(Guid id, int row, int column, Guid gameId, CellStatus status)
    {
        Id = id;
        Row = row;
        Column = column;
        GameId = gameId;
        Status = status;
    }

    public static GameCell CreateNew(Guid gameId, int row, int column)
    {
        if (gameId == Guid.Empty)
            throw new ArgumentException("Game ID cannot be empty.", nameof(gameId));

        if (row < 0)
            throw new ArgumentException("Row  must be non-negative integer.", nameof(row));

        if (column < 0)
            throw new ArgumentException("Column must be non-negative integer.", nameof(column));

        return new GameCell(
            Guid.NewGuid(),
            row,
            column,
            gameId,
            CellStatus.Empty
        );
    }
}