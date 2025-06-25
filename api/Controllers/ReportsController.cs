using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Models;
using api.Services;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
       
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ReportService _reportService;


        public ReportsController(AppDbContext db, UserManager<ApplicationUser> userManager, ReportService reportService)
        {
            _db = db;
            _userManager = userManager;
            _reportService = reportService;
        }

        [HttpPost]
        [Authorize(Roles = "Player")]
        public async Task<IActionResult> CreateReport([FromBody] Report report)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(new ApiResponse<object>(false, "Unauthorized"));
            var created = await _reportService.CreateReportAsync(report, user);
            return CreatedAtAction(nameof(GetReport), new { id = created.Id }, new ApiResponse<Report>(true, null, created));
        }

        [HttpGet]
        public async Task<IActionResult> GetReports()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(new ApiResponse<object>(false, "Unauthorized"));
            var reports = await _reportService.GetReportsAsync(user);
            return Ok(new ApiResponse<IEnumerable<Report>>(true, null, reports));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReport(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(new ApiResponse<object>(false, "Unauthorized"));
            var report = await _reportService.GetReportAsync(id, user);
            if (report == null) return NotFound(new ApiResponse<object>(false, "Not found"));

            return Ok(new ApiResponse<Report>(true, null, report));
        }

        [HttpPost("csv")]
        [Authorize(Roles = "Coach,Admin")]
        public async Task<IActionResult> UploadCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new ApiResponse<object>(false, "CSV file is required"));

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
            return Ok(new ApiResponse<object>(true, "CSV uploaded"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReport(int id, [FromBody] Report updated)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(new ApiResponse<object>(false, "Unauthorized"));
            var success = await _reportService.UpdateReportAsync(id, updated, user);
            if (!success) return NotFound(new ApiResponse<object>(false, "Not found"));

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReport(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(new ApiResponse<object>(false, "Unauthorized"));
            var success = await _reportService.DeleteReportAsync(id, user);
            if (!success) return NotFound(new ApiResponse<object>(false, "Not found"));

            return NoContent();
        }
    }
}
