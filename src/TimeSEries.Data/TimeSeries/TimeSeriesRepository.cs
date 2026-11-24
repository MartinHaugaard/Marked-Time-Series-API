using TimeSeries.Core.TimeSeries;

namespace TimeSeries.Data.TimeSeries
{
    public class TimeSeriesRepository: ITimeSeriesRepository
    {
        public TimeSeriesRecord Create(TimeSeriesRecord record)
        {
            throw new NotImplementedException();
        }

        public TimeSeriesRecord? Get(Guid id)
        {
            return null;
        }
    }
}
