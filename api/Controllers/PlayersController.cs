using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.Services;


namespace api.Controllers
{
    [ApiController]
     [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PlayerService _playerService;

        public PlayersController(UserManager<ApplicationUser> userManager, PlayerService playerService)
        {
            _userManager = userManager;
            _playerService = playerService;
        }

        [HttpGet]
        [Authorize(Policy = "CoachPolicy")]
        public async Task<IActionResult> GetPlayers()
        {
            var players = await _playerService.GetPlayersAsync();
            var data = players.Select(u => new { u.Id, u.FullName, u.Email });
            return Ok(new ApiResponse<object>(true, null, data));
        }

        [HttpGet("me")]
        [Authorize(Roles = "Player,Coach,Admin")]
        public async Task<IActionResult> GetMe()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound(new ApiResponse<object>(false, "User not found"));
            return Ok(new ApiResponse<object>(true, null, new { user.Id, user.FullName, user.Email, user.Position, user.Team }));
        }
    }
}
