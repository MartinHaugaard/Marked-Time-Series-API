using TimeSeries.Core.TimeSeries;
using TimeSeries.Data.Mapping;

namespace TimeSeries.Data.TimeSeries
{
    internal class TimeSeriesRepository(TimeSeriesDbContext db) : ITimeSeriesRepository
    {
        public async Task<TimeSeriesRecord> CreateAsync(TimeSeriesRecord record)
        {
            var entity = record.ToEntity(Guid.NewGuid());
            db.TimeSeries.Add(entity);
            await db.SaveChangesAsync();
            return entity.ToRecord();
        }

        public async Task<TimeSeriesRecord?> GetAsync(Guid id)
        {
            var entity = await db.TimeSeries.FindAsync(id);
            return entity?.ToRecord();
        }
    }
}
