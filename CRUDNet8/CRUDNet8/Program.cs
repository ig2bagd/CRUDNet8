using CRUDNet8.Client.Pages;
using CRUDNet8.Components;
using CRUDNet8.Controllers;
using CRUDNet8.Data;
using CRUDNet8.Implementations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SharedLibrary.Models;
using SharedLibrary.ProductRepositories;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Logging.ClearProviders();
builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

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

//builder.Services.AddControllers();            

// https://stackoverflow.com/questions/71932980/what-is-addendpointsapiexplorer-in-asp-net-core-6/71933535#71933535
// https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-8.0&tabs=visual-studio
// https://www.youtube.com/watch?v=TlytBx3-k-k  -  How to use swagger in asp.net core web api
builder.Services.AddEndpointsApiExplorer();     // Minimal APIs
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Wooo! Connection is not found"));
});
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Not using & no need to register HttpClient on the server
//builder.Services.AddScoped(http => new HttpClient
//{
//    BaseAddress = new Uri(builder.Configuration.GetSection("BaseAddress").Value!)
//});

builder.Services.AddProblemDetails();

var app = builder.Build();

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
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
//app.MapControllers();                 //Only needed for API controllers
app.MapProductEndpoints();              //Minimal APIs
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

// Serilog
app.UseSerilogIngestion();
app.UseSerilogRequestLogging();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(CRUDNet8.Client._Imports).Assembly);

app.Run();
