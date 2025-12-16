using Microsoft.EntityFrameworkCore;
using TimeSpot.Infrastructure.Entities;

namespace TimeSpot.Infrastructure.Data;

public class WorldTimeDbContext : DbContext
{
    public WorldTimeDbContext(DbContextOptions<WorldTimeDbContext> options)
        : base(options)
    {
    }

    public DbSet<CityEntity> Cities { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CityEntity>(entity =>
        {
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.Country);
        });
    }
}
