using SalesStock.Application.Features.Reports.DTOs;

namespace SalesStock.Application.Interfaces
{
    public interface IReportService
    {
        Task<DailySalesReportDTO> GetDailySalesAsync(DateTime date);
        Task<IReadOnlyList<LowStockItemDTO>> GetLowStockAsync(int threshold);
    }
}
