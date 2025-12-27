using Microsoft.EntityFrameworkCore;
using TournamentApi.Models;

namespace TournamentApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Tournament> Tournaments { get; set; }
    public DbSet<Bracket> Brackets { get; set; }
    public DbSet<Match> Matches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Match>()
            .HasOne(m => m.Player1)
            .WithMany(u => u.MatchesAsPlayer1)
            .HasForeignKey(m => m.Player1Id)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        modelBuilder.Entity<Match>()
            .HasOne(m => m.Player2)
            .WithMany(u => u.MatchesAsPlayer2)
            .HasForeignKey(m => m.Player2Id)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        modelBuilder.Entity<Match>()
            .HasOne(m => m.Winner)
            .WithMany(u => u.MatchesAsWinner)
            .HasForeignKey(m => m.WinnerId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        modelBuilder.Entity<Match>()
            .HasOne(m => m.Bracket)
            .WithMany(b => b.Matches)
            .HasForeignKey(m => m.BracketId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        modelBuilder.Entity<Bracket>()
            .HasOne(b => b.Tournament)
            .WithOne(t => t.Bracket)
            .HasForeignKey<Bracket>(b => b.TournamentId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        modelBuilder.Entity<Tournament>()
            .HasMany(t => t.Participants)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "TournamentUser",
                j => j.HasOne<User>().WithMany().HasForeignKey("UserId"),
                j => j.HasOne<Tournament>().WithMany().HasForeignKey("TournamentId"),
                j => j.HasKey("TournamentId", "UserId"));
    }
}

