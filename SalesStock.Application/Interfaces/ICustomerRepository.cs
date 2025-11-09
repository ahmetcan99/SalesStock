using SalesStock.Domain.Entities;
namespace SalesStock.Application.Interfaces
{
    public interface ICustomerRepository
    {
        IQueryable<Customer> GetAsQueryable();
        Task<Customer?> GetByIdAsync(int id);
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task<bool> CodeExistsAsync(string code);
        Task<bool> CodeExistsForOtherCustomerAsync(int id, string code);
    }
}
