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
        var allPlayers = await _userManager.GetUsersInRoleAsync("Player");
        var requesterRoles = await _userManager.GetRolesAsync(requester);

        // Hvis Coach (men ikke Admin), filtrer kun spillere på samme Team
        if (requesterRoles.Contains("Coach") && !requesterRoles.Contains("Admin"))
        {
            return allPlayers.Where(p => p.TeamId == requester.TeamId).ToList();
        }

        return allPlayers;
    }

    public async Task<ApplicationUser?> GetPlayerAsync(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }
}
