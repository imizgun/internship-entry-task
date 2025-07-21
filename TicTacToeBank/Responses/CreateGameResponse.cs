namespace TicTacToeBank.Responses;

public class CreateGameResponse {
	public Guid Id { get; set; }
	public Guid XPlayerId { get; set; }
	public Guid OPlayerId { get; set; }
}