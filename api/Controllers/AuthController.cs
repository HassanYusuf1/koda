using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using api.Models;
using api.DTOs;
using api.Models.Email;
using api.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Net;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            IConfiguration config,
            IEmailService emailService)
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
            var body = $"Hello {dto.Email},<br/>Please confirm your email by clicking <a href='{confirmationUrl}'>this link</a>.";

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
            if (user == null)
                return Unauthorized(new ApiResponse<object>(false, "Invalid credentials"));

            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized(new ApiResponse<object>(false, "Invalid credentials"));

            if (!user.EmailConfirmed)
                return Unauthorized(new ApiResponse<object>(false, "Email not confirmed"));

            var token = await GenerateJwtToken(user);
            return Ok(new ApiResponse<string>(true, null, token));
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return NotFound(new ApiResponse<object>(false, "User not found"));

            var result = await _userManager.ConfirmEmailAsync(user, dto.Token);
            if (!result.Succeeded)
                return BadRequest(new ApiResponse<object>(false, "Email confirmation failed", result.Errors));

            return Ok(new ApiResponse<object>(true, "Email confirmed"));
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.UserName!)
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiresInMinutes"]!)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
