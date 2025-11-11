using AutoMapper;
using AutoMapper.QueryableExtensions;
using SalesStock.Application.Features.Customers.DTOs;
using SalesStock.Application.Interfaces;
using SalesStock.Domain.Entities;
using SalesStock.Shared.Common;
using Microsoft.EntityFrameworkCore;

namespace SalesStock.Application.Features.Customers.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedList<CustomerDTO>> GetCustomersPagedAsync(string? sortOrder, string? searchTerm, string? statusFilter, int pageNumber, int pageSize)
        {
            var query = _customerRepository.GetAsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c => c.Code.Contains(searchTerm)
                               || c.Name.Contains(searchTerm)
                               || (c.Email != null && c.Email.Contains(searchTerm)));
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter.Equals("active", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(p => p.IsActive);
                }
                else if (statusFilter.Equals("passive", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(p => !p.IsActive);
                }
            }

            IOrderedQueryable<Customer> orderedQuery;
            switch (sortOrder)
            {
                case "name_desc":
                    orderedQuery = query.OrderByDescending(c => c.Name);
                    break;
                case "Code":
                    orderedQuery = query.OrderBy(c => c.Code);
                    break;
                case "code_desc":
                    orderedQuery = query.OrderByDescending(c => c.Code);
                    break;
                case "Email":
                    orderedQuery = query.OrderBy(c => c.Email);
                    break;
                case "email_desc":
                    orderedQuery = query.OrderByDescending(c => c.Email);
                    break;
                case "Status":
                    orderedQuery = query.OrderBy(c => c.IsActive);
                    break;
                case "status_desc":
                    orderedQuery = query.OrderByDescending(c => c.IsActive);
                    break;
                default:
                    orderedQuery = query.OrderBy(c => c.Name);
                    break;
            }

            var dtoQuery = orderedQuery.ProjectTo<CustomerDTO>(_mapper.ConfigurationProvider);
            return await PaginatedList<CustomerDTO>.CreateAsync(dtoQuery, pageNumber, pageSize);
        }

        public async Task AddCustomerAsync(CreateCustomerDTO customerDTO)
        {
            var customer = _mapper.Map<Customer>(customerDTO);
            await _customerRepository.AddAsync(customer);
        }

        public async Task<UpdateCustomerDTO?> GetCustomerForUpdateAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return null;
            }
            return _mapper.Map<UpdateCustomerDTO>(customer);
        }

        public async Task UpdateCustomerAsync(UpdateCustomerDTO customerDTO)
        {
            var customer = await _customerRepository.GetByIdAsync(customerDTO.Id)
                ?? throw new KeyNotFoundException($"Customer with Id {customerDTO.Id} not found.");

            _mapper.Map(customerDTO, customer);
            await _customerRepository.UpdateAsync(customer);
        }

        public async Task ToggleCustomerStatusAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Customer could not be found.");

            customer.IsActive = !customer.IsActive;
            customer.UpdatedAt = DateTime.UtcNow;

            await _customerRepository.UpdateAsync(customer);
        }

        public async Task<IEnumerable<CustomerSelectListDTO>> GetAllActiveCustomersForSelectListAsync()
        {
            var customersQuery = _customerRepository.GetAsQueryable()
                                                    .Where(c => c.IsActive)
                                                    .OrderBy(c => c.Name);

            return await _mapper.ProjectTo<CustomerSelectListDTO>(customersQuery)
                                .ToListAsync();
        }
    }
}