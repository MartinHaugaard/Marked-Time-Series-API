using TimeSeries.Core.Exceptions;

namespace TimeSeries.Core.TimeSeries
{
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
    }
}
