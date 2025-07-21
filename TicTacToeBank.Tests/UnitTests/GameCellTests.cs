using TicTacToeBank.Core.Domain;
using TicTacToeBank.Core.Domain.Enums;
using Xunit;

namespace TicTacToeBank.Tests.UnitTests;

public class GameCellTests
{
    [Fact]
    public void CreateNewCell_Test()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        int row = 1;
        int column = 2;

        // Act
        var cell = GameCell.CreateNew(gameId, row, column);

        // Assert
        Assert.NotNull(cell);
        Assert.Equal(gameId, cell.GameId);
        Assert.Equal(row, cell.Row);
        Assert.Equal(column, cell.Column);
        Assert.Equal(CellStatus.Empty, cell.Status);
        Assert.NotEqual(Guid.Empty, cell.Id);
    }

    [Fact]
    public void CreateInvalidNewCell_Test()
    {
        // Arrange
        var invalidGameId = Guid.Empty;
        int invalidRow = -1;
        int invalidColumn = -1;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => GameCell.CreateNew(invalidGameId, 0, 0));
        Assert.Throws<ArgumentException>(() => GameCell.CreateNew(Guid.NewGuid(), invalidRow, 0));
        Assert.Throws<ArgumentException>(() => GameCell.CreateNew(Guid.NewGuid(), 0, invalidColumn));
    }
}