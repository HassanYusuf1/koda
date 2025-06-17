using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NextPlay.Models;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public PlayersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Policy = "CoachPolicy")]
        public IActionResult GetPlayers()
        {
            var players = _userManager.GetUsersInRoleAsync("Player").Result;
            return Ok(players.Select(u => new { u.Id, u.FullName, u.Email }));
        }

        [HttpGet("me")]
        [Authorize(Roles = "Player,Coach,Admin")]
        public async Task<IActionResult> GetMe()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            return Ok(new { user.Id, user.FullName, user.Email, user.Position, user.Team });
        }
    }
}