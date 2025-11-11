

namespace SalesStock.Application.Features.Reports.DTOs
{
    public class DailySalesReportDTO
    {
        public DateTime Date { get; set; }
        public decimal TotalSales { get; set; }
        public int OrderCount { get; set; }
        public string? TopCustomer { get; set; }
        public IReadOnlyList<TopProductDTO> TopProducts { get; set; } = new List<TopProductDTO>();
        public IReadOnlyList<DailySalesByCurrencyDTO> SalesByCurrency { get; set; } = new List<DailySalesByCurrencyDTO>();

    }
}
