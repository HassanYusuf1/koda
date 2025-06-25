using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Models;

namespace api.Services;

public class TeamService
{
    private readonly AppDbContext _db;

    public TeamService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Team> CreateTeamAsync(Team team)
    {
        _db.Teams.Add(team);
        await _db.SaveChangesAsync();
        return team;
    }

    public async Task<IEnumerable<Team>> GetTeamsAsync(int clubId)
    {
        return await _db.Teams.Where(t => t.ClubId == clubId).Include(t => t.Players).ToListAsync();
    }

    public async Task<Team?> GetTeamAsync(int id)
    {
        return await _db.Teams.Include(t => t.Players).FirstOrDefaultAsync(t => t.Id == id);
    }
}
