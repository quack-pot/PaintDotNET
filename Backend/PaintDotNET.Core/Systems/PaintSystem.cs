using PaintDotNET.Core.DataStructs;
using PaintDotNET.Core.Entities;
using PaintDotNET.Core.Enums;
using PaintDotNET.Core.Math;
using PaintDotNET.Core.Meta;

namespace PaintDotNET.Core.Systems;

public class PaintSystem(GameState injected_game_state, ItemsStore<Player> injected_players)
{
    private readonly GameState game_state = injected_game_state;
    private readonly ItemsStore<Player> players = injected_players;

    public void UpdatePainting(float delta_time)
    {
        foreach (ref Player player in players)
        {
            if (player.paint_cooldown_secs > 0.0f)
            {
                player.paint_cooldown_secs -= delta_time;
                continue;
            }

            int grid_x = (int)MathF.Floor(player.position.X);
            int grid_y = (int)MathF.Floor(player.position.Y);

            if (!game_state.IsInBounds(grid_x, grid_y)) {
                continue;
            }

            ref Tile tile = ref game_state.GetTile(grid_x, grid_y);
            
            if (tile.team != player.team)
            {
                tile.strength--;
                player.paint_cooldown_secs = GameRules.PLAYER_PAINT_COOLDOWN_SECS;
            }
            else if (tile.strength < GameRules.MAX_PAINT_STRENGTH)
            {
                tile.strength++;
                player.paint_cooldown_secs = GameRules.PLAYER_PAINT_COOLDOWN_SECS;
            }

            if (tile.strength <= 0)
            {
                tile.team = player.team;
                tile.strength = 1;
            }
        }
    }
}
