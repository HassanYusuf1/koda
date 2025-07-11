using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class Player
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [ForeignKey(nameof(Id))]
        public ApplicationUser? User { get; set; }

        public int? TeamId { get; set; }
        public Team? Team { get; set; }

        public ICollection<PlayerSession> PlayerSessions { get; set; } = new List<PlayerSession>();
        

    }
}