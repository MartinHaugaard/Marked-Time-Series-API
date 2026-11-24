# Time Series API

## What is this?

This is an API to showcase a simplified Time Series endpoint for use in the electricity industry.

## How to run

The code is written targeting .NET 10 SDK and can be run by calling `dotnet run --project src/TimeSeries.Api`

The SQLite database is created automatically on startup. The database currently starts empty, making the create endpoint a good starting point for testing.

**note** if the schema changes, it's best to delete the timeseries.db file and restart the service.

The project features a handful of unit tests to be executed with `dotnet test`

The unit tests cover mainly the core service class, that holds business logic, including the validation logic and documents the expected behavior for scenarios like missing records, or validation violations.

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

## The Design

The application features simple CRUD operations, for creating, reading, updating and deleting "TimeSeries" records.

### Assumptions

A few assumptions have been made regarding these records.

One key assumption is that records work on an hourly basis, at the start of the hour, and that all time is in UTC. Non-UTC timestamps will get rejected.

All values are in MWh.

The flow goes from BiddingZone to ToBiddingZone, and the direction is expressed by zones, meaning the value may not be negative.

The server assigns an ID on creation. It's returned in the response body and the Location header.

Enums are sent and returned as strings.

### Validation Rules

Most validations can be inspected in the TimeSeriesRecordValidator.cs file. Only the first triggered violation is reported.

Timestamps must be in UTC and on a whole hour.

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

Requests for unknown records return 404, and invalid records return 400. Both are returned as ProblemDetails. 

Internally, Core throws custom exceptions, which an exception handler in the API layer maps to these status codes.

## What I'd do next
EF-migrations instead of EnsureCreated and a seeded start.

Some sort of Authentication, for example being able to read shouldn't always make you able to create or delete data.

Duplicate detection: currently nothing prevents multiple records for the same zone, type and timestamp.

Pagination or maximum interval on list endpoint.

CI/CD if it was to be called for real.