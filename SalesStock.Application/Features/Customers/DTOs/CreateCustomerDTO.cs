namespace SalesStock.Application.Features.Customers.DTOs
{
    public class CreateCustomerDTO
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Email { get; set; }
        public string Phone { get; set; } = null!;
    }
}
