using TimeSeries.Core.Exceptions;

namespace TimeSeries.Core.TimeSeries;

internal class TimeSeriesRecordValidator
{
    internal void Validate(TimeSeriesRecord record)
    {
        if (record.Value < 0)
            throw new RecordValidationException("Value can't be negative.");

        if (record.Timestamp.Offset != TimeSpan.Zero)
            throw new RecordValidationException("Timestamp must be in UTC.");

        if (record.Timestamp.Ticks % TimeSpan.TicksPerHour != 0)
            throw new RecordValidationException("Timestamp must be on a whole hour.");

        switch (record.Type)
        {
            case RecordType.Flow:
                if (record.ToBiddingZone is null)
                    throw new RecordValidationException("ToBiddingZone can't be empty for flow.");

                if (record.BiddingZone == record.ToBiddingZone)
                    throw new RecordValidationException("BiddingZone must differ from ToBiddingZone.");

                if (record.EnergySource is not null)
                    throw new RecordValidationException("EnergySource can't be specified for flow.");
                break;

            case RecordType.Production:
                if (record.EnergySource is null)
                    throw new RecordValidationException("EnergySource can't be empty for production.");

                if (record.ToBiddingZone is not null)
                    throw new RecordValidationException("ToBiddingZone can't be specified for production.");
                break;

            case RecordType.Consumption:
                if (record.EnergySource is not null)
                    throw new RecordValidationException("EnergySource can't be specified for consumption.");

                if (record.ToBiddingZone is not null)
                    throw new RecordValidationException("ToBiddingZone can't be specified for consumption.");
                break;

            default:
                throw new RecordValidationException($"Unknown record type '{record.Type}'.");
        }
    }
}
