using Microsoft.EntityFrameworkCore;
using SalesStock.Application.Interfaces;
using SalesStock.Domain.Entities;
using SalesStock.Infrastructure.Persistence;

namespace SalesStock.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Product> GetAsQueryable()
        {
            return _context.Products.AsNoTracking();
        }
        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> SkuExistsAsync(string sku)
        {
            return await _context.Products.AnyAsync(p => p.SKU == sku);
        }
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }
        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> SkuExistsForOtherProductAsync(int id, string sku)
        {
            return await _context.Products.AnyAsync(p => p.SKU == sku && p.Id != id);
        }
        public async Task<Product?> GetBySKUAsync(string SKU)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.SKU == SKU);
        }
        public async Task<List<Product>> SearchAvailableProductsAsync(string searchTerm)
        {
            return await _context.Products
                .Where(p => p.IsActive &&
                            (EF.Functions.Like(p.Name, $"%{searchTerm}%") ||
                             EF.Functions.Like(p.SKU, $"%{searchTerm}%")))
                .ToListAsync();
        }

    }
}