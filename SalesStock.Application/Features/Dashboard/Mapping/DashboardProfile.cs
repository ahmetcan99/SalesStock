using AutoMapper;
using SalesStock.Application.Features.Dashboard.DTOs;
using SalesStock.Domain.Entities;

namespace SalesStock.Application.Features.Dashboard.Mapping
{
    public class DashboardProfile : Profile
    {
        public DashboardProfile()
        {
            CreateMap<Order, DashboardOrderRowDTO>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.OrderNo, opt => opt.MapFrom(src => src.OrderNo))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.GrandTotal, opt => opt.MapFrom(src => src.GrandTotal));
        }
    }
}
