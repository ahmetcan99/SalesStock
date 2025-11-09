namespace SalesStock.Application.Features.Customers.DTOs
{
    public class CustomerDTO
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Email { get; set; }
        public bool IsActive { get; set; }
    }
}
