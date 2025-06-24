using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Models;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SessionsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;


        public SessionsController(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetSessions()
        {
           
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var roles = await _userManager.GetRolesAsync(user);

            IQueryable<Session> query = _db.Sessions.Include(s => s.Coach);

            if (roles.Contains("Player"))
            {
                query = query.Where(s => _db.PlayerSessions.Any(ps => ps.SessionId == s.Id && ps.PlayerId == user.Id));
            }
            else if (roles.Contains("Coach") && !roles.Contains("Admin"))
            {
                query = query.Where(s => s.CoachId == user.Id);
            }

            var sessions = await query.ToListAsync();
            return Ok(sessions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSession(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var roles = await _userManager.GetRolesAsync(user);

            var session = await _db.Sessions.Include(s => s.Coach).FirstOrDefaultAsync(s => s.Id == id);
            if (session == null) return NotFound();

            if (roles.Contains("Player") && !_db.PlayerSessions.Any(ps => ps.SessionId == id && ps.PlayerId == user.Id))
                return Forbid();
            if (roles.Contains("Coach") && !roles.Contains("Admin") && session.CoachId != user.Id)
                return Forbid();

            return Ok(session);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "CoachPolicy")]
        public async Task<IActionResult> UpdateSession(int id, [FromBody] Session updated)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var roles = await _userManager.GetRolesAsync(user);

            var session = await _db.Sessions.FindAsync(id);
            if (session == null) return NotFound();

            if (roles.Contains("Coach") && !roles.Contains("Admin") && session.CoachId != user.Id)
                return Forbid();

            session.Title = updated.Title;
            session.Date = updated.Date;
            session.Location = updated.Location;
            session.Intensity = updated.Intensity;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "CoachPolicy")]
        public async Task<IActionResult> DeleteSession(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var roles = await _userManager.GetRolesAsync(user);

            var session = await _db.Sessions.FindAsync(id);
            if (session == null) return NotFound();

            if (roles.Contains("Coach") && !roles.Contains("Admin") && session.CoachId != user.Id)
                return Forbid();

            _db.Sessions.Remove(session);
            await _db.SaveChangesAsync();
            return NoContent();
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