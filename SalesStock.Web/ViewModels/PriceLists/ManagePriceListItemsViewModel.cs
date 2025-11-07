namespace SalesStock.Web.ViewModels.PriceLists
{
    public class ManagePriceListItemsViewModel
    {
        public int PriceListId { get; set; }
        public string PriceListName { get; set; }
        public List<PriceListItemViewModel> Items { get; set; }
        public AddPriceListItemViewModel NewItem { get; set; }
    }
}
