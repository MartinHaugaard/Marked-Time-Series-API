using TimeSeries.Core.Exceptions;

namespace TimeSeries.Core.TimeSeries;

internal class TimeSeriesRecordValidator
{
    internal void Validate(TimeSeriesRecord record)
    {
        var errors = new ValidationErrors();

        if (record.Value < 0)
            errors.Add(nameof(record.Value), "Value can't be negative.");

        if (record.Timestamp.Offset != TimeSpan.Zero)
            errors.Add(nameof(record.Timestamp), "Timestamp must be in UTC.");

        if (record.Timestamp.Ticks % TimeSpan.TicksPerHour != 0)
            errors.Add(nameof(record.Timestamp), "Timestamp must be on a whole hour.");

        switch (record.Type)
        {
            case RecordType.Flow:
                if (record.ToBiddingZone is null)
                    errors.Add(nameof(record.ToBiddingZone), "ToBiddingZone can't be empty for flow.");

                if (record.BiddingZone == record.ToBiddingZone)
                    errors.Add(nameof(record.ToBiddingZone), "BiddingZone must differ from ToBiddingZone.");

                if (record.EnergySource is not null)
                    errors.Add(nameof(record.EnergySource), "EnergySource can't be specified for flow.");
                break;

            case RecordType.Production:
                if (record.EnergySource is null)
                    errors.Add(nameof(record.EnergySource), "EnergySource can't be empty for production.");

                if (record.ToBiddingZone is not null)
                    errors.Add(nameof(record.ToBiddingZone), "ToBiddingZone can't be specified for production.");
                break;

            case RecordType.Consumption:
                if (record.EnergySource is not null)
                    errors.Add(nameof(record.EnergySource), "EnergySource can't be specified for consumption.");

                if (record.ToBiddingZone is not null)
                    errors.Add(nameof(record.ToBiddingZone), "ToBiddingZone can't be specified for consumption.");
                break;

            default:
                errors.Add(nameof(record.Type), $"Unknown record type '{record.Type}'.");
                break;
        }

        errors.ThrowIfAny();
    }
}
