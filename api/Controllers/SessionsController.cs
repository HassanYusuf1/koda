using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.Services;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SessionsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SessionService _sessionService;


        public SessionsController(UserManager<ApplicationUser> userManager, SessionService sessionService)
        {
            _userManager = userManager;
            _sessionService = sessionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSessions()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var sessions = await _sessionService.GetSessionsAsync(user);
            return Ok(sessions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSession(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var session = await _sessionService.GetSessionAsync(id, user);
            if (session == null) return NotFound();

            return Ok(session);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "CoachPolicy")]
        public async Task<IActionResult> UpdateSession(int id, [FromBody] Session updated)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var success = await _sessionService.UpdateSessionAsync(id, updated, user);
            if (!success) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "CoachPolicy")]
        public async Task<IActionResult> DeleteSession(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var success = await _sessionService.DeleteSessionAsync(id, user);
            if (!success) return NotFound();

            return NoContent();
        }

        [HttpPost]
        [Authorize(Policy = "CoachPolicy")]
        public async Task<IActionResult> CreateSession([FromBody] Session session)
        {
            var created = await _sessionService.CreateSessionAsync(session);
            return CreatedAtAction(nameof(GetSessions), new { id = created.Id }, created);
        }
    }
}