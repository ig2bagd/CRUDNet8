using CRUDNet8.Data;
using Microsoft.EntityFrameworkCore;

namespace CRUDNet8.Implementations
{
    public class ProductRepository(IDbContextFactory<AppDbContext> DbFactory) : IProductRepository
    {
        //private readonly IDbContextFactory<AppDbContext> factory;
        //public ProductRepository(IDbContextFactory<AppDbContext> DbFactory)
        //{
        //    factory = DbFactory;
        //}

        // https://code-maze.com/efcore-modifying-data/
        public async Task<Product> AddProductAsync(Product model)
        {
            if (model is null) return null!;

            // ✅ Create a fresh DbContext per method call
            var dbContext = DbFactory.CreateDbContext();
            var product = await dbContext.Products.Where(_ => _.Name.ToLower().Equals(model.Name.ToLower())).FirstOrDefaultAsync();
            if (product is not null) return null!;

            var newDataAdded = dbContext.Products.Add(model).Entity;
            await dbContext.SaveChangesAsync();
            return newDataAdded;
        }

        public async Task<Product> UpdateProductAsync(Product model)
        {
            if (model is null) return null!;

            using var dbContext = DbFactory.CreateDbContext();

            // Override global NoTracking for this query so EF will track the entity
            var product = await dbContext.Products
                                .AsTracking()
                                .FirstOrDefaultAsync(_ => _.Id == model.Id);
            if (product is null) return null!;
            //product.Name = model.Name;
            //product.Quantity = model.Quantity;
            //appDbContext.Update(product);           // Force updating all properties of the entity into database
            // https://stackoverflow.com/questions/73714355/bettercleaner-way-to-update-record-using-entity-framework-core
            dbContext.Entry(product).CurrentValues.SetValues(model);
            await dbContext.SaveChangesAsync();

            // return a no-tracking read of the updated entity
            return await dbContext.Products.AsNoTracking().FirstOrDefaultAsync(_ => _.Id == model.Id) ?? new Product();


            /* Method 2: Attach and mark as modified - Not recommended as it updates all scalar properties even if they are not changed
            if (model is null) return null!;

            using (var dbContext = factory.CreateDbContext())
            {
                // attach the detached model and mark it as modified so EF will update all scalar properties
                dbContext.Products.Attach(model);
                dbContext.Entry(model).State = EntityState.Modified;

                await dbContext.SaveChangesAsync();

                // return the refreshed entity from DB
                return await dbContext.Products.FirstOrDefaultAsync(_ => _.Id == model.Id) ?? new Product();
            }
            */
        }

        public async Task<Product> DeleteProductAsync(int productId)
        {
            using var dbContext = DbFactory.CreateDbContext();
            var product = await dbContext.Products.FirstOrDefaultAsync(_ => _.Id == productId);
            if (product is null) return null!;
            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync();
            return product;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            using var dbContext = DbFactory.CreateDbContext();
            return await dbContext.Products.ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            using var dbContext = DbFactory.CreateDbContext();
            var product = await dbContext.Products.FirstOrDefaultAsync(_ => _.Id == productId);
            if (product is null) return null!;
            return product;
        }
    }
}
