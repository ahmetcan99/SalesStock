using AutoMapper;
using SalesStock.Application.Features.Reports.DTOs;
using SalesStock.Domain.Entities;

namespace SalesStock.Application.Features.Reports.Mapping
{
    public class ReportProfile : Profile
    {
        public ReportProfile()
        {
            CreateMap<Product, LowStockItemDTO>()
            .ForMember(dest => dest.ProductName, m => m.MapFrom(src => src.Name))
            .ForMember(dest => dest.SKU, m => m.MapFrom(src => src.SKU))
            .ForMember(dest => dest.StockOnHand, m => m.MapFrom(src => src.StockOnHand));
        }
    }
}
