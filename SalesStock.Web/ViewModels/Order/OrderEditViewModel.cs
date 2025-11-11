using SalesStock.Application.Features.Orders.DTOs;

namespace SalesStock.Web.ViewModels.Order
{
    public class OrderEditViewModel
    {
        public OrderDetailDTO Order { get; set; } = new OrderDetailDTO();
        public AddOrderItemDTO NewOrderItem { get; set; } = new AddOrderItemDTO();
    }
}
