using Microsoft.EntityFrameworkCore;
using SalesStock.Application.Interfaces;
using SalesStock.Domain.Entities;
using SalesStock.Infrastructure.Persistence;
namespace SalesStock.Infrastructure.Repositories
{
    public class PriceListRepository : IPriceListRepository
    {
        private readonly AppDbContext _context;
        public PriceListRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<PriceList>> GetAllSortedByIdAsync()
        {
            return await _context.PriceLists
                .AsNoTracking()
                .OrderBy(pl => pl.Id)
                .ToListAsync();
        }
    }
}
