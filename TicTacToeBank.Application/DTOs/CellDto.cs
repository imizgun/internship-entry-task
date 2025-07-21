using TicTacToeBank.Core.Domain.Enums;

namespace TicTacToeBank.Application.DTOs;

public class CellDto
{
    public Guid Id { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
    public Guid GameId { get; set; }
    public CellStatus Status { get; set; }
}