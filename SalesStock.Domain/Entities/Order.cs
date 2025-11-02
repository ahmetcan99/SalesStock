using SalesStock.Domain.Common;
using SalesStock.Domain.Enums;

namespace SalesStock.Domain.Entities
{
    public class  Order : BaseEntity
    {
        public string OrderNo { get; set; } = null!;
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        public int PriceListId { get; set; }
        public PriceList PriceList { get; set; } = null!;
        public string Currency { get; set; } = "TRY";
        public OrderStatus Status { get; set; } = OrderStatus.Draft;
        public decimal NetTotal { get; set; }
        public decimal VatTotal { get; set; }
        public decimal GrandTotal { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
