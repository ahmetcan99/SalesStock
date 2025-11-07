using SalesStock.Domain.Common;

namespace SalesStock.Domain.Entities
{
    public class PriceList : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Currency { get; set; } = "TRY";
        public bool IsDefault { get; set; }

        public ICollection<PriceListItem> Items { get; set; } = new List<PriceListItem>();

    }
}
