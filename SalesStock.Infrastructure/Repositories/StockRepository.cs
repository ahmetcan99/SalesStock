using Microsoft.EntityFrameworkCore;
using SalesStock.Application.Interfaces;
using SalesStock.Domain.Entities;
using SalesStock.Infrastructure.Persistence;

namespace SalesStock.Infrastructure.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly AppDbContext _context;
        public StockRepository(AppDbContext context)
        {
            _context = context;
        }
        public IQueryable<StockMovement> GetStockMovementsAsQueryable()
        {
            return _context.StockMovements.AsNoTracking().Include(sm => sm.Product).AsQueryable();
        }
        public async Task AddMovementAndUpdateProductStockAsync(StockMovement movement, Product productToUpdate)
        {
            await _context.StockMovements.AddAsync(movement);
            _context.Products.Update(productToUpdate);
            await _context.SaveChangesAsync();
        }
    }
}
