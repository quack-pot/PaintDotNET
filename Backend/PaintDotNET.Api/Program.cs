using PaintDotNET.Api.Hubs;
using PaintDotNET.Api.Repos;
using PaintDotNET.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<GamesRepo>();

builder.Services.AddSingleton<GameStateQueueService>();
builder.Services.AddSingleton<JoinGameQueueService>();
builder.Services.AddSingleton<GameLoopService>();

builder.Services.AddHostedService(p => p.GetRequiredService<GameLoopService>());

builder.Services.AddValidation();
builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:4321")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("ClientPolicy");

app.MapHub<GameHub>("/game");
app.MapControllers();

app.Run();