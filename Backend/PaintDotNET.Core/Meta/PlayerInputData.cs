namespace PaintDotNET.Core.Meta;

public record struct PlayerInputData(
    uint PlayerID,

    bool IsUpPressed,
    bool IsDownPressed,
    bool IsLeftPressed,
    bool IsRightPressed
);
