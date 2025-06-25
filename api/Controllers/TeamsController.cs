using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.Services;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TeamService _teamService;

        public TeamsController(UserManager<ApplicationUser> userManager, TeamService teamService)
        {
            _userManager = userManager;
            _teamService = teamService;
        }

        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> CreateTeam([FromBody] Team team)
        {
            var created = await _teamService.CreateTeamAsync(team);
            return CreatedAtAction(nameof(GetTeam), new { id = created.Id }, new ApiResponse<Team>(true, null, created));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetTeam(int id)
        {
            var team = await _teamService.GetTeamAsync(id);
            if (team == null) return NotFound(new ApiResponse<object>(false, "Not found"));
            return Ok(new ApiResponse<Team>(true, null, team));
        }
    }
}
