using SalesStock.Domain.Enums;

namespace SalesStock.Application.Features.Orders.DTOs
{
    public class OrderListDTO
    {
        public int Id { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public OrderStatus Status { get; set; }
        public decimal GrandTotal { get; set; }
        public string Currency { get; set; } = string.Empty;
    }
}
