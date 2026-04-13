namespace PaintDotNET.Core.Meta;

public record struct TileUpdateData(
    bool IsRedTeam,
    int Strength,
    int XIndex,
    int YIndex
);
