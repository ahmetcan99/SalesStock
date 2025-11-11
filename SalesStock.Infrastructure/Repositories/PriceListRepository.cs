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
        public IQueryable<PriceList> GetAllSortedByNameAsQueryable()
        {
            return _context.PriceLists
                .AsNoTracking()
                .OrderBy(pl => pl.Name);
        }
        public async Task SetAsDefaultAsync(PriceList priceList)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.PriceLists
                    .Where(pl => pl.IsDefault)
                    .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDefault, false));
                priceList.IsDefault = true;
                _context.PriceLists.Update(priceList);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task UnsetDefaultPriceListAsync()
        {
            var defaultPriceLists = await _context.PriceLists
                .Where(pl => pl.IsDefault)
                .ToListAsync();
            foreach (var priceList in defaultPriceLists)
            {
                priceList.IsDefault = false;
            }
            await _context.SaveChangesAsync();
        }
        public async Task AddAsync(PriceList priceList)
        {
            await _context.PriceLists.AddAsync(priceList);
            await _context.SaveChangesAsync();
        }
        public async Task<PriceList?> GetByIdAsync(int id)
        {
            return await _context.PriceLists
                .AsNoTracking()
                .FirstOrDefaultAsync(pl => pl.Id == id);
        }
        public async Task UpdateAsync(PriceList priceList)
        {
            _context.PriceLists.Update(priceList);
            await _context.SaveChangesAsync();
        }
    }
}
