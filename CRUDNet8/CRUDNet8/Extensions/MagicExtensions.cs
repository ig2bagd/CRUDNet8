using CRUDNet8.Middleware;

namespace CRUDNet8.Extensions;

public static class MagicExtensions
{
    public static void AddExceptionHandlers(this IServiceCollection services)
    {
        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?view=aspnetcore-8.0#problem-details
        // https://stackoverflow.com/questions/76318752/using-net-7-problemdetails-instead-of-hellang
        services.AddExceptionHandler<ValidationExceptionHandler>()
                .AddExceptionHandler<GlobalExceptionHandler>()
                .AddProblemDetails();
    }
}
