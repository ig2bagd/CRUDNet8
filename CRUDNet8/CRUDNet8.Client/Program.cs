using CRUDNet8.Client.DelegatingHandlers;
using CRUDNet8.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Refit;
using Serilog;
using Serilog.Core;
using Serilog.Debugging;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

#region Serilog
SelfLog.Enable(m => Console.Error.WriteLine(m));

var levelSwitch = new LoggingLevelSwitch();
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.ControlledBy(levelSwitch)
    .Enrich.WithProperty("InstanceId", Guid.NewGuid().ToString("n"))
    .WriteTo.BrowserHttp(endpointUrl: $"{builder.HostEnvironment.BaseAddress}ingest", controlLevelSwitch: levelSwitch)
    .CreateLogger();

builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
#endregion

builder.Services.AddScoped<IProductRepository, ProductService>();
//builder.Services.AddScoped(http => new HttpClient
//{
//    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
//});

//Console.WriteLine($"builder.HostEnvironment.BaseAddress: {builder.HostEnvironment.BaseAddress}");        // "https://localhost:7012/"

builder.Services.AddTransient<LoggingHandler>();

builder.Services
    .AddRefitClient<IProductApi>()
    //.ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));                    
    //.ConfigureHttpClient(c => { c.BaseAddress = new Uri("https://localhost:7012/api/Product"); })
    .ConfigureHttpClient(c => c.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/Product"))
    .AddHttpMessageHandler<LoggingHandler>()
    //.AddStandardResilienceHandler()
    .AddResilienceHandler("demo", builder =>
    {
        builder.AddConcurrencyLimiter(100);
        builder.AddTimeout(TimeSpan.FromSeconds(5));			// Order is important!  Notice similar AddTimeout at bottom
        builder.AddRetry(new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = 5,
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = true,
            Delay = TimeSpan.Zero,
            OnRetry = static args =>
            {
                Console.WriteLine($"    Retry {args.AttemptNumber} after {args.RetryDelay.TotalMilliseconds:F2}ms, due to: {args.Outcome.Result?.StatusCode.ToString() ?? args.Outcome.Exception?.GetType().Name}");
                return default;
            }
        });
    
        builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
        {
            SamplingDuration = TimeSpan.FromSeconds(5),
            FailureRatio = 0.9,
            MinimumThroughput = 5,
            BreakDuration = TimeSpan.FromSeconds(5)
        });
    
        builder.AddTimeout(TimeSpan.FromSeconds(1));
    });


await builder.Build().RunAsync();
