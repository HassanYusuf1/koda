using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class Coach
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        public int? TeamId { get; set; }
        public Team? Team { get; set; }

        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}