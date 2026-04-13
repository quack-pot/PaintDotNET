using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using PaintDotNET.Api.DTOs;
using PaintDotNET.Api.Hubs;
using PaintDotNET.Api.Repos;
using PaintDotNET.Core.Meta;

namespace PaintDotNET.Api.Services;

public class GameLoopService(
    ILogger<GameLoopService> injected_logger,
    IHubContext<GameHub> injected_hub_ctx,
    GameStateQueueService injected_state_queue,
    JoinGameQueueService injected_join_queue,

    GamesRepo injected_games_repo
) : BackgroundService
{
    private readonly ILogger<GameLoopService> logger = injected_logger;
    private readonly IHubContext<GameHub> hub_ctx = injected_hub_ctx;
    private readonly GameStateQueueService state_queue = injected_state_queue;
    private readonly JoinGameQueueService join_queue = injected_join_queue;

    private readonly ConcurrentQueue<PlayerInputDTO> input_queue = new();

    private readonly GamesRepo games_repo = injected_games_repo;

    public static readonly TimeSpan TickRate = TimeSpan.FromMilliseconds(30);

    public void QueuePlayerInput(in PlayerInputDTO input) => input_queue.Enqueue(input);

    protected override async Task ExecuteAsync(CancellationToken stopping_token)
    {
        while (!stopping_token.IsCancellationRequested)
        {
            DateTime frame_start = DateTime.UtcNow;

            await DrainGameStateQueue();
            DrainJoinQueue();
            UpdateGames();

            TimeSpan delay = TickRate - (DateTime.UtcNow - frame_start);
            if (delay > TimeSpan.Zero)
            {
                await Task.Delay(delay, stopping_token);
            }
        }
    }

    private async Task DrainGameStateQueue()
    {
        while (state_queue.Reader.TryRead(out var req))
        {
            try
            {
                switch (req.Type)
                {
                    case GameStateRequestType.NEW_GAME:
                    {
                        GameSession session = new();
                        var host_player_data = session.AddNewPlayer();
                        string client_id = "GAME-" + games_repo.GetNextID().ToString();

                        uint game_id = games_repo.InsertGame(new(
                            host_player_data.PlayerID,
                            client_id,
                            session
                        ));

                        req.Response.TrySetResult(new(GameStateResultStatus.SUCCESS, game_id, host_player_data));
                        return;
                    }

                    case GameStateRequestType.END_GAME:
                    {
                        if (!games_repo.HasGame(req.GameID))
                        {
                            req.Response.TrySetResult(new(GameStateResultStatus.GAME_NOT_FOUND, 0u, new()));
                            return;
                        }

                        ref Game game = ref games_repo.GetGame(req.GameID);

                        if (req.PlayerID != game.HostPlayerID)
                        {
                            req.Response.TrySetResult(new(GameStateResultStatus.HOST_ONLY_ACTION, 0u, new()));
                            return;
                        }

                        IClientProxy proxy = hub_ctx.Clients.Group(game.ClientGroupID);
                        await proxy.SendAsync(GameHubEvents.GAME_END);

                        games_repo.RemoveGame(req.GameID);

                        req.Response.TrySetResult(new(GameStateResultStatus.SUCCESS, 0u, new()));
                        return;
                    }

                    case GameStateRequestType.START_GAME:
                    {
                        if (!games_repo.HasGame(req.GameID))
                        {
                            req.Response.TrySetResult(new(GameStateResultStatus.GAME_NOT_FOUND, 0u, new()));
                            return;
                        }

                        ref Game game = ref games_repo.GetGame(req.GameID);

                        if (req.PlayerID != game.HostPlayerID)
                        {
                            req.Response.TrySetResult(new(GameStateResultStatus.HOST_ONLY_ACTION, 0u, new()));
                            return;
                        }

                        if (game.Session.IsRunning())
                        {
                            req.Response.TrySetResult(new(GameStateResultStatus.GAME_ALREADY_RUNNING, 0u, new()));
                            return;
                        }

                        game.Session.StartGame();

                        IClientProxy proxy = hub_ctx.Clients.Group(game.ClientGroupID);
                        await proxy.SendAsync(GameHubEvents.GAME_STARTED);

                        break;
                    }

                    default: { throw new Exception("Unknown game state request type."); }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process game state request of type {Type}.", req.Type);
                req.Response.TrySetException(ex);
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
                    if (game.HostPlayerID == req.PlayerID)
                    {
                        req.Response.TrySetResult(new(JoinResultStatus.HOST_CANNOT_LEAVE, new()));
                        return;
                    }

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

    private void UpdateGames()
    {
        while (input_queue.TryDequeue(out var dto))
        {
            if (!games_repo.HasGame(dto.GameID))
            {
                continue;
            }

            ref Game game = ref games_repo.GetGame(dto.GameID);
            game.Session.ApplyPlayerInput(new(
                dto.PlayerID,
                dto.IsUpPressed,
                dto.IsDownPressed,
                dto.IsLeftPressed,
                dto.IsRightPressed
            ));
        }

        foreach (ref Game game in games_repo)
        {
            if (!game.Session.IsRunning()) continue;

            IClientProxy proxy = hub_ctx.Clients.Group(game.ClientGroupID);

            if (game.Session.AttemptUpdate())
            {
                _ = proxy.SendAsync(
                    GameHubEvents.GAME_UPDATE,
                    new GameUpdateDTO() // TODO: This should send something, only if changes are available...
                );

                continue;
            }

            _ = proxy.SendAsync(
                GameHubEvents.GAME_OVER,
                new GameOverDTO(
                    game.Session.GetTeamCoverage(Core.Enums.Team.RED_TEAM),
                    game.Session.GetTeamCoverage(Core.Enums.Team.BLUE_TEAM)
                )
            );
        }
    }
}
