using SalesStock.Domain.Entities;

namespace SalesStock.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> AddAsync(Order order);
        IQueryable<Order> GetAsQueryable();
        Task<Order?> GetByIdAsync(int orderId);
        Task UpdateAsync(Order order);
        Task AddItemAsync(OrderItem orderItem);
        Task<Order?> GetByIdWithItemsAndProductsAsync(int orderId);
    }
}
