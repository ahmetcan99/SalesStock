using Microsoft.EntityFrameworkCore;
using SalesStock.Application.Interfaces;
using SalesStock.Domain.Entities;
using SalesStock.Infrastructure.Persistence;

namespace SalesStock.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;
        public CustomerRepository(AppDbContext context) { _context = context; }
        public IQueryable<Customer> GetAsQueryable()
        {
            return _context.Customers.AsNoTracking();
        }
        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers.FindAsync(id);
        }
        public async Task AddAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> CodeExistsAsync(string code)
        {
            return await _context.Customers.AnyAsync(c => c.Code == code);
        }
        public async Task<bool> CodeExistsForOtherCustomerAsync(int id, string code)
        {
            return await _context.Customers.AnyAsync(c => c.Code == code && c.Id != id);
        }

    }
}
