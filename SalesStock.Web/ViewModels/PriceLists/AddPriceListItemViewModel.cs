using System.ComponentModel.DataAnnotations;

namespace SalesStock.Web.ViewModels.PriceLists
{
    public class AddPriceListItemViewModel
    {
        public int PriceListId { get; set; }

        [Required]
        [Display(Name = "Product (SKU)")]
        public string Sku { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Valid From")]
        public DateTime ValidFrom { get; set; } = DateTime.Today;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Valid To")]
        public DateTime ValidTo { get; set; } = DateTime.Today.AddYears(1);
    }
}
