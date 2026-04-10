using System.Collections.Concurrent;
using System.Diagnostics;
using PaintDotNET.Core.DataStructs;
using PaintDotNET.Core.Entities;
using PaintDotNET.Core.Math;
using PaintDotNET.Core.Systems;

namespace PaintDotNET.Core.Meta;

public class GameSession
{
    private readonly GameState game_state;
    private readonly ItemsStore<Player> players;

    private readonly PaintSystem paint_system;
    private readonly MovementSystem move_system;
    private readonly GameClockSystem clock_system;

    private readonly Stopwatch stopwatch = new();
    private float last_frame_time = 0.0f;

    private bool is_running = false;

    private readonly ConcurrentQueue<PlayerInputData> player_input = new();

    public GameSession()
    {
        game_state = new(
            GameRules.TILE_GRID_WIDTH,
            GameRules.TILE_GRID_HEIGHT,
            GameRules.GAME_TIME_SECS
        );
        players = new();

        paint_system = new(game_state, players);
        move_system = new(game_state, players);
        clock_system = new(game_state);
    }

    public bool IsRunning() => is_running;

    public void StartGame()
    {
        if (is_running)
        {
            return;
        }

        is_running = true;

        game_state.game_time_secs = GameRules.GAME_TIME_SECS;

        foreach (ref Player player in players)
        {
            move_system.PickPlayerSpawn(ref player);
        }

        last_frame_time = 0.0f;
        stopwatch.Start();
    }

    public bool AttemptUpdate()
    {
        if (!is_running)
        {
            return false;
        }

        float current_frame_time = (float)stopwatch.Elapsed.TotalSeconds;
        float delta_time = current_frame_time - last_frame_time;
        last_frame_time = current_frame_time;

        while (player_input.TryDequeue(out PlayerInputData input))
        {
            if (!players.HasItem(input.PlayerID))
            {
                continue;
            }

            ref Player player = ref players.GetItem(input.PlayerID);

            player.input_direction.X = MathGen.GetAxis<float>(input.IsRightPressed, input.IsLeftPressed);
            player.input_direction.Y = MathGen.GetAxis<float>(input.IsDownPressed, input.IsUpPressed);
        }

        move_system.UpdatePlayers(delta_time);
        paint_system.UpdatePainting(delta_time);
        clock_system.TickGameClock(delta_time);

        is_running = clock_system.IsGameStillGoing();
        return is_running;
    }

    public void QueuePlayerInput(in PlayerInputData input) => player_input.Enqueue(input);
}
