using Microsoft.AspNetCore.SignalR;
using PaintDotNET.Api.DTOs;
using PaintDotNET.Api.Services;

namespace PaintDotNET.Api.Hubs;

public static class GameHubEvents
{
    public const string GAME_UPDATE = "GameUpdate";
    public const string GAME_OVER = "GameOver";
    public const string GAME_STARTED = "GameStarted";
    public const string GAME_END = "GameEnded";
    public const string PLAYER_INPUT = "SendInput";
}

public class GameHub(
    GameLoopService injected_game_loop_service
) : Hub
{
    private readonly GameLoopService game_loop_service = injected_game_loop_service;

    public async Task SendInput(PlayerInputDTO input) => game_loop_service.QueuePlayerInput(input);
}
