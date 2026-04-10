using PaintDotNET.Core.Enums;
using PaintDotNET.Core.Math;

namespace PaintDotNET.Core.Entities;

public struct Player(Team player_team, in Vec2 initial_position)
{
    public readonly Team team = player_team != Team.NONE
        ? player_team
        : throw new ArgumentException(nameof(player_team), "Cannot be assigned to NONE team value.");
    
    public Vec2 position = new(initial_position);
    public Vec2 input_direction = new(0.0f, 0.0f);

    public float paint_cooldown_secs = 0.0f;
}
