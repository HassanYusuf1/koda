using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Models;

namespace api.Services;

public class SessionService
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public SessionService(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // ✅ Sentral metode for tilgangsstyring
    private async Task<IQueryable<Session>> FilterSessionsByUser(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var query = _db.Sessions.Include(s => s.Coach);

        if (roles.Contains("Player"))
        {
            query = query.Where(s =>
                _db.PlayerSessions.Any(ps => ps.SessionId == s.Id && ps.PlayerId == user.Id));
        }
        else if (roles.Contains("Coach") && !roles.Contains("Admin"))
        {
            query = query.Where(s => s.CoachId == user.Id && s.TeamId == user.TeamId);
        }

        // Admin får alt (ingen filtrering)
        return query;
    }

    public async Task<IEnumerable<Session>> GetSessionsAsync(ApplicationUser user)
    {
        var query = await FilterSessionsByUser(user);
        return await query.ToListAsync();
    }

    public async Task<Session?> GetSessionAsync(int id, ApplicationUser user)
    {
        var query = await FilterSessionsByUser(user);
        return await query.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Session> CreateSessionAsync(Session session)
    {
        _db.Sessions.Add(session);
        await _db.SaveChangesAsync();
        return session;
    }

    public async Task<bool> UpdateSessionAsync(int id, Session updated, ApplicationUser user)
    {
        var session = await GetSessionAsync(id, user);
        if (session == null) return false;

        session.Title = updated.Title;
        session.Date = updated.Date;
        session.Location = updated.Location;
        session.Intensity = updated.Intensity;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteSessionAsync(int id, ApplicationUser user)
    {
        var session = await GetSessionAsync(id, user);
        if (session == null) return false;

        _db.Sessions.Remove(session);
        await _db.SaveChangesAsync();
        return true;
    }
}
