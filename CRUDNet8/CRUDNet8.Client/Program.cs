using CRUDNet8.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SharedLibrary.ProductRepositories;
using Refit;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<IProductRepository, ProductService>();
//builder.Services.AddScoped(http => new HttpClient
//{
//    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
//});

//Console.WriteLine($"builder.HostEnvironment.BaseAddress: {builder.HostEnvironment.BaseAddress}");        // "https://localhost:7012/"

builder.Services
    .AddRefitClient<IProductApi>()
    //.ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));                    
    .ConfigureHttpClient(c => c.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/Product"));          
    //.ConfigureHttpClient(c => { c.BaseAddress = new Uri("https://localhost:7012/api/Product"); });

await builder.Build().RunAsync();
