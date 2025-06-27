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
using api.Services;

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
        private readonly AuthService _authService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            IConfiguration config,
            IEmailService emailService,
            AuthService authService)
        {
            _userManager = userManager;
            _config = config;
            _emailService = emailService;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (!result.Success)
            {
                return BadRequest(new ApiResponse<object>(false, result.Message, result.Errors));
            }

            return Ok(new ApiResponse<object>(true, result.Message, new { result.UserId, EmailToken = result.Token }));
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

            [HttpGet("test-email")]
        public async Task<IActionResult> TestEmail()
        {
            await _emailService.SendAsync(new EmailMessage
            {
                To = "hassansahal123@outlook.com",
                Subject = "Test Email",
                Body = "Dette er en test av e-posttjenesten."
            });

            return Ok(new ApiResponse<string>(true, "E-post sendt!"));
        }
    }
}
