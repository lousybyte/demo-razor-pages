using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoRazor.Contexts;
using DemoRazor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DemoRazor.Services
{
    public class ProductService
    {
        private readonly IConfiguration Config;
        private readonly AppDbContext   AppDb;

        public ProductService(IConfiguration config, AppDbContext appDbContext)
        {
            Config = config;
            AppDb  = appDbContext;
        }

        public async Task<List<ProductData>> GetProducts()
        {
            return await AppDb.Products
                .AsNoTracking()
                .Include(data => data.Brand)
                .Include(data => data.Category)
                .OrderBy(data => data.Id)
                .Take(100)
                .ToListAsync();
        }

        public async Task<ProductData> FindProductById(long id)
        {
            return await AppDb.Products.FindAsync(id);
        }

        public async Task<List<ProductsBrandData>> GetBrands()
        {
            return await AppDb.ProductsBrands
                .AsNoTracking()
                .OrderBy(data => data.Id)
                .Take(100)
                .ToListAsync();
        }

        public async Task<List<ProductsCategoryData>> GetCategories()
        {
            return await AppDb.ProductsCategories
                .AsNoTracking()
                .OrderBy(data => data.Id)
                .Take(100)
                .ToListAsync();
        }

        public async Task<bool> AddProduct(ProductData product)
        {
            AppDb.Products.Add(product);

            await AppDb.SaveChangesAsync();

            return true;
        }
        public async Task<int> RemoveProduct(ProductData product)
        {
            AppDb.Products.Remove(product);
            return await AppDb.SaveChangesAsync();
        }

        public async Task<bool> IsBrandValidAsync(long brandId)
        {
            return await AppDb.ProductsBrands.AnyAsync(brand => brand.Id == brandId);
        }

        public async Task<bool> IsCategoryValidAsync(long categoryId)
        {
            return await AppDb.ProductsCategories.AnyAsync(cat => cat.Id == categoryId);
        }
    }
}
