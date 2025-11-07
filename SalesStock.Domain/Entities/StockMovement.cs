using SalesStock.Domain.Common;
using SalesStock.Domain.Enums;

namespace SalesStock.Domain.Entities
{
    public class StockMovement : BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
        public StockMovementType MovementType { get; set; }
        public string? ReferenceNo { get; set; }
    }

}
