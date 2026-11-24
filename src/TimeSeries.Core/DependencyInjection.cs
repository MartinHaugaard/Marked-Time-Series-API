using Microsoft.Extensions.DependencyInjection;
using TimeSeries.Core.TimeSeries;

namespace TimeSeries.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddScoped<ITimeSeriesService, TimeSeriesService>();

        return services;
    }
}
