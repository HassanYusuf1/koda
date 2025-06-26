using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using api.Models;
using api.DTOs;
using api.Services;
using System.ComponentModel.DataAnnotations;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("AuthPolicy")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<ApplicationUser> userManager, 
            AuthService authService,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);
                    return BadRequest(new ApiResponse<object>(false, "Validation failed", errors));
                }

                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(dto.Email);
                if (existingUser != null)
                {
                    return Conflict(new ApiResponse<object>(false, "User with this email already exists"));
                }

                var result = await _authService.RegisterAsync(dto);
                if (!result.Success)
                {
                    return BadRequest(new ApiResponse<object>(false, result.Message, result.Errors));
                }

                _logger.LogInformation("User registered successfully: {Email}", dto.Email);
                return Ok(new ApiResponse<object>(true, "User registered successfully", new { result.UserId }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration for email: {Email}", dto.Email);
                return StatusCode(500, new ApiResponse<object>(false, "An error occurred during registration"));
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>(false, "Invalid input"));
                }

                var result = await _authService.LoginAsync(dto.Email, dto.Password);
                if (!result.Success)
                {
                    _logger.LogWarning("Failed login attempt for email: {Email}", dto.Email);
                    return Unauthorized(new ApiResponse<object>(false, "Invalid email or password"));
                }

                _logger.LogInformation("User logged in successfully: {Email}", dto.Email);
                return Ok(new ApiResponse<string>(true, "Login successful", result.Token));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", dto.Email);
                return StatusCode(500, new ApiResponse<object>(false, "An error occurred during login"));
            }
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>(false, "Invalid input"));
                }

                var user = await _userManager.FindByIdAsync(dto.UserId);
                if (user == null)
                {
                    return NotFound(new ApiResponse<object>(false, "User not found"));
                }

                var result = await _userManager.ConfirmEmailAsync(user, dto.Token);
                if (!result.Succeeded)
                {
                    return BadRequest(new ApiResponse<object>(false, "Email confirmation failed"));
                }

                _logger.LogInformation("Email confirmed for user: {UserId}", dto.UserId);
                return Ok(new ApiResponse<object>(true, "Email confirmed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during email confirmation for user: {UserId}", dto.UserId);
                return StatusCode(500, new ApiResponse<object>(false, "An error occurred during email confirmation"));
            }
        }
    }
}