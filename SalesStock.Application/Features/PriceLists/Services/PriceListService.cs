using AutoMapper;
using AutoMapper.QueryableExtensions;
using SalesStock.Application.Features.PriceLists.DTOs;
using SalesStock.Application.Interfaces;
using SalesStock.Domain.Entities;
using SalesStock.Shared.Common;
using Microsoft.EntityFrameworkCore;

namespace SalesStock.Application.Features.PriceLists.Services
{
    public class PriceListService : IPriceListService
    {
        private readonly IPriceListRepository _priceListRepository;
        private readonly IPriceListItemRepository _priceListItemRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public PriceListService(
            IPriceListRepository priceListRepository,
            IPriceListItemRepository priceListItemRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            _priceListRepository = priceListRepository;
            _priceListItemRepository = priceListItemRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedList<PriceListDTO>> GetPriceListsPagedAsync(int pageNumber, int pageSize)
        {
            var query = _priceListRepository.GetAllSortedByNameAsQueryable();

            var dtoQuery = query.ProjectTo<PriceListDTO>(_mapper.ConfigurationProvider);

            return await PaginatedList<PriceListDTO>.CreateAsync(dtoQuery, pageNumber, pageSize);
        }

        public async Task CreatePriceListAsync(CreatePriceListDTO createPriceListDTO)
        {
            if (createPriceListDTO.IsDefault)
            {
                await _priceListRepository.UnsetDefaultPriceListAsync();
            }
            var priceList = _mapper.Map<PriceList>(createPriceListDTO);
            await _priceListRepository.AddAsync(priceList);
        }

        public async Task UpdatePriceListAsync(int id, UpdatePriceListDTO updatePriceListDTO)
        {
            var existingPriceList = await _priceListRepository.GetByIdAsync(id);
            if (existingPriceList == null)
            {
                throw new KeyNotFoundException($"PriceList with ID {id} not found.");
            }
            if (updatePriceListDTO.IsDefault && !existingPriceList.IsDefault)
            {
                await _priceListRepository.UnsetDefaultPriceListAsync();
            }
            _mapper.Map(updatePriceListDTO, existingPriceList);
            await _priceListRepository.UpdateAsync(existingPriceList);
        }

        public async Task<UpdatePriceListDTO?> GetPriceListByIdAsync(int id)
        {
            var priceList = await _priceListRepository.GetByIdAsync(id);
            return _mapper.Map<UpdatePriceListDTO>(priceList);
        }

        public async Task<bool> SetAsDefaultPriceListAsync(int id)
        {
            var priceList = await _priceListRepository.GetByIdAsync(id);
            if (priceList == null)
            {
                throw new KeyNotFoundException("Price list not found.");
            }
            if (priceList.IsDefault)
            {
                return false;
            }
            await _priceListRepository.SetAsDefaultAsync(priceList);
            return true;
        }

        public async Task<ManagePriceListItemsDTO?> GetPriceListItemsByIdAsync(int priceListId)
        {
            var priceList = await _priceListRepository.GetByIdAsync(priceListId);
            if (priceList == null)
            {
                return null;
            }
            var itemsWithProduct = await _priceListItemRepository.GetItemsByPriceListIdWithProductAsync(priceListId);
            var itemDTOs = _mapper.Map<List<PriceListItemDTO>>(itemsWithProduct);
            var model = new ManagePriceListItemsDTO
            {
                PriceListId = priceList.Id,
                PriceListName = priceList.Name,
                Items = itemDTOs,
                NewItem = new AddPriceListItemDTO { PriceListId = priceList.Id }
            };
            return model;
        }

        public async Task AddItemAsync(AddPriceListItemDTO newItemDto)
        {
            var product = await _productRepository.GetByIdAsync(newItemDto.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException($"'{newItemDto.ProductId}' Id number could not be found.");
            }

            var isOverlapping = await _priceListItemRepository.HasOverlappingItemsAsync(newItemDto.PriceListId, product.Id, newItemDto.ValidFrom, newItemDto.ValidTo);
            if (isOverlapping)
            {
                throw new InvalidOperationException("The specified date range conflicts with a current price for this product.");
            }

            var priceListItem = new PriceListItem
            {
                PriceListId = newItemDto.PriceListId,
                ProductId = product.Id,
                UnitPrice = newItemDto.Price,
                ValidFrom = newItemDto.ValidFrom,
                ValidTo = newItemDto.ValidTo
            };

            await _priceListItemRepository.AddAsync(priceListItem);
        }

        public async Task RemoveItemAsync(int itemId)
        {
            await _priceListItemRepository.RemoveAsync(itemId);
        }

        public async Task<List<ProductSearchResultDTO>> SearchProductsAsync(string searchTerm)
        {
            var products = await _productRepository.SearchAvailableProductsAsync(searchTerm);
            return _mapper.Map<List<ProductSearchResultDTO>>(products);
        }
        public async Task<IEnumerable<PriceListSelectListDTO>> GetAllActivePriceListsForSelectListAsync()
        {
            var priceLists = _priceListRepository.GetAllSortedByNameAsQueryable();
            var activePriceLists = priceLists.Where(pl => pl.IsActive);
            var priceListDTO = await priceLists.ProjectTo<PriceListSelectListDTO>(_mapper.ConfigurationProvider).ToListAsync();
            return priceListDTO;
        }
    }
}