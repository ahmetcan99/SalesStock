using Microsoft.EntityFrameworkCore;
using SalesStock.Domain.Entities;
using SalesStock.Application.Interfaces;
using SalesStock.Infrastructure.Persistence;

namespace SalesStock.Infrastructure.Repositories
{
    public class PriceListItemRepository : IPriceListItemRepository
    {
        private readonly AppDbContext _context;
        public PriceListItemRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(PriceListItem priceListItem)
        {
            await _context.PriceListItems.AddAsync(priceListItem);
            await _context.SaveChangesAsync();
        }
        public async Task<List<PriceListItem>> GetItemsByPriceListIdWithProductAsync(int priceListId)
        {
            return await _context.PriceListItems
                .AsNoTracking()
                .Include(pli => pli.Product)
                .Where(pli => pli.PriceList.Id == priceListId)
                .OrderBy(pli => pli.Product.SKU)
                .ThenBy(pli => pli.ValidFrom)
                .ToListAsync();
        }
        public async Task<bool> HasOverlappingItemsAsync(int priceListId, int productId, DateTime validFrom, DateTime validTo)
        {
            return await _context.PriceListItems
                .AnyAsync(pli =>
                    pli.PriceListId == priceListId &&
                    pli.ProductId == productId &&
                    pli.ValidFrom < validTo &&
                    pli.ValidTo > validFrom);
        }
        public async Task RemoveAsync(int itemId)
        {
            var item = await _context.PriceListItems.FindAsync(itemId);
            if (item != null)
            {
                _context.PriceListItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<PriceListItem?> GetPriceForItemAsync(int priceListId, int productId, DateTime date)
        {
            return await _context.PriceListItems
                .AsNoTracking()
                .FirstOrDefaultAsync(pli =>
                    pli.PriceListId == priceListId &&
                    pli.ProductId == productId &&
                    pli.ValidFrom <= date &&
                    pli.ValidTo >= date);
        }
    }
}
