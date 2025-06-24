namespace api.Models
{
    public class PlayerSession
    {
        public string PlayerId { get; set; } = string.Empty;
        public Player? Player { get; set; }

        public int SessionId { get; set; }
        public Session? Session { get; set; }
    }
}

