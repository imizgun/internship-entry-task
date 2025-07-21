namespace TicTacToeBank.Responses;

public class MakeMoveResponse {
	public string Message { get; set; } = string.Empty;
	public Guid GameId { get; set; }
	public bool Success { get; set; }
}