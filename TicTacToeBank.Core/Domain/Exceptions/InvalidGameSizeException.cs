namespace TicTacToeBank.Core.Domain.Exceptions;

[Serializable]
public class InvalidGameSizeException : Exception {
	public InvalidGameSizeException(string msg) : base(msg) { }
	
	public InvalidGameSizeException(string msg, Exception inner) : base(msg, inner) { }
}