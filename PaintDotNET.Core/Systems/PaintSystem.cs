using PaintDotNET.Core.Entities;
using PaintDotNET.Core.Enums;
using PaintDotNET.Core.Math;
using PaintDotNET.Core.Meta;
using PaintDotNET.Core.Stores;

namespace PaintDotNET.Core.Systems;

public class PaintSystem(GameState injected_game_state, PlayersStore injected_players)
{
    private readonly GameState game_state = injected_game_state;
    private readonly PlayersStore players = injected_players;

    public void UpdatePainting(float delta_time)
    {
        foreach (ref Player player in players)
        {
            if (player.paint_cooldown_secs > 0.0f)
            {
                player.paint_cooldown_secs -= delta_time;
                continue;
            }

            player.paint_cooldown_secs = GameRules.PLAYER_PAINT_COOLDOWN_SECS;

            int grid_x = (int)MathF.Floor(player.position.X);
            int grid_y = (int)MathF.Floor(player.position.Y);

            if (!game_state.IsInBounds(grid_x, grid_y)) {
                continue;
            }

            ref Tile tile = ref game_state.GetTile(grid_x, grid_y);
            
            if (tile.team == player.team || tile.team == Team.NONE || tile.strength == 0u)
            {
                tile.team = player.team;
                tile.strength = MathGen.Max(tile.strength + 1u, GameRules.MAX_PAINT_STRENGTH);
                continue;
            }

            tile.strength = MathGen.Min(tile.strength - 1u, 0u);
            tile.team = tile.strength == 0u ? Team.NONE : tile.team;
        }
    }
}
