namespace CRUDNet8.Client.DelegatingHandlers;

//https://www.milanjovanovic.tech/blog/extending-httpclient-with-delegating-handlers-in-aspnetcore
public class LoggingHandler : DelegatingHandler
{
    private readonly ILogger<LoggingHandler> logger;

    public LoggingHandler(ILogger<LoggingHandler> logger)
    {
        this.logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Before HTTP request");

            var result = await base.SendAsync(request, cancellationToken);

            result.EnsureSuccessStatusCode();

            logger.LogInformation("After HTTP request");

            return result;
        }
        catch (Exception e)
        {
            logger.LogError(e, "HTTP request failed");
            throw;
        }

    }
}
