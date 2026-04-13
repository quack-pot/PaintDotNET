namespace PaintDotNET.Core.Meta;

public record struct PlayerJoinData(
    uint PlayerID,
    float XPosition,
    float YPosition,
    bool IsLeaving,
    bool IsRedTeam
);
