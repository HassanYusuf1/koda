using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using api.Models;
using api.DTOs;
using api.Models.Email;
using System.Net;
using api.Data;
using api.Services.Interfaces;

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
        private readonly AppDbContext _db;
        private readonly InviteService _inviteService;
        private readonly IEmailService _emailService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            JwtService jwtService,
            ILogger<AuthService> logger,
            AppDbContext db,
            InviteService inviteService,
            IEmailService emailService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _logger = logger;
            _db = db;
            _inviteService = inviteService;
            _emailService = emailService;
        }

        public async Task<AuthResult> RegisterAsync(RegisterDto dto)
        {
            try
            {
                var assignedRole = dto.Role;

                if (!_userManager.Users.Any())
                {
                    assignedRole = "PlatformAdmin";
                    _logger.LogInformation("First user registered. Assigning PlatformAdmin role to {Email}", dto.Email);
                }

                Invite? invite = null;

                if (assignedRole == "ClubAdmin")
                {
                    if (string.IsNullOrWhiteSpace(dto.ClubName))
                    {
                        return new AuthResult { Success = false, Message = "Club name is required" };
                    }

                    var exists = await _db.Clubs.AnyAsync(c => c.Name == dto.ClubName);
                    if (exists)
                    {
                        return new AuthResult { Success = false, Message = "Klubben finnes allerede" };
                    }
                }
                else if (assignedRole == "Coach" || assignedRole == "Player")
                {
                    invite = await _inviteService.ValidateInviteAsync(dto.Email, dto.InvitationCode);
                    if (invite == null)
                    {
                        _logger.LogWarning("Invalid invite for {Email}", dto.Email);
                        return new AuthResult { Success = false, Message = "Ugyldig invitasjon" };
                    }

                    assignedRole = invite.Role;
                }

                var user = new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    FullName = dto.FullName,
                    DateOfBirth = dto.DateOfBirth?.ToUniversalTime(),
                    Role = assignedRole,
                    ClubId = invite?.ClubId,
                    TeamId = invite?.TeamId,
                    EmailConfirmed = false
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

                var roleResult = await _userManager.AddToRoleAsync(user, assignedRole);
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    return new AuthResult
                    {
                        Success = false,
                        Message = "Failed to assign user role",
                        Errors = roleResult.Errors.Select(e => e.Description)
                    };
                }

                if (assignedRole == "ClubAdmin")
                {
                    var club = new Club { Name = dto.ClubName!, CreatedByAdminId = user.Id };
                    _db.Clubs.Add(club);
                    await _db.SaveChangesAsync();

                    user.ClubId = club.Id;
                    await _userManager.UpdateAsync(user);
                }

                if (invite != null)
                {
                    await _inviteService.AcceptInviteAsync(invite);
                }

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationUrl = $"https://nextplay.app/confirm?userId={user.Id}&token={WebUtility.UrlEncode(token)}";
                var body = $"Hello {dto.Email},<br/>Please confirm your email by clicking <a href='{confirmationUrl}'>this link</a>.<br/>Your confirmation token is: {WebUtility.HtmlEncode(token)}";
                await _emailService.SendAsync(new EmailMessage
                {
                    To = dto.Email,
                    Subject = "Confirm your email",
                    Body = body
                });

                return new AuthResult
                {
                    Success = true,
                    Message = "User registered successfully",
                    UserId = user.Id,
                    Token = token
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