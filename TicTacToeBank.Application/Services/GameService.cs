using AutoMapper;
using Microsoft.Extensions.Configuration;
using TicTacToeBank.Application.DTOs;
using TicTacToeBank.Core.Abstraction;
using TicTacToeBank.Core.Domain;
using TicTacToeBank.Core.Domain.Enums;

namespace TicTacToeBank.Application.Services;

public class GameService(IGameRepository gameRepository,
    IGameCellRepository gameCellRepository,
    IMapper mapper,
    IConfiguration configuration,
    IRandomProvider randomProvider)
{
    private readonly IGameRepository _gameRepository = gameRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IGameCellRepository _gameCellRepository = gameCellRepository;
    private readonly IConfiguration _configuration = configuration;
    private readonly IRandomProvider _randomProvider = randomProvider;

    public async Task<Guid> CreateGameAsync(CreateGameDto game)
    {
        if (game == null) throw new ArgumentNullException(nameof(game));

        var newGame = Game.CreateNew(
            int.Parse(_configuration["GameSetting:BoardSize"] ?? "3"),
            game.XPlayerId,
            game.OPlayerId,
            int.Parse(_configuration["GameSetting:BoardSize"] ?? "3")
        );

        return await _gameRepository.CreateGameAsync(newGame);
    }

    public async Task<GameDto> GetGameByIdAsync(Guid id)
    {
        if (id == Guid.Empty) throw new ArgumentException("Invalid game ID.", nameof(id));

        var game = await _gameRepository.GetByIdAsync(id);
        if (game == null) throw new KeyNotFoundException($"Game with ID {id} not found.");

        return _mapper.Map<GameDto>(game);
    }

    public async Task<(bool res, string ETag)> MakeMove(MakeMoveDto move)
    {
        var game = await _gameRepository.GetByIdAsync(move.GameId);

        if (game == null) throw new KeyNotFoundException($"Game with ID {move.GameId} not found.");

        try
        {
            var res = game.MakeMove(move.PlayerId, move.Row, move.Column, _randomProvider.OpponentSignProbability());
            var cell = res.affectedCell;
            var moveSuccessfullyMade = await _gameCellRepository.UpdateAsync(cell);
            var updateGame = await _gameRepository.UpdateAsync(game);

            return (moveSuccessfullyMade && updateGame, res.ETag);
        }
        catch (Exception e)
        {
            return (false, "");
        }
    }
}