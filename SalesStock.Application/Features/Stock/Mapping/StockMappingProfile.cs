using AutoMapper;
using SalesStock.Application.Features.Stock.DTOs;
using SalesStock.Domain.Entities;

namespace SalesStock.Application.Features.Stock.Mapping
{
    public class StockMappingProfile : Profile
    {
        public StockMappingProfile()
        {
            CreateMap<StockMovement, StockMovementDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductSKU, opt => opt.MapFrom(src => src.Product.SKU)); ;
        }
    }
}
