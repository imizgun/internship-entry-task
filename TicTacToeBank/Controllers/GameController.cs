using Microsoft.AspNetCore.Mvc;
using TicTacToeBank.Application.DTOs;
using TicTacToeBank.Application.Services;
using TicTacToeBank.Requests;
using TicTacToeBank.Responses;

namespace TicTacToeBank.Controllers;

[ApiController]
[Route("api/games")]
public class GameController(GameService gameService) : ControllerBase
{
    private readonly GameService _gameService = gameService;

    [HttpPost]
    public async Task<ActionResult<CreateGameResponse>> CreateGame([FromBody] CreateGameDto game)
    {
        var gameId = await _gameService.CreateGameAsync(game);
        if (gameId == Guid.Empty)
            return BadRequest("Failed to create game.");

        return Ok(new CreateGameResponse
        {
            Id = gameId,
            XPlayerId = game.XPlayerId,
            OPlayerId = game.OPlayerId,
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GameDto>> GetGameById(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("Invalid game ID.");

        try
        {
            var game = await _gameService.GetGameByIdAsync(id);
            return Ok(game);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost("{id}/moves")]
    public async Task<ActionResult<MakeMoveResponse>> MakeMove(Guid id, [FromBody] MakeMoveRequest move)
    {
        if (id == Guid.Empty)
            return BadRequest("Invalid game ID or move data.");

        var moveDto = new MakeMoveDto
        {
            GameId = id,
            PlayerId = move.PlayerId,
            Row = move.Row,
            Column = move.Column
        };

        try
        {
            var result = await _gameService.MakeMove(moveDto);
            if (result.res)
            {
                Response.Headers.ETag = result.ETag;
                return Ok(new MakeMoveResponse
                {
                    GameId = id,
                    Message = "Move made successfully.",
                    Success = result.res
                });
            }
            return BadRequest(new MakeMoveResponse
            {
                GameId = id,
                Message = "Failed to make move.",
                Success = false
            });
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new MakeMoveResponse
            {
                GameId = id,
                Message = e.Message,
                Success = false
            });
        }
        catch (Exception e)
        {
            return Conflict(new MakeMoveResponse
            {
                GameId = id,
                Message = e.Message,
                Success = false
            });
        }
    }
}