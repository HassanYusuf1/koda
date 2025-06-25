using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Models;

namespace api.Services;

public class InviteService
{
    private readonly AppDbContext _db;

    public InviteService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Invite> CreateInviteAsync(Invite invite)
    {
        _db.Invites.Add(invite);
        await _db.SaveChangesAsync();
        return invite;
    }

    public async Task<Invite?> GetInviteByTokenAsync(string token)
    {
        return await _db.Invites.FirstOrDefaultAsync(i => i.Token == token && !i.IsAccepted);
    }

    public async Task AcceptInviteAsync(Invite invite)
    {
        invite.IsAccepted = true;
        await _db.SaveChangesAsync();
    }
}