using AutoMapper;
using AutoMapper.QueryableExtensions;
using SalesStock.Application.Features.Stock.DTOs;
using SalesStock.Application.Interfaces;
using SalesStock.Domain.Entities;
using SalesStock.Domain.Enums;
using SalesStock.Shared.Common; 

namespace SalesStock.Application.Features.Stock.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepository;
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public StockService(IStockRepository stockRepository, IProductRepository productRepository, IMapper mapper)
        {
            _stockRepository = stockRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedList<StockMovementDTO>> GetStockMovementsPagedAsync(int pageNumber, int pageSize)
        {
            var query = _stockRepository.GetStockMovementsAsQueryable()
                                        .OrderByDescending(sm => sm.CreatedAt);

            var dtoQuery = query.ProjectTo<StockMovementDTO>(_mapper.ConfigurationProvider);

            int currentPage = pageNumber < 1 ? 1 : pageNumber;

            return await PaginatedList<StockMovementDTO>.CreateAsync(dtoQuery, pageNumber, pageSize);
        }
        public async Task<StockAdjustmentResultDTO> AdjustStockAsync(AdjustStockDTO adjustStockDTO)
        {
            var product = await _productRepository.GetByIdAsync(adjustStockDTO.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with SKU '{adjustStockDTO.ProductId}' not found.");
            }
            int oldStock = product.StockOnHand;

            if (product.StockOnHand + adjustStockDTO.Quantity < 0)
            {
                throw new InvalidOperationException(
                    $"Operation failed. This adjustment would result in negative stock " +
                    $"({product.StockOnHand + adjustStockDTO.Quantity}). Current stock is {product.StockOnHand}.");
            }
            product.StockOnHand += adjustStockDTO.Quantity;

            var stockMovement = new StockMovement
            {
                Product = product,
                ProductId = product.Id,
                Quantity = adjustStockDTO.Quantity,
                MovementType = StockMovementType.Adjust,
                Reason = adjustStockDTO.Reason,
                ReferenceNo = $"ADJ-{DateTime.UtcNow:yyyMMddHHmmssfff}"
            };
            await _stockRepository.AddMovementAndUpdateProductStockAsync(stockMovement, product);
            
            return new StockAdjustmentResultDTO
            {
                SKU = product.SKU,
                ProductName = product.Name,
                OldStock = oldStock,
                NewStock = product.StockOnHand,
                AdjustedQuantity = adjustStockDTO.Quantity,
                Reason = adjustStockDTO.Reason ?? "N/A"
            };
        }
        public async Task ReleaseStockAsync(int productId, int quantity, int orderId)
        {
            await _stockRepository.ReleaseStockAsync(productId, quantity, orderId);
        }
    }
}
