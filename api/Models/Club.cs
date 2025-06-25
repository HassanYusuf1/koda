using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class Club
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? CreatedByAdminId { get; set; }
        public ApplicationUser? CreatedByAdmin { get; set; }
        public bool IsSubscribed { get; set; } = false;
        public ICollection<Team> Teams { get; set; } = new List<Team>();
    }
}