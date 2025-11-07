namespace SalesStock.Web.ViewModels.PriceLists
{
    public class PriceListItemViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }
}
