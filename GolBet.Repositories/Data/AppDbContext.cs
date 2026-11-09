// GolBet.Repositories/Data/AppDbContext.cs
using GolBet.Entities;
using GolBet.Entities.Common;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Repositories.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<Bet> Bets => Set<Bet>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);   // PRIMERO: Identity mapea sus 6 tablas aquí

        // Nombres de equipos únicos (sin distinción de mayúsculas/minúsculas ni tildes)
        modelBuilder.Entity<Team>()
            .Property(t => t.Name)
            .UseCollation("SQL_Latin1_General_CP1_CI_AI");

        modelBuilder.Entity<Team>()
            .HasIndex(t => t.Name)
            .IsUnique();

        // Relación doble Match -> Team (Local y Visitante)
        modelBuilder.Entity<Match>()
            .HasOne(m => m.HomeTeam)
            .WithMany()
            .HasForeignKey(m => m.HomeTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Match>()
            .HasOne(m => m.AwayTeam)
            .WithMany()
            .HasForeignKey(m => m.AwayTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        // Un partido con apuestas asociadas no se puede eliminar
        modelBuilder.Entity<Bet>()
            .HasOne(b => b.Match)
            .WithMany(m => m.Bets)
            .OnDelete(DeleteBehavior.Restrict);

        // ---- Agregado en el Módulo 7: Identity ----

        // Bet -> User: las apuestas de un usuario no se destruyen si se borra el usuario
        modelBuilder.Entity<Bet>()
            .HasOne(b => b.User)
            .WithMany(u => u.Bets)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Regla de negocio #3: un mismo usuario no puede repetir la misma selección en un partido
        modelBuilder.Entity<Bet>()
            .HasIndex(b => new { b.MatchId, b.UserId, b.Pick })
            .IsUnique();

        // Precisión del saldo en FutCoins (hasta 9,999,999,999.99)
        modelBuilder.Entity<AppUser>()
            .Property(u => u.Balance)
            .HasPrecision(12, 2);
    }

    // Auditoría automática de fechas de creación y modificación
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = utcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.ModifiedDate = utcNow;
                    entry.Property(e => e.CreatedDate).IsModified = false;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}