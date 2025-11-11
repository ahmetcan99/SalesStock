using SalesStock.Application.Features.Orders.DTOs;
using SalesStock.Domain.Enums;
using SalesStock.Shared.Common;

namespace SalesStock.Application.Interfaces
{
    public interface IOrderService
    {
        Task<int> CreateDraftOrderAsync(CreateOrderDTO createOrderDTO);
        Task<OrderDetailDTO?> GetOrderDetailsForEditAsync(int orderId);
        Task AddItemToOrderAsync(AddOrderItemDTO addOrderItemDTO);
        Task<PaginatedList<OrderListDTO>> GetOrdersPagedAsync(
            string? customerSearchTerm,
            OrderStatus? statusFilter,
            DateTime? startDate,
            DateTime? endDate,
            int pageNumber,
            int pageSize);
        Task CancelOrderAsync(int orderId);
        Task ApproveOrderAsync(int orderId);
        Task ShipOrderAsync(int orderId);
    }
}
