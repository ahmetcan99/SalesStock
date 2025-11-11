using SalesStock.Application.Features.PriceLists.DTOs;

namespace SalesStock.Application.Features.PriceLists.DTOs
{
    public class ManagePriceListItemsDTO
    {
        public int PriceListId { get; set; }
        public string PriceListName { get; set; } = string.Empty;
        public List<PriceListItemDTO> Items { get; set; } = new();
        public AddPriceListItemDTO NewItem { get; set; } = new();
    }
}
