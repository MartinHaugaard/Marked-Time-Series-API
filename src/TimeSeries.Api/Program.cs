using System.Text.Json.Serialization;
using TimeSeries.Api.Binding;
using TimeSeries.Api.ErrorHandling;
using TimeSeries.Core;
using TimeSeries.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Timestamps must state their offset, both in bodies and query strings.
builder.Services.AddControllers(o => o.ModelBinderProviders.Insert(0, new ExplicitOffsetModelBinderProvider()))
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
        o.JsonSerializerOptions.Converters.Add(new ExplicitOffsetJsonConverter());
    });

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

// Lets the integration tests reference the app through WebApplicationFactory<Program>.
public partial class Program;