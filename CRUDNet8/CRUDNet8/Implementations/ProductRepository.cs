using CRUDNet8.Data;
using Microsoft.EntityFrameworkCore;

namespace CRUDNet8.Implementations
{
    //public class ProductRepository(AppDbContext appDbContext) : IProductRepository
    public class ProductRepository : IProductRepository, IDisposable
    {
        //private readonly AppDbContext appDbContext;
        //public ProductRepository(AppDbContext appDbContext)
        //{
        //    this.appDbContext = appDbContext;
        //}

        private readonly AppDbContext appDbContext;
        public ProductRepository(IDbContextFactory<AppDbContext> DbFactory)
        {
            appDbContext = DbFactory.CreateDbContext();
        }

        public void Dispose()
        {
            appDbContext.Dispose();
        }

        // https://code-maze.com/efcore-modifying-data/
        public async Task<Product> AddProductAsync(Product model)
        {
            if (model is null) return null!;
            var chk = await appDbContext.Products.Where(_ => _.Name.ToLower().Equals(model.Name.ToLower())).FirstOrDefaultAsync();
            if (chk is not null) return null!;

            var newDataAdded = appDbContext.Products.Add(model).Entity;
            await appDbContext.SaveChangesAsync();
            return newDataAdded;
        }

        public async Task<Product> UpdateProductAsync(Product model)
        {
            var product = await appDbContext.Products.FirstOrDefaultAsync(_ => _.Id == model.Id);
            if (product is null) return null!;
            //product.Name = model.Name;
            //product.Quantity = model.Quantity;
            //appDbContext.Update(product);           // Force updating all properties of the entity into database
            // https://stackoverflow.com/questions/73714355/bettercleaner-way-to-update-record-using-entity-framework-core
            appDbContext.Entry(product).CurrentValues.SetValues(model);
            await appDbContext.SaveChangesAsync();
            return await appDbContext.Products.FirstOrDefaultAsync(_ => _.Id == model.Id) ?? new Product();
        }

        public async Task<Product> DeleteProductAsync(int productId)
        {
            var product = await appDbContext.Products.FirstOrDefaultAsync(_ => _.Id == productId);
            if (product is null) return null!;
            appDbContext.Products.Remove(product);
            await appDbContext.SaveChangesAsync();
            return product;
        }

        public async Task<List<Product>> GetAllProductsAsync() => await appDbContext.Products.ToListAsync();

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            var product = await appDbContext.Products.FirstOrDefaultAsync(_ => _.Id == productId);
            if (product is null) return null!;
            return product;
        }
    }
}
