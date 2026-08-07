using Microsoft.EntityFrameworkCore;
using NeoWatcher.Models;

public class NeoContext : DbContext
{
    public NeoContext(DbContextOptions<NeoContext> options)
        : base(options)
    {
    }

    public DbSet<NearEarthObject> NearEarthObjects => Set<NearEarthObject>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NearEarthObject>().HasKey(x => x.Id);
        modelBuilder.Entity<NearEarthObject>().HasIndex(x => x.CloseApproachDate);
        base.OnModelCreating(modelBuilder);
    }
}