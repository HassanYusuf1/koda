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
            var requester = await _userManager.GetUserAsync(User);
            if (requester == null)
                return Unauthorized(new ApiResponse<object>(false, "Unauthorized"));

            var players = await _playerService.GetPlayersAsync(requester);
            var data = players.Select(p => new
            {
                p.Id,
                p.FullName,
                p.Email,
                p.TeamId,
                p.Role
            });

            return Ok(new ApiResponse<object>(true, null, data));
        }

        [HttpGet("me")]
        [Authorize(Roles = "Player,Coach,Admin")]
        public async Task<IActionResult> GetMe()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound(new ApiResponse<object>(false, "User not found"));

            var data = new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.Position,
                user.TeamId,
                user.ClubId,
                user.Role
            };

            return Ok(new ApiResponse<object>(true, null, data));
        }
    }
}
