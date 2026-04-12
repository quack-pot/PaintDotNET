using Microsoft.AspNetCore.SignalR;
using PaintDotNET.Api.DTOs;
using PaintDotNET.Api.Services;

namespace PaintDotNET.Api.Hubs;

public static class GameHubEvents
{
    public const string GAME_UPDATE = "GameUpdate";
    public const string GAME_OVER = "GameOver";
}

public class GameHub(GameService injected_game_service) : Hub
{
    private readonly GameService game_service = injected_game_service;

    public async Task SendInput(PlayerInputDTO input)
    {
        
    }
}
