using Microsoft.Extensions.DependencyInjection;
using TimeSeries.Data.TimeSeries;

namespace TimeSeries.Data;

public static class InitializeDb
{
    public static void InitializeDatabase(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TimeSeriesDbContext>();
        db.Database.EnsureCreated();
    }
}
