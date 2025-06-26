using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Position { get; set; }

        [MaxLength(100)]
        public string? Team { get; set; }

        public DateTime? DateOfBirth { get; set; }

        // Note: Consider removing this as roles are handled by Identity
        [MaxLength(20)]
        public string Role { get; set; } = string.Empty;

        // Foreign key relationships
        public int? ClubId { get; set; }
        public Club? Club { get; set; }

        public int? TeamId { get; set; }
        public Team? TeamEntity { get; set; }
    }
}