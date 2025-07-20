namespace TicTacToeBank.Application.DTOs;

public class CreateGameDto {
	public Guid XPlayerId { get; set; }
	public Guid OPlayerId { get; set; }
}