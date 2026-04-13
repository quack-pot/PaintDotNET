using PaintDotNET.Api.Hubs;
using PaintDotNET.Api.Repos;
using PaintDotNET.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<GamesRepo>();

builder.Services.AddSingleton<GameStateQueueService>();
builder.Services.AddSingleton<JoinGameQueueService>();
builder.Services.AddHostedService<GameLoopService>();

builder.Services.AddValidation();
builder.Services.AddControllers();
builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<GameHub>("/game");
app.MapControllers();

app.Run();