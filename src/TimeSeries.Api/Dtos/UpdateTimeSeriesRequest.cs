using TimeSeries.Core.TimeSeries;

namespace TimeSeries.Api.Dtos;

public class UpdateTimeSeriesRequest
{
    public required DateTimeOffset Timestamp { get; set; }

    public required RecordType Type { get; set; }

    public required BiddingZone BiddingZone { get; set; }

    public BiddingZone? ToBiddingZone { get; set; }

    public EnergySource? EnergySource { get; set; }

    public required decimal Value { get; set; }
}
