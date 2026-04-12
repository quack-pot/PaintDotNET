using PaintDotNET.Core.Enums;
using PaintDotNET.Core.Math;

namespace PaintDotNET.Core.Meta;

public record struct PlayerAddData(
    uint PlayerID,

    Vec2 InitialPosition,
    Team PlayerTeam
);
