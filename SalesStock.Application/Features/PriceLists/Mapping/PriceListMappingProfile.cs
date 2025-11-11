using AutoMapper;
using SalesStock.Domain.Entities;
using SalesStock.Application.Features.PriceLists.DTOs;

namespace SalesStock.Application.Features.PriceLists.Mapping
{
    public class PriceListMappingProfile : Profile
    {
        public PriceListMappingProfile()
        {
            CreateMap<PriceList, PriceListDTO>().ReverseMap();
            CreateMap<PriceList, CreatePriceListDTO>().ReverseMap();
            CreateMap<PriceList, UpdatePriceListDTO>().ReverseMap();
            CreateMap<PriceListItem, PriceListItemDTO>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.Product.SKU))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProductId));
            CreateMap<PriceList, PriceListSelectListDTO>();
        }
    }
}
