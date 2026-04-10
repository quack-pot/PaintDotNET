namespace PaintDotNET.Api.Services;

public class GameLoopService(GameService injected_game_service) : BackgroundService
{
    private const int FRAME_TIME_MS = 17; // Roughly 60 FPS

    private readonly GameService game_service = injected_game_service;

    protected override async Task ExecuteAsync(CancellationToken stopping_token)
    {
        while (!stopping_token.IsCancellationRequested)
        {
            game_service.UpdateGames();
            await Task.Delay(FRAME_TIME_MS, stopping_token);
        }
    }
}
