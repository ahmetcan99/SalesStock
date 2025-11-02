namespace SalesStock.Domain.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public bool isActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
    }
}
