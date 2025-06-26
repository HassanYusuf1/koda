using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]", 
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full name is required")]
        [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "Position cannot exceed 50 characters")]
        public string? Position { get; set; }

        [MaxLength(100, ErrorMessage = "Team name cannot exceed 100 characters")]
        public string? Team { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("^(Player|Coach|Admin)$", ErrorMessage = "Role must be Player, Coach, or Admin")]
        public string Role { get; set; } = string.Empty;

        public int? ClubId { get; set; }
        public int? TeamId { get; set; }
    }
}