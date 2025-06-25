using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    public class RegisterDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string FullName { get; set; } = string.Empty;
        public string? Position { get; set; }
        public string? Team { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [Required]
        public string Role { get; set; } = string.Empty;

        public int? ClubId { get; set; }
        public int? TeamId { get; set; }
    }
}