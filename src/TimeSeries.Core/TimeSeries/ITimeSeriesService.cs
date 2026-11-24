namespace TimeSeries.Core.TimeSeries
{
    public interface ITimeSeriesService
    {
        public Task<TimeSeriesRecord> CreateAsync(TimeSeriesRecord record);
        public Task<TimeSeriesRecord> GetAsync(Guid id);
        public Task DeleteAsync(Guid id);
    }
}
