using TimeSeries.Core.Exceptions;

namespace TimeSeries.Core.TimeSeries;

internal class TimeSeriesService : ITimeSeriesService
{
    private readonly ITimeSeriesRepository _repository;
    private readonly TimeSeriesRecordValidator _validator;

    public TimeSeriesService(ITimeSeriesRepository repository)
    {
        _repository = repository;
        _validator = new TimeSeriesRecordValidator();
    }

    public Task<TimeSeriesRecord> CreateAsync(TimeSeriesRecord record)
    {
        if (record is null)
            throw new ArgumentNullException(nameof(record));

        _validator.Validate(record);

        var created = _repository.CreateAsync(record);
        return created;
    }

    public async Task<TimeSeriesRecord> GetAsync(Guid id)
    {
        var record = await _repository.GetAsync(id);
        if (record is null)
            throw new NotFoundException($"Time series record '{id}' was not found.");

        return record;
    }

    public Task<IReadOnlyList<TimeSeriesRecord>> GetAsync(TimeSeriesFilter filter)
    {
        if (filter is null)
            throw new ArgumentNullException(nameof(filter));

        var errors = new ValidationErrors();

        if (filter.From is not null && filter.From.Value.Offset != TimeSpan.Zero)
            errors.Add(nameof(filter.From), "From must be in UTC.");

        if (filter.To is not null && filter.To.Value.Offset != TimeSpan.Zero)
            errors.Add(nameof(filter.To), "To must be in UTC.");

        if (filter.From is not null && filter.To is not null && filter.From >= filter.To)
            errors.Add(nameof(filter.From), "From must be before To.");

        errors.ThrowIfAny();

        var recordsTask = _repository.GetAsync(filter);
        return recordsTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var success = await _repository.DeleteAsync(id);
        if (!success)
            throw new NotFoundException($"Time series record '{id}' was not found.");
    }

    public async Task<TimeSeriesRecord> UpdateAsync(Guid id, TimeSeriesRecord record)
    {
        if (record is null)
            throw new ArgumentNullException(nameof(record));

        _validator.Validate(record);

        var updated = await _repository.UpdateAsync(id, record);

        if (updated is null)
            throw new NotFoundException($"Time series record '{id}' was not found.");

        return updated;
    }
}
