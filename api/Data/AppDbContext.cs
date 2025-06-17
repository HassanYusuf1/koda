using Microsoft.EntityFrameworkCore;
using api.Models;

namespace NextPlay.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Player> Players => Set<Player>();
        public DbSet<Session> Sessions => Set<Session>();
        public DbSet<Report> Reports => Set<Report>();
        public DbSet<PlayerSession> PlayerSessions => Set<PlayerSession>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PlayerSession>()
                .HasKey(ps => new { ps.PlayerId, ps.SessionId });

            modelBuilder.Entity<PlayerSession>()
                .HasOne(ps => ps.Player)
                .WithMany()
                .HasForeignKey(ps => ps.PlayerId);

            modelBuilder.Entity<PlayerSession>()
                .HasOne(ps => ps.Session)
                .WithMany()
                .HasForeignKey(ps => ps.SessionId);
        }
    }

    public class PlayerSession
    {
        public string PlayerId { get; set; } = string.Empty;
        public Player? Player { get; set; }

        public int SessionId { get; set; }
        public Session? Session { get; set; }
    }
}