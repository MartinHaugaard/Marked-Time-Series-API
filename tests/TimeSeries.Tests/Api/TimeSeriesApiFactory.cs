using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;

namespace TimeSeries.Tests.Api;

/// <summary>
/// Runs the real API in memory, backed by its own in-memory SQLite database.
/// </summary>
public class TimeSeriesApiFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString = $"Data Source=tests-{Guid.NewGuid()};Mode=Memory;Cache=Shared";
    private readonly SqliteConnection _keepAlive;

    public TimeSeriesApiFactory()
    {
        // An in-memory database only lives while a connection to it is open.
        _keepAlive = new SqliteConnection(_connectionString);
        _keepAlive.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:TimeSeries", _connectionString);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
            _keepAlive.Dispose();
    }
}
