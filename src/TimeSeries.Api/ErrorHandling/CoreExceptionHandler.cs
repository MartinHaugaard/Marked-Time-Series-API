using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TimeSeries.Core.Exceptions;

namespace TimeSeries.Api.ErrorHandling;

/// <summary>
/// A handler that takes 'expected' Exceptions from domain, and turns them into meaningful http responses.
/// </summary>
internal sealed class CoreExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails? problemDetails = exception switch
        {
            NotFoundException => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Resource not found",
                Detail = exception.Message
            },
            // Same shape as the framework's own model validation errors, so clients only handle one 400 format.
            RecordValidationException validation => new ValidationProblemDetails(
                validation.Errors.ToDictionary(e => e.Key, e => e.Value))
            {
                Status = StatusCodes.Status400BadRequest
            },
            _ => null
        };

        if (problemDetails is null)
            return false; // not ours, let the default handling produce a 500

        httpContext.Response.StatusCode = problemDetails.Status!.Value;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails
        });
    }
}
