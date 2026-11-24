# Time Series API

## What is this?

This is an API to showcase a simplified Time Series endpoint for use in the electricity industry.

## How to run

The code is written targeting .NET 10 SDK and can be run by calling `dotnet run --project src/TimeSeries.Api`

The SQLite database is created automatically on startup. The database currently starts empty, making the create endpoint a good starting point for testing.

**note** if the schema changes, it's best to delete the timeseries.db file and restart the service.

The tests are executed with `dotnet test`.

The unit tests cover mainly the core service class, that holds business logic, including the validation logic and documents the expected behavior for scenarios like missing records, or validation violations.

The integration tests run the full API in memory via `WebApplicationFactory`, each against its own in-memory SQLite database, covering status codes, error responses and filtering.

To experience the code manually TimeSeries.Api.http contains example requests.


## Endpoints

| Method | Path | Description | Responses |
|---|---|---|---|
| POST | `/api/timeseries` | Create a record | 201, 400 |
| GET | `/api/timeseries/{id}` | Get a record by ID | 200, 404 |
| PUT | `/api/timeseries/{id}` | Update a record | 200, 400, 404 |
| DELETE | `/api/timeseries/{id}` | Delete a record | 204, 404 |
| GET | `/api/timeseries` | List records, optionally filtered | 200, 400 |

Example request body:

```json
{
  "timestamp": "2026-11-21T12:00:00Z",
  "type": "Production",
  "biddingZone": "DK1",
  "energySource": "Wind",
  "value": 460.5
}
```

**Filtering:** `GET /api/timeseries` accepts the optional query parameters
`biddingZone`, `from` and `to`. All are optional; without any, all records are returned.
`from` is inclusive and `to` is exclusive, so `from=2026-11-21T00:00:00Z&to=2026-11-22T00:00:00Z`
returns exactly one day. Timestamps must be in UTC (`Z`); `from` must be before `to`.
The `biddingZone` filter matches records where the zone is either `biddingZone` or `toBiddingZone`, so a flow shows up for both zones involved.

## The Design

The application features simple CRUD operations, for creating, reading, updating and deleting "TimeSeries" records.

### Assumptions

I don't have a background in the electricity market, so the model is based on my own reading of the assignment. The assumptions below (hourly resolution, MWh, the three record types, the zones listed) are simplifications I'd want to check with domain experts before building further.

One key assumption is that records work on an hourly basis, at the start of the hour, and that all time is in UTC. Non-UTC timestamps will get rejected, and so will timestamps without an offset, since those would otherwise be read in the server's local time zone.

All values are in MWh.

The flow goes from BiddingZone to ToBiddingZone, and the direction is expressed by zones, meaning the value may not be negative.

The server assigns an ID on creation. It's returned in the response body and the Location header.

Enums are sent and returned as strings.

### Validation Rules

Most validations can be inspected in the TimeSeriesRecordValidator.cs file. All violations are reported, grouped by field.

Timestamps must include an offset, be in UTC and be on a whole hour.

The value may not be negative.

A flow requires a ToBiddingZone. The BiddingZone and ToBiddingZone can't be the same value. Flow can't have an EnergySource.

Production requires an EnergySource and can't have a ToBiddingZone.

Consumption can't have an EnergySource or ToBiddingZone.

### Architecture

The solution consists of an outer API layer with controllers, building on top of a core library holding the essential services and interfaces for repositories.

Data is stored in SQLite via Entity Framework Core.

DTO, domain model and EF Entities are kept separate. This allows for the projects to be changed independently, at the cost of some mapping logic.

Implementations of libraries are internal to them, and exposed through thin AddCore() / AddData functions, exposing minimal implementation details.

Central package management to keep nuget package versions aligned.

### API behavior

PUT is a full replacement; omitted optional fields are cleared.

Delete of unknown records gives 404.

A filter with no matches returns 200 with an empty list.

### Error handling

Requests for unknown records return 404 as ProblemDetails.

Invalid input returns 400 as ValidationProblemDetails, with an `errors` object listing the messages per field. This is the same shape ASP.NET Core uses for malformed requests (e.g. a missing field or an unknown enum value), so clients only have to handle one 400 format.

Internally, Core throws custom exceptions, which an exception handler in the API layer maps to these status codes.

## What I'd do next
EF-migrations instead of EnsureCreated and a seeded start.

Some sort of Authentication, for example being able to read shouldn't always make you able to create or delete data.

Duplicate detection: currently nothing prevents multiple records for the same zone, type and timestamp.

Pagination or maximum interval on list endpoint.

CI/CD if it was to be called for real.