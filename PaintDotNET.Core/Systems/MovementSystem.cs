using PaintDotNET.Core.Entities;
using PaintDotNET.Core.Math;
using PaintDotNET.Core.Meta;
using PaintDotNET.Core.Stores;

namespace PaintDotNET.Core.Systems;

public class MovementSystem(GameState injected_game_state, PlayersStore injected_players)
{
    private readonly GameState game_state = injected_game_state;
    private readonly PlayersStore players = injected_players;

    public void UpdatePlayers(float delta_time)
    {
        float frame_speed = GameRules.PLAYER_MOVE_SPEED * delta_time;

        float upper_x = game_state.grid_width - GameRules.PLAYER_SIZE_RADIUS;
        float upper_y = game_state.grid_height - GameRules.PLAYER_SIZE_RADIUS;

        foreach (ref Player player in players)
        {
            Vec2 move_dir = player.input_direction.Normalized() * frame_speed;
            player.position += move_dir;

            player.position.Clamp(
                GameRules.PLAYER_SIZE_RADIUS,
                upper_x,
                GameRules.PLAYER_SIZE_RADIUS,
                upper_y
            );
        }
    }
}
