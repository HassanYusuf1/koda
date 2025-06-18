// Models/PlayerSession.cs
namespace api.Models
{
    public class PlayerSession
    {
        public int PlayerId { get; set; }
        public Player Player { get; set; }

        public int SessionId { get; set; }
        public Session Session { get; set; }
    }
}