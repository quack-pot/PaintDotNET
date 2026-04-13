using PaintDotNET.Core.DataStructs;
using PaintDotNET.Core.Entities;
using PaintDotNET.Core.Enums;
using PaintDotNET.Core.Math;
using PaintDotNET.Core.Meta;

namespace PaintDotNET.Core.Systems;

public class MovementSystem(GameState injected_game_state, ItemsStore<Player> injected_players)
{
    private readonly GameState game_state = injected_game_state;
    private readonly ItemsStore<Player> players = injected_players;

    public void UpdatePlayers(float delta_time, List<PlayerUpdateData> player_updates)
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

            player_updates.Add(new(player.id, player.position.X, player.position.Y));
        }
    }

    public void PickPlayerSpawn(ref Player player)
    {
        player.position.Y = Random.Shared.NextSingle() * game_state.grid_height;

        float half_grid_width = 0.5f * (game_state.grid_width - 1.0f);
        player.position.X = Random.Shared.NextSingle() * half_grid_width;

        if (player.team == Team.BLUE_TEAM)
        {
            player.position.X += half_grid_width;
        }
    }
}
