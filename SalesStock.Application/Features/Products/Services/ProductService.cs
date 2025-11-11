using AutoMapper;
using AutoMapper.QueryableExtensions;
using SalesStock.Application.Features.Products.DTOs;
using SalesStock.Application.Interfaces;
using SalesStock.Shared.Common;
using SalesStock.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SalesStock.Application.Features.Products.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedList<ProductDTO>> GetProductsPagedAsync(string sortOrder, string searchTerm, string statusFilter, int currentPage, int pageNumber, int pageSize)
        {
            var query = _productRepository.GetAsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.SKU.Contains(searchTerm)
                                       || p.Name.Contains(searchTerm)
                                       || (p.BarCode != null && p.BarCode.Contains(searchTerm)));
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter.Equals("active", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(p => p.IsActive);
                }
                else if (statusFilter.Equals("passive", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(p => !p.IsActive);
                }
            }

            switch (sortOrder)
            {
                case "sku_desc":
                    query = query.OrderByDescending(p => p.SKU);
                    break;
                case "Name":
                    query = query.OrderBy(p => p.Name);
                    break;
                case "name_desc":
                    query = query.OrderByDescending(p => p.Name);
                    break;
                default:
                    query = query.OrderBy(p => p.SKU);
                    break;
            }

            var dtoQuery = query.ProjectTo<ProductDTO>(_mapper.ConfigurationProvider);

            return await PaginatedList<ProductDTO>.CreateAsync(dtoQuery, pageNumber, pageSize);
        }

        public async Task AddProductAsync(CreateProductDTO productDTO)
        {
            if (await _productRepository.SkuExistsAsync(productDTO.SKU))
            {
                throw new InvalidOperationException($"A product with SKU '{productDTO.SKU}' already exists.");
            }
            var product = _mapper.Map<Product>(productDTO);
            product.StockOnHand = 0;
            product.StockReserved = 0;
            product.IsActive = true;

            await _productRepository.AddAsync(product);
        }
        public async Task<UpdateProductDTO?> GetProductForUpdateAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return null;
            }
            return _mapper.Map<UpdateProductDTO>(product);
        }
        public async Task UpdateProductAsync(UpdateProductDTO productDTO)
        {
            if (await _productRepository.SkuExistsForOtherProductAsync(productDTO.Id, productDTO.SKU))
            {
                throw new InvalidOperationException($"A product with SKU '{productDTO.SKU}' already exists.");
            }
            var product = await _productRepository.GetByIdAsync(productDTO.Id);
            if (product == null)
            {
                throw new InvalidOperationException("Product not found.");
            }
            _mapper.Map(productDTO, product);
            await _productRepository.UpdateAsync(product);
        }
        public async Task ToggleProductStatusAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new InvalidOperationException("Product not found.");
            }
            product.IsActive = !product.IsActive;
            await _productRepository.UpdateAsync(product);
        }
    }
}