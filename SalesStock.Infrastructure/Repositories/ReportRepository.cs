using Microsoft.EntityFrameworkCore;
using SalesStock.Application.Features.Reports.DTOs;
using SalesStock.Application.Interfaces;
using SalesStock.Infrastructure.Persistence;
using SalesStock.Domain.Enums;

namespace SalesStock.Infrastructure.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;
        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DailySalesReportDTO> GetDailySalesAsync(DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            var orders = _context.Orders
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Items)
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt < endDate && o.Status != OrderStatus.Cancelled);

            var totalSales = await orders.SumAsync(o => (decimal?)o.GrandTotal) ?? 0m;
            var orderCount = await orders.CountAsync();

            var topCustomer = await orders
                .GroupBy(o => o.Customer.Name)
                .Select(g => new { Customer = g.Key, Total = g.Sum(x => x.GrandTotal) })
                .OrderByDescending(x => x.Total)
                .Select(x => x.Customer)
                .FirstOrDefaultAsync();

            var topProducts = await _context.OrderItems
                .AsNoTracking()
                .Include(i => i.Product)
                .Include(i => i.Orders)
                .Where(i => i.Orders.CreatedAt >= startDate && i.Orders.CreatedAt < endDate && i.Orders.Status != OrderStatus.Cancelled)
                .GroupBy(i => i.Product.Name)
                .Select(g => new TopProductDTO
                {
                    ProductName = g.Key,
                    QuantitySold = g.Sum(x => x.Quantity),
                    TotalSales = g.Sum(x => x.GrandTotal)
                })
                .OrderByDescending(x => x.TotalSales)
                .Take(5)
                .ToListAsync();
            var firstCurrency = await orders
                .Select(o => o.PriceList.Currency)
                .FirstOrDefaultAsync() ?? "TRY";
            var salesByCurrency = await orders
                .AsNoTracking()
                .Include(o => o.PriceList)
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt < endDate && o.Status != OrderStatus.Cancelled)
                .GroupBy(o => o.PriceList.Currency)
                .Select(g => new DailySalesByCurrencyDTO
                {
                    Currency = g.Key,
                    TotalSales = g.Sum(x => x.GrandTotal),
                    OrderCount = g.Count()
                })
                .OrderByDescending(x => x.TotalSales)
                .ToListAsync();

            return new DailySalesReportDTO
            {
                Date = startDate,
                TotalSales = totalSales,
                OrderCount = orderCount,
                TopCustomer = topCustomer ?? "No data",
                TopProducts = topProducts,
                SalesByCurrency = salesByCurrency
            };
        }
        public async Task<IReadOnlyList<LowStockItemDTO>> GetLowStockAsync(int threshold)
        {
            return await _context.Products
                .AsNoTracking()
                .Where(p => p.IsActive && p.StockOnHand <= threshold)
                .Select(p => new LowStockItemDTO
                {
                    SKU = p.SKU,
                    ProductName = p.Name,
                    StockOnHand = p.StockOnHand
                })
                .OrderBy(p => p.StockOnHand)
                .ToListAsync();
        }
    }
}
