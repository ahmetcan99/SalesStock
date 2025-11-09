namespace SalesStock.Application.Features.PriceLists.DTOs
{
    public class CreatePriceListDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Currency { get; set; } = "TRY";
        public bool IsDefault { get; set; } = false;
    }
}
