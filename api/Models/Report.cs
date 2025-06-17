using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class Report
    {
        public int Id { get; set; }
        [Required]
        public string PlayerId { get; set; } = string.Empty;
        [ForeignKey(nameof(PlayerId))]
        public ApplicationUser? Player { get; set; }
        [Required]
        public int SessionId { get; set; }
        [ForeignKey(nameof(SessionId))]
        public Session? Session { get; set; }
        public int EffortLevel { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? InjuryStatus { get; set; }
        public string? VideoUrl { get; set; }
    }
}
