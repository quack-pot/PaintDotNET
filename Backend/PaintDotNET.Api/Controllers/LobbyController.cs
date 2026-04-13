using Microsoft.AspNetCore.Mvc;
using PaintDotNET.Api.DTOs;
using PaintDotNET.Api.Services;

namespace PaintDotNET.Api.Controllers;

[ApiController]
[Route("api/lobby")]
public class LobbyController(
    GameStateQueueService injected_state_queue,
    JoinGameQueueService injected_join_queue
) : ControllerBase
{
    private readonly GameStateQueueService state_queue = injected_state_queue;
    private readonly JoinGameQueueService join_queue = injected_join_queue;

    [HttpPost("create")]
    public async Task<IActionResult> CreateNewGame(
        CancellationToken cancel_token
    ) {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancel_token);
        cts.CancelAfter(GameLoopService.TickRate * 10);

        try
        {
            var create_task = state_queue.EnqueueNewGame();
            await create_task.WaitAsync(cts.Token);

            return create_task.Result.Status switch
            {
                GameStateResultStatus.SUCCESS => StatusCode(
                    StatusCodes.Status200OK,
                    new NewGameResultDTO(
                        create_task.Result.GameID,
                        create_task.Result.HostPlayerData.PlayerID,
                        create_task.Result.HostPlayerData.InitialPosition.X,
                        create_task.Result.HostPlayerData.InitialPosition.Y,
                        create_task.Result.HostPlayerData.PlayerTeam == Core.Enums.Team.RED_TEAM
                    )
                ),
                _ => StatusCode(StatusCodes.Status500InternalServerError, "Internal server error."),
            };
        }
        catch (OperationCanceledException)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "Join request not processed in time.");
        }
    }

    [HttpDelete("end/{GameID}")]
    public async Task<IActionResult> EndGame(
        uint GameID,
        [FromBody] EndGameDTO dto,
        CancellationToken cancel_token
    ) {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancel_token);
        cts.CancelAfter(GameLoopService.TickRate * 10);

        try
        {
            var end_task = state_queue.EnqueueEndGame(GameID, dto.PlayerID);
            await end_task.WaitAsync(cts.Token);

            return end_task.Result.Status switch
            {
                GameStateResultStatus.SUCCESS => StatusCode(StatusCodes.Status200OK),
                GameStateResultStatus.HOST_ONLY_ACTION => StatusCode(StatusCodes.Status401Unauthorized, "Only the host may do this."),
                GameStateResultStatus.GAME_NOT_FOUND => StatusCode(StatusCodes.Status404NotFound, "Could not find game with given id."),
                _ => StatusCode(StatusCodes.Status500InternalServerError, "Internal server error."),
            };
        }
        catch (OperationCanceledException)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "Join request not processed in time.");
        }
    }

    [HttpPut("start/{GameID}")]
    public async Task<IActionResult> StartGame(
        uint GameID,
        [FromBody] StartGameDTO dto,
        CancellationToken cancel_token
    ) {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancel_token);
        cts.CancelAfter(GameLoopService.TickRate * 10);

        try
        {
            var start_task = state_queue.EnqueueStartGame(GameID, dto.PlayerID);
            await start_task.WaitAsync(cts.Token);

            return start_task.Result.Status switch
            {
                GameStateResultStatus.SUCCESS => StatusCode(StatusCodes.Status200OK),
                GameStateResultStatus.HOST_ONLY_ACTION => StatusCode(StatusCodes.Status401Unauthorized, "Only the host may do this."),
                GameStateResultStatus.GAME_ALREADY_RUNNING => StatusCode(StatusCodes.Status409Conflict, "Game is already running."),
                GameStateResultStatus.GAME_NOT_FOUND => StatusCode(StatusCodes.Status404NotFound, "Could not find game with given id."),
                _ => StatusCode(StatusCodes.Status500InternalServerError, "Internal server error."),
            };
        }
        catch (OperationCanceledException)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "Join request not processed in time.");
        }
    }

    [HttpPut("join/{GameID}")]
    public async Task<IActionResult> JoinGame(
        uint GameID,
        CancellationToken cancel_token
    ) {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancel_token);
        cts.CancelAfter(GameLoopService.TickRate * 10);

        try
        {
            var join_task = join_queue.EnqueueJoin(GameID);
            await join_task.WaitAsync(cts.Token);

            return join_task.Result.Status switch
            {
                JoinResultStatus.SUCCESS => StatusCode(
                    StatusCodes.Status200OK,
                    new JoinGameResultDTO(
                        join_task.Result.PlayerData.PlayerID,
                        join_task.Result.PlayerData.InitialPosition.X,
                        join_task.Result.PlayerData.InitialPosition.Y,
                        join_task.Result.PlayerData.PlayerTeam == Core.Enums.Team.RED_TEAM
                    )
                ),
                JoinResultStatus.GAME_NOT_FOUND => StatusCode(StatusCodes.Status404NotFound, "Could not find game with given id."),
                _ => StatusCode(StatusCodes.Status500InternalServerError, "Internal server error."),
            };
        }
        catch (OperationCanceledException)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "Join request not processed in time.");
        }
    }

    [HttpPut("leave/{GameID}/{PlayerID}")]
    public async Task<IActionResult> LeaveGame(
        uint GameID,
        uint PlayerID,
        CancellationToken cancel_token
    ) {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancel_token);
        cts.CancelAfter(GameLoopService.TickRate * 10);

        try
        {
            var leave_task = join_queue.EnqueueLeave(GameID, PlayerID);
            await leave_task.WaitAsync(cts.Token);

            return leave_task.Result.Status switch
            {
                JoinResultStatus.SUCCESS => StatusCode(StatusCodes.Status200OK),
                JoinResultStatus.GAME_NOT_FOUND => StatusCode(StatusCodes.Status404NotFound, "Could not find game with given id."),
                JoinResultStatus.HOST_CANNOT_LEAVE => StatusCode(StatusCodes.Status400BadRequest, "Host cannot leave game, but can end game."),
                _ => StatusCode(StatusCodes.Status500InternalServerError, "Internal server error."),
            };
        }
        catch (OperationCanceledException)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "Join request not processed in time.");
        }
    }
}
