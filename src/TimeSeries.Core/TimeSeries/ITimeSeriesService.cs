namespace TimeSeries.Core.TimeSeries
{
    public interface ITimeSeriesService
    {
        public TimeSeriesRecord Create(TimeSeriesRecord record);
        public TimeSeriesRecord Get(Guid id);
    }
}
