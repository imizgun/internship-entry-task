namespace TicTacToeBank.Requests;

public class GameCreateRequest
{
    public int Size { get; set; }
    public Guid XPlayerId { get; set; }
    public Guid OPlayerId { get; set; }
}