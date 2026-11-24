namespace TimeSeries.Core.TimeSeries
{
    public interface ITimeSeriesRepository
    {
        public TimeSeriesRecord Create(TimeSeriesRecord record);

        public TimeSeriesRecord? Get(Guid id);
    }
}
