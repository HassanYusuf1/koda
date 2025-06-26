using Microsoft.AspNetCore.Identity;
using api.Models;
using api.DTOs;

namespace api.Services
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public IEnumerable<string>? Errors { get; set; }
    }

    public class AuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtService _jwtService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<ApplicationUser> userManager, 
            JwtService jwtService,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<AuthResult> RegisterAsync(RegisterDto dto)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    FullName = dto.FullName,
                    Position = dto.Position,
                    Team = dto.Team,
                    DateOfBirth = dto.DateOfBirth?.ToUniversalTime(),
                    Role = dto.Role,
                    ClubId = dto.ClubId,
                    TeamId = dto.TeamId,
                    EmailConfirmed = false // Require email confirmation
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Message = "Registration failed",
                        Errors = result.Errors.Select(e => e.Description)
                    };
                }

                // Add user to role
                var roleResult = await _userManager.AddToRoleAsync(user, dto.Role);
                if (!roleResult.Succeeded)
                {
                    // Cleanup: delete the user if role assignment fails
                    await _userManager.DeleteAsync(user);
                    return new AuthResult
                    {
                        Success = false,
                        Message = "Failed to assign user role",
                        Errors = roleResult.Errors.Select(e => e.Description)
                    };
                }

                return new AuthResult
                {
                    Success = true,
                    Message = "User registered successfully",
                    UserId = user.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RegisterAsync for email: {Email}", dto.Email);
                return new AuthResult
                {
                    Success = false,
                    Message = "An unexpected error occurred during registration"
                };
            }
        }

        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Message = "Invalid credentials"
                    };
                }

                // Check if email is confirmed
                if (!user.EmailConfirmed)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Message = "Please confirm your email before logging in"
                    };
                }

                // Check if account is locked
                if (await _userManager.IsLockedOutAsync(user))
                {
                    return new AuthResult
                    {
                        Success = false,
                        Message = "Account is temporarily locked due to multiple failed login attempts"
                    };
                }

                var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
                if (!isPasswordValid)
                {
                    // Record failed login attempt
                    await _userManager.AccessFailedAsync(user);
                    
                    return new AuthResult
                    {
                        Success = false,
                        Message = "Invalid credentials"
                    };
                }

                // Reset failed login count on successful login
                await _userManager.ResetAccessFailedCountAsync(user);

                var roles = await _userManager.GetRolesAsync(user);
                var token = _jwtService.GenerateToken(user, roles);

                return new AuthResult
                {
                    Success = true,
                    Message = "Login successful",
                    Token = token,
                    UserId = user.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LoginAsync for email: {Email}", email);
                return new AuthResult
                {
                    Success = false,
                    Message = "An unexpected error occurred during login"
                };
            }
        }
    }
}