using System.Threading.Channels;

namespace PaintDotNET.Api.Services;

public enum JoinResultStatus
{
    SUCCESS,
    INTERNAL_ERROR,
    GAME_NOT_FOUND
}

public record JoinResult(
    JoinResultStatus Status,
    uint PlayerID
);

public record JoinRequest(
    uint GameID,
    uint PlayerID,
    bool IsLeaving,

    TaskCompletionSource<JoinResult> Response
);

public class JoinGameQueueService
{
    private readonly Channel<JoinRequest> req_channel = Channel.CreateUnbounded<JoinRequest>(
        new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        }
    );

    public ChannelReader<JoinRequest> Reader => req_channel.Reader;

    private Task<JoinResult> EnqueueAsync(uint game_id, uint player_id, bool is_leaving)
    {
        var tcs = new TaskCompletionSource<JoinResult>(TaskCreationOptions.RunContinuationsAsynchronously);

        if (!req_channel.Writer.TryWrite(new(game_id, player_id, is_leaving, tcs)))
        {
            tcs.SetException(new InvalidOperationException("Queue is closed."));
        }

        return tcs.Task;
    }

    public Task<JoinResult> EnqueueJoin(uint game_id) => EnqueueAsync(game_id, 0u, false);
    public Task<JoinResult> EnqueueLeave(uint game_id, uint player_id) => EnqueueAsync(game_id, player_id, true);
}
