namespace TicTacToeBank.Requests;

public class MakeMoveRequest
{
    public Guid PlayerId { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
}