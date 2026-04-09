using PaintDotNET.Core.Enums;

namespace PaintDotNET.Core.Entities;

public struct Tile(Team initial_team = Team.NONE, uint initial_strength = 0)
{
    public Team team = initial_team;
    public uint strength = initial_strength;
}
