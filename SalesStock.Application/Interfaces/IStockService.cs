using SalesStock.Application.Features.Stock.DTOs;
using SalesStock.Shared.Common;

namespace SalesStock.Application.Interfaces
{
    public interface IStockService
    {
        Task<PaginatedList<StockMovementDTO>> GetStockMovementsPagedAsync(int pageNumber, int pageSize);
        Task<StockAdjustmentResultDTO> AdjustStockAsync(AdjustStockDTO adjustStockDTO);
        Task ReleaseStockAsync(int productId, int quantity, int orderId);
    }
}
