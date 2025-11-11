using SalesStock.Domain.Entities;

namespace SalesStock.Application.Interfaces
{
    public interface IPriceListRepository 
    {
        IQueryable<PriceList> GetAllSortedByNameAsQueryable();
        Task<PriceList?> GetByIdAsync(int id);
        Task SetAsDefaultAsync(PriceList priceList);
        Task UnsetDefaultPriceListAsync();
        Task AddAsync(PriceList priceList);
        Task UpdateAsync(PriceList priceList);

    }
}
