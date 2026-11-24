# Time Series API

## What is this?

This is an API to showcase a simplified Time Series endpoint for use in the electricity industry.

## How to run

The code is written targeting .NET 10 SDK and can be run by calling `dotnet run --project src/TimeSeries.Api`

The project features a handful of unit tests to be executed with `dotnet test`

To experience the code manually a series of calls are also available by using the TimeSeries.Api.http file

## Endpoints

## Endpoints

| Method | Path | Description | Responses |
|---|---|---|---|
| POST | `/api/timeseries` | Create a record | 201, 400 |
| GET | `/api/timeseries/{id}` | Get a record by ID | 200, 404 |
| DELETE | `/api/timeseries/{id}` | Delete a record | 204, 404 |

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


## The Design

The application features simple CRUD operations, for creating, reading, updating and deleting "TimeSeries" records.

### Assumptions

A few assumptions have been made regarding these records.

One key assumption is that they work on an hourly basis and that all time is in UTC.

A record gets an id assigned by the server when created which can be retrieved from the record in the reply of a successful created record.

### Validation Rules

Some validations are in place such as setting the BiddingZone and ToBiddingZone to the same value

### Architecture

The solution consists of an outer API layer with controllers, building on top of a core library holding the essential services and interfaces for repositories.

Data is stored in SQLite via Entity Framework Core.

### Error handling

Requests for unknown records return 404, and invalid records return 400. Both are returned as ProblemDetails. 

Internally, Core throws custom exceptions, which an exception handler in the API layer maps to these status codes.

## What I'd do next

Finish the CRUD operations (Update and fetch for interval)
