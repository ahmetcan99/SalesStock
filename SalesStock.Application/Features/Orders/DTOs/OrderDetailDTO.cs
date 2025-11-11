using SalesStock.Domain.Enums;

namespace SalesStock.Application.Features.Orders.DTOs
{
    public class OrderDetailDTO
    {
        public int Id { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string PriceListName { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal NetTotal { get; set; }
        public decimal VatTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public string Currency { get; set; } = string.Empty;
        public List<OrderItemDTO> Items { get; set; } = new();


    }
}
