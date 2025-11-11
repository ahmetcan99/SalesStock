using Microsoft.EntityFrameworkCore;
using SalesStock.Application.Interfaces;
using SalesStock.Domain.Entities;
using SalesStock.Infrastructure.Persistence;

namespace SalesStock.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Order> AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            
            return order;
        }
        public IQueryable<Order> GetAsQueryable()
        {
            return _context.Orders.AsNoTracking();
        }
        public async Task<Order?> GetByIdAsync(int orderId)
        {
            return await _context.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }
        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
        public async Task AddItemAsync(OrderItem orderItem)
        {
            await _context.OrderItems.AddAsync(orderItem);
            await _context.SaveChangesAsync();
        }
        public async Task<Order?> GetByIdWithItemsAndProductsAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

    }
}
