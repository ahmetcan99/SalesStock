using SalesStock.Domain.Entities;

namespace SalesStock.Application.Interfaces
{
    public interface IStockRepository
    {
        IQueryable<StockMovement> GetStockMovementsAsQueryable();
        Task AddMovementAndUpdateProductStockAsync(StockMovement movement, Product productToUpdate);
        Task ReleaseStockAsync(int productId, int quantity, int orderId);

    }
}
