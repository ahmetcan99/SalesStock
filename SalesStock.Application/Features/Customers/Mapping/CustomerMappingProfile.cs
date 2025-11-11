using AutoMapper;
using SalesStock.Domain.Entities;
using SalesStock.Application.Features.Customers.DTOs;

public class CustomerMappingProfile : Profile
{
    public CustomerMappingProfile()
    {
        CreateMap<Customer, CustomerDTO>().ReverseMap();

        CreateMap<CreateCustomerDTO, Customer>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        CreateMap<UpdateCustomerDTO, Customer>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        CreateMap<Customer, UpdateCustomerDTO>();
        CreateMap<Customer, CustomerSelectListDTO>();
    }
}