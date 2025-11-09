using SalesStock.Domain.Entities;

namespace SalesStock.Application.Interfaces
{
    public interface IPriceListRepository 
    {
        Task<IEnumerable<PriceList>> GetAllSortedByIdAsync();
    }
}
