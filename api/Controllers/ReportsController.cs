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
    public class ReportsController : ControllerBase
    {
       
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

       
        public ReportsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpPost]
        [Authorize(Roles = "Player")]
        public async Task<IActionResult> CreateReport([FromBody] Report report)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            report.PlayerId = user.Id;
            _db.Reports.Add(report);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetReport), new { id = report.Id }, report);
        }

        [HttpGet]
        public async Task<IActionResult> GetReports()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var roles = await _userManager.GetRolesAsync(user);

            IQueryable<Report> query = _db.Reports
                .Include(r => r.Player)
                .Include(r => r.Session);

            if (roles.Contains("Player"))
            {
                query = query.Where(r => r.PlayerId == user.Id);
            }
            else if (roles.Contains("Coach") && !roles.Contains("Admin"))
            {
                query = query.Where(r => r.Session!.CoachId == user.Id);
            }

            var reports = await query.ToListAsync();
            return Ok(reports);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReport(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var roles = await _userManager.GetRolesAsync(user);

            var report = await _db.Reports.Include(r => r.Player).Include(r => r.Session).FirstOrDefaultAsync(r => r.Id == id);
            if (report == null) return NotFound();

            if (roles.Contains("Player") && report.PlayerId != user.Id)
                return Forbid();
            if (roles.Contains("Coach") && !roles.Contains("Admin") && report.Session!.CoachId != user.Id)
                return Forbid();

            return Ok(report);
        }

        [HttpPost("csv")]
        [Authorize(Roles = "Coach,Admin")]
        public async Task<IActionResult> UploadCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("CSV file is required");

            using var reader = new StreamReader(file.OpenReadStream());
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                var parts = line.Split(',');
                if (parts.Length < 4) continue;
                var report = new Report
                {
                    PlayerId = parts[0],
                    SessionId = int.Parse(parts[1]),
                    EffortLevel = int.Parse(parts[2]),
                    Comment = parts[3]
                };
                _db.Reports.Add(report);
            }
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReport(int id, [FromBody] Report updated)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var roles = await _userManager.GetRolesAsync(user);

            var report = await _db.Reports.FindAsync(id);
            if (report == null) return NotFound();

            if (roles.Contains("Player") && !roles.Contains("Admin") && report.PlayerId != user.Id)
                return Forbid();
            if (roles.Contains("Coach") && !roles.Contains("Admin") && report.Session!.CoachId != user.Id)
                return Forbid();

            report.SessionId = updated.SessionId;
            report.EffortLevel = updated.EffortLevel;
            report.Comment = updated.Comment;
            report.InjuryStatus = updated.InjuryStatus;
            report.VideoUrl = updated.VideoUrl;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReport(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var roles = await _userManager.GetRolesAsync(user);

            var report = await _db.Reports.FindAsync(id);
            if (report == null) return NotFound();

            if (roles.Contains("Player") && !roles.Contains("Admin") && report.PlayerId != user.Id)
                return Forbid();
            if (roles.Contains("Coach") && !roles.Contains("Admin") &&
                (await _db.Sessions.FindAsync(report.SessionId))?.CoachId != user.Id)
                return Forbid();

            _db.Reports.Remove(report);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}