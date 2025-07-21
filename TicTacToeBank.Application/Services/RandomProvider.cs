using TicTacToeBank.Core.Abstraction;

namespace TicTacToeBank.Application.Services;

public class RandomProvider : IRandomProvider
{

    public bool OpponentSignProbability()
    {
        var random = new Random();
        return random.Next(0, 10) == 0;
    }
}