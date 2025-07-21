using System.Text;
using System.Text.Json;
using TicTacToeBank.Application.DTOs;
using TicTacToeBank.Core.Domain;
using TicTacToeBank.Core.Domain.Enums;
using TicTacToeBank.Responses;
using Xunit;
using Xunit.Abstractions;

namespace TicTacToeBank.Tests.IntegrationTest;

public class GameTests : BaseIntegrationTest {
	public GameTests(IntegrationTestWebFactory factory, ITestOutputHelper output) : base(factory, output) { }
	
	[Fact]
	public async Task CreateGame_ShouldReturnCreatedGame() {
		// Arrange
		var xPlayerId = Guid.NewGuid();
		var oPlayerId = Guid.NewGuid();

		var requestContent = new StringContent(
			JsonSerializer.Serialize(new CreateGameDto{
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

		Assert.NotNull(createdGame);
		Assert.Equal(xPlayerId, createdGame.XPlayerId);
		Assert.Equal(oPlayerId, createdGame.OPlayerId);
		Assert.NotEqual(Guid.Empty, createdGame.Id);
	}
}