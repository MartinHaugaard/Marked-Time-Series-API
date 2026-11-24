using TimeSeries.Core.TimeSeries;

namespace TimeSeries.Api.Dtos;

public class TimeSeriesQuery
{
    public BiddingZone? BiddingZone { get; set; }

    public DateTimeOffset? From { get; set; }

    public DateTimeOffset? To { get; set; }
}
