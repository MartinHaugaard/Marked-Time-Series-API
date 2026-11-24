using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TimeSeries.Core.TimeSeries;
using TimeSeries.Data.TimeSeries;

namespace TimeSeries.Data
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddData(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<TimeSeriesDbContext>(o => o.UseSqlite(connectionString));
            services.AddScoped<ITimeSeriesRepository, TimeSeriesRepository>();
            return services;
        }
    }
}
