namespace TimeSeries.Core.TimeSeries;

public interface ITimeSeriesService
{
    public Task<TimeSeriesRecord> CreateAsync(TimeSeriesRecord record);
    public Task<TimeSeriesRecord> GetAsync(Guid id);
    Task<IReadOnlyList<TimeSeriesRecord>> GetAsync(TimeSeriesFilter filter);
    public Task DeleteAsync(Guid id);
    public Task<TimeSeriesRecord> UpdateAsync(Guid id, TimeSeriesRecord record);
}
