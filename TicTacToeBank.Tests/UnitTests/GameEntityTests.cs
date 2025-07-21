using TicTacToeBank.Core.Domain;
using TicTacToeBank.Core.Domain.Enums;
using TicTacToeBank.Core.Domain.Exceptions;
using Xunit;

namespace TicTacToeBank.Tests.UnitTests;

public class GameEntityTests
{
    [Fact]
    public void CreateNormalGame_Test()
    {
        // Arrange
        var xPlayerId = Guid.NewGuid();
        var oPlayerId = Guid.NewGuid();
        int size = 4;

        // Act
        var game = Game.CreateNew(size, xPlayerId, oPlayerId, 3);

        // Assert
        Assert.NotNull(game);
        Assert.Equal(size, game.Size);
        Assert.Equal(xPlayerId, game.XPlayerId);
        Assert.Equal(oPlayerId, game.OPlayerId);
        Assert.Equal(GameStatus.Pending, game.Status);
        Assert.Equal(0, game.MovesCount);
        Assert.Equal(3, game.InRowWinCount);
        Assert.Equal(size * size, game.Cells.Count);
        foreach (var cell in game.Cells)
        {
            Assert.Equal(game.Id, cell.GameId);
            Assert.InRange(cell.Row, 0, size - 1);
            Assert.InRange(cell.Column, 0, size - 1);
            Assert.Equal(CellStatus.Empty, cell.Status);
        }
    }

    [Fact]
    public void CreateWierdGame_Test()
    {
        // Arrange
        var xPlayerId = Guid.NewGuid();
        var oPlayerId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<InvalidGameSizeException>(() => Game.CreateNew(2, xPlayerId, oPlayerId));
        Assert.Throws<ArgumentException>(() => Game.CreateNew(3, xPlayerId, xPlayerId));
        Assert.Throws<ArgumentException>(() => Game.CreateNew(3, Guid.Empty, oPlayerId));
        Assert.Throws<ArgumentException>(() => Game.CreateNew(3, xPlayerId, Guid.Empty));
        Assert.Throws<ArgumentException>(() => Game.CreateNew(3, xPlayerId, oPlayerId, 2));
    }

    [Fact]
    public void MakeMove_Test()
    {
        // Arrange
        var xPlayerId = Guid.NewGuid();
        var oPlayerId = Guid.NewGuid();
        var game = Game.CreateNew(3, xPlayerId, oPlayerId);

        // Act
        var moveResult = game.MakeMove(xPlayerId, 0, 0);

        // Assert
        Assert.Equal(GameStatus.Pending, moveResult.newGameStatus);
        Assert.Equal(1, game.MovesCount);
        Assert.Equal(CellStatus.X, moveResult.affectedCell.Status);
        Assert.Equal(game.Id, moveResult.affectedCell.GameId);

        var cell = game.Cells.FirstOrDefault(c => c.Row == 0 && c.Column == 0);
        Assert.NotNull(cell);
        Assert.Equal(CellStatus.X, cell.Status);

        Assert.Equal(GameStatus.Pending, game.Status);

        var secondMoveResult = game.MakeMove(xPlayerId, 0, 0);

        Assert.Equal(CellStatus.X, secondMoveResult.affectedCell.Status);
        Assert.Throws<InvalidOperationException>(() => game.MakeMove(oPlayerId, 0, 0));
        Assert.Throws<InvalidOperationException>(() => game.MakeMove(xPlayerId, 0, 1));
    }

    [Fact]
    public void WinHorizontal_Test()
    {
        // Arrange
        var xPlayerId = Guid.NewGuid();
        var oPlayerId = Guid.NewGuid();
        var game = Game.CreateNew(4, xPlayerId, oPlayerId, 3);

        // Act
        game.MakeMove(xPlayerId, 0, 0);
        game.MakeMove(oPlayerId, 1, 0);
        game.MakeMove(xPlayerId, 0, 1);
        game.MakeMove(oPlayerId, 1, 1);
        var moveResult = game.MakeMove(xPlayerId, 0, 2);
        var cell = game.Cells.FirstOrDefault(c => c.Row == 0 && c.Column == 2);

        // Assert
        Assert.NotNull(cell);
        Assert.Equal(GameStatus.WonX, moveResult.newGameStatus);
        Assert.Throws<InvalidOperationException>(() => game.MakeMove(oPlayerId, 1, 2));
        Assert.Equal(5, game.MovesCount);
        Assert.Equal(CellStatus.X, moveResult.affectedCell.Status);

        Assert.Equal(CellStatus.X, cell.Status);
    }

    [Fact]
    public void WinVertical_Test()
    {
        // Arrange
        var xPlayerId = Guid.NewGuid();
        var oPlayerId = Guid.NewGuid();
        var game = Game.CreateNew(4, xPlayerId, oPlayerId, 3);

        // Act
        game.MakeMove(xPlayerId, 1, 1);
        game.MakeMove(oPlayerId, 0, 0);
        game.MakeMove(xPlayerId, 2, 1);
        game.MakeMove(oPlayerId, 0, 1);
        game.MakeMove(xPlayerId, 2, 2);
        var moveResult = game.MakeMove(oPlayerId, 0, 2);
        var cell = game.Cells.FirstOrDefault(c => c.Row == 0 && c.Column == 2);

        // Assert
        Assert.NotNull(cell);
        Assert.Throws<InvalidOperationException>(() => game.MakeMove(xPlayerId, 3, 3));
        Assert.Equal(GameStatus.WonO, moveResult.newGameStatus);
        Assert.Equal(CellStatus.O, cell.Status);
        Assert.Equal(6, game.MovesCount);
    }

