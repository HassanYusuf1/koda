using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;
        [Required]
        [MinLength(8)]
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
        [RegularExpression("^(PlatformAdmin|ClubAdmin|Coach|Player)$", ErrorMessage = "Role must be one of: PlatformAdmin, ClubAdmin, Coach, Player")]
        public string Role { get; set; } = string.Empty;

        public string? ClubName { get; set; }

        public int? ClubId { get; set; }

        public int? TeamId { get; set; }

        public string? InvitationCode { get; set; }
    }
}
