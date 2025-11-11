using AutoMapper;
using SalesStock.Application.Features.Products.DTOs;
using SalesStock.Application.Features.PriceLists.DTOs;
using SalesStock.Domain.Entities;


namespace SalesStock.Application.Features.Products.Mapping
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Product, CreateProductDTO>().ReverseMap();
            CreateMap<Product, UpdateProductDTO>().ReverseMap();
            CreateMap<Product, ProductSearchResultDTO>()
                .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.SKU))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
        }
    }
}
