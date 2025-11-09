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
        }
    }
}
