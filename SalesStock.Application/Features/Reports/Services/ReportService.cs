using SalesStock.Application.Features.Reports.DTOs;
using SalesStock.Application.Interfaces;

namespace SalesStock.Application.Features.Reports.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<DailySalesReportDTO> GetDailySalesAsync(DateTime date)
        {
            return await _reportRepository.GetDailySalesAsync(date);
        }
        public async Task<IReadOnlyList<LowStockItemDTO>> GetLowStockAsync(int threshold)
        {
            return await _reportRepository.GetLowStockAsync(threshold);
        }
    }
}
