using PaintDotNET.Api.DTOs;
using PaintDotNET.Core.DataStructs;
using PaintDotNET.Core.Meta;

namespace PaintDotNET.Api.Services;

using GameID = uint;
using PlayerID = uint;

public readonly struct GameData
{
    public readonly PlayerID host_id;
    public readonly GameSession session;
}

public class GameService
{
    private readonly ItemsStore<GameData> games = new();

    public void UpdateGames()
    {
        foreach (ref GameData game_data in games)
        {
            if (!game_data.session.IsRunning()) continue;
            if (game_data.session.AttemptUpdate()) continue;

            // TODO: Report game ending status
        }
    }

    public void ApplyPlayerInput(in PlayerInputDTO input)
    {
        if (!games.HasItem(input.GameID))
        {
            return;
        }

        games.GetItem(input.GameID).session.QueuePlayerInput(new(
            input.PlayerID,
            input.IsUpPressed,
            input.IsDownPressed,
            input.IsLeftPressed,
            input.IsRightPressed
        ));
    }
}
