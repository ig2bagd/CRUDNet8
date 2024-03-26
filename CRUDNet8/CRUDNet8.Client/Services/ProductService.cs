using SharedLibrary.Models;
using SharedLibrary.ProductRepositories;

namespace CRUDNet8.Client.Services;

public class ProductService(IProductApi ProductClient) : IProductRepository
{
    //private readonly IProductApi ProductClient;
    //public ProductService(IProductApi ProductClient)
    //{
    //    this.ProductClient = ProductClient;
    //}
    public async Task<Product> AddProductAsync(Product model)
    {
        return await ProductClient.AddProductAsync(model);
    }

    public async Task<Product> DeleteProductAsync(int productId)
    {
        return await ProductClient.DeleteProductAsync(productId);
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await ProductClient.GetAllProductsAsync();
    }

    public async Task<Product> GetProductByIdAsync(int productId)
    {
        return await ProductClient.GetProductByIdAsync(productId); ;
    }

    public async Task<Product> UpdateProductAsync(Product model)
    {
        return await ProductClient.UpdateProductAsync(model);
    }
}
