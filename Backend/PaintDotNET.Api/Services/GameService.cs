using Microsoft.AspNetCore.SignalR;
using PaintDotNET.Api.Hubs;

namespace PaintDotNET.Api.Services;

public class GameService(IHubContext<GameHub> injected_hub_ctx)
{
    private readonly IHubContext<GameHub> hub_ctx = injected_hub_ctx;

    // private readonly ItemsStore<GameData> games = new();

    public async Task UpdateGames()
    {
        // foreach (ref GameData game_data in games)
        // {
        //     if (!game_data.session.IsRunning()) continue;

        //     IClientProxy proxy = hub_ctx.Clients.Group(game_data.GetGameID().ToString());

        //     if (game_data.session.AttemptUpdate())
        //     {
        //         await proxy.SendAsync(
        //             GameHubEvents.GAME_UPDATE,
        //             new GameUpdateDTO() // TODO: This should send something, only if changes are available...
        //         );

        //         continue;
        //     }

        //     await proxy.SendAsync(
        //         GameHubEvents.GAME_OVER,
        //         new GameOverDTO(
        //             game_data.session.GetTeamCoverage(Core.Enums.Team.RED_TEAM),
        //             game_data.session.GetTeamCoverage(Core.Enums.Team.BLUE_TEAM)
        //         )
        //     );
        // }
    }
}
