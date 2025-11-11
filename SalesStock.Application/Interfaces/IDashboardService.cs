using SalesStock.Application.Features.Dashboard.DTOs;

namespace SalesStock.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStatsDTO> GetAsync(DashboardFilterDTO filter);
    }
}
