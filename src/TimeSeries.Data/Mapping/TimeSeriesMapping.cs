using TimeSeries.Core.TimeSeries;
using TimeSeries.Data.TimeSeries;

namespace TimeSeries.Data.Mapping;

public static class TimeSeriesMappings
{
    public static TimeSeriesRecord ToRecord(this TimeSeriesEntity entity)
    {
        return new TimeSeriesRecord
        {
            Id = entity.Id,
            Timestamp = entity.Timestamp,
            BiddingZone = entity.BiddingZone,
            ToBiddingZone = entity.ToBiddingZone,
            Type = entity.Type,
            EnergySource = entity.EnergySource,
            Value = entity.Value
        };
    }

    public static TimeSeriesEntity ToEntity(this TimeSeriesRecord record, Guid id)
    {
        return new TimeSeriesEntity
        {
            Id = id,
            Timestamp = record.Timestamp,
            BiddingZone = record.BiddingZone,
            ToBiddingZone = record.ToBiddingZone,
            Type = record.Type,
            EnergySource = record.EnergySource,
            Value = record.Value

        };
    }
}