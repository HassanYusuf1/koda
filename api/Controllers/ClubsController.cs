using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.Services;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminPolicy")]
    public class ClubsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ClubService _clubService;

        public ClubsController(UserManager<ApplicationUser> userManager, ClubService clubService)
        {
            _userManager = userManager;
            _clubService = clubService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateClub([FromBody] Club club)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(new ApiResponse<object>(false, "Unauthorized"));
            club.CreatedByAdminId = user.Id;
            var created = await _clubService.CreateClubAsync(club);
            return CreatedAtAction(nameof(GetClubs), new { id = created.Id }, new ApiResponse<Club>(true, null, created));
        }

        [HttpGet]
        public async Task<IActionResult> GetClubs()
        {
            var clubs = await _clubService.GetClubsAsync();
            return Ok(new ApiResponse<IEnumerable<Club>>(true, null, clubs));
        }
    }
}
