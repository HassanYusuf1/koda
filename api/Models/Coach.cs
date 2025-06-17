using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class Coach
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}