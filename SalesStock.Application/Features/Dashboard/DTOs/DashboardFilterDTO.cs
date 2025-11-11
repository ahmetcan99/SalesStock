public enum DashboardQuickRange {     
    Today,
    ThisWeek,
    ThisMonth,
    Custom
}
namespace SalesStock.Application.Features.Dashboard.DTOs
{
    public class DashboardFilterDTO
    {
        public DashboardQuickRange Range { get; set; } = DashboardQuickRange.Today;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int LowStockThreshold { get; set; } = 5;
    }
}
