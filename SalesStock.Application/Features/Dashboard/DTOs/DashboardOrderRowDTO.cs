

namespace SalesStock.Application.Features.Dashboard.DTOs
{
    public class DashboardOrderRowDTO
    {
        public int OrderId { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal GrandTotal { get; set; }
    }
}
