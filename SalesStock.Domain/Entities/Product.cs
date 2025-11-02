using SalesStock.Domain.Common;

namespace SalesStock.Domain.Entities
{
    public class  Product : BaseEntity
    {
        public string SKU { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Unit { get; set; } = null!;
        public decimal VatRate { get; set; }
        public decimal UnitPrice { get; set; }
        public string BarCode { get; set; } = null!;
        public decimal StockOnHand { get; set; }
        public decimal StockReserved { get; set; }
    }
}
