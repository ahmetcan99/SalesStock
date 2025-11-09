using SalesStock.Application.Features.Products.DTOs;
using SalesStock.Shared.Common;

namespace SalesStock.Application.Interfaces
{
    public interface IProductService
    {
        Task<PaginatedList<ProductDTO>> GetProductsPagedAsync(
            string sortOrder, 
            string searchTerm, 
            string statusFilter,
            int currentPage,
            int pageNumber, 
            int pageSize);
        Task AddProductAsync(CreateProductDTO productDTO);
        Task<UpdateProductDTO?> GetProductForUpdateAsync(int id);
        Task UpdateProductAsync(UpdateProductDTO productDTO);
        Task ToggleProductStatusAsync(int id);
    }
}
