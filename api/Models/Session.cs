using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class Session
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string? Location { get; set; }
        public int Intensity { get; set; }
        [Required]
        public string CoachId { get; set; } = string.Empty;
        [ForeignKey(nameof(CoachId))]
        public ApplicationUser? Coach { get; set; }
    }
}
