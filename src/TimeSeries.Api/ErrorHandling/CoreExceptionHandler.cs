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
        var (status, title) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
            RecordValidationException => (StatusCodes.Status400BadRequest, "Validation failed for Record"),
            _ => (0, null)
        };

        if (status == 0)
            return false; // not ours, let the default handling produce a 500

        httpContext.Response.StatusCode = status;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = exception.Message
            }
        });
    }
}
