using SalesStock.Domain.Entities;
using SalesStock.Application.Features.Customers.DTOs;
using AutoMapper;
namespace SalesStock.Application.Features.Customers.Mapping
{
    public class CustomerMappingProfile : Profile
    {
        public CustomerMappingProfile() 
        {
            CreateMap<Customer, CustomerDTO>().ReverseMap();
            CreateMap<Customer, CreateCustomerDTO>().ReverseMap();
            CreateMap<Customer, UpdateCustomerDTO>().ReverseMap();
        }
    }
}
