using SalesStock.Domain.Common;

namespace SalesStock.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }
        public Order Orders { get; set; } = null!;
        public int ProductId { get; set; }
        public string ProductSKU { get; set; } = string.Empty;  
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal NetTotal { get; set; }
        public decimal VatTotal { get; set; }
        public decimal GrandTotal { get; set; }
    }
}
