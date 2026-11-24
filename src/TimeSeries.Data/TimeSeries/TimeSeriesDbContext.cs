using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TimeSeries.Data.TimeSeries;

internal class TimeSeriesDbContext(DbContextOptions<TimeSeriesDbContext> options) : DbContext(options)
{
    public DbSet<TimeSeriesEntity> TimeSeries => Set<TimeSeriesEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<TimeSeriesEntity>();

        entity.ToTable("TimeSeries");
        entity.HasKey(e => e.Id);
        // the repository assigns IDs
        entity.Property(e => e.Id).ValueGeneratedNever();

        // SQLite has no DateTimeOffset type; store as a sortable long
        entity.Property(e => e.Timestamp)
              .HasConversion(new DateTimeOffsetToBinaryConverter());

        // Readable enums in the database
        entity.Property(e => e.Type).HasConversion<string>();
        entity.Property(e => e.BiddingZone).HasConversion<string>();
        entity.Property(e => e.ToBiddingZone).HasConversion<string>();
        entity.Property(e => e.EnergySource).HasConversion<string>();

        // Supports the zone + interval filter
        entity.HasIndex(e => new { e.BiddingZone, e.Timestamp });
    }
}
