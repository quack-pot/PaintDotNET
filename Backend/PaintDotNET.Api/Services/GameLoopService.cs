namespace PaintDotNET.Api.Services;

public class GameLoopService(
    ILogger<GameLoopService> injected_logger,
    JoinGameQueueService injected_join_queue
) : BackgroundService
{
    private readonly ILogger<GameLoopService> logger = injected_logger;
    private readonly JoinGameQueueService join_queue = injected_join_queue;

    public static readonly TimeSpan TickRate = TimeSpan.FromMilliseconds(30);

    protected override async Task ExecuteAsync(CancellationToken stopping_token)
    {
        while (!stopping_token.IsCancellationRequested)
        {
            DateTime frame_start = DateTime.UtcNow;

            DrainJoinQueue();

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
                JoinResult result = new(JoinResultStatus.SUCCESS, 0u); // TODO:

                req.Response.TrySetResult(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process join/leave request for game {GameID}", req.GameID);
                req.Response.TrySetException(ex);
            }
        }
    }
}
