using Microsoft.Extensions.DependencyInjection;
using TimeSeries.Core.TimeSeries;
using TimeSeries.Data.TimeSeries;

namespace TimeSeries.Data
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddData(this IServiceCollection services)
        {
            services.AddSingleton<ITimeSeriesRepository, TimeSeriesRepository>();

            return services;
        }
    }
}
