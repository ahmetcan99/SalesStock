using SalesStock.Application.Features.Customers.DTOs;
using SalesStock.Shared.Common;

namespace SalesStock.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<PaginatedList<CustomerDTO>> GetCustomersPagedAsync(string sortOrder, string searchTerm, string statusFilter, int pageNumber, int pageSize);
        Task AddCustomerAsync(CreateCustomerDTO CustomerDTO);
        Task<UpdateCustomerDTO?> GetCustomerForUpdateAsync(int id);
        Task UpdateCustomerAsync(UpdateCustomerDTO CustomerDTO);
        Task ToggleCustomerStatusAsync(int id);
    }
}