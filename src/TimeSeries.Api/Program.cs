using System.Text.Json.Serialization;
using TimeSeries.Api.ErrorHandling;
using TimeSeries.Core;
using TimeSeries.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter(allowIntegerValues: false)));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add TimeSeries.Data
builder.Services.AddData(builder.Configuration.GetConnectionString("TimeSeries")!);

// Add TimeSeries.Core
builder.Services.AddCore();

// Deal with 'expected' exceptions such as not finding a record by id.
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<CoreExceptionHandler>();

var app = builder.Build();

app.Services.InitializeDatabase();
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// app.UseAuthorization();

app.MapControllers();

app.Run();