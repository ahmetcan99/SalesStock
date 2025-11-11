using Microsoft.EntityFrameworkCore;
using SalesStock.Application.Features.Dashboard.DTOs;
using SalesStock.Application.Interfaces;
using SalesStock.Infrastructure.Persistence;
using SalesStock.Domain.Enums;
namespace SalesStock.Infrastructure.Repositories
{
    public class DashboardRepository : IDashBoardRepository
    {
        private readonly AppDbContext _context;

        public DashboardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetOrdersCountAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Orders
                .AsNoTracking()
                .Where(o => EF.Functions.DateDiffDay(o.CreatedAt, DateTime.Today) == 0)
                .CountAsync();
        }
        public async Task<int> GetLowStockCountAsync(int stockThreshold)
        {
            return await _context.Products
                .AsNoTracking()
                .CountAsync(p => p.IsActive && p.StockOnHand <= stockThreshold);
        }
        public async Task<List<DashboardOrderRowDTO>> GetLastOrdersAsync()
        {
            return await _context.Orders
                .AsNoTracking()
                .OrderByDescending(o => o.CreatedAt)
                .Take(5)
                .Select(o => new DashboardOrderRowDTO
                {
                    OrderId = o.Id,
                    OrderNo = o.OrderNo,
                    CustomerName = o.Customer.Name,
                    CreatedAt = o.CreatedAt,
                    Status = o.Status.ToString(),
                    GrandTotal = o.GrandTotal
                })
                .ToListAsync();
        }
        public async Task<IReadOnlyList<DashboardSalesByCurrencyDTO>> GetSalesByCurrencyAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.PriceList)
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt < endDate && o.Status != OrderStatus.Cancelled)
                .GroupBy(o => o.PriceList.Currency)
                .Select(g => new DashboardSalesByCurrencyDTO
                {
                    Currency = g.Key,
                    TotalSales = g.Sum(x => x.GrandTotal),
                    OrderCount = g.Count()
                })
                .OrderByDescending(x => x.TotalSales)
                .ToListAsync();
        }

    }
}
