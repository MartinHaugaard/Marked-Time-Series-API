namespace TimeSeries.Core.TimeSeries;

public class TimeSeriesFilter
{
    public BiddingZone? BiddingZone { get; set; }

    public DateTimeOffset? From { get; set; }

    public DateTimeOffset? To { get; set; }
}