    [Fact]
    public void WinDiagonal_Test()
    {
        // Arrange
        var xPlayerId = Guid.NewGuid();
        var oPlayerId = Guid.NewGuid();
        var game = Game.CreateNew(4, xPlayerId, oPlayerId, 3);

        // Act
        game.MakeMove(xPlayerId, 1, 0);
        game.MakeMove(oPlayerId, 0, 0);
        game.MakeMove(xPlayerId, 2, 1);
        game.MakeMove(oPlayerId, 0, 1);
        var moveResult = game.MakeMove(xPlayerId, 3, 2);
        var cell = game.Cells.FirstOrDefault(c => c.Row == 2 && c.Column == 1);

        // Assert
        Assert.NotNull(cell);
        Assert.Throws<InvalidOperationException>(() => game.MakeMove(xPlayerId, 3, 2));
        Assert.Equal(GameStatus.WonX, moveResult.newGameStatus);
        Assert.Equal(5, game.MovesCount);
        Assert.Equal(CellStatus.X, moveResult.affectedCell.Status);


        Assert.Equal(5, game.MovesCount);

        Assert.Equal(CellStatus.X, cell.Status);
    }

    [Fact]
    public void WinDiagonalReverse_Test()
    {
        // Arrange
        var xPlayerId = Guid.NewGuid();
        var oPlayerId = Guid.NewGuid();
        var game = Game.CreateNew(4, xPlayerId, oPlayerId, 3);

        // Act
        game.MakeMove(xPlayerId, 0, 3);
        game.MakeMove(oPlayerId, 0, 0);
        game.MakeMove(xPlayerId, 1, 2);
        game.MakeMove(oPlayerId, 1, 1);
        var moveResult = game.MakeMove(xPlayerId, 2, 1);
        var cell = game.Cells.FirstOrDefault(c => c.Row == 2 && c.Column == 1);

        // Assert
        Assert.NotNull(cell);
        Assert.Equal(GameStatus.WonX, moveResult.newGameStatus);
        Assert.Throws<InvalidOperationException>(() => game.MakeMove(xPlayerId, 2, 1));
        Assert.Equal(5, game.MovesCount);
        Assert.Equal(CellStatus.X, moveResult.affectedCell.Status);

        Assert.NotNull(cell);
        Assert.Equal(CellStatus.X, cell.Status);
    }

    [Fact]
    public void ReverseDiagonal_Test()
    {
        // Arrange
        var xPlayerId = Guid.NewGuid();
        var oPlayerId = Guid.NewGuid();
        var game = Game.CreateNew(3, xPlayerId, oPlayerId, 3);

        // Act
        game.MakeMove(xPlayerId, 0, 2);
        game.MakeMove(oPlayerId, 0, 1);
        game.MakeMove(xPlayerId, 1, 1);
        game.MakeMove(oPlayerId, 1, 0);
        var moveResult = game.MakeMove(xPlayerId, 2, 0);
        var cell = game.Cells.FirstOrDefault(c => c.Row == 0 && c.Column == 2);

        // Assert
        Assert.NotNull(cell);
        Assert.Equal(GameStatus.WonX, moveResult.newGameStatus);
        Assert.Throws<InvalidOperationException>(() => game.MakeMove(oPlayerId, 2, 2));
        Assert.Equal(5, game.MovesCount);

        Assert.Equal(CellStatus.X, cell.Status);
    }

    [Fact]
    public void DrawGame_Test()
    {
        // Arrange
        var xPlayerId = Guid.NewGuid();
        var oPlayerId = Guid.NewGuid();
        var game = Game.CreateNew(3, xPlayerId, oPlayerId, 3);

        // Act
        game.MakeMove(xPlayerId, 0, 0);
        game.MakeMove(oPlayerId, 0, 1);
        game.MakeMove(xPlayerId, 0, 2);
        game.MakeMove(oPlayerId, 1, 1);
        game.MakeMove(xPlayerId, 1, 0);
        game.MakeMove(oPlayerId, 2, 0);
        game.MakeMove(xPlayerId, 1, 2);
        game.MakeMove(oPlayerId, 2, 2);
        var moveResult = game.MakeMove(xPlayerId, 2, 1);

        // Assert
        Assert.Equal(GameStatus.Draw, moveResult.newGameStatus);
        Assert.Equal(9, game.MovesCount);

        foreach (var cell in game.Cells)
        {
            Assert.NotEqual(CellStatus.Empty, cell.Status);
        }

        Assert.Throws<InvalidOperationException>(() => game.MakeMove(oPlayerId, 2, 2));
    }
}