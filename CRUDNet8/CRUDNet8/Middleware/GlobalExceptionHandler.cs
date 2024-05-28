using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;

namespace CRUDNet8.Middleware;

// https://code-maze.com/dotnet-use-iexceptionhandler-to-handle-exceptions/
// https://anthonygiretti.com/2023/06/14/asp-net-core-8-improved-exception-handling-with-iexceptionhandler/
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        logger.LogError($"An error occurred while processing your request: {exception.Message} on machine {Environment.MachineName}. TraceId: {traceId}");
        /*
                var errorResponse = new ErrorResponse
                {
                    Message = exception.Message
                };

                switch (exception)
                {
                    case BadHttpRequestException:
                        errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                        errorResponse.Title = exception.GetType().Name;
                        break;

                    default:
                        errorResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                        errorResponse.Title = "Internal Server Error";
                        break;
                }

                httpContext.Response.StatusCode = errorResponse.StatusCode;

                await httpContext
                    .Response
                    .WriteAsJsonAsync(errorResponse, cancellationToken);
        */
        /*
                var problemDetails = new ProblemDetails
                {
                    Detail = exception.Message
                };

                switch (exception)
                {
                    case BadHttpRequestException:
                        problemDetails.Status = (int)HttpStatusCode.BadRequest;
                        problemDetails.Title = exception.GetType().Name;
                        break;

                    default:
                        problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                        problemDetails.Title = "Internal Server Error";
                        break;
                }

                httpContext.Response.StatusCode = (int)problemDetails.Status;

                await httpContext
                    .Response
                    .WriteAsJsonAsync(problemDetails, cancellationToken);
        */

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            //Status = (int)HttpStatusCode.InternalServerError,
            Status = httpContext.Response.StatusCode,
            Type = exception.GetType().Name,
            Title = "An unexpected error occurred",
            Detail = exception.Message,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        }, cancellationToken);

        return true;
    }
}
