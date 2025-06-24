using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using api.Models;

namespace api.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Player> Players { get; set; } = default!;
        public DbSet<PlayerSession> PlayerSessions { get; set; } = default!;
        public DbSet<Session> Sessions { get; set; } = default!;
        public DbSet<Report> Reports { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PlayerSession>()
                .HasKey(ps => new { ps.PlayerId, ps.SessionId });

            modelBuilder.Entity<PlayerSession>()
                .HasOne(ps => ps.Player)
                .WithMany(p => p.PlayerSessions)
                .HasForeignKey(ps => ps.PlayerId);

            modelBuilder.Entity<PlayerSession>()
                .HasOne(ps => ps.Session)
                .WithMany()
                .HasForeignKey(ps => ps.SessionId);
        }
    }
}