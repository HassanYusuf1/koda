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
        public DbSet<Club> Clubs { get; set; } = default!;
        public DbSet<Team> Teams { get; set; } = default!;
        public DbSet<Invite> Invites { get; set; } = default!;

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

            modelBuilder.Entity<Team>()
                .HasOne(t => t.Club)
                .WithMany(c => c.Teams)
                .HasForeignKey(t => t.ClubId);

            modelBuilder.Entity<Player>()
                .HasOne(p => p.Team)
                .WithMany(t => t.Players)
                .HasForeignKey(p => p.TeamId);

            modelBuilder.Entity<Session>()
                .HasOne(s => s.Team)
                .WithMany()
                .HasForeignKey(s => s.TeamId);

            modelBuilder.Entity<Invite>()
                .HasOne(i => i.Club)
                .WithMany()
                .HasForeignKey(i => i.ClubId);

            modelBuilder.Entity<Invite>()
                .HasOne(i => i.Team)
                .WithMany()
                .HasForeignKey(i => i.TeamId);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.TeamEntity)
                .WithMany()
                .HasForeignKey(u => u.TeamId);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Club)
                .WithMany()
                .HasForeignKey(u => u.ClubId);
        }
    }
}