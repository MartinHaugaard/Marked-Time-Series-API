using Microsoft.EntityFrameworkCore;
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

        public async Task<IReadOnlyList<TimeSeriesRecord>> GetAsync(TimeSeriesFilter filter)
        {
            var query = db.TimeSeries.AsNoTracking();

            if (filter.BiddingZone is not null)
                query = query.Where(e => e.BiddingZone == filter.BiddingZone.Value);

            if (filter.From is not null)
                query = query.Where(e => e.Timestamp >= filter.From.Value);

            if (filter.To is not null)
                query = query.Where(e => e.Timestamp < filter.To.Value);

            var entities = await query.OrderBy(e => e.Timestamp).ToListAsync();
            return entities.Select(e => e.ToRecord()).ToList();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await db.TimeSeries.FindAsync(id);
            if (entity is null)
                return false;

            db.TimeSeries.Remove(entity);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<TimeSeriesRecord?> UpdateAsync(Guid id, TimeSeriesRecord record)
        {
            var entity = await db.TimeSeries.FindAsync(id);
            if (entity is null)
                return null;

            db.Entry(entity).CurrentValues.SetValues(record.ToEntity(id));
            await db.SaveChangesAsync();

            return entity.ToRecord();
        }
    }
}
