using Refit;

namespace CRUDNet8.Client.Services;

public interface IProductApi
{
    [Get("/All-Products")]
    Task<List<Product>> GetAllProductsAsync();

    [Get("/Single-Product/{id}")]
    Task<Product> GetProductByIdAsync(int id);

    [Post("/Add-Product")]
    Task<Product> AddProductAsync([Body] Product model);

    [Put("/Update-Product")]
    Task<Product> UpdateProductAsync([Body] Product model);

    [Delete("/Delete-Product/{id}")]
    Task<Product> DeleteProductAsync(int id);
}
