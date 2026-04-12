using Microsoft.AspNetCore.Mvc;
using PaintDotNET.Api.Services;

namespace PaintDotNET.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LobbyController(GameService injected_game_service) : ControllerBase
    {
        private readonly GameService game_service = injected_game_service;
        
        [HttpPost("/create")]
        public void CreateGameLobby()
        {
            
        }

        [HttpPut("/join")]
        public void JoinGameLobby()
        {
            
        }

        [HttpDelete("/leave")]
        public void LeaveGameLobby()
        {
            
        }
    }
}
