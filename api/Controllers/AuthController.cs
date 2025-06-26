using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using api.Models;
using api.DTOs;
<<<<<<< HEAD
using api.Models.Email;
using api.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Net;
=======
using api.Services;
using System.ComponentModel.DataAnnotations;
>>>>>>> 6f15557910fac7aa527fe342b5bd5df048f7cd36

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("AuthPolicy")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
<<<<<<< HEAD
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            IConfiguration config,
            IEmailService emailService)
        {
            _userManager = userManager;
            _config = config;
=======
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly IEmailService _emailService;

        public AuthController(
            UserManager<ApplicationUser> userManager, 
            AuthService authService,
            ILogger<AuthController> logger,
            IEmailService emailService)
        {
            _userManager = userManager;
            _authService = authService;
            _logger = logger;
>>>>>>> 6f15557910fac7aa527fe342b5bd5df048f7cd36
            _emailService = emailService;
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

<<<<<<< HEAD
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationUrl = $"https://nextplay.app/confirm?userId={user.Id}&token={WebUtility.UrlEncode(token)}";
            var body = $"Hello {dto.Email},<br/>Please confirm your email by clicking <a href='{confirmationUrl}'>this link</a>.";

            await _emailService.SendAsync(new EmailMessage
            {
                To = dto.Email,
                Subject = "Confirm your email",
                Body = body
            });
=======
                // Send email confirmation
                var user = await _userManager.FindByIdAsync(result.UserId);
                if (user != null)
                {
                    var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    try
                    {
                        await _emailService.SendEmailConfirmationAsync(user.Email!, user.FullName, emailToken, user.Id);
                        _logger.LogInformation("Email confirmation sent to: {Email}", dto.Email);
                    }
                    catch (Exception emailEx)
                    {
                        _logger.LogWarning(emailEx, "Failed to send confirmation email to: {Email}", dto.Email);
                        // Don't fail registration if email fails
                    }
                }
>>>>>>> 6f15557910fac7aa527fe342b5bd5df048f7cd36

                _logger.LogInformation("User registered successfully: {Email}", dto.Email);
                return Ok(new ApiResponse<object>(true, "User registered successfully. Please check your email to confirm your account.", new { result.UserId }));
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
                    return Unauthorized(new ApiResponse<object>(false, result.Message));
                }

<<<<<<< HEAD
            if (!user.EmailConfirmed)
                return Unauthorized(new ApiResponse<object>(false, "Email not confirmed"));

            var token = await GenerateJwtToken(user);
            return Ok(new ApiResponse<string>(true, null, token));
=======
                _logger.LogInformation("User logged in successfully: {Email}", dto.Email);
                return Ok(new ApiResponse<string>(true, "Login successful", result.Token));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", dto.Email);
                return StatusCode(500, new ApiResponse<object>(false, "An error occurred during login"));
            }
>>>>>>> 6f15557910fac7aa527fe342b5bd5df048f7cd36
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

                if (user.EmailConfirmed)
                {
                    return Ok(new ApiResponse<object>(true, "Email is already confirmed"));
                }

                var result = await _userManager.ConfirmEmailAsync(user, dto.Token);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(new ApiResponse<object>(false, "Email confirmation failed", errors));
                }

                // Send welcome email after successful confirmation
                try
                {
                    await _emailService.SendWelcomeEmailAsync(user.Email!, user.FullName);
                }
                catch (Exception emailEx)
                {
                    _logger.LogWarning(emailEx, "Failed to send welcome email to: {Email}", user.Email);
                    // Don't fail confirmation if welcome email fails
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

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            try
            {
<<<<<<< HEAD
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.UserName!)
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
=======
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>(false, "Invalid input"));
                }

                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                {
                    // For security, don't reveal if the email exists or not
                    _logger.LogWarning("Password reset requested for non-existent email: {Email}", dto.Email);
                    return Ok(new ApiResponse<object>(true, "If the email exists, a password reset link has been sent"));
                }
