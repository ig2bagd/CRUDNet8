using CRUDNet8.Client.Pages;
using CRUDNet8.Components;
using CRUDNet8.Controllers;
using CRUDNet8.Data;
using CRUDNet8.Extensions;
using CRUDNet8.Implementations;
using CRUDNet8.Middleware;

//using Hellang.Middleware.ProblemDetails;                  Doesn't worked: Remove Nuget package
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SharedLibrary.Models;
using SharedLibrary.ProductRepositories;
using System.Text.RegularExpressions;

//using ProblemDetailsOptions = Hellang.Middleware.ProblemDetails.ProblemDetailsOptions;

var builder = WebApplication.CreateBuilder(args);

// Serilog
//builder.Logging.ClearProviders();
builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));
//builder.Services.AddSingleton(Log.Logger);

//builder.WebHost.GetSetting(WebHostDefaults.ServerUrlsKey);

//builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();       // intended for use in server-side Blazor
//builder.Services.AddScoped<AuthenticationStateProvider, PersistingServerAuthenticationStateProvider>();

// Blazor Authentication Tutorial - How to Authorize in Blazor	(Coding Droplets)
// https://www.youtube.com/watch?v=GKvEuA80FAE	    
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "auth_token";
        options.LoginPath = "/login";
        options.Cookie.MaxAge = TimeSpan.FromMinutes(30);
        options.AccessDeniedPath = "/access-denied";
    });
builder.Services.AddAuthorization();

// https://learn.microsoft.com/en-us/aspnet/core/blazor/security/?view=aspnetcore-8.0#troubleshoot-errors
builder.Services.AddCascadingAuthenticationState();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

//builder.Services.AddControllers();            //Only needed for API controllers    

// https://stackoverflow.com/questions/71932980/what-is-addendpointsapiexplorer-in-asp-net-core-6/71933535#71933535
// https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-8.0&tabs=visual-studio
// https://www.youtube.com/watch?v=TlytBx3-k-k  -  How to use swagger in asp.net core web api
builder.Services.AddEndpointsApiExplorer();     // Minimal APIs
builder.Services.AddSwaggerGen();

// https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/#using-a-dbcontext-factory-eg-for-blazor
// https://learn.microsoft.com/en-us/aspnet/core/blazor/blazor-ef-core?view=aspnetcore-8.0
// https://stackoverflow.com/questions/73710513/what-is-the-difference-between-adddbcontextpool-and-adddbcontextfactory
// AddDbContextFactory gives you the ability to create and manage DbContext instances yourself
// https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/#other-dbcontext-configuration ***
//builder.Services.AddDbContext<AppDbContext>(options =>
#if DEBUG
    builder.Services.AddDbContextFactory<AppDbContext>(options =>
    {
        options
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection is not found"));
    });
#else
    builder.Services.AddDbContextFactory<AppDbContext>(options =>
    {
        options
            .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection is not found"));
    });
#endif
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Not using & no need to register HttpClient on the server
//builder.Services.AddScoped(http => new HttpClient
//{
//    BaseAddress = new Uri(builder.Configuration.GetSection("BaseAddress").Value!)
//});

builder.Services.AddExceptionHandlers();

// https://andrewlock.net/handling-web-api-exceptions-with-problemdetails-middleware/
// https://github.com/khellang/Middleware/issues/191
//builder.Services.AddProblemDetails(opts => 
//{
//    opts.IncludeExceptionDetails = (ctx, ex) =>
//    {
//        // Fetch services from HttpContext.RequestServices
//        var env = ctx.RequestServices.GetRequiredService<IHostEnvironment>();
//        return env.IsDevelopment() || env.IsStaging();
//    };
//});

var app = builder.Build();

//app.UseProblemDetails();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    //app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePages();
app.UseExceptionHandler();
//app.UseExceptionHandler(new ExceptionHandlerOptions()
//{
//    AllowStatusCode404Response = true,
//    ExceptionHandlingPath = "/Home/WRONG_URL"
//});

app.UseHttpsRedirection();
//app.MapControllers();                 //Only needed for API controllers
app.MapProductApi();                    //Minimal APIs
app.UseStaticFiles();

app.UseAuthentication();                // If app.UseAntiforgery() is used, no need to add app.UseAuthentication() & app.UseAuthorization() ???
app.UseAuthorization();                 // https://www.youtube.com/watch?v=asa2ucbZlCI&t=160s

app.UseAntiforgery();                   // A call to UseAntiforgery must be placed after calls to UseAuthentication and UseAuthorization. 
                                        // https://learn.microsoft.com/en-us/aspnet/core/blazor/forms/?view=aspnetcore-8.0#antiforgery-support
// Serilog
app.UseSerilogIngestion();
app.UseSerilogRequestLogging();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(CRUDNet8.Client._Imports).Assembly);

app.Run();
