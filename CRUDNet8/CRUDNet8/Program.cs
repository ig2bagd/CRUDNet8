using CRUDNet8.Client.Pages;
using CRUDNet8.Components;
using CRUDNet8.Data;
using CRUDNet8.Implementations;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Models;
using SharedLibrary.ProductRepositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();


//builder.Services.AddControllers();    
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
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
//app.MapControllers();                 //Only needed for API controllers
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Counter).Assembly);

// Minimal APIs
app.MapGet("/All-Products", async (IProductRepository productRepository) =>
{
    var products = await productRepository.GetAllProductsAsync();
    //return TypedResults.Ok(products);
    return Results.Ok(products);
});

app.MapGet("/Single-Product/{id}", async (IProductRepository productRepository, int id) =>
{
    var product = await productRepository.GetProductByIdAsync(id);
    return TypedResults.Ok(product);
});

app.MapPost("/Add-Product", async (IProductRepository productRepository, Product model) =>
{
    var product = await productRepository.AddProductAsync(model);
    return TypedResults.Ok(product);
});

app.MapPut("/Update-Product", async (IProductRepository productRepository, Product model) =>
{
    var product = await productRepository.UpdateProductAsync(model);
    return TypedResults.Ok(product);
});

app.MapDelete("/Delete-Product/{id}", async (IProductRepository productRepository, int id) =>
{
    var product = await productRepository.DeleteProductAsync(id);
    return TypedResults.Ok(product);
});

app.Run();
