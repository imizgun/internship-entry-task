using Microsoft.EntityFrameworkCore;
using TicTacToeBank.DatabaseAccess;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<TicTacToeDbContext>(options => {
	options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(TicTacToeDbContext)));
});

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
	app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();