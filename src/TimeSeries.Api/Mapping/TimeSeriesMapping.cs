using TimeSeries.Api.Dtos;
using TimeSeries.Core.TimeSeries;

namespace TimeSeries.Api.Mapping;

public static class TimeSeriesMappings
{
    public static TimeSeriesRecord ToModel(this CreateTimeSeriesRequest request)
    {
        return new TimeSeriesRecord
        {
            Timestamp = request.Timestamp,
            BiddingZone = request.BiddingZone,
            ToBiddingZone = request.ToBiddingZone,
            Type = request.Type,
            EnergySource = request.EnergySource,
            Value = request.Value
        };
    }

    public static TimeSeriesRecord ToModel(this UpdateTimeSeriesRequest request)
    {
        return new TimeSeriesRecord
        {
            Timestamp = request.Timestamp,
            BiddingZone = request.BiddingZone,
            ToBiddingZone = request.ToBiddingZone,
            Type = request.Type,
            EnergySource = request.EnergySource,
            Value = request.Value
        };
    }

    public static TimeSeriesResponse ToResponse(this TimeSeriesRecord record)
    {
        return new TimeSeriesResponse
        {
            Id = record.Id ?? throw new InvalidOperationException("Record has no id"),
            Timestamp = record.Timestamp,
            BiddingZone = record.BiddingZone,
            ToBiddingZone = record.ToBiddingZone,
            Type = record.Type,
            EnergySource = record.EnergySource,
            Value = record.Value

        };
    }

    public static TimeSeriesFilter ToFilter(this TimeSeriesQuery query)
    {
        return new TimeSeriesFilter
        {
            To = query.To,
            From = query.From,
            BiddingZone = query.BiddingZone
        };
    }

    public static List<TimeSeriesResponse> ToResponseList(this IEnumerable<TimeSeriesRecord> records)
        => records.Select(r => r.ToResponse()).ToList();
}