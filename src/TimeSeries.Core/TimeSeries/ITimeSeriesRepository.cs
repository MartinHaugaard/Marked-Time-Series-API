namespace TimeSeries.Core.TimeSeries
{
    public interface ITimeSeriesRepository
    {
        public Task<TimeSeriesRecord> CreateAsync(TimeSeriesRecord record);

        public Task<TimeSeriesRecord?> GetAsync(Guid id);

        public Task<bool> DeleteAsync(Guid id);

        public Task<TimeSeriesRecord?> UpdateAsync(Guid id, TimeSeriesRecord record);
    }
}
