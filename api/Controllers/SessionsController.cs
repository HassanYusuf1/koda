using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextPlay.Data;
using NextPlay.Models;

namespace NextPlay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SessionsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public SessionsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetSessions()
        {
            var sessions = await _db.Sessions.Include(s => s.Coach).ToListAsync();
            return Ok(sessions);
        }

        [HttpPost]
        [Authorize(Policy = "CoachPolicy")]
        public async Task<IActionResult> CreateSession([FromBody] Session session)
        {
            _db.Sessions.Add(session);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSessions), new { id = session.Id }, session);
        }
    }
}