using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SalesStock.Application.Features.Orders.DTOs;
using SalesStock.Application.Features.Stock.Services;
using SalesStock.Application.Interfaces;
using SalesStock.Domain.Entities;
using SalesStock.Domain.Enums;
using SalesStock.Shared.Common;

namespace SalesStock.Application.Features.Orders.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IPriceListRepository _priceListRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPriceListItemRepository _priceListItemRepository;
        private readonly IStockRepository _stockRepository;

        public OrderService(
            IOrderRepository orderRepository, 
            IMapper mapper, 
            IPriceListRepository customerRepository, 
            IPriceListItemRepository priceListItemRepository,
            IProductRepository productRepository,
            IStockRepository stockRepository)
        {
            _orderRepository = orderRepository;
            _stockRepository = stockRepository;
            _mapper = mapper;
            _priceListRepository = customerRepository;
            _priceListItemRepository = priceListItemRepository;
            _productRepository = productRepository;
        }
        public async Task<int> CreateDraftOrderAsync(CreateOrderDTO createOrderDTO)
        {
            var priceList = await _priceListRepository.GetByIdAsync(createOrderDTO.CustomerId);
            if (priceList == null)
            {
                throw new ArgumentException("Invalid Price List");
            }
            var order = _mapper.Map<Order>(createOrderDTO);
            order.Currency = priceList.Currency;
            order.OrderNo = GenerateOrderNumber();
            var createdOrder = await _orderRepository.AddAsync(order);

            return createdOrder.Id;
        }

        public async Task<PaginatedList<OrderListDTO>> GetOrdersPagedAsync(
        string? customerSearchTerm, OrderStatus? status, DateTime? startDate,
        DateTime? endDate, int pageNumber, int pageSize)
        {
            IQueryable<Order> query = _orderRepository.GetAsQueryable();
            
            query = query.Include(o => o.Customer);

            if (!string.IsNullOrEmpty(customerSearchTerm))
            {
                query = query.Where(o => o.Customer.Name.Contains(customerSearchTerm));
            }

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt <= endDate.Value.AddDays(1).AddTicks(-1));
            }

            var orderedQuery = query.OrderByDescending(o => o.CreatedAt);

            var projectedQuery = orderedQuery.ProjectTo<OrderListDTO>(_mapper.ConfigurationProvider);

            return await PaginatedList<OrderListDTO>.CreateAsync(projectedQuery, pageNumber, pageSize);
        }
        public async Task AddItemToOrderAsync(AddOrderItemDTO addOrderItemDTO)
        {
            var order = await _orderRepository.GetByIdWithItemsAndProductsAsync(addOrderItemDTO.OrderId);
            if (order == null) throw new ArgumentException("Order not found.");
            if (order.Status != OrderStatus.Draft) throw new InvalidOperationException("Items can only be added to orders in 'Draft' status.");

            var productToAdd = await _productRepository.GetByIdAsync(addOrderItemDTO.ProductId);
            if (productToAdd == null) throw new ArgumentException("Product not found.");

            var priceListItem = await _priceListItemRepository.GetPriceForItemAsync(order.PriceListId, addOrderItemDTO.ProductId, DateTime.UtcNow);
            if (priceListItem == null) throw new ArgumentException("Product does not have a price in the selected Price List.");

            decimal unitPrice = priceListItem.UnitPrice;

            var existingItem = order.Items.FirstOrDefault(oi => oi.ProductId == addOrderItemDTO.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += addOrderItemDTO.Quantity;
            }
            else
            {
                order.Items.Add(new OrderItem
                {
                    ProductId = addOrderItemDTO.ProductId,
                    ProductSKU = productToAdd.SKU,
                    Quantity = addOrderItemDTO.Quantity,
                    UnitPrice = unitPrice,
                });
            }

            decimal totalNet = 0;
            decimal totalVat = 0;

            foreach (var item in order.Items)
            {

                var currentProduct = item.Product ?? productToAdd;

                item.NetTotal = item.Quantity * item.UnitPrice;
                item.VatTotal = item.NetTotal * currentProduct.VatRate;
                item.GrandTotal = item.NetTotal + item.VatTotal;

                totalNet += item.NetTotal;
                totalVat += item.VatTotal;
            }

            order.NetTotal = totalNet;
            order.VatTotal = totalVat;
            order.GrandTotal = totalNet + totalVat;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
        }
        public async Task RemoveItemAsync(int orderId, int itemId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId)
                ?? throw new InvalidOperationException("Order not found.");

            if (order.Status != OrderStatus.Draft)
                throw new InvalidOperationException("Only draft orders can be modified.");

            var item = await _orderRepository.GetOrderItemAsync(orderId, itemId);
            if (item == null)
                throw new InvalidOperationException("Item not found in order.");

            var removed = await _orderRepository.RemoveItemByProductAsync(orderId, itemId);
            if (!removed)
                throw new InvalidOperationException("Item could not be removed.");

            await _stockRepository.ReleaseStockAsync(item.ProductId, item.Quantity, orderId);
        }
        public async Task<OrderDetailDTO?> GetOrderDetailsForEditAsync(int orderId)
        {
            var order = await _orderRepository.GetAsQueryable()
                .Include(o => o.Customer)
                .Include(o => o.PriceList)
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return null;
            }

            return _mapper.Map<OrderDetailDTO>(order);
        }
        public async Task CancelOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdWithItemsAndProductsAsync(orderId);
            if (order == null) throw new KeyNotFoundException("Order could not be found.");
            if (order.Status != OrderStatus.Draft && order.Status != OrderStatus.Approved)
            {
                throw new InvalidOperationException($"Cannot cancel an order with status '{order.Status}'.");
            }

            if (order.Status == OrderStatus.Approved)
            {
                foreach (var item in order.Items)
                {
                    item.Product.StockReserved -= item.Quantity;

                    var stockMovement = new StockMovement
                    {
                        ProductId = item.ProductId,
                        Quantity = -item.Quantity,
                        MovementType = StockMovementType.Release,
                        ReferenceNo = $"CANCEL-{order.OrderNo}",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _stockRepository.AddMovementAndUpdateProductStockAsync(stockMovement, item.Product);
                }
            }

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);
        }
        public async Task ApproveOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdWithItemsAndProductsAsync(orderId);
            if (order == null) throw new KeyNotFoundException("Order could not be found.");
            if (order.Status != OrderStatus.Draft)
            {
                throw new InvalidOperationException($"Only 'Draft' orders can be approved. Current status: '{order.Status}'.");
            }
            foreach (var item in order.Items)
            {
                item.Product.StockReserved += item.Quantity;
                var stockMovement = new StockMovement
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    MovementType = StockMovementType.Reserve,
                    ReferenceNo = $"APPROVE-{order.OrderNo}",
                    CreatedAt = DateTime.UtcNow
                };
                await _stockRepository.AddMovementAndUpdateProductStockAsync(stockMovement, item.Product);
            }
            order.Status = OrderStatus.Approved;
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);
        }
        public async Task ShipOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdWithItemsAndProductsAsync(orderId);
            if (order == null) throw new KeyNotFoundException("Order could not be found.");
            if (order.Status != OrderStatus.Approved)
            {
                throw new InvalidOperationException($"Only 'Approved' orders can be shipped. Current status: '{order.Status}'.");
            }
            foreach (var item in order.Items)
            {
                item.Product.StockReserved -= item.Quantity;
                item.Product.StockOnHand -= item.Quantity;
                var stockMovement = new StockMovement
                {
                    ProductId = item.ProductId,
                    Quantity = -item.Quantity,
                    MovementType = StockMovementType.Ship,
                    ReferenceNo = $"SHIP-{order.OrderNo}",
                    CreatedAt = DateTime.UtcNow
                };
                await _stockRepository.AddMovementAndUpdateProductStockAsync(stockMovement, item.Product);
            }
            order.Status = OrderStatus.Shipped;
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);
        }
        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }
    }
}
