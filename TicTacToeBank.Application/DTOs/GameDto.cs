using TicTacToeBank.Core.Domain.Enums;

namespace TicTacToeBank.Application.DTOs;

public class GameDto
{
    public Guid Id { get; set; }
    public int Size { get; set; }
    public GameStatus Status { get; set; }
    public Guid XPlayerId { get; set; }
    public Guid OPlayerId { get; set; }
    public int InRowWinCount { get; set; }
    public List<CellDto> Cells { get; set; } = new();

    public GameDto(int size, Guid xPlayerId, Guid oPlayerId, int inRowWinCount)
    {
        Size = size;
        XPlayerId = xPlayerId;
        OPlayerId = oPlayerId;
        InRowWinCount = inRowWinCount;
    }
}