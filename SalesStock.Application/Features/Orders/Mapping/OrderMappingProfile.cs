using AutoMapper;
using SalesStock.Application.Features.Orders.DTOs;
using SalesStock.Domain.Entities;

namespace SalesStock.Application.Features.Orders.Mapping
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<CreateOrderDTO, Order>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OrderNo, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Domain.Enums.OrderStatus.Draft))
                .ForMember(dest => dest.PriceList, opt => opt.Ignore())
                .ForMember(dest => dest.Items, opt => opt.Ignore());

            CreateMap<Order, OrderListDTO>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name));
            
            CreateMap<Order, OrderDetailDTO>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(dest => dest.PriceListName, opt => opt.MapFrom(src => src.PriceList.Name));

            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductSku, opt => opt.MapFrom(src => src.Product.SKU))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Id));

        }
    }
}
