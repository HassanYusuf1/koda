using Microsoft.AspNetCore.Identity;
using api.Models;

namespace api.Services;

public class PlayerService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public PlayerService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IEnumerable<ApplicationUser>> GetPlayersAsync(ApplicationUser requester)
    {
        var players = await _userManager.GetUsersInRoleAsync("Player");
        var roles = await _userManager.GetRolesAsync(requester);
        if (roles.Contains("Coach") && !roles.Contains("Admin"))
        {
            players = players.Where(p => p.TeamId == requester.TeamId).ToList();
        }
        return players;
    }

    public async Task<ApplicationUser?> GetPlayerAsync(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }
}