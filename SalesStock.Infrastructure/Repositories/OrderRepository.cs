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
        public async Task<OrderItem?> GetOrderItemAsync(int orderId, int productId)
        {
            return await _context.OrderItems
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.OrderId == orderId && i.ProductId == productId);
        }
        public async Task<bool> RemoveItemByProductAsync(int orderId, int productId)
        {
            var item = await _context.OrderItems
       .FirstOrDefaultAsync(i => i.OrderId == orderId && i.ProductId == productId);

            if (item == null)
                return false;

            _context.OrderItems.Remove(item);
            await _context.SaveChangesAsync();

            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order != null)
            {
                order.NetTotal = order.Items.Sum(i => i.NetTotal);
                order.VatTotal = order.Items.Sum(i => i.VatTotal);
                order.GrandTotal = order.Items.Sum(i => i.GrandTotal);
                await _context.SaveChangesAsync();
            }

            return true;
        }
        public async Task SaveChangesAsync()
        {
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
