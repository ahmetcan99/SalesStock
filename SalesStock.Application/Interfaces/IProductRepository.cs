using SalesStock.Domain.Entities;

namespace SalesStock.Application.Interfaces
{
    public interface IProductRepository
    {
        IQueryable<Product> GetAsQueryable();
        Task AddAsync(Product product);
        Task<bool> SkuExistsAsync(string sku);
        Task<Product?> GetByIdAsync(int id);
        Task UpdateAsync(Product product);
        Task<bool> SkuExistsForOtherProductAsync(int id, string sku);
    }
}
