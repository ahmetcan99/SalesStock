using SalesStock.Domain.Entities;

namespace SalesStock.Application.Interfaces
{
    public interface IPriceListItemRepository
    {
        Task<List<PriceListItem>> GetItemsByPriceListIdWithProductAsync(int priceListId);
        Task<bool> HasOverlappingItemsAsync(int priceListId, int productId, DateTime validFrom, DateTime validTo);
        Task AddAsync(PriceListItem priceListItem);
        Task RemoveAsync(int itemId);
        Task<PriceListItem?> GetPriceForItemAsync(int priceListId, int ProductId, DateTime date);
    }
}
