using SalesStock.Application.Features.PriceLists.DTOs;
using SalesStock.Shared.Common;

namespace SalesStock.Application.Interfaces
{
    public interface IPriceListService
    {
        Task<PaginatedList<PriceListDTO>> GetPriceListsPagedAsync(int pageNumber, int pageSize);
        Task CreatePriceListAsync(CreatePriceListDTO createPriceListDTO);
        Task UpdatePriceListAsync(int id, UpdatePriceListDTO updatePriceListDTO);
        Task<bool> SetAsDefaultPriceListAsync(int id);
        Task <UpdatePriceListDTO?> GetPriceListByIdAsync(int id);
        Task <ManagePriceListItemsDTO?> GetPriceListItemsByIdAsync(int id);
        Task AddItemAsync(AddPriceListItemDTO addPriceListItemDTO);
        Task RemoveItemAsync(int itemId);
        Task<List<ProductSearchResultDTO>> SearchProductsAsync(string searchTerm);
        Task<IEnumerable<PriceListSelectListDTO>> GetAllActivePriceListsForSelectListAsync();
    }
}
