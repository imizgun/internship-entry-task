namespace TicTacToeBank.Application.DTOs;

public class MakeMoveDto
{
    public Guid GameId { get; set; }
    public Guid PlayerId { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
}