using SalesStock.Domain.Common;

namespace SalesStock.Domain.Entities
{
    public class PriceListItem : BaseEntity
    {
        public int PriceListId { get; set; }
        public PriceList PriceList { get; set; } = null!;
        public int ProductId { get; set; }
        public string ProductSKU { get; set; } = string.Empty;
        public Product Product { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }
}
