using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        public string? Position { get; set; }
        public string? Team { get; set; }
        public DateTime? DateOfBirth { get; set; }

        // Role is stored both in Identity and here for convenience for the frontend
        public string Role { get; set; } = string.Empty;

        // Relations to club and team (nullable as not all users belong to one)
        public int? ClubId { get; set; }
        public Club? Club { get; set; }

        public int? TeamId { get; set; }
        public Team? TeamEntity { get; set; }
    }
}