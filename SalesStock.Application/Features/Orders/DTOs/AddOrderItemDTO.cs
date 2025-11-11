namespace SalesStock.Application.Features.Orders.DTOs
{
    public class AddOrderItemDTO
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductSKU { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
