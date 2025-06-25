using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class Team
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public int ClubId { get; set; }
        public Club? Club { get; set; }
        public ICollection<Player> Players { get; set; } = new List<Player>();
    }
}