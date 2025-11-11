namespace SalesStock.Application.Features.PriceLists.DTOs
{
    public class AddPriceListItemDTO
    {
        public int PriceListId { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

    }
}
