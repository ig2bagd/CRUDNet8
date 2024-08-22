using Serilog.Context;

namespace CRUDNet8.Middleware;

public class RequestLogContextMiddleware
{
    // 7 Serilog Best Practices for Better Structured Logging
    // https://www.youtube.com/watch?v=w7yDuoCLVvQ
    private readonly RequestDelegate next;

    public RequestLogContextMiddleware(RequestDelegate next) => this.next = next;

    public Task InvokeAsync(HttpContext context)
    {
        using(LogContext.PushProperty("TraceIdentifier", context.TraceIdentifier))
        {
            return next(context);
        }
    }
}
