using Microsoft.EntityFrameworkCore;
using TicTacToeBank;
using TicTacToeBank.Application.Services;
using TicTacToeBank.Core.Abstraction;
using TicTacToeBank.DatabaseAccess;
using TicTacToeBank.DatabaseAccess.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<AutoMapperProfile>());
builder.Services.AddDbContext<TicTacToeDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(TicTacToeDbContext)));
});
builder.Services.AddScoped<IGameCellRepository, GameCellRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<GameService>();
builder.Services.AddScoped<IRandomProvider, RandomProvider>();

var app = builder.Build();
app.UseRouting();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TicTacToeDbContext>();
    dbContext.Database.Migrate();
}

// app.UseHttpsRedirection();

app.Run();

public partial class Program { }