namespace SalesStock.Application.Features.PriceLists.DTOs
{
    public class UpdatePriceListDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Currency { get; set; } = "TRY";
        public bool IsDefault { get; set; } = false;
    }
}
