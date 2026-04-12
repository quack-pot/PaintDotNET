using Microsoft.AspNetCore.SignalR;
using PaintDotNET.Api.DTOs;
using PaintDotNET.Api.Hubs;
using PaintDotNET.Api.Repos;

namespace PaintDotNET.Api.Services;

public class GameLoopService(
    ILogger<GameLoopService> injected_logger,
    IHubContext<GameHub> injected_hub_ctx,
    JoinGameQueueService injected_join_queue,

    GamesRepo injected_games_repo
) : BackgroundService
{
    private readonly ILogger<GameLoopService> logger = injected_logger;
    private readonly IHubContext<GameHub> hub_ctx = injected_hub_ctx;
    private readonly JoinGameQueueService join_queue = injected_join_queue;

    private readonly GamesRepo games_repo = injected_games_repo;

    public static readonly TimeSpan TickRate = TimeSpan.FromMilliseconds(30);

    protected override async Task ExecuteAsync(CancellationToken stopping_token)
    {
        while (!stopping_token.IsCancellationRequested)
        {
            DateTime frame_start = DateTime.UtcNow;

            DrainJoinQueue();
            await UpdateGames();

            TimeSpan delay = TickRate - (DateTime.UtcNow - frame_start);
            if (delay > TimeSpan.Zero)
            {
                await Task.Delay(delay, stopping_token);
            }
        }
    }

    private void DrainJoinQueue()
    {
        while (join_queue.Reader.TryRead(out var req))
        {
            try
            {
                if (!games_repo.HasGame(req.GameID))
                {
                    req.Response.TrySetResult(new(JoinResultStatus.GAME_NOT_FOUND, new()));
                    return;
                }

                ref Game game = ref games_repo.GetGame(req.GameID);

                if (req.IsLeaving)
                {
                    game.Session.RemovePlayer(req.PlayerID);
                    req.Response.TrySetResult(new(JoinResultStatus.SUCCESS, new()));
                    return;
                }

                req.Response.TrySetResult(new(JoinResultStatus.SUCCESS, game.Session.AddNewPlayer()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process join/leave request for game {GameID}", req.GameID);
                req.Response.TrySetException(ex);
            }
        }
    }

    private async Task UpdateGames()
    {
        foreach (ref Game game in games_repo)
        {
            if (!game.Session.IsRunning()) continue;

            IClientProxy proxy = hub_ctx.Clients.Group(game.ClientGroupID);

            if (game.Session.AttemptUpdate())
            {
                await proxy.SendAsync(
                    GameHubEvents.GAME_UPDATE,
                    new GameUpdateDTO() // TODO: This should send something, only if changes are available...
                );

                continue;
            }

            await proxy.SendAsync(
                GameHubEvents.GAME_OVER,
                new GameOverDTO(
                    game.Session.GetTeamCoverage(Core.Enums.Team.RED_TEAM),
                    game.Session.GetTeamCoverage(Core.Enums.Team.BLUE_TEAM)
                )
            );
        }
    }
}
