using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using TimeSeries.Api.Dtos;

namespace TimeSeries.Tests.Api;

/// <summary>
/// End-to-end tests through HTTP, covering routing, model binding, error handling and the database query.
/// Each test gets a fresh app and database.
/// </summary>
public class TimeSeriesApiTests : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly TimeSeriesApiFactory _factory = new();
    private readonly HttpClient _client;

    public TimeSeriesApiTests()
    {
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Fact]
    public async Task Create_ValidRecord_Returns201AndCanBeFetchedFromLocation()
    {
        // Act
        var response = await PostAsync(new { timestamp = "2026-11-21T12:00:00Z", type = "Production", biddingZone = "DK1", energySource = "Wind", value = 460.5 });

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await ReadAsync<TimeSeriesResponse>(response);

        var fetched = await ReadAsync<TimeSeriesResponse>(await _client.GetAsync(response.Headers.Location));
        Assert.Equal(created.Id, fetched.Id);
        Assert.Equal(460.5m, fetched.Value);
    }

    [Fact]
    public async Task Get_UnknownId_Returns404ProblemDetails()
    {
        // Act
        var response = await _client.GetAsync($"/api/timeseries/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problem = await ReadAsync<ProblemDetails>(response);
        Assert.Equal(404, problem.Status);
    }

    [Fact]
    public async Task Create_InvalidRecord_Returns400WithEveryViolationPerField()
    {
        // Act: a flow without a target zone, and a negative value
        var response = await PostAsync(new { timestamp = "2026-11-21T12:00:00Z", type = "Flow", biddingZone = "DK1", value = -5 });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await ReadAsync<ValidationProblemDetails>(response);
        Assert.Contains("ToBiddingZone", problem.Errors.Keys);
        Assert.Contains("Value", problem.Errors.Keys);
    }

    [Fact]
    public async Task Create_TimestampWithoutOffset_Returns400()
    {
        // Act
        var response = await PostAsync(new { timestamp = "2026-11-21T12:00:00", type = "Consumption", biddingZone = "DK1", value = 1 });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        // Rejected while reading the body, not by the UTC rule, so it doesn't depend on the server's time zone.
        var problem = await ReadAsync<ValidationProblemDetails>(response);
        Assert.Contains("offset", Assert.Single(problem.Errors["$.timestamp"]));
    }

    [Fact]
    public async Task List_FilterWithoutOffset_Returns400()
    {
        // Act
        var response = await _client.GetAsync("/api/timeseries?from=2026-11-21T00:00:00");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await ReadAsync<ValidationProblemDetails>(response);
        Assert.Contains("offset", Assert.Single(problem.Errors["From"]));
    }

    [Fact]
    public async Task List_FilterByZone_IncludesFlowsInBothDirections()
    {
        // Arrange
        await PostAsync(new { timestamp = "2026-11-21T12:00:00Z", type = "Flow", biddingZone = "DK2", toBiddingZone = "DK1", value = 5 });
        await PostAsync(new { timestamp = "2026-11-21T12:00:00Z", type = "Flow", biddingZone = "DK1", toBiddingZone = "DE", value = 7 });
        await PostAsync(new { timestamp = "2026-11-21T12:00:00Z", type = "Consumption", biddingZone = "DK2", value = 9 });

        // Act
        var records = await ReadAsync<List<TimeSeriesResponse>>(await _client.GetAsync("/api/timeseries?biddingZone=DK1"));

        // Assert
        Assert.Equal([5m, 7m], records.Select(r => r.Value).Order());
    }

    [Fact]
    public async Task List_FilterByInterval_FromIsInclusiveAndToIsExclusive()
    {
        // Arrange
        foreach (var hour in new[] { "2026-11-20T23:00:00Z", "2026-11-21T00:00:00Z", "2026-11-21T23:00:00Z", "2026-11-22T00:00:00Z" })
            await PostAsync(new { timestamp = hour, type = "Consumption", biddingZone = "DK1", value = 1 });

        // Act
        var records = await ReadAsync<List<TimeSeriesResponse>>(
            await _client.GetAsync("/api/timeseries?from=2026-11-21T00:00:00Z&to=2026-11-22T00:00:00Z"));

        // Assert
        Assert.Equal(
            [new DateTimeOffset(2026, 11, 21, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2026, 11, 21, 23, 0, 0, TimeSpan.Zero)],
            records.Select(r => r.Timestamp));
    }

    [Fact]
    public async Task Update_UnknownId_Returns404()
    {
        // Act
        var response = await _client.PutAsJsonAsync($"/api/timeseries/{Guid.NewGuid()}",
            new { timestamp = "2026-11-21T12:00:00Z", type = "Consumption", biddingZone = "DK1", value = 1 });

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingRecord_Returns204AndRecordIsGone()
    {
        // Arrange
        var created = await PostAsync(new { timestamp = "2026-11-21T12:00:00Z", type = "Consumption", biddingZone = "DK1", value = 1 });

        // Act
        var response = await _client.DeleteAsync(created.Headers.Location);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync(created.Headers.Location)).StatusCode);
    }

    private Task<HttpResponseMessage> PostAsync(object body) => _client.PostAsJsonAsync("/api/timeseries", body);

    private static async Task<T> ReadAsync<T>(HttpResponseMessage response)
        => (await response.Content.ReadFromJsonAsync<T>(JsonOptions))!;
}