>>>>>>> 6f15557910fac7aa527fe342b5bd5df048f7cd36

                // Check if email is confirmed
                if (!user.EmailConfirmed)
                {
                    _logger.LogWarning("Password reset requested for unconfirmed email: {Email}", dto.Email);
                    return BadRequest(new ApiResponse<object>(false, "Please confirm your email before requesting a password reset"));
                }

                // Generate password reset token
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                
                try
                {
                    // Send email with reset link
                    await _emailService.SendPasswordResetEmailAsync(user.Email!, user.FullName, resetToken, user.Id);
                    _logger.LogInformation("Password reset email sent to: {Email}", dto.Email);
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Failed to send password reset email to: {Email}", dto.Email);
                    return StatusCode(500, new ApiResponse<object>(false, "Failed to send password reset email"));
                }

                return Ok(new ApiResponse<object>(true, "If the email exists, a password reset link has been sent"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during forgot password for email: {Email}", dto.Email);
                return StatusCode(500, new ApiResponse<object>(false, "An error occurred while processing your request"));
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
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

                var user = await _userManager.FindByIdAsync(dto.UserId);
                if (user == null)
                {
                    return BadRequest(new ApiResponse<object>(false, "Invalid reset request"));
                }

                var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(new ApiResponse<object>(false, "Password reset failed", errors));
                }

                // Reset failed login attempts
                await _userManager.ResetAccessFailedCountAsync(user);

                _logger.LogInformation("Password reset successful for user: {UserId}", dto.UserId);
                return Ok(new ApiResponse<object>(true, "Password has been reset successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset for user: {UserId}", dto.UserId);
                return StatusCode(500, new ApiResponse<object>(false, "An error occurred during password reset"));
            }
        }

        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendEmailConfirmation([FromBody] ResendConfirmationDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>(false, "Invalid input"));
                }

                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                {
                    // For security, don't reveal if the email exists or not
                    return Ok(new ApiResponse<object>(true, "If the email exists, a confirmation link has been sent"));
                }

                if (user.EmailConfirmed)
                {
                    return BadRequest(new ApiResponse<object>(false, "Email is already confirmed"));
                }

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                
                try
                {
                    await _emailService.SendEmailConfirmationAsync(user.Email!, user.FullName, token, user.Id);
                    _logger.LogInformation("Email confirmation resent to: {Email}", dto.Email);
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Failed to resend confirmation email to: {Email}", dto.Email);
                    return StatusCode(500, new ApiResponse<object>(false, "Failed to send confirmation email"));
                }

                return Ok(new ApiResponse<object>(true, "If the email exists, a confirmation link has been sent"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during resend confirmation for email: {Email}", dto.Email);
                return StatusCode(500, new ApiResponse<object>(false, "An error occurred while processing your request"));
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>(false, "Invalid input"));
                }

                // This is a placeholder for refresh token logic
                // You would need to implement refresh token storage and validation
                // For now, we'll just validate the user still exists and generate a new token

                var user = await _userManager.FindByIdAsync(dto.UserId);
                if (user == null)
                {
                    return BadRequest(new ApiResponse<object>(false, "Invalid refresh request"));
                }

                if (!user.EmailConfirmed)
                {
                    return BadRequest(new ApiResponse<object>(false, "Email not confirmed"));
                }

                var roles = await _userManager.GetRolesAsync(user);
                var jwtService = HttpContext.RequestServices.GetRequiredService<JwtService>();
                var newToken = jwtService.GenerateToken(user, roles);

                _logger.LogInformation("Token refreshed for user: {UserId}", dto.UserId);
                return Ok(new ApiResponse<string>(true, "Token refreshed successfully", newToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh for user: {UserId}", dto.UserId);
                return StatusCode(500, new ApiResponse<object>(false, "An error occurred during token refresh"));
            }
        }
    }
}