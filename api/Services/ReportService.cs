using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Models;

namespace api.Services;

public class ReportService
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReportService(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    private async Task<IQueryable<Report>> FilterReportsByUser(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var query = _db.Reports
            .Include(r => r.Player)
            .Include(r => r.Session);

        if (roles.Contains("Player"))
        {
            query = query.Where(r => r.PlayerId == user.Id);
        }
        else if (roles.Contains("Coach") && !roles.Contains("Admin"))
        {
            query = query.Where(r => r.Session!.CoachId == user.Id && r.Session.TeamId == user.TeamId);
        }

        return query;
    }

    public async Task<IEnumerable<Report>> GetReportsAsync(ApplicationUser user)
    {
        var query = await FilterReportsByUser(user);
        return await query.ToListAsync();
    }

    public async Task<Report?> GetReportAsync(int id, ApplicationUser user)
    {
        var query = await FilterReportsByUser(user);
        return await query.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Report> CreateReportAsync(Report report, ApplicationUser user)
    {
        report.PlayerId = user.Id;
        _db.Reports.Add(report);
        await _db.SaveChangesAsync();
        return report;
    }

    public async Task<bool> UpdateReportAsync(int id, Report updated, ApplicationUser user)
    {
        var report = await GetReportAsync(id, user);
        if (report == null) return false;

        report.SessionId = updated.SessionId;
        report.EffortLevel = updated.EffortLevel;
        report.Comment = updated.Comment;
        report.InjuryStatus = updated.InjuryStatus;
        report.VideoUrl = updated.VideoUrl;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteReportAsync(int id, ApplicationUser user)
    {
        var report = await GetReportAsync(id, user);
        if (report == null) return false;

        _db.Reports.Remove(report);
        await _db.SaveChangesAsync();
        return true;
    }
}
