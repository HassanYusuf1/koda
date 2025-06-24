using Microsoft.AspNetCore.Identity;
using api.Models;
using api.Helpers;

namespace api.Services;

public class AuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtService _jwtService;

    public AuthService(UserManager<ApplicationUser> userManager, JwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    public async Task<IdentityResult> RegisterAsync(ApplicationUser user, string password, string role)
    {
        user.PasswordHash = PasswordHelper.HashPassword(password);
        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            return result;
        }

        await _userManager.AddToRoleAsync(user, role);
        return result;
    }

    public async Task<string?> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return null;

        if (!PasswordHelper.VerifyPassword(password, user.PasswordHash ?? string.Empty))
            return null;

        var roles = await _userManager.GetRolesAsync(user);
        return _jwtService.GenerateToken(user, roles);
    }
}