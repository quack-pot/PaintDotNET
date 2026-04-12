using Microsoft.AspNetCore.Mvc;
using PaintDotNET.Api.DTOs;
using PaintDotNET.Api.Services;

namespace PaintDotNET.Api.Controllers;

[ApiController]
[Route("api/lobby")]
public class LobbyController(
    JoinGameQueueService injected_join_queue
) : ControllerBase
{
    private readonly JoinGameQueueService join_queue = injected_join_queue;

    [HttpPut("/join/{GameID}")]
    public async Task<IActionResult> JoinGameLobby(
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

    [HttpPut("/leave/{GameID}/{PlayerID}")]
    public async Task<IActionResult> LeaveGameLobby(
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
                _ => StatusCode(StatusCodes.Status500InternalServerError, "Internal server error."),
            };
        }
        catch (OperationCanceledException)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "Join request not processed in time.");
        }
    }
}
