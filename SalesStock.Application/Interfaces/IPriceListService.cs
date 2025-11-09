using SalesStock.Application.Features.PriceLists.DTOs;

namespace SalesStock.Application.Interfaces
{
    public interface IPriceListService
    {
        Task<IEnumerable<PriceListDTO>> GetAllPriceListsAsync();
    }
}
