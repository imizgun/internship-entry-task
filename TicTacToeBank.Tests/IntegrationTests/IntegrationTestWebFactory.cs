using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using TicTacToeBank.DatabaseAccess;
using Xunit;

namespace TicTacToeBank.Tests.IntegrationTest;

public class IntegrationTestWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
	private PostgreSqlContainer? _dbContainer;

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureAppConfiguration((context, config) =>
		{
			config.AddJsonFile("appsettings.json");
		});

		builder.ConfigureTestServices(services =>
		{
			var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

			var dbName = configuration["Postgres:Database"] ?? "tictactoe";
			var dbUser = configuration["Postgres:Username"] ?? "postgres";
			var dbPass = configuration["Postgres:Password"] ?? "123";

			_dbContainer = new PostgreSqlBuilder()
				.WithImage("postgres:latest")
				.WithDatabase(dbName)
				.WithUsername(dbUser)
				.WithPassword(dbPass)
				.Build();

			_dbContainer.StartAsync().GetAwaiter().GetResult();

			var connectionString = _dbContainer.GetConnectionString();

			var descriptor = services.SingleOrDefault(
				d => d.ServiceType == typeof(DbContextOptions<TicTacToeDbContext>));

			if (descriptor is not null)
				services.Remove(descriptor);

			services.AddDbContext<TicTacToeDbContext>(options =>
			{
				options.UseNpgsql(connectionString);
			});
		});
	}

	public Task InitializeAsync() {
		return _dbContainer?.StartAsync() ?? Task.CompletedTask;
	}
	
	public new Task DisposeAsync() {
		return _dbContainer?.StopAsync() ?? Task.CompletedTask;
	}
}
