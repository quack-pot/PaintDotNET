using System.Threading.Channels;
using PaintDotNET.Core.Meta;

namespace PaintDotNET.Api.Services;

public enum GameStateResultStatus
{
    SUCCESS,
    HOST_ONLY_ACTION,
    GAME_NOT_FOUND,
    GAME_ALREADY_RUNNING,
    
    INTERNAL_ERROR
}

public record GameStateResult(
    GameStateResultStatus Status,

    uint GameID,
    PlayerAddData HostPlayerData
);

public enum GameStateRequestType
{
    NEW_GAME,
    END_GAME,
    START_GAME
}

public record GameStateRequest(
    GameStateRequestType Type,
    uint GameID,
    uint PlayerID,

    TaskCompletionSource<GameStateResult> Response
);

public class GameStateQueueService
{
    private readonly Channel<GameStateRequest> req_channel = Channel.CreateUnbounded<GameStateRequest>(
        new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        }
    );

    public ChannelReader<GameStateRequest> Reader => req_channel.Reader;

    private Task<GameStateResult> EnqueueAsync(GameStateRequestType type, uint game_id, uint player_id)
    {
        var tcs = new TaskCompletionSource<GameStateResult>(TaskCreationOptions.RunContinuationsAsynchronously);

        if (!req_channel.Writer.TryWrite(new(type, game_id, player_id, tcs)))
        {
            tcs.SetException(new InvalidOperationException("Queue is closed."));
        }

        return tcs.Task;
    }

    public Task<GameStateResult> EnqueueNewGame() => EnqueueAsync(GameStateRequestType.NEW_GAME, 0u, 0u);
    public Task<GameStateResult> EnqueueEndGame(uint game_id, uint player_id) => EnqueueAsync(GameStateRequestType.END_GAME, game_id, player_id);
    public Task<GameStateResult> EnqueueStartGame(uint game_id, uint player_id) => EnqueueAsync(GameStateRequestType.START_GAME, game_id, player_id);
}
