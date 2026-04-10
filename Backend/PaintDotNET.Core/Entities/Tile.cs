using PaintDotNET.Core.Enums;

namespace PaintDotNET.Core.Entities;

public struct Tile(Team initial_team = Team.NONE, int initial_strength = 0)
{
    public Team team = initial_team;
    public int strength = initial_strength;
}
