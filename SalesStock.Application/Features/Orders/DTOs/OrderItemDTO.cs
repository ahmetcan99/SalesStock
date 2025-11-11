namespace SalesStock.Application.Features.Orders.DTOs
{
    public class OrderItemDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSku { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal NetTotal { get; set; }
        public decimal VatTotal { get; set; }
        public decimal GrandTotal { get; set; }
    }
}
