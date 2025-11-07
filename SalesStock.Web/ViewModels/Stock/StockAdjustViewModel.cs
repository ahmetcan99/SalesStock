using System.ComponentModel.DataAnnotations;
using SalesStock.Domain.Enums;
namespace SalesStock.Web.ViewModels.Stock
{
    public class StockAdjustViewModel
    {
        [Required]
        [Display(Name = "Product (SKU)")]
        public string SKU { get; set; }

        [Required]
        [Display(Name = "Quantity (+/-)")]
        public int Quantity { get; set; }
        [Required]
        [MaxLength(200)]
        public string Reason { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;
        public int OldStock { get; set; }
        public int NewStock { get; set; }
    }
}
