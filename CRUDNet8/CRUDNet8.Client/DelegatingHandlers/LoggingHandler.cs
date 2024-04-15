using Refit;

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
        catch (ApiException ex)
        {
            var errors = await ex.GetContentAsAsync<Dictionary<string, string>>();
            var message = string.Join("; ", errors!.Values);
            throw new Exception(message);
        }
        catch (Exception e)
        {
            logger.LogError(e, "HTTP request failed");
            throw;
        }

    }
}
