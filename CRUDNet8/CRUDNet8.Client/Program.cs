using CRUDNet8.Client.Auth;
using CRUDNet8.Client.DelegatingHandlers;
using CRUDNet8.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Refit;
using Serilog;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Extensions.Logging;


var builder = WebAssemblyHostBuilder.CreateDefault(args);

#region Memory Configuration Source
var vehicleData = new Dictionary<string, string?>()
{
    { "color", "blue" },
    { "type", "car" },
    { "wheels:count", "3" },
    { "wheels:brand", "Blazin" },
    { "wheels:brand:type", "rally" },
    { "wheels:year", "2008" },
};

var memoryConfig = new MemoryConfigurationSource { InitialData = vehicleData };
builder.Configuration.Add(memoryConfig);

//var configuration = builder.Configuration;

//// Retrieve values
//var color = configuration["color"];
//var type = configuration["type"];
//var wheelsCount = configuration["wheels:count"];
//var wheelsBrand = configuration["wheels:brand"];
//var wheelsBrandType = configuration["wheels:brand:type"];
//var wheelsYear = configuration["wheels:year"];

//Console.WriteLine($"Color: {color}");
//Console.WriteLine($"Type: {type}");
//Console.WriteLine($"Wheels Count: {wheelsCount}");
//Console.WriteLine($"Wheels Brand: {wheelsBrand}");
//Console.WriteLine($"Wheels Brand Type: {wheelsBrandType}");
//Console.WriteLine($"Wheels Year: {wheelsYear}");
#endregion


#region Serilog
SelfLog.Enable(m => Console.Error.WriteLine(m));

var levelSwitch = new LoggingLevelSwitch();
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.ControlledBy(levelSwitch)
    .Enrich.WithProperty("InstanceId", Guid.NewGuid().ToString("n"))
    .WriteTo.BrowserHttp(endpointUrl: $"{builder.HostEnvironment.BaseAddress}ingest", controlLevelSwitch: levelSwitch)
    .CreateLogger();

//builder.Logging.AddSerilog();
builder.Logging.AddProvider(new SerilogLoggerProvider());
// https://github.com/serilog/serilog-extensions-logging
//builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
#endregion

/*
Making an Authentication System in Blazor WebAssembly
https://www.youtube.com/watch?v=WuKBx5YoIu0         https://github.com/gavilanch/Blazor-Wasm-CRUD
https://learn.microsoft.com/en-us/aspnet/core/blazor/security/?view=aspnetcore-8.0#client-side-blazor-authentication
*/
//builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
//builder.Services.AddScoped<CustomAuthStateProvider>();
//builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthStateProvider>());

builder.Services.AddScoped<BrowserStorageService>();

builder.Services.AddScoped<IProductRepository, ProductService>();
//builder.Services.AddScoped(http => new HttpClient
//{
//    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
//});

//Console.WriteLine($"builder.HostEnvironment.BaseAddress: {builder.HostEnvironment.BaseAddress}");        // "https://localhost:7012/"

builder.Services.AddTransient<LoggingHandler>();

// https://blog.nimblepros.com/blogs/getting-started-with-refit/
// https://devblogs.microsoft.com/dotnet/building-resilient-cloud-services-with-dotnet-8/
builder.Services
    .AddRefitClient<IProductApi>()
    //.ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));                    
    //.ConfigureHttpClient(c => { c.BaseAddress = new Uri("https://localhost:7012/api/Product"); })
    //.ConfigureHttpClient(c => c.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/Product"))
    .ConfigureHttpClient(c => { 
        c.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/Product");
        c.Timeout = TimeSpan.FromSeconds(15);
    })
    .AddHttpMessageHandler<LoggingHandler>();
    //.AddStandardResilienceHandler()
    //.AddResilienceHandler("demo", builder =>
    //{
    //    builder.AddConcurrencyLimiter(100);
    //    builder.AddTimeout(TimeSpan.FromSeconds(5));			// Order is important!  Notice similar AddTimeout at bottom
    //    builder.AddRetry(new HttpRetryStrategyOptions
    //    {
    //        MaxRetryAttempts = 5,
    //        BackoffType = DelayBackoffType.Exponential,
    //        UseJitter = true,
    //        Delay = TimeSpan.Zero,
    //        OnRetry = static args =>
    //        {
    //            Console.WriteLine($"    Retry {args.AttemptNumber} after {args.RetryDelay.TotalMilliseconds:F2}ms, due to: {args.Outcome.Result?.StatusCode.ToString() ?? args.Outcome.Exception?.GetType().Name}");
    //            return default;
    //        }
    //    });

    //    builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
    //    {
    //        SamplingDuration = TimeSpan.FromSeconds(5),
    //        FailureRatio = 0.9,
    //        MinimumThroughput = 5,
    //        BreakDuration = TimeSpan.FromSeconds(5)
    //    });

    //    builder.AddTimeout(TimeSpan.FromSeconds(30));
    //});

builder.Services.AddScoped<SessionStorage>();


builder.Services.AddCascadingValue(sp => new Dalek { Units = 123 });
builder.Services.AddCascadingValue("AlphaGroup", sp => new Dalek { Units = 456 });

await builder.Build().RunAsync();