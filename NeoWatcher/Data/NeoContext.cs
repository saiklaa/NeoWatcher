using Microsoft.EntityFrameworkCore;
using NeoWatcher.Models;

public class NeoContext : DbContext
{
    public NeoContext(DbContextOptions<NeoContext> options)
        : base(options)
    {
    }

    public DbSet<Asteroid> Asteroids => Set<Asteroid>();

    public DbSet<CloseApproach> CloseApproaches => Set<CloseApproach>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Asteroid>().HasKey(x => x.Id);

        modelBuilder.Entity<CloseApproach>().HasKey(x => x.Id);
        modelBuilder.Entity<CloseApproach>()
            .HasIndex(x => x.CloseApproachDate);
        modelBuilder.Entity<CloseApproach>()
            .HasIndex(x => new { x.AsteroidId, x.CloseApproachDate })
            .IsUnique();
        modelBuilder.Entity<CloseApproach>()
            .HasOne(x => x.Asteroid)
            .WithMany(x => x.CloseApproaches)
            .HasForeignKey(x => x.AsteroidId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}
