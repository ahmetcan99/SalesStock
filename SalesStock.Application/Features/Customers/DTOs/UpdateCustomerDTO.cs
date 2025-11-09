namespace SalesStock.Application.Features.Customers.DTOs
{
    public class UpdateCustomerDTO
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Email { get; set; }
        public string Phone { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
