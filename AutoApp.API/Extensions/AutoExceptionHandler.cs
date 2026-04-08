using System.ComponentModel.DataAnnotations;
using AutoApp.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AutoApp.API.Extensions;

/// <summary>
/// Exception handler
/// </summary>
public class AutoExceptionHandler : IExceptionHandler
{
    /// <summary>
    /// Handle exceptions
    /// </summary>
    /// <param name="httpContext">HTTP Context</param>
    /// <param name="exception">Exception fired</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Bool value, also writes HTTP Response</returns>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            ValidationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };
        
        httpContext.Response.StatusCode = statusCode;
        var details = Factory();
        details.Title = "";
        details.Status = statusCode;
        details.Detail = exception.Message;
        details.Type = exception.GetType().Name;

        await httpContext.Response.WriteAsJsonAsync(details, cancellationToken: cancellationToken);
        return true;

        ProblemDetails Factory()
        {
            if (exception is ValidationException)
            {
                return new ValidationProblemDetails();
            }
            return new ProblemDetails();
        }
    }
}