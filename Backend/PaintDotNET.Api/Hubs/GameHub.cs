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
    public const string JOIN_GROUP = "JoinGroup";
    public const string LEAVE_GROUP = "LeaveGroup";
}

public class GameHub(
    GameLoopService injected_game_loop_service
) : Hub
{
    private readonly GameLoopService game_loop_service = injected_game_loop_service;

    public async Task SendInput(PlayerInputDTO input)
    {
        game_loop_service.QueuePlayerInput(input);
    }

    public async Task JoinGroup(string game_client_id) => await Groups.AddToGroupAsync(Context.ConnectionId, game_client_id);
    public async Task LeaveGroup(string game_client_id) => await Groups.RemoveFromGroupAsync(Context.ConnectionId, game_client_id);
}
