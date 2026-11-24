using TimeSeries.Core.TimeSeries;

namespace TimeSeries.Tests;

internal static class TestData
{
    public static TimeSeriesRecord ValidRecord() => new()
    {
        Timestamp = new DateTimeOffset(2026, 11, 21, 12, 0, 0, TimeSpan.Zero),
        Type = RecordType.Production,
        BiddingZone = BiddingZone.DK1,
        EnergySource = EnergySource.Wind,
        Value = 100m
    };
}