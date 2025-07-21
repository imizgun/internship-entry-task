using System.Net;
using System.Text;
using System.Text.Json;
using TicTacToeBank.Application.DTOs;
using TicTacToeBank.Core.Domain.Enums;
using TicTacToeBank.Requests;
using TicTacToeBank.Responses;
using TicTacToeBank.Tests.IntegrationTest;
using Xunit;
using Xunit.Abstractions;

namespace TicTacToeBank.Tests.IntegrationTests;

public class GameTests : BaseIntegrationTest
{
    public GameTests(IntegrationTestWebFactory factory, ITestOutputHelper output) : base(factory, output) { }

    [Fact]
    public async Task CreateGame_ShouldReturnCreatedGame()
    {
        // Arrange
        var xPlayerId = Guid.NewGuid();
        var oPlayerId = Guid.NewGuid();

        var requestContent = new StringContent(
            JsonSerializer.Serialize(new CreateGameDto
            {
                XPlayerId = xPlayerId,
                OPlayerId = oPlayerId
            }), Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync("/api/games", requestContent);

        // Assert
        var responseBody = await response.Content.ReadAsStringAsync();
        Output.WriteLine(responseBody);
        Output.WriteLine(response.StatusCode.ToString());
        Assert.True(response.IsSuccessStatusCode);
        var createdGame = JsonSerializer.Deserialize<CreateGameResponse>(responseBody, JsonSerializerOptions);

        var game = await Client.GetAsync($"/api/games/{createdGame!.Id}");
        var gameText = await game.Content.ReadAsStringAsync();
        Output.WriteLine(gameText);
        var gameBody = JsonSerializer.Deserialize<GameDto>(gameText, JsonSerializerOptions);

        Assert.NotNull(createdGame);
        Assert.NotNull(gameBody);
        Assert.Equal(xPlayerId, createdGame.XPlayerId);
        Assert.Equal(oPlayerId, createdGame.OPlayerId);
        Assert.Equal(createdGame.Id, gameBody.Id);
        Assert.Equal(createdGame.XPlayerId, gameBody.XPlayerId);
        Assert.Equal(createdGame.OPlayerId, gameBody.OPlayerId);

        foreach (var cell in gameBody.Cells)
            Assert.Equal(CellStatus.Empty, cell.Status);
    }

    [Fact]
    public async Task CreateGameWithOnePlayer_ShouldReturnError()
    {
        var xPlayerId = Guid.NewGuid();
        var oPlayerId = xPlayerId;
        var xPlayerId1 = Guid.Empty;
        var oPlayerId1 = Guid.NewGuid();

        var requestContent = new StringContent(
            JsonSerializer.Serialize(new CreateGameDto
            {
                XPlayerId = xPlayerId,
                OPlayerId = oPlayerId
            }), Encoding.UTF8, "application/json");

        var requestContent1 = new StringContent(
            JsonSerializer.Serialize(new CreateGameDto
            {
                XPlayerId = xPlayerId1,
                OPlayerId = oPlayerId1
            }), Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync("/api/games", requestContent);
        var response1 = await Client.PostAsync("/api/games", requestContent1);

        // Assert
        var responseBody = await response.Content.ReadAsStringAsync();
        var responseBody1 = await response1.Content.ReadAsStringAsync();

        Assert.NotNull(responseBody);
        Assert.NotNull(responseBody1);
        Assert.False(response.IsSuccessStatusCode);
        Assert.False(response1.IsSuccessStatusCode);
    }

    [Fact]
    public async Task CreateGameAndMakeMove_ShouldBeOk()
    {
        // Arrange
        var xPlayerId = Guid.NewGuid();
        var oPlayerId = Guid.NewGuid();

        var createGame = new CreateGameDto
        {
            OPlayerId = oPlayerId,
            XPlayerId = xPlayerId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(createGame), Encoding.UTF8, "application/json");
        var req = await Client.PostAsync("api/games", requestContent);

        var response = await req.Content.ReadAsStringAsync();
        var game = JsonSerializer.Deserialize<CreateGameResponse>(response, JsonSerializerOptions);

        Assert.NotNull(game);

        // Act

        var makeMove = new MakeMoveRequest
        {
            Column = 0,
            Row = 0,
            PlayerId = xPlayerId
        };
        var makeMoveString = new StringContent(JsonSerializer.Serialize(makeMove), Encoding.UTF8, "application/json");
        var makeMoveRes = await Client.PostAsync($"api/games/{game.Id}/moves", makeMoveString);
        var makeMoveResponse = JsonSerializer.Deserialize<MakeMoveResponse>(await makeMoveRes.Content.ReadAsStringAsync(), JsonSerializerOptions);
        var newGame = await Client.GetAsync($"api/games/{game.Id}");
        var newGameText = await newGame.Content.ReadAsStringAsync();
        var newGameObj = JsonSerializer.Deserialize<GameDto>(newGameText, JsonSerializerOptions);

        // Assert
        Assert.NotNull(makeMoveResponse);
        Assert.NotNull(newGameObj);
        Assert.True(makeMoveResponse.Success);
        Assert.Equal(makeMoveResponse.GameId, game.Id);
        Assert.Equal(CellStatus.X, newGameObj.Cells.FirstOrDefault(x => x.Column == 0 && x.Row == 0)!.Status);
        Assert.Equal(GameStatus.Pending, newGameObj.Status);
    }

    [Fact]
    public async Task CreateWinX_ShouldBeOk()
    {
        // Arrange
        var xPlayerId = Guid.NewGuid();
        var oPlayerId = Guid.NewGuid();

        var createGame = new CreateGameDto
        {
            OPlayerId = oPlayerId,
            XPlayerId = xPlayerId
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(createGame), Encoding.UTF8, "application/json");
        var req = await Client.PostAsync("api/games", requestContent);

        var response = await req.Content.ReadAsStringAsync();
        var game = JsonSerializer.Deserialize<CreateGameResponse>(response, JsonSerializerOptions);

        Assert.NotNull(game);

        // Act

        var makeMove0 = new MakeMoveRequest
        {
            Column = 0,
            Row = 0,
            PlayerId = xPlayerId
        };
        var makeMove1 = new MakeMoveRequest
        {
            Column = 0,
            Row = 1,
            PlayerId = oPlayerId
        };
        var makeMove2 = new MakeMoveRequest
        {
            Row = 0,
            Column = 1,
            PlayerId = xPlayerId
        };
        var makeMove3 = new MakeMoveRequest
        {
            Column = 0,
            Row = 2,
            PlayerId = oPlayerId
        };
        var makeMove4 = new MakeMoveRequest
        {
            Row = 0,
            Column = 2,
            PlayerId = xPlayerId
        };
        var moves = new List<MakeMoveRequest> {
            makeMove0,
            makeMove1,
            makeMove2,
            makeMove3,
            makeMove4
        };
        var moveResults = new List<HttpStatusCode>();

        foreach (var move in moves)
        {
            var makeMoveString = new StringContent(JsonSerializer.Serialize(move), Encoding.UTF8, "application/json");
            var makeMoveRes = await Client.PostAsync($"api/games/{game.Id}/moves", makeMoveString);
            var moveText = await makeMoveRes.Content.ReadAsStringAsync();
            Output.WriteLine(moveText);
            moveResults.Add(makeMoveRes.StatusCode);
        }
        var newGame = await Client.GetAsync($"api/games/{game.Id}");
        var newGameText = await newGame.Content.ReadAsStringAsync();
        var newGameObj = JsonSerializer.Deserialize<GameDto>(newGameText, JsonSerializerOptions);

        // Assert
        foreach (var moveResult in moveResults)
        {
            Output.WriteLine(moveResult.ToString());
            Assert.Equal(HttpStatusCode.OK, moveResult);
        }

        Assert.NotNull(newGameObj);
        Assert.Equal(GameStatus.WonX, newGameObj.Status);
    }
}