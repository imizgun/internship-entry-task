using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace TicTacToeBank.Tests.IntegrationTest;

public class BaseIntegrationTest : IClassFixture<IntegrationTestWebFactory> {
	protected readonly HttpClient Client;
	protected readonly ITestOutputHelper Output;
	protected readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions {
		PropertyNameCaseInsensitive = true,
	};
	
	public BaseIntegrationTest(IntegrationTestWebFactory factory, ITestOutputHelper output) {
		Client = factory.CreateClient();
		Output = output;
	}
}