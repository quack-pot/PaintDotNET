using PaintDotNET.Api.Hubs;
using PaintDotNET.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidation();
builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddSingleton<GameService>();
builder.Services.AddHostedService<GameLoopService>();

var app = builder.Build();

app.MapHub<GameHub>("/game");
app.MapControllers();

app.Run();