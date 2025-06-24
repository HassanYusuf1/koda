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

    public async Task<IEnumerable<ApplicationUser>> GetPlayersAsync()
    {
        return await _userManager.GetUsersInRoleAsync("Player");
    }

    public async Task<ApplicationUser?> GetPlayerAsync(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }
}