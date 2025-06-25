using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class Invite
    {
        public int Id { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = string.Empty;
        public int? ClubId { get; set; }
        public Club? Club { get; set; }
        public int? TeamId { get; set; }
        public Team? Team { get; set; }
        public string Token { get; set; } = Guid.NewGuid().ToString();
        public bool IsAccepted { get; set; } = false;
    }
}
