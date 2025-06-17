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
    }
}
