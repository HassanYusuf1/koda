// ✅ Dette er en renset og fikset versjon av AuthController.cs, med alle merge-konflikter fjernet.

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Net;
using api.Models;
using api.DTOs;
using api.Services.Interfaces;
using api.Models.Email;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("AuthPolicy")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration config, IEmailService emailService)
        {
            _userManager = userManager;
            _config = config;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                Position = dto.Position,
                Team = dto.Team,
                DateOfBirth = dto.DateOfBirth,
                Role = dto.Role,
                ClubId = dto.ClubId,
                TeamId = dto.TeamId
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(new ApiResponse<object>(false, "Registration failed", result.Errors));

            await _userManager.AddToRoleAsync(user, dto.Role);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationUrl = $"https://nextplay.app/confirm?userId={user.Id}&token={WebUtility.UrlEncode(token)}";
            var body = $"Hello {dto.Email},<br/>Please confirm your email by clicking <a href='{confirmationUrl}'>this link</a>.<br/>Your confirmation token is: {WebUtility.HtmlEncode(token)}";
            await _emailService.SendAsync(new EmailMessage
            {
                To = dto.Email,
                Subject = "Confirm your email",
                Body = body
            });

            return Ok(new ApiResponse<object>(true, "User registered", new { user.Id, EmailToken = token }));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized(new ApiResponse<object>(false, "Invalid credentials"));

            if (!user.EmailConfirmed)
                return Unauthorized(new ApiResponse<object>(false, "Email not confirmed"));

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.UserName!)
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new ApiResponse<string>(true, null, jwt));
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return NotFound(new ApiResponse<object>(false, "User not found"));

            if (user.EmailConfirmed)
                return Ok(new ApiResponse<object>(true, "Email is already confirmed"));

            var result = await _userManager.ConfirmEmailAsync(user, dto.Token);
            if (!result.Succeeded)
                return BadRequest(new ApiResponse<object>(false, "Email confirmation failed", result.Errors));

            return Ok(new ApiResponse<object>(true, "Email confirmed successfully"));
        }
    }
}
