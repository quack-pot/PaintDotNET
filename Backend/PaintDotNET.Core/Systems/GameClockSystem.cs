using PaintDotNET.Core.Entities;
using PaintDotNET.Core.Math;

namespace PaintDotNET.Core.Systems;

public class GameClockSystem(GameState injected_game_state)
{
    private readonly GameState game_state = injected_game_state;

    public void TickGameClock(float delta_time)
        => game_state.game_time_secs = MathGen.Max(0.0f, game_state.game_time_secs - delta_time);

    public bool IsGameStillGoing() => game_state.game_time_secs > 0.0f;
}
