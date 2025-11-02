using SalesStock.Domain.Common;

namespace SalesStock.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
