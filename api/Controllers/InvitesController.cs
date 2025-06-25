using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.Services;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvitesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly InviteService _inviteService;

        public InvitesController(UserManager<ApplicationUser> userManager, InviteService inviteService)
        {
            _userManager = userManager;
            _inviteService = inviteService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> CreateInvite([FromBody] Invite invite)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(new ApiResponse<object>(false, "Unauthorized"));
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Coach"))
            {
                invite.TeamId = user.TeamId;
            }
            else if (roles.Contains("Admin"))
            {
                invite.ClubId = user.ClubId;
            }
            var created = await _inviteService.CreateInviteAsync(invite);
            return Ok(new ApiResponse<Invite>(true, null, created));
        }

        [HttpPost("accept/{token}")]
        public async Task<IActionResult> AcceptInvite(string token)
        {
            var invite = await _inviteService.GetInviteByTokenAsync(token);
            if (invite == null) return NotFound(new ApiResponse<object>(false, "Invalid invite"));
            await _inviteService.AcceptInviteAsync(invite);
            return Ok(new ApiResponse<object>(true, "Invite accepted"));
        }
    }
}