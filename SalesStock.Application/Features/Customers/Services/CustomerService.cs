using AutoMapper;
using AutoMapper.QueryableExtensions;
using SalesStock.Application.Features.Customers.DTOs;
using SalesStock.Application.Interfaces;
using SalesStock.Domain.Entities;
using SalesStock.Shared.Common;

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

        public async Task<PaginatedList<CustomerDTO>> GetCustomersPagedAsync(string sortOrder, string searchTerm, string statusFilter, int pageNumber, int pageSize)
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
            switch (sortOrder)
            {
                case "name_desc":
                    query = query.OrderByDescending(c => c.Name);
                    break;
                case "Code":
                    query = query.OrderBy(c => c.Code);
                    break;
                case "code_desc":
                    query = query.OrderByDescending(c => c.Code);
                    break;
                case "Email":
                    query = query.OrderBy(c => c.Email);
                    break;
                case "email_desc":
                    query = query.OrderByDescending(c => c.Email);
                    break;
                case "Status":
                    query = query.OrderBy(c => c.IsActive);
                    break;
                case "status_desc":
                    query = query.OrderByDescending(c => c.IsActive);
                    break;
                default:
                    query = query.OrderBy(c => c.Name);
                    break;
            }

            var dtoQuery = query.ProjectTo<CustomerDTO>(_mapper.ConfigurationProvider);
            return await PaginatedList<CustomerDTO>.CreateAsync(dtoQuery, pageNumber, pageSize);
        }

        public async Task AddCustomerAsync(CreateCustomerDTO CustomerDTO)
        {
            if (await _customerRepository.CodeExistsAsync(CustomerDTO.Code))
            {
                throw new InvalidOperationException("Customer code must be unique.");
            }
            var customer = _mapper.Map<Customer>(CustomerDTO);
            customer.IsActive = true;
            await _customerRepository.AddAsync(customer);
        }

        public async Task<UpdateCustomerDTO?> GetCustomerForUpdateAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            return _mapper.Map<UpdateCustomerDTO>(customer);
        }

        public async Task UpdateCustomerAsync(UpdateCustomerDTO CustomerDTO)
        {
            if (await _customerRepository.CodeExistsForOtherCustomerAsync(CustomerDTO.Id, CustomerDTO.Code))
            {
                throw new InvalidOperationException("Customer code must be unique.");
            }
            var customer = await _customerRepository.GetByIdAsync(CustomerDTO.Id)
                ?? throw new KeyNotFoundException("Customer could not be found.");

            _mapper.Map(CustomerDTO, customer);
            customer.UpdatedAt = DateTime.UtcNow;
            await _customerRepository.UpdateAsync(customer);
        }
        public async Task ToggleCustomerStatusAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Customer could not be found.");

            customer.IsActive = !customer.IsActive;
            await _customerRepository.UpdateAsync(customer);
        }
    }
}