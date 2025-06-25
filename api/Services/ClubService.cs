using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Models;

namespace api.Services;

public class ClubService
{
    private readonly AppDbContext _db;

    public ClubService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Club> CreateClubAsync(Club club)
    {
        _db.Clubs.Add(club);
        await _db.SaveChangesAsync();
        return club;
    }

    public async Task<IEnumerable<Club>> GetClubsAsync()
    {
        return await _db.Clubs.Include(c => c.Teams).ToListAsync();
    }

    public async Task<Club?> GetClubAsync(int id)
    {
        return await _db.Clubs.Include(c => c.Teams).FirstOrDefaultAsync(c => c.Id == id);
    }
}
